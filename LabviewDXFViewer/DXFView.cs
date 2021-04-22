using DXFImporter;
using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabviewDXFViewer
{
    public class DXFLayer
    {
        public string LayerName { get; set; }
        public bool Visible { get; set; }

        public Color color { get; set; }

        public override string ToString()
        {
            return LayerName;
        }
    }

    public class DXFView
    {
        public DXFLayer[] Layers { get; set; }
        public void LoadFile(string filename)
        {
            DxfDocument loaded = DxfDocument.Load(filename);
            Layers = loaded.Layers.OrderBy(x => x.Name).Select(x => new DXFLayer { LayerName = x.Name, Visible = true, color = x.Color.ToColor() }).ToArray();


            Shapes = new List<DXFShape>();

            foreach (var block in loaded.Blocks)
            {
                foreach (var shape in block.Entities)
                {
                    switch (shape.Type)
                    {
                        case EntityType.Polyline:
                            Shapes.Add(new DXFPolyline(shape.Color.ToColor(), 1, shape.Layer.Name));
                            break;
                        case EntityType.LightWeightPolyline:

                            Color color = shape.Color.ToColor();
                            if (shape.Color.IsByLayer)
                                color = shape.Layer.Color.ToColor();
                            var pl = new DXFPolyline(color, 1, shape.Layer.Name);
                            var specificShape = (LwPolyline)shape;
                            for (int i = 1; i < specificShape.Vertexes.Count; i++)
                            {
                                pl.AppendLine(new DFXLine(
                                     new System.Drawing.Point((int)specificShape.Vertexes[i - 1].Position.X, (int)specificShape.Vertexes[i - 1].Position.Y),
                                     new System.Drawing.Point((int)specificShape.Vertexes[i].Position.X, (int)specificShape.Vertexes[i].Position.Y),
                                     color,
                                     (int)specificShape.Vertexes[i].StartWidth)
                                    );
                            }
                            Shapes.Add(pl);
                            break;
                        case EntityType.Hatch:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public List<DXFShape> Shapes = new List<DXFShape>();

        private double XMax = double.MinValue, XMin = double.MaxValue;
        private double YMax = double.MinValue, YMin = double.MaxValue;

        private double scaleX = 1;
        private double scaleY = 1;
        private double mainScale = 1;


        private void ScaleImage(int pbWidth, int pbHeight)
        {
            foreach (var shape in Shapes)
            {
                if (shape.Bounds.Right > XMax)
                    XMax = shape.Bounds.Right;

                if (shape.Bounds.Left < XMin)
                    XMin = shape.Bounds.Left;

                if (shape.Bounds.Bottom > YMax)
                    YMax = shape.Bounds.Bottom;

                if (shape.Bounds.Top < YMin)
                    YMin = shape.Bounds.Top;
            }



            scaleX = (double)(pbWidth) / (double)XMax * .5;

            scaleY = (double)(pbHeight) / (double)YMax * .5;

            mainScale = Math.Min(scaleX, scaleY);
        }

        public void Draw(PictureBox pictureBox1)
        {
            ScaleImage(pictureBox1.Image.Width, pictureBox1.Image.Height);

            var drawList = Layers.Where(x => x.Visible).Select(x => x.LayerName).ToList();
            var colors = Enum.GetNames(typeof(KnownColor)).Where(item => !item.StartsWith("Control")).OrderBy(item => item).Select(x => Color.FromName(x)).ToArray();

            using (var g = Graphics.FromImage(pictureBox1.Image))
            {

                Rectangle rect = new Rectangle(0, 0, pictureBox1.Size.Width, pictureBox1.Size.Height);


                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                                                                                                                rect,
                                                                                                                Color.Black,
                                                                                                                Color.Black,
                                                                                                                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
                g.FillRectangle(brush, rect);

                Pen lePen = new Pen(Color.White, 3);

                int cc = 0;
                foreach (var obj in Shapes)                     //iterates through the objects
                {
                    DXFPolyline temp = (DXFPolyline)obj;
                    if (drawList.Contains(temp.LayerIndex))
                    {

                        if (temp.AccessContourColor.Name == "White")
                            lePen.Color = colors[cc];
                        else
                            lePen.Color = temp.AccessContourColor;
                        lePen.Width = (float)Math.Max(1, temp.AccessLineWidth * mainScale);


                        if (mainScale == 0)
                            mainScale = 1;

                        temp.Draw(lePen, g, mainScale, new System.Drawing.Point((int)XMin, (int)YMin));
                        cc++;
                    }
                }

                lePen.Dispose();
                brush.Dispose();
            }


            pictureBox1.Invalidate();
        }

        public System.Drawing.Point[] SelectedLocations
        {
            get
            {
                List<System.Drawing.Point> selected = new List<System.Drawing.Point>();
                foreach (var obj in Shapes)                     //iterates through the objects
                {
                    var selec = obj.SelectedLocations;
                    if (selec != null)
                    {
                        foreach (var s in selec)
                            if (s != null)
                                selected.Add(s);
                    }
                }
                return selected.ToArray();
            }
        }
        public bool Hilight(System.Drawing.Point picPoint, bool mirror, bool rotate90)
        {
            var x = picPoint.X / mainScale + XMin;
            var y = picPoint.Y / mainScale + YMin;
            var drawList = Layers.Where(it => it.Visible).Select(it => it.LayerName).ToList();

            bool hitTest = false;
            foreach (var obj in Shapes)                     //iterates through the objects
            {
                DXFPolyline temp = (DXFPolyline)obj;
                if (drawList.Contains(temp.LayerIndex))
                {

                    if (temp.Highlight(new System.Drawing.Point((int)x, (int)y)))
                    {
                        hitTest |= true;
                        break;
                    }
                }
            }

            if (rotate90)
            {
                foreach (var obj in Shapes)                     //iterates through the objects
                {
                    DXFPolyline temp = (DXFPolyline)obj;
                    if (drawList.Contains(temp.LayerIndex))
                    {

                        if (temp.Highlight(new System.Drawing.Point(-1*(int)y, (int)x)))
                        {
                            hitTest |= true;
                            break;
                        }
                    }
                }
            }

            if (mirror)
            {
                foreach (var obj in Shapes)                     //iterates through the objects
                {
                    DXFPolyline temp = (DXFPolyline)obj;
                    if (drawList.Contains(temp.LayerIndex))
                    {

                        if (temp.Highlight(new System.Drawing.Point(-1*(int)x,-1* (int)y)))
                        {
                            hitTest |= true;
                            break;
                        }
                    }
                }

                if (rotate90)
                {
                    foreach (var obj in Shapes)                     //iterates through the objects
                    {
                        DXFPolyline temp = (DXFPolyline)obj;
                        if (drawList.Contains(temp.LayerIndex))
                        {

                            if (temp.Highlight(new System.Drawing.Point((int)y, -1*(int)x)))
                            {
                                hitTest |= true;
                                break;
                            }
                        }
                    }
                }
            }


            return hitTest;
        }
    }
}
