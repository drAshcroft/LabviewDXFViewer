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
            dxfCanvas1.AddSiteViewer( microsites1);



           

         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var file = @"C:\Users\bashc\Downloads\2021-03-26 ORNL Single.dxf";
           // file = @"C:\Users\847695.CORPAA\Downloads\2021-04-07 Chip Pads.dxf";
            dxfCanvas1.LoadFile(file);

          var cameras=   webCamViewer1.GetCameras();

            webCamViewer1.InitializeCamera(cameras[0]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            microsites1.SaveListSites("test");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            microsites1.LoadListSites("test");
        }
    }
}
