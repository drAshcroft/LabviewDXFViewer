using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabviewDXFViewer.DataTypes
{
    public class ProbeSite
    {
        public string JunctionName { get; set; }
        public string Orientation { get; set; }
        public Point Position { get; set; }

        public double Area { get; set; }
        public ProbeSite() { }
        public ProbeSite(string position)
        {
            var parts = position.Trim().Split(',');
            Position = new Point(int.Parse(parts[0]), int.Parse(parts[1]));
        }
    }

    public enum ProbeOrientation:int
    {
        Horizontal=0, Vertical=1
    }
}
