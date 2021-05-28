using LabviewDXFViewer.DataTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Flurl;
using Flurl.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace LabviewDXFViewer
{
    public partial class Microsites : UserControl
    {
        public static string WebHost = "https://raxdatastore.azurewebsites.net/api/";
        public Microsites()
        {
            InitializeComponent();

            deleteEvent = DeleteRow;
        }



        public List<ProbeSite> Rows = new List<ProbeSite>();

        public class WaferInfo
        {
            public string waferName { get; set; }
            public ProbeSite[] probes { get; set; }
            public string activeLayer { get; set; }

            public string uploadedDate { get; set; }
        }

        public void SaveListSitesCloud(string waferName)
        {

            var saveData = new WaferInfo
            {
                activeLayer = Canvas.SaveLayerActivationDelimited(),
                probes = GetListData(),
                waferName = waferName
            };

            var result = (WebHost + "WaferTestSiteLoad?code=dG3i8BEApZF3cS00grJdfbpClSsPfPJ9oH2lLa4FyLtcReGbrmyp0w==").PostJsonAsync(saveData).Result;

        }

        public string SaveListSitesLV(string waferName)
        {

            var saveData = new WaferInfo
            {
                activeLayer = Canvas.SaveLayerActivationDelimited(),
                probes = GetListData(),
                waferName = waferName
            };

            return JsonConvert.SerializeObject(saveData);

        }

        static HttpClient client = new HttpClient();

        public string[] LoadWaferPlansCloud()
        {
            var result = (WebHost + "WaferPlanNames?code=SZ7L74ejL0kPdEXzzjsz7eHToJ/JoEd80ocG5DEbaqE3mv4sdO6euA==").GetJsonAsync<string[]>().Result;
            return result;

        }
        public string LoadListSitesLV(string waferName, string dxfFile, string waferTestInfos)
        {

            var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SPSProbes";
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            var filename = dir + "\\" + waferName + ".dxf";

            File.WriteAllText(filename, dxfFile);
            Canvas.LoadFile(filename);

            var result = JsonConvert.DeserializeObject<WaferInfo>(waferTestInfos);

            var addData = new List<ProbeSite>();
            Canvas.LoadLayerActivation(result.activeLayer.Replace("||", " "));

            if (result.probes != null)
            {
                for (int lineI = 0; lineI < result.probes.Length; lineI++)
                {
                    var newPoint = result.probes[lineI];
                    ExistingData.Add(newPoint);
                    addData.Add(newPoint);
                }
                foreach (var point in addData)
                {
                    Canvas.AddListSite(point.Position.X, point.Position.Y);
                }
            }
            return filename;
        }
        public void LoadListSitesCloud(string waferName)
        {

            var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SPSProbes";
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            var filename = dir + "\\" + waferName + ".dxf";

            var dxf = (WebHost + "WaferPlanDXFView?WaferName=" + waferName + "&code=BdTxfpvmw2aahMiFoXpPgHHxyuUv8yq5Svd3BmcinOAaGLkNtXJayQ==").GetStreamAsync().Result;
            using (var fileStream = File.Create(filename))
            {
                dxf.CopyTo(fileStream);
            }
            Canvas.LoadFile(filename);

            var result = (WebHost + "WaferTestSiteView?WaferName=" + waferName + "&code=7qEXL895pHq3Sl9YRkamCvsoCAoVcacaz0OFLx5uM2/m9tc8eWo5hA==").GetJsonAsync<WaferInfo>().Result;

            var addData = new List<ProbeSite>();
            Canvas.LoadLayerActivation(result.activeLayer.Replace("||", " "));

            if (result.probes != null)
            {
                for (int lineI = 0; lineI < result.probes.Length; lineI++)
                {
                    var newPoint = result.probes[lineI];
                    ExistingData.Add(newPoint);
                    addData.Add(newPoint);
                }
                foreach (var point in addData)
                {
                    Canvas.AddListSite(point.Position.X, point.Position.Y);
                }
            }
        }


        public void SaveListSites(string waferName)
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SPSProbes";
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            var filename = dir + "\\" + waferName + ".csv";

            var outText = "Visible Layers\n";
            outText += Canvas.SaveLayerActivation() + "\n";

            outText += "Junction Name, Orientation, X(um), Y(um)\n";
            foreach (var line in GetListData(ProbeOrientation.Horizontal))
            {
                outText += line.JunctionName + "," + line.Orientation.ToString() + "," + line.Position.X + "," + line.Position.Y + "\n";
            }
            foreach (var line in GetListData(ProbeOrientation.Vertical))
            {
                outText += line.JunctionName + "," + line.Orientation.ToString() + "," + line.Position.X + "," + line.Position.Y + "\n";
            }
            File.WriteAllText(filename, outText);
        }

        public DXFCanvas Canvas { get; set; }
        public void LoadListSites(string waferName)
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SPSProbes";
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            var filename = dir + "\\" + waferName + ".csv";

            var addData = new List<ProbeSite>();
            var inText = File.ReadAllLines(filename);
            Canvas.LoadLayerActivation(inText[1]);
            for (int lineI = 3; lineI < inText.Length; lineI++)
            {
                var parts = inText[lineI].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var newPoint = new ProbeSite { JunctionName = parts[0], Orientation = parts[1], Position = new Point(int.Parse(parts[2]), int.Parse(parts[3])) };
                ExistingData.Add(newPoint);
                addData.Add(newPoint);
            }
            foreach (var point in addData)
            {
                Canvas.AddListSite(point.Position.X, point.Position.Y);
            }
        }

        public void AddListSite(string junctionName, ProbeOrientation orientation, Point location)
        {
            var newPoint = new ProbeSite { JunctionName = junctionName, Orientation = orientation.ToString(), Position = location };
            ExistingData.Add(newPoint);
            dataGridView1.Rows.Add(newPoint.JunctionName, newPoint.Position.X + "," + newPoint.Position.Y, newPoint.Orientation);
            GetFirstCorner(ProbeOrientation.Horizontal);
        }
        public void AddResult(ProbeSite site, string result)
        {
            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == site.JunctionName)
                    dataGridView1.Rows[i].Cells[3].Value = result;
            }
        }

        public void AddListData(Point[] selectedLocations)
        {
            Rows.Clear();

            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                if (dataGridView1.Rows[i].IsNewRow == false)
                    dataGridView1.Rows.RemoveAt(i);
            }

            var addData = new List<ProbeSite>();
            foreach (var point in selectedLocations)
            {
                var hits = ExistingData.Where(x => Math.Abs(point.X - x.Position.X) < 10 && Math.Abs(point.Y - x.Position.Y) < 10).FirstOrDefault();
                if (hits == null)
                {
                    var newPoint = new ProbeSite { JunctionName = "UNK"+ ExistingData.Count, Orientation = "Horizontal", Position = point };
                    ExistingData.Add(newPoint);
                    addData.Add(newPoint);
                }
                else
                    addData.Add(hits);
            }

            foreach (var row in addData)
            {
                dataGridView1.Rows.Add(row.JunctionName, row.Position.X + "," + row.Position.Y, row.Orientation,"");
            }

            GetFirstCorner(ProbeOrientation.Horizontal);
        }

        public ProbeSite[] GetListData()
        {
            var sOrientation = Orientation.ToString().ToLower();
            var sites = new List<ProbeSite>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null)
                {
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                    });
                }
            }

            return sites.OrderBy(x => x.Position.Y * 10000 + x.Position.X / 100).ToArray();
        }

        public ProbeSite[] GetListData(ProbeOrientation Orientation)
        {
            var sOrientation = Orientation.ToString().ToLower();
            var sites = new List<ProbeSite>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null && dataGridView1.Rows[i].Cells[2].Value.ToString().ToLower() == sOrientation)
                {
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                    });
                }
            }

            return sites.OrderBy(x => x.Position.Y * 10000 + x.Position.X / 100).ToArray();
        }

        public int[] GetFirstCorner(ProbeOrientation Orientation)
        {
            var sOrientation = Orientation.ToString().ToLower();
            var sites = new List<ProbeSite>();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null && dataGridView1.Rows[i].Cells[2].Value.ToString().ToLower() == sOrientation)
                {
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                    });
                }
            }

            var middleX = 0;// sites.Average(x => x.Position.X);
            var middleY = 0;// sites.Average(y => y.Position.Y);

            var maxX = 0d;
            var maxI = -1;
            for (int i = 0; i < sites.Count; i++)
            {
                var x = sites[i].Position.X - middleX;
                var y = sites[i].Position.Y - middleY;
                if (y < 0 && x < maxX)
                {
                    maxI = i;
                    maxX = x;
                }
            }

            if (maxI == -1)
            {
                return new int[] { 0, 0 };
            }

            if (Canvas != null)
                Canvas.SetCorner(0, new Point((int)sites[maxI].Position.X, (int)sites[maxI].Position.Y));


            return new int[] { (int)sites[maxI].Position.X, (int)sites[maxI].Position.Y };
        }

        public int[] GetSecondCorner(ProbeOrientation Orientation)
        {
            var sOrientation = Orientation.ToString().ToLower();
            var sites = new List<ProbeSite>();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null && dataGridView1.Rows[i].Cells[2].Value.ToString().ToLower() == sOrientation)
                {
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                    });
                }
            }

            var middleX = 0;// sites.Average(x => x.Position.X);
            var middleY = 0;// sites.Average(y => y.Position.Y);

            var maxX = 0d;
            var maxI = -1;
            for (int i = 0; i < sites.Count; i++)
            {
                var x = sites[i].Position.X - middleX;
                var y = sites[i].Position.Y - middleY;
                if (y < 0 && x > maxX)
                {
                    maxI = i;
                    maxX = x;
                }
            }

            if (maxI == -1)
            {
                return new int[] { 0, 0 };
            }
            if (Canvas != null)
                Canvas.SetCorner(1, new Point((int)sites[maxI].Position.X, (int)sites[maxI].Position.Y));


            return new int[] { (int)sites[maxI].Position.X, (int)sites[maxI].Position.Y };
        }

        public int[] GetThirdCorner(ProbeOrientation Orientation)
        {
            var sOrientation = Orientation.ToString().ToLower();
            var sites = new List<ProbeSite>();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null && dataGridView1.Rows[i].Cells[2].Value.ToString().ToLower() == sOrientation)
                {
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                    });
                }
            }

            var middleX = 0;// sites.Average(x => x.Position.X);
            var middleY = 0;// sites.Average(y => y.Position.Y);


            var maxY = 0d;
            var maxI = -1;
            for (int i = 0; i < sites.Count; i++)
            {
                var x = sites[i].Position.X - middleX;
                var y = sites[i].Position.Y - middleY;
                if (y > maxY)
                {
                    maxI = i;
                    maxY = y;
                }
            }

            if (maxI == -1)
            {
                return new int[] { 0, 0 };
            }

            if (Canvas != null)
                Canvas.SetCorner(2, new Point((int)sites[maxI].Position.X, (int)sites[maxI].Position.Y));

            return new int[] { (int)sites[maxI].Position.X, (int)sites[maxI].Position.Y };
        }



        private int MouseOverRow;
        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Delete Selected Rows", deleteEvent));
                MouseOverRow = e.RowIndex;
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        EventHandler deleteEvent;

        private void DeleteRow(object sender, EventArgs e)
        {
            var selected = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                selected.Add(row);
            }
            selected = selected.OrderByDescending(x => x.Index).ToList();
            foreach (var row in selected)
                dataGridView1.Rows.RemoveAt(row.Index);
        }

        List<ProbeSite> ExistingData = new List<ProbeSite>();
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var addData = new List<ProbeSite>();
            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace((string)dataGridView1.Rows[i].Cells[1].Value) == false)
                {
                    addData.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                    });
                }
            }

            foreach (var point in addData)
            {
                var hits = ExistingData.Where(x => Math.Abs(point.Position.X - x.Position.X) < 10 && Math.Abs(point.Position.Y - x.Position.Y) < 10).FirstOrDefault();
                if (hits == null)
                {
                    ExistingData.Add(point);
                }
                else
                {
                    hits.JunctionName = point.JunctionName;
                    hits.Orientation = point.Orientation;
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var i = e.RowIndex;

            if (string.IsNullOrWhiteSpace((string)dataGridView1.Rows[i].Cells[1].Value) == false)
            {
                var selected = new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                {
                    JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                    Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                };

                Canvas.SetMarker(selected.Position);
            }
        }
    }
}
