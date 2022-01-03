using netDxf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabviewDXFViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dxfCanvas1.AddSiteViewer(microsites1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
                openFileDialog1.Title = "Open DXF file";
                openFileDialog1.DefaultExt = "txt";
                openFileDialog1.Filter = "dxf files (*.dxf)|*.dxf";

                if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
                {
                    // file = @"C:\Users\847695.CORPAA\Downloads\2021-04-07 Chip Pads.dxf";
                    dxfCanvas1.LoadFile(openFileDialog1.FileName);
                }
            }
            catch
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                microsites1.SaveListSitesCloudO(comboBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var dxf = File.ReadAllText(@"E:\WaferPlans\DXFFiles\FGmask-N1.dxf");
                var json = File.ReadAllText(@"E:\WaferPlans\FGmask-N1.json");

                microsites1.LoadListSitesLV("FGmask-N1", dxf, json);// comboBox1.Text);

                microsites1.GetSecondCorner();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static string pass="";
        private void Form1_Load(object sender, EventArgs e)
        {
            //var sites=
            //microsites1.AlignmentProjection(
            //    new int[2, 2] { { -5625, -7000 }, { 4875, -6999 } },//, { -5624, 7000 } },
            //    new int[3, 3] { { 39965, 34317,7883 }, { 33360, 34316,6625 }, { 39964, 25511,5367 } },
            //    new int[4,2] { {-5625,-7000 },{4875,-6999 },{-5624,7000 }, { 4875, 7000 } });


            //var sites2 =
            //   microsites1.AlignmentProjection(
            //       new int[3, 2] { { -1050, -625 }, { 300, -625 } , { -1050, 625 } },
            //       new int[3, 3] { { 36947, 30348, 8702 }, { 36035, 30348, 8702 }, { 36947, 29562, 8701 } },
            //       new int[3, 2] { { -1050, -625 }, { 300, -625 }, { -1050, 625 } });

            //try
            //{
            //    pass = File.ReadAllText(".\\settings.txt");
            //    comboBox1.Items.AddRange(microsites1.LoadWaferPlansCloud());
            //}
            //catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            microsites1.SetTestFunctions(new string[] { "1", "2", "3" });
        }
    }
}
