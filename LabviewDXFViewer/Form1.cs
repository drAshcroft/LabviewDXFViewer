﻿using netDxf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var file = @"C:\Users\847695.CORPAA\Downloads\2021-03-26 ORNL Single.dxf";
           // file = @"C:\Users\847695.CORPAA\Downloads\2021-04-07 Chip Pads.dxf";
            dxfCanvas1.LoadFile(file);
        }
    }
}