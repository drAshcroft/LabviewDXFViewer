using System;
using System.Windows.Forms;

namespace LabviewDXFViewer
{
    
    public partial class ImageButton : UserControl
    {
        public ImageButton()
        {
            InitializeComponent();
        }
        public event EventHandler Latched;

        public void SetToolTip(string toolTip)
        {
            toolTip1.SetToolTip(this.bLeft, toolTip);
        }

        public void SetImage(ImageOptionsEnum image)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageButton));
            switch (image)
            {

                case ImageOptionsEnum.Left:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("left")));
                   
                    break;

                case ImageOptionsEnum.LeftBig:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("leftB")));
                    break;

                case ImageOptionsEnum.LeftSmall:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("leftS")));
                    break;

                case ImageOptionsEnum.Right:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("right")));
                    break;

                case ImageOptionsEnum.RightBig:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rightB")));
                    break;

                case ImageOptionsEnum.RightSmall:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rightS")));
                    break;

                case ImageOptionsEnum.Up:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("up")));
                    break;

                case ImageOptionsEnum.UpBig:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("upB")));
                    break;

                case ImageOptionsEnum.UpSmall:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("upS")));
                    break;

                case ImageOptionsEnum.Down:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("down")));
                    
                    break;

                case ImageOptionsEnum.DownBig:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("downB")));
                    break;

                case ImageOptionsEnum.DownSmall:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("downS")));
                    break;

                case ImageOptionsEnum.Zero:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("zero")));
                    break;

                case ImageOptionsEnum.Play:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("play")));
                    break;

                case ImageOptionsEnum.Pause:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pause")));
                    break;

                case ImageOptionsEnum.Load:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("load")));
                    break;

                case ImageOptionsEnum.Lower:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lower")));
                    break;

                case ImageOptionsEnum.Raise:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("raise")));
                    break;

                case ImageOptionsEnum.Home:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("home")));
                    break;

                case ImageOptionsEnum.Home2:
                    this.bLeft.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("home2")));
                    break;


            }

        }
        private void bLeft_Click(object sender, EventArgs e)
        {
            Clicked = true;
            if (Latched != null)
                Latched(sender, e);

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

    public enum ImageOptionsEnum
    {
        Left, LeftBig, LeftSmall,
        Right, RightBig, RightSmall,
        Up, UpBig, UpSmall,
        Down, DownBig, DownSmall,
        Zero, Play, Pause, Load,
        Lower, Raise, Home, Home2
    }
}
