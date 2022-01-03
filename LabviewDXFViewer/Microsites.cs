using Accord.Imaging;
using Flurl.Http;
using LabviewDXFViewer.DataTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;

namespace LabviewDXFViewer
{
    public partial class Microsites : UserControl
    {
        public static string WebHost = "https://raxdatastore2.azurewebsites.net/api/";
        //public static string WebHost = "http://localhost:7073/api/";
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


            var jsonS = JsonConvert.SerializeObject(result);
            File.WriteAllText(@"S:\Research\ProbeStation\WaferPlans\" + waferName + ".json", jsonS);

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
            foreach (var line in GetListData())
            {
                outText += line.JunctionName + "," + line.Function.ToString() + "," + line.Position.X + "," + line.Position.Y + "\n";
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
                var newPoint = new ProbeSite { JunctionName = parts[0], Function = parts[1], Position = new Point(int.Parse(parts[2]), int.Parse(parts[3])) };
                ExistingData.Add(newPoint);
                addData.Add(newPoint);
            }
            foreach (var point in addData)
            {
                Canvas.AddListSite(point.Position.X, point.Position.Y);
            }
        }

        public void AddListSite(string junctionName, ProbeOrientation orientation, ProbeFunction function, Point location)
        {

            var newPoint = new ProbeSite { JunctionName = junctionName, Orientation = orientation.ToString(), Function = function.ToDescription(), Position = location };
            ExistingData.Add(newPoint);
            dataGridView1.Rows.Add(newPoint.JunctionName, newPoint.Position.X + "," + newPoint.Position.Y, newPoint.Function, 0, 0, 0, newPoint.TopWidth, newPoint.BottomWidth, newPoint.Area);
            GetFirstCorner();
        }

        public string ToSignificantDigits(double value, int significant_digits)
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

        public void AddResult(ProbeSite site, double conductance, double capacitance, double oxide, string conductUnit, string capUnit, string oxideUnit, string extraInfo, double extraValue)
        {
            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString() == site.JunctionName)
                    {
                        dataGridView1.Rows[i].Cells[3].Value = ToEngineeringNotation(conductance) + conductUnit;
                        dataGridView1.Rows[i].Cells[4].Value = ToEngineeringNotation(capacitance) + capUnit;
                        dataGridView1.Rows[i].Cells[5].Value = ToEngineeringNotation(oxide) + oxideUnit;
                        dataGridView1.Rows[i].Cells[6].Value = extraInfo+ ":"+ ToEngineeringNotation(extraValue) ;
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
                    var newPoint = new ProbeSite { JunctionName = "UNK" + ExistingData.Count, Orientation = "Horizontal", Function = "IVC", Position = point };
                    ExistingData.Add(newPoint);
                    addData.Add(newPoint);
                }
                else
                    addData.Add(hits);
            }

            foreach (var row in addData)
            {
                dataGridView1.Rows.Add(row.JunctionName, row.Position.X + "," + row.Position.Y, row.Function, 0, 0, 0, row.TopWidth, row.BottomWidth);
            }

