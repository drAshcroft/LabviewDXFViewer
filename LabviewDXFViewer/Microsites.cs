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

        public class JunctionRow
        {
            public string JunctionName { get; set; }
            public string Orientation { get; set; }
            public Point Position { get; set; }
            public JunctionRow() { }
            public JunctionRow(string position)
            {
                var parts = position.Trim().Split(',');
                Position = new Point( int.Parse( parts[0]), int.Parse(parts[1]));
            }
        }

        public List<JunctionRow> Rows = new List<JunctionRow>();
        public void ListData(Point[] selectedLocations)
        {
            Rows.Clear();

            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                if (dataGridView1.Rows[i].IsNewRow==false)
                    dataGridView1.Rows.RemoveAt(i);
            }
            
            var addData = new List<JunctionRow>();
            foreach (var point in selectedLocations)
            {
                var hits = ExistingData.Where(x => Math.Abs(point.X - x.Position.X) < 10 && Math.Abs(point.Y - x.Position.Y) < 10).FirstOrDefault();
                if (hits == null)
                {
                    var newPoint = new JunctionRow { JunctionName = "UNK", Orientation = "Horizontal", Position = point };
                    ExistingData.Add(newPoint);
                    addData.Add(newPoint);
                }
                else
                    addData.Add(hits);
            }

            foreach (var row in addData)
            {
                dataGridView1.Rows.Add(row.JunctionName, row.Position.X  + "," + row.Position.Y, row.Orientation);
            }
        }


        private int MouseOverRow;
        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right )
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Delete Selected Rows", deleteEvent));
                MouseOverRow = e.RowIndex;
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        EventHandler deleteEvent;

        private void DeleteRow(object sender,EventArgs e)
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

        List<JunctionRow> ExistingData = new List<JunctionRow>();
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var addData = new List<JunctionRow>();
            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace((string)dataGridView1.Rows[i].Cells[1].Value) == false)
                {
                    addData.Add(new JunctionRow((string)dataGridView1.Rows[i].Cells[1].Value)
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
