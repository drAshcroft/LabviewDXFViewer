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
        // public static string WebHost = "https://raxdatastore2.azurewebsites.net/api/";
        public static string WebHost = "http://localhost:7073/api/";
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

        private readonly HttpClient _httpClient;

        Random rnd = new Random();
        public void SaveListSitesCloud(string waferName)
        {

            var filename = Canvas.Filename;
            var saveData = new WaferInfo
            {
                activeLayer = Canvas.SaveLayerActivationDelimited(),
                probes = GetListData(),
                waferName = waferName,

            };


            var result = (WebHost + $"DataUploadJson?Tags=W005|D2|JUNCTIONS|B1B2|IV|UNSHUNT&DataType=IV_SERIES").WithHeader("Authorization", Form1.pass).WithHeader("x-user", "probe").PostStringAsync("test").Result;

        }
        public void SaveListSitesCloudO(string waferName)
        {

            var filename = Canvas.Filename;
            var saveData = new WaferInfo
            {
                activeLayer = Canvas.SaveLayerActivationDelimited(),
                probes = GetListData(),
                waferName = waferName,

            };


            var resp = (WebHost + "WaferPlanLoad").WithHeader("Authorization", Form1.pass).WithHeader("x-user", "probe").PostMultipartAsync(mp => mp
                    .AddString("title", waferName)
                    .AddFile("file", filename)
                 ).Result;


            var result = (WebHost + "WaferTestSiteLoad?blob=" + Path.GetFileName(filename)).WithHeader("Authorization", Form1.pass).WithHeader("x-user", "probe").PostJsonAsync(saveData).Result;

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
            var result = (WebHost + "WaferPlanNames").WithHeader("Authorization", Form1.pass).WithHeader("x-user", "probe").GetJsonAsync<string[]>().Result;
            return result;

        }
        public string LoadListSitesLV(string waferName, string dxfFile, string waferTestInfos)
        {
            ExistingData.Clear();
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
            ExistingData.Clear();
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SPSProbes";
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            var filename = dir + "\\" + waferName + ".dxf";

            var dxf = (WebHost + "WaferPlanDXFView?WaferName=" + waferName).WithHeader("Authorization", Form1.pass).WithHeader("x-user", "probe").GetStreamAsync().Result;
            using (var fileStream = File.Create(filename))
            {
                dxf.CopyTo(fileStream);
            }
            Canvas.LoadFile(filename);

            var result = (WebHost + "WaferTestSiteView?WaferName=" + waferName).WithHeader("Authorization", Form1.pass).WithHeader("x-user", "probe").GetJsonAsync<WaferInfo>().Result;

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
            ExistingData.Clear();
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

        public string ToSignificantDigits(
     double value, int significant_digits)
        {
            // Use G format to get significant digits.
            // Then convert to double and use F format.
            string format1 = "{0:G" + significant_digits.ToString() + "}";
            string result = Convert.ToDouble(
                String.Format(format1, value)).ToString("F99");

            // Rmove trailing 0s.
            result = result.TrimEnd('0');

            // Rmove the decimal point and leading 0s,
            // leaving just the digits.
            string test = result.Replace(".", "").TrimStart('0');

            // See if we have enough significant digits.
            if (significant_digits > test.Length)
            {
                // Add trailing 0s.
                result += new string('0', significant_digits - test.Length);
            }
            else
            {
                // See if we should remove the trailing decimal point.
                if ((significant_digits < test.Length) &&
                    result.EndsWith("."))
                    result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        private string ToEngineeringNotation(double d)
        {
            double exponent = Math.Log10(Math.Abs(d));
            if (Math.Abs(d) >= 1)
            {
                switch ((int)Math.Floor(exponent))
                {
                    case 0:
                    case 1:
                    case 2:
                        return ToSignificantDigits(d, 3);
                    case 3:
                    case 4:
                    case 5:
                        return ToSignificantDigits(d / 1e3, 3) + "k";
                    case 6:
                    case 7:
                    case 8:
                        return ToSignificantDigits(d / 1e6, 3) + "M";
                    case 9:
                    case 10:
                    case 11:
                        return ToSignificantDigits(d / 1e9, 3) + "G";
                    case 12:
                    case 13:
                    case 14:
                        return ToSignificantDigits(d / 1e12, 3) + "T";
                    case 15:
                    case 16:
                    case 17:
                        return ToSignificantDigits(d / 1e15, 3) + "P";
                    case 18:
                    case 19:
                    case 20:
                        return ToSignificantDigits(d / 1e18, 3) + "E";
                    case 21:
                    case 22:
                    case 23:
                        return ToSignificantDigits(d / 1e21, 3) + "Z";
                    default:
                        return ToSignificantDigits(d / 1e24, 3) + "Y";
                }
            }
            else if (Math.Abs(d) > 0)
            {
                switch ((int)Math.Floor(exponent))
                {
                    case -1:
                    case -2:
                    case -3:
                        return ToSignificantDigits(d * 1e3, 3) + "m";
                    case -4:
                    case -5:
                    case -6:
                        return ToSignificantDigits(d * 1e6, 3) + "μ";
                    case -7:
                    case -8:
                    case -9:
                        return ToSignificantDigits(d * 1e9, 3) + "n";
                    case -10:
                    case -11:
                    case -12:
                        return ToSignificantDigits(d * 1e12, 3) + "p";
                    case -13:
                    case -14:
                    case -15:
                        return ToSignificantDigits(d * 1e15, 3) + "f";
                    case -16:
                    case -17:
                    case -18:
                        return ToSignificantDigits(d * 1e15, 3) + "a";
                    case -19:
                    case -20:
                    case -21:
                        return ToSignificantDigits(d * 1e15, 3) + "z";
                    default:
                        return ToSignificantDigits(d * 1e15, 3) + "y";
                }
            }
            else
            {
                return "0";
            }
        }

        public void AddResult(ProbeSite site, double conductance, double capacitance, double intercept, string conductUnit, string capUnit, string interceptUnit)
        {
            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString() == site.JunctionName)
                    {
                        dataGridView1.Rows[i].Cells[3].Value = ToEngineeringNotation(conductance) + conductUnit;
                        dataGridView1.Rows[i].Cells[4].Value = ToEngineeringNotation(capacitance) + capUnit;
                        dataGridView1.Rows[i].Cells[5].Value = ToEngineeringNotation(intercept) + interceptUnit;
                        dataGridView1.Rows[i].Selected = true;
                    }
                }
                catch { }
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
                    var newPoint = new ProbeSite { JunctionName = "UNK" + ExistingData.Count, Orientation = "Horizontal", Position = point };
                    ExistingData.Add(newPoint);
                    addData.Add(newPoint);
                }
                else
                    addData.Add(hits);
            }

            foreach (var row in addData)
            {
                dataGridView1.Rows.Add(row.JunctionName, row.Position.X + "," + row.Position.Y, row.Orientation, "", "", "", "0");
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
                    double w = double.Parse(dataGridView1.Rows[i].Cells[6].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[6].Value.ToString());
                    double h = double.Parse(dataGridView1.Rows[i].Cells[7].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[7].Value.ToString());
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                        Area = w * h,
                        BottomWidth = w,
                        TopWidth = h
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
                    double w = double.Parse(dataGridView1.Rows[i].Cells[6].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[6].Value.ToString());
                    double h = double.Parse(dataGridView1.Rows[i].Cells[7].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[6].Value.ToString());
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                        Area = w * h,
                        BottomWidth = w,
                        TopWidth = h
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

        private int _ClickedRow = -1;
        public int ClickedRow
        {
            get
            {
                var t = _ClickedRow;
                _ClickedRow = -1;
                return t;
            }
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
