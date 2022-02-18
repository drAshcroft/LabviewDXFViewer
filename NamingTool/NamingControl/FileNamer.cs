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
    public partial class FileNamer : UserControl
    {
        public FileNamer()
        {
            InitializeComponent();
            timer1.Tick += Timer1_Tick;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            CheckFile();
        }

        public bool IVCurves
        {
            get; set;
        } = false;

        public int GetFileNumber(string filename)
        {
            var file = Path.GetFileNameWithoutExtension(filename).Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries).Last();
            int num = 0;
            int.TryParse(file, out num);
            return num;
        }

        public int GetFileIndexNumber
        {
            get
            {
                return Index;
            }
        }

        public string GetBaseFileName
        {
            get
            {
                return mFilename;
            }
        }
        int Index = 0;
        public bool CheckFile()
        {
            timer1.Enabled = false;
            timer1.Stop();
            Index = 0;
            if (mFilename != "")
            {
                try
                {
                    var dir = Path.GetDirectoryName(mFilename);
                    if (Directory.Exists(dir))
                    {
                        var files = Directory.GetFiles(dir, Path.GetFileNameWithoutExtension(mFilename) + "_*" + FileExtention);
                        var exists = files.Length > 0;
                        if (exists)
                        {
                            int nMax = 0;
                            foreach (var file in files)
                            {
                                var numberS = file.Replace(mFilename + "_", "").Split('.')[0];
                                int num = 0;
                                int.TryParse(numberS, out num);
                                if (num > nMax)
                                    nMax = num;
                            }
                            Index = (nMax + 1);

                            bIndicate.BackColor = Color.Red;
                            textBox1.Text = mFilename + "_" + (nMax + 1) + FileExtention;
                            return false;
                        }
                    }
                }
                catch
                { }
            }
            bIndicate.BackColor = Color.Green;
            textBox1.Text = mFilename + "_0" + FileExtention;
            return true;
        }
        public string FileExtention { get; set; } = ".tdms";
        private string mFilename = "";
        public string Filename
        {
            get
            {
                timer1.Enabled = false;
                timer1.Stop();
                timer1.Interval = 500;
                timer1.Enabled = true;
                timer1.Start();
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        public void SetFilename(string filename)
        {
            mFilename = filename;
            timer1.Enabled = false;
            timer1.Stop();
            timer1.Interval = 700;
            timer1.Enabled = true;
            timer1.Start();

            textBox1.Text = filename;
        }

    }
}
