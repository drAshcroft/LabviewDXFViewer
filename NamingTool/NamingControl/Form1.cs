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

namespace NamingControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            namingBox1.onFilenameUpdated += NamingBox1_onFilenameUpdated;

            namingBox1.FileBoxRT = fileNamer1;
            namingBox1.FileBoxIV = fileNamer2;

            fileNamer1.GetFileNumber(@"C:\DataStore\W007\D1\Device48_C-1mMPB_B-100mV_R-0mV__RT_0.tdms" );
        }

        private void NamingBox1_onFilenameUpdated(string ivFilename, string rtFilename)
        {
            textBox1.Text = ivFilename;
            textBox2.Text = rtFilename;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            var mat = new MathClass();
            var data = File.ReadAllText(@"c:\my_data5.csv").Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            double[,] dataGrid=null;
            for (int i=0;i<data.Length;i++)
            {
                var line = data[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (i == 0)
                    dataGrid = new double[data.Length, line.Length];
                for (int j = 0; j < line.Length; j++)
                    dataGrid[i, j] = double.Parse(line[j]);
            }
            mat.CleanIVCurves(dataGrid, .1,1, 20000,-8);
        }
    }
}
