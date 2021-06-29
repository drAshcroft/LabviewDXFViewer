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
    public partial class Joystick2 : UserControl
    {
        public Joystick2()
        {
            InitializeComponent();
        }

        public event EventHandler Latched;

        private void bUpMore_Click(object sender, EventArgs e)
        {
            UpMore = true; if (Latched != null)
                Latched(sender, e);

        }
        private bool UpMoreState = false;
        public bool UpMore
        {
            get
            {
                var t = UpMoreState;
                UpMoreState = false;
                bUpMore.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                UpMoreState = value;
                bUpMore.FlatStyle = FlatStyle.Popup;
            }
        }





        private void bZero_Click(object sender, EventArgs e)
        {
            ZeroMove = true;
            if (Latched != null)
                Latched(sender, e);
        }

        private bool ZeroState = false;
        public bool ZeroMove
        {
            get
            {
                var t = ZeroState;
                ZeroState = false;
                bZero.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                ZeroState = value;
                bZero.FlatStyle = FlatStyle.Popup;
            }
        }




        private void bUp_Click(object sender, EventArgs e)
        {
            UpMove = true;
            if (Latched != null)
                Latched(sender, e);
        }
        private bool UpState = false;
        public bool UpMove
        {
            get
            {
                var t = UpState;
                UpState = false;
                bUp.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                UpState = value;
                bUp.FlatStyle = FlatStyle.Popup;
            }
        }





        private void bRightMore_Click(object sender, EventArgs e)
        {
            RightMore = true;
            if (Latched != null)
                Latched(sender, e);
        }
        private bool RightMoreState = false;
        public bool RightMore
        {
            get
            {
                var t = RightMoreState;
                RightMoreState = false;
                bRightMore.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                RightMoreState = value;
                bRightMore.FlatStyle = FlatStyle.Popup;
            }
        }





        private void bRight_Click(object sender, EventArgs e)
        {
            RightMove = true;
            if (Latched != null)
                Latched(sender, e);
        }
        private bool RightState = false;
        public bool RightMove
        {
            get
            {
                var t = RightState;
                RightState = false;
                bRight.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                RightState = value;
                bRight.FlatStyle = FlatStyle.Popup;
            }
        }




        private void bDown_Click(object sender, EventArgs e)
        {
            DownMove = true;
            if (Latched != null)
                Latched(sender, e);
        }
        private bool DownState = false;
        public bool DownMove
        {
            get
            {
                var t = DownState;
                DownState = false;
                bDown.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                DownState = value;
                bDown.FlatStyle = FlatStyle.Popup;
            }
        }





        private void bDownMore_Click(object sender, EventArgs e)
        {
            DownMore = true;
            if (Latched != null)
                Latched(sender, e);
        }
        private bool DownMoreState = false;
        public bool DownMore
        {
            get
            {
                var t = DownMoreState;
                DownMoreState = false;
                bDownMore.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                DownMoreState = value;
                bDownMore.FlatStyle = FlatStyle.Popup;
            }
        }







        private void bLeft_Click(object sender, EventArgs e)
        {
            LeftMove = true;
            if (Latched != null)
                Latched(sender, e);
        }
        private bool LeftState = false;
        public bool LeftMove
        {
            get
            {
                var t = LeftState;
                LeftState = false;
                bLeft.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                LeftState = value;
                bLeft.FlatStyle = FlatStyle.Popup;
            }
        }







        private void bLeftMore_Click(object sender, EventArgs e)
        {
            LeftMore = true;
            if (Latched != null)
                Latched(sender, e);
        }
        private bool LeftMoreState = false;
        public bool LeftMore
        {
            get
            {
                var t = LeftMoreState;
                LeftMoreState = false;
                bLeftMore.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                LeftMoreState = value;
                bLeftMore.FlatStyle = FlatStyle.Popup;
            }
        }







        private void bHome_Click(object sender, EventArgs e)
        {
            HomeMove = true;
            if (Latched != null)
                Latched(sender, e);
        }
        private bool HomeState = false;
        public bool HomeMove
        {
            get
            {
                var t = HomeState;
                HomeState = false;
                bHome.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                HomeState = value;
                bHome.FlatStyle = FlatStyle.Popup;
            }
        }







        private void bLoad_Click(object sender, EventArgs e)
        {
            LoadMove = true;
            if (Latched != null)
                Latched(sender, e);
        }

        private bool LoadState = false;
        public bool LoadMove
        {
            get
            {
                var t = LoadState;
                LoadState = false;
                bLoad.FlatStyle = FlatStyle.Standard;
                return t;
            }
            set
            {
                LoadState = value;
                bLoad.FlatStyle = FlatStyle.Popup;
            }
        }



    }
}
