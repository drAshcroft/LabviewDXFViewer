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
    public partial class DownButton : UserControl
    {
        public DownButton()
        {
            InitializeComponent();
        }



        private void bLeft_Click(object sender, EventArgs e)
        {
            Clicked = true;
        }
        private bool ClickedState = false;
        public bool Clicked
        {
            get
            {
                var t = ClickedState;
                ClickedState = false;
                bLeft.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                ClickedState = value;
                bLeft.FlatStyle = FlatStyle.Popup;
            }
        }



    }
}
