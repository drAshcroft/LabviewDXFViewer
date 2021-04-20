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
    class DXFView
    {
        public string[] Layers { get; set; }
        public void LoadFile(string filename)
        {
            DxfDocument loaded = DxfDocument.Load(filename);
            Layers = loaded.Layers.Select(x => x.Name).OrderBy(x => x).ToArray();


            Shapes = new List<DXFShape>();

            foreach (var block in loaded.Blocks)
            {
                foreach (var shape in block.Entities)
                {
                    switch (shape.Type)
                    {
                        case EntityType.Polyline:
                            Shapes.Add(new DXFPolyline(shape.Color.ToColor(), 1));
                            break;
                        case EntityType.LightWeightPolyline:
                            var pl = new DXFPolyline(shape.Color.ToColor(), 1);
                            var specificShape = (LwPolyline)shape;
                            for (int i = 1; i < specificShape.Vertexes.Count; i++)
                            {
                                pl.AppendLine(new DFXLine(
                                     new System.Drawing.Point((int)specificShape.Vertexes[i - 1].Position.X, (int)specificShape.Vertexes[i - 1].Position.Y),
                                     new System.Drawing.Point((int)specificShape.Vertexes[i].Position.X, (int)specificShape.Vertexes[i].Position.Y),
                                     shape.Color.ToColor(),
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


            if (XMax > pbWidth)
                scaleX = (double)(pbWidth) / (double)XMax;

            if (YMax > pbHeight)
                scaleY = (double)(pbHeight) / (double)YMax;

            mainScale = Math.Min(scaleX, scaleY);
        }

        public void Draw(PictureBox pictureBox1)
        {
            ScaleImage(pictureBox1.Image.Width, pictureBox1.Image.Height);


            using (var g = Graphics.FromImage(pictureBox1.Image))
            {

                Rectangle rect = new Rectangle(0, 0, pictureBox1.Size.Width, pictureBox1.Size.Height);


                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                                                                                                                rect,
                                                                                                                Color.SteelBlue,
                                                                                                                Color.Black,
                                                                                                                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
                g.FillRectangle(brush, rect);

                Pen lePen = new Pen(Color.White, 3);

                //g.TranslateTransform(pictureBox1.Location.X + 1, pictureBox1.Location.Y + pictureBox1.Size.Height - 1);

                //if (YMin < 0)
                //    g.TranslateTransform(0, -(int)Math.Abs(YMin));          //transforms point-of-origin to the lower left corner of the canvas.

                //if (XMin < 0)
                //    g.TranslateTransform((int)Math.Abs(XMin), 0);

                //	g.SmoothingMode = SmoothingMode.AntiAlias; 

                foreach (var obj in Shapes)                     //iterates through the objects
                {
                    DXFPolyline temp = (DXFPolyline)obj;

                    lePen.Color = temp.AccessContourColor;
                    lePen.Width =(float) Math.Max(1, temp.AccessLineWidth*mainScale);

                    
                    if (mainScale == 0)
                        mainScale = 1;

                    temp.Draw(lePen, g, mainScale, new System.Drawing.Point((int)XMin,(int) YMin));

                    
                }

                lePen.Dispose();
                brush.Dispose();
            }


            pictureBox1.Invalidate();
        }
    }
}
