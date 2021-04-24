using LabviewDXFViewer.DataTypes;
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
    public partial class Microsites : UserControl
    {
        public Microsites()
        {
            InitializeComponent();

            deleteEvent = DeleteRow;
        }



        public List<ProbeSite> Rows = new List<ProbeSite>();
        public void AddListData(Point[] selectedLocations)
        {
            Rows.Clear();

            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                if (dataGridView1.Rows[i].IsNewRow == false)
                    dataGridView1.Rows.RemoveAt(i);
            }

            var addData = new List<ProbeSite>();
            foreach (var point in selectedLocations)
            {
                var hits = ExistingData.Where(x => Math.Abs(point.X - x.Position.X) < 10 && Math.Abs(point.Y - x.Position.Y) < 10).FirstOrDefault();
                if (hits == null)
                {
                    var newPoint = new ProbeSite { JunctionName = "UNK", Orientation = "Horizontal", Position = point };
                    ExistingData.Add(newPoint);
                    addData.Add(newPoint);
                }
                else
                    addData.Add(hits);
            }

            foreach (var row in addData)
            {
                dataGridView1.Rows.Add(row.JunctionName, row.Position.X + "," + row.Position.Y, row.Orientation);
            }
        }



        public ProbeSite[] GetListData(ProbeOrientation Orientation)
        {
            var sOrientation = Orientation.ToString().ToLower();
            var sites = new ProbeSite[dataGridView1.Rows.Count];
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value.ToString().ToLower() == sOrientation)
                {
                    sites[i] = new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                    };
                }
            }

            sites = sites.OrderBy(x => x.Position.Y * 10000 + x.Position.X/100).ToArray();

            return sites;
        }



        private int MouseOverRow;
        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Delete Selected Rows", deleteEvent));
                MouseOverRow = e.RowIndex;
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        EventHandler deleteEvent;

        private void DeleteRow(object sender, EventArgs e)
        {
            var selected = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                selected.Add(row);
            }
            selected = selected.OrderByDescending(x => x.Index).ToList();
            foreach (var row in selected)
                dataGridView1.Rows.RemoveAt(row.Index);
        }

        List<ProbeSite> ExistingData = new List<ProbeSite>();
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var addData = new List<ProbeSite>();
            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace((string)dataGridView1.Rows[i].Cells[1].Value) == false)
                {
                    addData.Add(new ProbeSite((string)dataGridView1.Rows[i].Cells[1].Value)
                    {
                        JunctionName = (string)dataGridView1.Rows[i].Cells[0].Value,
                        Orientation = (string)dataGridView1.Rows[i].Cells[2].Value,
                    });
                }
            }

            foreach (var point in addData)
            {
                var hits = ExistingData.Where(x => Math.Abs(point.Position.X - x.Position.X) < 10 && Math.Abs(point.Position.Y - x.Position.Y) < 10).FirstOrDefault();
                if (hits == null)
                {
                    ExistingData.Add(point);
                }
                else
                {
                    hits.JunctionName = point.JunctionName;
                    hits.Orientation = point.Orientation;
                }
            }
        }
    }
}
