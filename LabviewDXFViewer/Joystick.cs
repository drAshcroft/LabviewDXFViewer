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
    public partial class Joystick : UserControl
    {
        public Joystick()
        {
            InitializeComponent();
            try
            {
                isMouseDown = false;
                CurrentJoystickPosition = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
            }
            catch { }
            RequestChange = new Point(0, 0);

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //  e.Graphics.DrawLine(
            //new Pen(Color.Red, 2f),
            //new Point(0, 0),
            //new Point(pictureBox1.Size.Width, pictureBox1.Size.Height));
            try
            {

                e.Graphics.FillEllipse(Brushes.Red, CurrentJoystickPosition.X - 15, CurrentJoystickPosition.Y - 15, 30, 30);
            }
            catch { }
        }

        public Point GetChangePosition ()
        {
            var request = RequestChange;
            return RequestChange;
        }
        private bool isMouseDown = false;
        private Point RequestChange;

        private Point CurrentJoystickPosition;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                RequestChange = new Point(e.X - pictureBox1.Width / 2, e.Y - pictureBox1.Height / 2);
                CurrentJoystickPosition = new Point(e.X, e.Y);
                pictureBox1.Invalidate();
               
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            CurrentJoystickPosition = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
            RequestChange = new Point(0, 0);
            pictureBox1.Invalidate();
        }
    }
}