            GetFirstCorner();
        }

        public ProbeSite[] GetListData()
        {
            var sOrientation = TestFunction.ToString().ToLower();
            var sites = new List<ProbeSite>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null)
                {
                    // double w = double.Parse(dataGridView1.Rows[i].Cells[6].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[6].Value.ToString());
                    double w = 1;
                    double h = double.Parse(dataGridView1.Rows[i].Cells[7].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[7].Value.ToString());
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Function = (string)dataGridView1.Rows[i].Cells[2].Value,
                        Area = w * h,
                        BottomWidth = w,
                        TopWidth = h
                    });
                }
            }

            return sites.OrderBy(x => x.Position.Y * 10000 + x.Position.X / 100).ToArray();
        }

        //public ProbeSite[] GetListData(ProbeOrientation Orientation)
        //{
        //    var sOrientation = Orientation.ToString().ToLower();
        //    var sites = new List<ProbeSite>();
        //    for (int i = 0; i < dataGridView1.Rows.Count; i++)
        //    {
        //        if (dataGridView1.Rows[i].Cells[2].Value != null )
        //        {
        //            double w = 0;
        //            if (dataGridView1.Rows[i].Cells[6].Value.GetType() == typeof(string))
        //                w = double.Parse(dataGridView1.Rows[i].Cells[6].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[6].Value.ToString());
        //            else
        //                w = (double)dataGridView1.Rows[i].Cells[6].Value;

        //            double h = 0;
        //            if (dataGridView1.Rows[i].Cells[7].Value.GetType() == typeof(string))
        //                h = double.Parse(dataGridView1.Rows[i].Cells[7].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[6].Value.ToString());
        //            else
        //                h = (double)dataGridView1.Rows[i].Cells[7].Value;
        //            sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
        //            {
        //                JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
        //                Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
        //                Area = w * h,
        //                BottomWidth = w,
        //                TopWidth = h
        //            });
        //        }
        //    }

        //    return sites.OrderBy(x => x.Position.Y * 10000 + x.Position.X / 100).ToArray();
        //}
        public void SetTestFunctions(string[] functionNames)
        {
            TestFunction.Items.Clear();
            TestFunction.Items.AddRange(functionNames);
        }

        public void SetTestFunctions(string functionNames)
        {
            TestFunction.Items.Clear();
            TestFunction.Items.AddRange(functionNames.Split(new string[] { "\n", "\r", " " }, StringSplitOptions.RemoveEmptyEntries));
        }
        public int[] GetFirstCorner()
        {

            var sites = new List<ProbeSite>();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null)
                {
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Function = (string)dataGridView1.Rows[i].Cells[2].Value,
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

        public int[] GetSecondCorner()
        {

            var sites = new List<ProbeSite>();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null)
                {
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Function = (string)dataGridView1.Rows[i].Cells[2].Value,
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

        public int[] GetThirdCorner()
        {
            var sOrientation = TestFunction.ToString().ToLower();
            var sites = new List<ProbeSite>();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value != null)
                {
                    sites.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Function = (string)dataGridView1.Rows[i].Cells[2].Value,
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

                    double w = 1;
                    //if (dataGridView1.Rows[i].Cells[6].Value.GetType() == typeof(string))
                    //    w = double.Parse(dataGridView1.Rows[i].Cells[6].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[6].Value.ToString());
                    //else
                    //    w = (double)dataGridView1.Rows[i].Cells[6].Value;

                    double h = 0;
                    if (dataGridView1.Rows[i].Cells[7].Value.GetType() == typeof(string))
                        h = double.Parse(dataGridView1.Rows[i].Cells[7].Value == null ? "0" : "0" + dataGridView1.Rows[i].Cells[6].Value.ToString());
                    else
                        h = (double)dataGridView1.Rows[i].Cells[7].Value;

                    var t = dataGridView1.Rows[i].Cells[2];

                    var ps = new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Function = (string)t.Value,
                        TopWidth = w,
                        BottomWidth = h,
                        Area = w * h
                    };
                    addData.Add(ps);
                    // dataGridView1.Rows[i].Cells[8].Value = ps.TopWidth * ps.BottomWidth;
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
                    hits.Function = point.Function;
                    hits.BottomWidth = point.BottomWidth;
                    hits.TopWidth = point.TopWidth;
                    hits.Area = point.Area;
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

        public event EventHandler RowClicked;

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var i = e.RowIndex;

            if (string.IsNullOrWhiteSpace((string)dataGridView1.Rows[i].Cells[1].Value) == false)
            {
                var selected = new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                {
                    JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                    Function = (string)dataGridView1.Rows[i].Cells[2].Value,
                };

                Canvas.SetMarker(selected.Position);
            }


        }


        public int[,] AlignmentProjection(int[,] dxfPoints, int[,] plattenPoints, int[,] padPoints)
        {
            Accord.DoublePoint[] points = new Accord.DoublePoint[dxfPoints.GetLength(0)];
            Accord.DoublePoint[] targetPoints = new Accord.DoublePoint[plattenPoints.GetLength(0)];
            for (int i = 0; i < dxfPoints.GetLength(0); i++)
            {
                points[i] = new Accord.IntPoint(dxfPoints[i, 0], dxfPoints[i, 1]);
                targetPoints[i] = new Accord.IntPoint(plattenPoints[i, 0], plattenPoints[i, 1]);
            }

            if (points.Length == 2)
            {
                var line = points[1] - points[0];

                var targetLine = targetPoints[1] - targetPoints[0];

                var reScale = targetLine.EuclideanNorm() / line.EuclideanNorm();

                var angleChange = Math.Atan2(targetLine.Y, targetLine.X) - Math.Atan2(line.Y, line.X);

                int[,] results = new int[padPoints.GetLength(0), 3];


                var slopeDir = targetLine / targetLine.EuclideanNorm();
                var d = (targetPoints[1].X - targetPoints[0].X) * slopeDir.X + (targetPoints[1].Y - targetPoints[0].Y) * slopeDir.Y;
                var slope = (plattenPoints[1, 2] - plattenPoints[0, 2]) / d;
                var b = plattenPoints[0, 2];



                var C = Math.Cos(angleChange);
                var S = Math.Sin(angleChange);
                for (int i = 0; i < padPoints.GetLength(0); i++)
                {

                    var x = padPoints[i, 0] - points[0].X;
                    var y = padPoints[i, 1] - points[0].Y;
                    var xC = (x * C - y * S) * reScale + targetPoints[0].X;
                    var yC = (x * S + y * C) * reScale + targetPoints[0].Y;

                    d = (xC - targetPoints[0].X) * slopeDir.X + (yC - targetPoints[0].Y) * slopeDir.Y;


                    results[i, 0] = (int)xC;
                    results[i, 1] = (int)yC;
                    results[i, 2] = (int)(d * slope + b);
                }
                return results;

            }
            if (points.Length == 3)
            {
                var line1 = points[1] - points[0];
                var line2 = points[2] - points[0];

                var targetLine1 = targetPoints[1] - targetPoints[0];
                var targetLine2 = targetPoints[2] - targetPoints[0];

                var reScale1 = targetLine1.EuclideanNorm() / line1.EuclideanNorm();
                var reScale2 = targetLine2.EuclideanNorm() / line2.EuclideanNorm();

                var angleChange1 = Math.Atan2(targetLine1.Y, targetLine1.X) - Math.Atan2(line1.Y, line1.X);
                var angleChange2 = Math.Atan2(targetLine2.Y, targetLine2.X) - Math.Atan2(line2.Y, line2.X);
                if (angleChange1 < 0) angleChange1 = 6.28 + angleChange1;
                if (angleChange2 < 0) angleChange2 = 6.28 + angleChange2;

                if (angleChange1 > 5 && angleChange2 < 3)
                    angleChange2 += 6.28;
                if (angleChange2 > 5 && angleChange1 < 3)
                    angleChange1 += 6.28;


                var angleChange = ((angleChange1 + angleChange2) / 2) % 6.28;
                var reScale = (reScale1 + reScale2) / 2;



                int[,] results = new int[padPoints.GetLength(0), 3];
                float[] equ = equation_plane(plattenPoints[0, 0], plattenPoints[0, 1], plattenPoints[0, 2],
                    plattenPoints[1, 0], plattenPoints[1, 1], plattenPoints[1, 2],
                    plattenPoints[2, 0], plattenPoints[2, 1], plattenPoints[2, 2]);

                var C = Math.Cos(angleChange);
                var S = Math.Sin(angleChange);
                for (int i = 0; i < padPoints.GetLength(0); i++)
                {

                    var x = padPoints[i, 0] - points[0].X;
                    var y = padPoints[i, 1] - points[0].Y;
                    var xC = (x * C - y * S) * reScale + targetPoints[0].X;
                    var yC = (x * S + y * C) * reScale + targetPoints[0].Y;


                    results[i, 0] = (int)xC;
                    results[i, 1] = (int)yC;
                    results[i, 2] = (int)(results[i, 0] * equ[0] + results[i, 1] * equ[1] + equ[2]);
                }
                return results;

            }
            return HomographyProjection(dxfPoints, plattenPoints, padPoints);




        }

        public int[,] HomographyProjection(int[,] alignPoints, int[,] recordedPoints, int[,] padPoints)
        {
            Accord.IntPoint[] correlationPoints1 = new Accord.IntPoint[alignPoints.GetLength(0)];
            Accord.IntPoint[] correlationPoints2 = new Accord.IntPoint[recordedPoints.GetLength(0)];
            for (int i = 0; i < alignPoints.GetLength(0); i++)
            {
                correlationPoints1[i] = new Accord.IntPoint(alignPoints[i, 0], alignPoints[i, 1]);
                correlationPoints2[i] = new Accord.IntPoint(recordedPoints[i, 0], recordedPoints[i, 1]);
            }
            RansacHomographyEstimator ransac = new RansacHomographyEstimator(0.001, 0.99);
            var homography = ransac.Estimate(correlationPoints1, correlationPoints2);

            PointF[] testPoints = new PointF[padPoints.GetLength(0)];
            for (int i = 0; i < padPoints.GetLength(0); i++)
            {
                testPoints[i] = new PointF(padPoints[i, 0], padPoints[i, 1]);
            }

            var transformed = homography.TransformPoints(testPoints);



            int[,] results = new int[padPoints.GetLength(0), 3];
            float[] equ = equation_plane(recordedPoints[0, 0], recordedPoints[0, 1], recordedPoints[0, 2],
                recordedPoints[1, 0], recordedPoints[1, 1], recordedPoints[1, 2],
                recordedPoints[2, 0], recordedPoints[2, 1], recordedPoints[2, 2]
                );
            for (int i = 0; i < padPoints.GetLength(0); i++)
            {
                results[i, 0] = (int)transformed[i].X;
                results[i, 1] = (int)transformed[i].Y;
                results[i, 2] = (int)(results[i, 0] * equ[0] + results[i, 1] * equ[1] + equ[2]);
            }
            return results;
        }

        float[] equation_plane(float x1, float y1, float z1,
                               float x2, float y2, float z2,
                               float x3, float y3, float z3)
        {
            float a1 = x2 - x1;
            float b1 = y2 - y1;
            float c1 = z2 - z1;
            float a2 = x3 - x1;
            float b2 = y3 - y1;
            float c2 = z3 - z1;
            float a = b1 * c2 - b2 * c1;
            float b = a2 * c1 - a1 * c2;
            float c = a1 * b2 - b1 * a2;
            float d = (-a * x1 - b * y1 - c * z1);

            return new float[] { -1 * a / c, -1 * b / c, -1 * d / c };
            //  Console.Write("equation of plane is " + a +
            //                   "x + " + b + "y + " + c +
            //                     "z + " + d + " = 0");
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2)
            {
                _ClickedRow = e.RowIndex;
                RowClicked?.Invoke(sender, EventArgs.Empty);
            }
        }
    }
}
