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
                microsites1.LoadListSitesCloud(comboBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static string pass="";
        private void Form1_Load(object sender, EventArgs e)
        {
            var sites=
            microsites1.HomographyProjection(
                new int[3, 2] { { -5625, -7000 }, { 4875, -6999 }, { -5624, 7000 } },
                new int[3, 2] { { 39769, 34322 }, { 33164, 34321 }, { 39767, 25516 } },
                new int[3,2] { {-5625,-7000 },{4875,-6999 },{-5624,7000 } });


            sites =
            microsites1.HomographyProjection(
                new int[2, 2] { { -5625, -7000 }, { 4875, -6999 } },
                new int[2, 2] { { 39769, 34322 }, { 33164, 34321 } },
                new int[3, 2] { { -5625, -7000 }, { 4875, -6999 }, { -5624, 7000 } });

            try
            {
                pass = File.ReadAllText(".\\settings.txt");
                comboBox1.Items.AddRange(microsites1.LoadWaferPlansCloud());
            }
            catch { }
        }

     
    }
}
