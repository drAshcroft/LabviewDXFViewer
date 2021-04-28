using System;
using System.Drawing;
using System.Windows.Forms;

namespace LabviewDXFViewer
{
    public partial class DXFCanvas : UserControl
    {

        private Microsites Microsites;
        public void AddSiteViewer(Microsites microsites)
        {
            microsites.Canvas = this;
            Microsites = microsites;
        }
        DXFView Viewer;
        public DXFCanvas()
        {
            InitializeComponent();

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            if (Viewer != null)
                Viewer.Draw(pictureBox1);
        }

        public void LoadFile(string filename)
        {
            Viewer = new DXFView();
            Viewer.LoadFile(filename);

            lbLayerSelect.Items.Clear();
            lbLayerSelect.Items.AddRange(Viewer.Layers);

            for (int i = 0; i < lbLayerSelect.Items.Count; i++)
                lbLayerSelect.SetItemChecked(i, true);

            Viewer.Draw(pictureBox1);
        }

        public void AddListSite(int x, int y)
        {
            var location = new System.Drawing.Point(x, y);
            if (Viewer != null && Viewer.HilightWaferPoint(location))
            {
                Viewer.Draw(pictureBox1);
                if (Microsites != null)
                    Microsites.AddListData(Viewer.SelectedLocations);
            }
        }

        private void lbLayerSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var item in lbLayerSelect.Items)
                ((DXFLayer)item).Visible = false;
            foreach (var item in lbLayerSelect.CheckedItems)
                ((DXFLayer)item).Visible = true;

            Viewer.Draw(pictureBox1);
        }

        public void LoadLayerActivation(string activeLayers)
        {
            for (var i = 0; i < lbLayerSelect.Items.Count; i++)
            {
                var checkedV = (activeLayers.Contains(lbLayerSelect.Items[i].ToString()));
                ((DXFLayer)lbLayerSelect.Items[i]).Visible = checkedV;
                lbLayerSelect.SetItemChecked(i, checkedV);
            }

            if (Viewer != null)
                Viewer.Draw(pictureBox1);
        }

        public string SaveLayerActivation()
        {
            var outString = "";
            for (var i = 0; i < lbLayerSelect.CheckedItems.Count; i++)
            {
                outString += lbLayerSelect.CheckedItems[i].ToString() + " ";
            }
            return outString;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {

                if (Viewer != null && Viewer.Hilight(e.Location, cbSelectionMirror.Checked, cbSelectionRotate.Checked))
                {
                    Viewer.Draw(pictureBox1);
                    if (Microsites != null)
                        Microsites.AddListData(Viewer.SelectedLocations);
                }
            }
            catch { }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
