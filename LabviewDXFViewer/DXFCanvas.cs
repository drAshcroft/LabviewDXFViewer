using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using netDxf;
using DXFImporter;
using netDxf.Entities;

namespace LabviewDXFViewer
{
    public partial class DXFCanvas : UserControl
    {

        public Microsites Microsites;
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

        private void lbLayerSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var item in lbLayerSelect.Items)
                ((DXFLayer)item).Visible = false;
            foreach (var item in lbLayerSelect.CheckedItems)
                ((DXFLayer)item).Visible = true;

            Viewer.Draw(pictureBox1);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Viewer.Hilight(e.Location, cbSelectionMirror.Checked, cbSelectionRotate.Checked))
            {
                Viewer.Draw(pictureBox1);
                if (Microsites != null)
                    Microsites.ListData(Viewer.SelectedLocations);
            }
            
        }
    }
}
