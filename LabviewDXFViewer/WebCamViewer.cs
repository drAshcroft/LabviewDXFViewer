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
                videoSourcePlayer.SignalToStop();
            }
            catch { }
        }

        public new void Dispose()
        {
            try
            {
                videoSourcePlayer.SignalToStop();
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

        private void VideoSource_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            //  pictureBox1.Image = eventArgs.Frame;
        }
        private void VideoSourcePlayer_DoubleClick(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetCameras()
        {
            VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            return VideoDevices.Select(x => x.Name).ToArray();
        }
    }
}
