using Accord.Video.DirectShow;
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
    public partial class WebCamViewer : UserControl, IDisposable
    {

        public void StopCamera()
        {
            try
            {
                VideoSource.Stop();
                videoSourcePlayer.SignalToStop();
                VideoSource = null;
            }
            catch { }
        }

        public new void Dispose()
        {
            try
            {
                VideoSource.Stop();
                videoSourcePlayer.SignalToStop();
                VideoSource = null;
            }
            catch { }
            base.Dispose();
        }

        public WebCamViewer()
        {
            InitializeComponent();
        }


        FilterInfoCollection VideoDevices = null;
        VideoCaptureDevice VideoSource;

        public void InitializeCamera(string cameraName)
        {
            if (VideoDevices == null)
                GetCameras();

            var selected = VideoDevices.Where(x => x.Name == cameraName).FirstOrDefault().MonikerString;
             VideoSource = new VideoCaptureDevice(selected);

            // Then, we just have to define what we would like to do once the device send 
            // us a new frame. This can be done using standard .NET events (the actual 
            // contents of the video_NewFrame method is shown at the bottom of this page)
            videoSourcePlayer.VideoSource = VideoSource;
            videoSourcePlayer.Start();

        }

        Accord.Imaging.Filters.Mirror filter = new Accord.Imaging.Filters.Mirror(true, true);

        bool _SaveFrame = false;
        string _FrameFile = "";
        private void VideoSource_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            eventArgs.Frame= filter.Apply(eventArgs.Frame);

            if (_SaveFrame)
            {
                try
                {
                    _SaveFrame = false;
                    eventArgs.Frame.Save(_FrameFile);
                }
                catch { }
            }
        }
        private void VideoSourcePlayer_DoubleClick(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
            
        }
        public void SaveFrame(string frameName)
        {
            _FrameFile = frameName;
            _SaveFrame = true;
        }
        public void ShowFrame (string frameName)
        {
            pictureBox1.Image = Bitmap.FromFile(frameName);
            videoSourcePlayer.Stop();
            button1.Visible = true;
            pictureBox1.Visible = true;
            videoSourcePlayer.Visible = false;
        }

        public string[] GetCameras()
        {
            VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            return VideoDevices.Select(x => x.Name).ToArray();
        }


        public void StartVideo()
        {
            videoSourcePlayer.Visible = true;
            pictureBox1.Image = new Bitmap(10, 10);
            pictureBox1.Visible = false;
            button1.Visible = false;
            videoSourcePlayer.Start();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            StartVideo();

        }
    }
}
