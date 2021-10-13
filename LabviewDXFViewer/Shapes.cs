/* Author: Evren DAGLIOGLU
 * E-mail: evrenda@yahoo.com
 * This software is copyrighted to the author himself. It can be used freely for educational purposes.
 * For commercial usage written consent of the author must be taken and a reference to the author should be provided. 
 * No responsibility will be taken for any loss or damage that will occur as a result of the usage of this code. 
 * 
 * Please feel free to inform me about any bugs, problems, ideas etc.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DXFImporter
{
    #region Shape class - abstract
    public abstract class DXFShape
    {
        protected Color contourColor;
        protected Color fillColor;
        protected int lineWidth;

        public int shapeIdentifier;
        public int rotation;
        public bool highlighted;

        public string LayerIndex { get; set; }

        public int GetLineWidth()
        {
            return lineWidth;
        }
        public abstract Color AccessContourColor
        {
            get;
            set;
        }

        public abstract Color AccessFillColor
        {
            get;
            set;
        }

        public abstract int AccessLineWidth
        {
            get;
            set;
        }

        public abstract int AccessRotation
        {
            get;
            set;
        }

        public abstract void Draw(Pen pen, Graphics g, double scale, Point offset);
        public virtual bool Highlight(Point point) { return false; }

        public abstract Rectangle Bounds
        {
            get;
        }

        public abstract System.Drawing.Point[] SelectedLocations
        {
            get;

        }

    }
    #endregion

    #region Line class
    public class DFXLine : DXFShape
    {
        protected Point startPoint;
        protected Point endPoint;

        public DFXLine(Point start, Point end, Color color, int w)
        {
            startPoint = start;
            endPoint = end;
            contourColor = color;
            lineWidth = w;
            shapeIdentifier = 1;
            rotation = 0;

        }

        public DFXLine()
        {

        }


        public override Color AccessContourColor
        {
            get
            {
                return contourColor;
            }
            set
            {
                contourColor = value;
            }
        }

        public override Color AccessFillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }

        public override int AccessLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }

        }

        public override int AccessRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public override void Draw(Pen pen, Graphics g, double scale, Point offset)
        {
            using (var lePen = new Pen(pen.Brush))
            {
                lePen.Color = contourColor;
                lePen.Width = (float)Math.Max(1, AccessLineWidth * scale);

                if (highlighted)
                {
                    lePen.Color = Color.Red;

                }

                g.DrawLine(lePen, (float)(startPoint.X - offset.X) * (float)scale, (float)(startPoint.Y - offset.Y) * (float)scale,
                                (float)(endPoint.X - offset.X) * (float)scale, (float)(endPoint.Y - offset.Y) * (float)scale);
            }
        }



        public virtual Point StartPoint
        {
            get
            {
                return startPoint;
            }

        }

        public virtual Point EndPoint
        {
            get
            {
                return endPoint;
            }
        }

        public override bool Highlight(Point point)
        {
            GraphicsPath areaPath;
            // Pen areaPen;
            Region areaRegion;

            // Create path which contains wide line
            // for easy mouse selection
            areaPath = new GraphicsPath();
            var areaPen = new Pen(Color.Red, lineWidth);

            areaPath.AddLine(StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);
            // startPoint and EndPoint are class members of type Point
            areaPath.Widen(areaPen);

            // Create region from the path
            areaRegion = new Region(areaPath);

            if (areaRegion.IsVisible(point) == true)
            {
                //g.DrawLine(pen, GetStartPoint, GetEndPoint);

                //g.DrawLine(pen, GetStartPoint.X, GetStartPoint.Y , GetEndPoint.X, GetEndPoint.Y);
                highlighted = !highlighted;
                areaPath.Dispose();
                areaPen.Dispose();
                areaRegion.Dispose();

                return true;
            }
            areaPath.Dispose();
            areaPen.Dispose();
            areaRegion.Dispose();
            return false;
        }

        public  bool HitTest(Point point)
        {
            GraphicsPath areaPath;
            // Pen areaPen;
            Region areaRegion;

            // Create path which contains wide line
            // for easy mouse selection
            areaPath = new GraphicsPath();
            var areaPen = new Pen(Color.Red, lineWidth);

            areaPath.AddLine(StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);
            // startPoint and EndPoint are class members of type Point
            areaPath.Widen(areaPen);

            // Create region from the path
            areaRegion = new Region(areaPath);

            if (areaRegion.IsVisible(point) == true)
            {
                //g.DrawLine(pen, GetStartPoint, GetEndPoint);

                //g.DrawLine(pen, GetStartPoint.X, GetStartPoint.Y , GetEndPoint.X, GetEndPoint.Y);
                areaPath.Dispose();
                areaPen.Dispose();
                areaRegion.Dispose();

                return true;
            }
            areaPath.Dispose();
            areaPen.Dispose();
            areaRegion.Dispose();
            return false;
        }


        public override System.Drawing.Point[] SelectedLocations
        {
            get
            {
                if (highlighted)
                {
                    return new System.Drawing.Point[] { new System.Drawing.Point( 10*(int)Math.Ceiling( (startPoint.X + endPoint.X+.5) / 20d), 10 * (int)Math.Ceiling((.5+startPoint.Y + endPoint.Y) / 20d)) };
                }
                else
                    return null;
            }

        }

        public override Rectangle Bounds => throw new NotImplementedException();
    }
    #endregion

    #region Rectangle class
    public class DXFRectangle : DXFImporter.DFXLine
    {
        public DXFRectangle(Point start, Point end, Color color, Color fill, int w, int angle)
        {
            startPoint = start;
            endPoint = end;
            contourColor = color;
            fillColor = fill;
            lineWidth = w;
            shapeIdentifier = 2;
            rotation = angle;

        }

        public override void Draw(Pen pen, Graphics g, double scale, Point offset)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            if (AccessRotation != 0)
            {
                DrawRotatedRectangle(pen, g);
                return;
            }

            g.DrawLine(pen, StartPoint.X, StartPoint.Y, EndPoint.X, StartPoint.Y);
            g.DrawLine(pen, EndPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);
            g.DrawLine(pen, EndPoint.X, EndPoint.Y, StartPoint.X, EndPoint.Y);
            g.DrawLine(pen, StartPoint.X, EndPoint.Y, StartPoint.X, StartPoint.Y);

            return;
        }

        private void DrawRotatedRectangle(Pen pen, Graphics g)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            Point P1 = StartPoint;
            Point P2 = EndPoint;

            Point P3 = new Point(P2.X, P1.Y);
            Point P4 = new Point(P1.X, P2.Y);


            Point center = new Point(P1.X + (P3.X - P1.X) / 2, P1.Y + (P4.Y - P1.Y) / 2);

            int angle = AccessRotation;

            if (angle != 0)
            {

                P1 = CalculateRotatedNewPoint(P1, center, angle);   //Top left
                P3 = CalculateRotatedNewPoint(P3, center, angle);   //Bottom right

                P2 = CalculateRotatedNewPoint(P2, center, angle);   //Top right
                P4 = CalculateRotatedNewPoint(P4, center, angle);   //Bottom left


                g.DrawLine(pen, P1, P3);
                g.DrawLine(pen, P3, P2);
                g.DrawLine(pen, P2, P4);
                g.DrawLine(pen, P4, P1);

                return;

            }

        }

        private Point CalculateRotatedNewPoint(Point P, Point center, int angle)
        {
            double angleRad = angle * 1 / 57.2957;

            Point tempPoint = new Point(P.X - center.X, P.Y - center.Y);

            double radius = Math.Sqrt((tempPoint.X * tempPoint.X) + (tempPoint.Y * tempPoint.Y));


            double radiant1 = Math.Acos(tempPoint.X / radius);

            if (tempPoint.X < 0 && tempPoint.Y < 0)
                radiant1 = -radiant1;

            if (tempPoint.X > 0 && tempPoint.Y < 0)
                radiant1 = -radiant1;

            double radiant2 = Math.Asin(tempPoint.Y / radius);

            radiant1 = radiant1 + angleRad;
            radiant2 = radiant2 + angleRad;

            double temp;
            temp = radius * Math.Cos(radiant1);
            P.X = (int)temp + center.X;



            temp = radius * Math.Sin(radiant1);
            P.Y = (int)temp + center.Y;


            return P;
        }

        /*		public override bool Highlight(Pen pen, Graphics g, Point point)
				{
					Point check1 = GetStartPoint;
					Point check2 = GetEndPoint;
					
					Point check1a = new Point (check2.X, check1.Y);

					Point check2a = new Point (check1.X, check2.Y);

					Point center = new Point ( check1.X + (check1a.X - check1.X)/2, check1.Y + (check2a.Y - check1.Y)/2);

					check1 = CalculateRotatedNewPoint (check1, center, AccessRotation);

					check2 = CalculateRotatedNewPoint (check2, center, AccessRotation);
					
					check1a = CalculateRotatedNewPoint (check1a, center, AccessRotation);

					check2a = CalculateRotatedNewPoint (check2a, center, AccessRotation);


					GraphicsPath areaPath;
					Pen areaPen;
					Region areaRegion;

					// Create path which contains wide line
					// for easy mouse selection
					areaPath = new GraphicsPath();
					areaPen = new Pen(Color.Red, 2);

					areaPath.AddLine(check1, check1a);
					areaPath.AddLine(check1a, check2);
					areaPath.AddLine(check2, check2a);
					areaPath.AddLine(check2a, check1);

					areaPath.CloseFigure();

					// startPoint and EndPoint are class members of type Point
					areaPath.Widen(areaPen);

					// Create region from the path
					areaRegion = new Region(areaPath);

					RectangleF arect = areaRegion.GetBounds(g);
					
					if (arect.Contains(point))
					{
				
						g.FillPath(Brushes.DarkOrange, areaPath);
						
						//g.FillRectangle(Brushes.Green, arect);

						areaPath.Dispose();
						areaPen.Dispose();
						areaRegion.Dispose();


						return true;

					}
			
					return false;
				}
		
		*/
        /*		public override bool Highlight(Pen pen, Graphics g, Point point)
				{
					Point P1 = GetStartPoint;
					Point P2 = GetEndPoint;

					Point P3 = new Point (P2.X, P1.Y);
					Point P4 = new Point (P1.X, P2.Y);

					if (AccessRotation != 0)
					{
						Point center = new Point ( P1.X + (P3.X - P1.X)/2, P1.Y + (P4.Y - P1.Y)/2);

						P1 = CalculateRotatedNewPoint(P1, center, AccessRotation);
						P2 = CalculateRotatedNewPoint(P2, center, AccessRotation);
						P3 = CalculateRotatedNewPoint(P3, center, AccessRotation);
						P4 = CalculateRotatedNewPoint(P4, center, AccessRotation);
					}

					int maxX = Math.Max(P1.X, P2.X);
					maxX = Math.Max(maxX, P3.X);
					maxX = Math.Max(maxX, P4.X);

					int minX = Math.Min(P1.X, P2.X);
					minX = Math.Min(minX, P3.X);
					minX = Math.Min(minX, P4.X);

					int maxY = Math.Max(P1.Y, P2.Y);
					maxY = Math.Max(maxY, P3.Y);
					maxY = Math.Max(maxY, P4.Y);

					int minY = Math.Min(P1.Y, P2.Y);
					minY = Math.Min(minY, P3.Y);
					minY = Math.Min(minY, P4.Y);

					if (point.X > minX && point.X < maxX && point.Y > minY && point.Y < maxY)
					{
						pen.Color = Color.LightGreen;

						Draw(pen, g);

						return true;
					}

					return false;
				}
		
		*/

        //public override bool Highlight(Pen pen, Graphics g, Point point)
        //{
        //    Point P1 = StartPoint;
        //    Point P2 = EndPoint;

        //    Point P3 = new Point(P2.X, P1.Y);
        //    Point P4 = new Point(P1.X, P2.Y);

        //    if (AccessRotation != 0)
        //    {
        //        Point bottom = new Point(0, 0);
        //        Point top = new Point(0, 0);
        //        Point left = new Point(0, 0);
        //        Point right = new Point(0, 0);

        //        Point center = new Point(P1.X + (P3.X - P1.X) / 2, P1.Y + (P4.Y - P1.Y) / 2);

        //        P1 = CalculateRotatedNewPoint(P1, center, AccessRotation);
        //        P2 = CalculateRotatedNewPoint(P2, center, AccessRotation);
        //        P3 = CalculateRotatedNewPoint(P3, center, AccessRotation);
        //        P4 = CalculateRotatedNewPoint(P4, center, AccessRotation);

        //        int maxX = Math.Max(P1.X, P2.X);
        //        maxX = Math.Max(maxX, P3.X);
        //        maxX = Math.Max(maxX, P4.X);

        //        if (maxX == P1.X)
        //            right = P1;
        //        if (maxX == P2.X)
        //            right = P2;
        //        if (maxX == P3.X)
        //            right = P3;
        //        if (maxX == P4.X)
        //            right = P4;

        //        int minX = Math.Min(P1.X, P2.X);
        //        minX = Math.Min(minX, P3.X);
        //        minX = Math.Min(minX, P4.X);


        //        if (minX == P1.X)
        //            left = P1;
        //        if (minX == P2.X)
        //            left = P2;
        //        if (minX == P3.X)
        //            left = P3;
        //        if (minX == P4.X)
        //            left = P4;


        //        int maxY = Math.Max(P1.Y, P2.Y);
        //        maxY = Math.Max(maxY, P3.Y);
        //        maxY = Math.Max(maxY, P4.Y);


        //        if (maxY == P1.Y)
        //            bottom = P1;
        //        if (maxY == P2.Y)
        //            bottom = P2;
        //        if (maxY == P3.Y)
        //            bottom = P3;
        //        if (maxY == P4.Y)
        //            bottom = P4;


        //        int minY = Math.Min(P1.Y, P2.Y);
        //        minY = Math.Min(minY, P3.Y);
        //        minY = Math.Min(minY, P4.Y);


        //        if (minY == P1.Y)
        //            top = P1;
        //        if (minY == P2.Y)
        //            top = P2;
        //        if (minY == P3.Y)
        //            top = P3;
        //        if (minY == P4.Y)
        //            top = P4;


        //        double c1 = checkPosition(left, top, point);
        //        double c2 = checkPosition(right, top, point);
        //        double c3 = checkPosition(right, bottom, point);
        //        double c4 = checkPosition(left, bottom, point);

        //        if ((c1 > 0 && c2 > 0 && c3 < 0 && c4 < 0))
        //        {

        //            pen.Color = Color.LightGreen;

        //           // Draw(pen, g, scale, offset);

        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        int maxX = Math.Max(P1.X, P2.X);
        //        maxX = Math.Max(maxX, P3.X);
        //        maxX = Math.Max(maxX, P4.X);

        //        int minX = Math.Min(P1.X, P2.X);
        //        minX = Math.Min(minX, P3.X);
        //        minX = Math.Min(minX, P4.X);

        //        int maxY = Math.Max(P1.Y, P2.Y);
        //        maxY = Math.Max(maxY, P3.Y);
        //        maxY = Math.Max(maxY, P4.Y);

        //        int minY = Math.Min(P1.Y, P2.Y);
        //        minY = Math.Min(minY, P3.Y);
        //        minY = Math.Min(minY, P4.Y);


        //        if (point.X > minX && point.X < maxX && point.Y > minY && point.Y < maxY)
        //        {
        //            pen.Color = Color.LightGreen;
        //            //	pen.Width = 1;

        //            //Draw(pen, g);

        //            return true;
        //        }
        //    }

        //    return false;
        //}

        private double checkPosition(Point P1, Point P2, Point current)
        {
            double m = (double)(P2.Y - P1.Y) / (P2.X - P1.X);
            return ((current.Y - P1.Y) - (m * (current.X - P1.X)));
        }

    }
    #endregion

    #region Circle Class
    public class DXFCircle : DXFImporter.DXFShape
    {
        private Point centerPoint;
        private double radius;

        public DXFCircle(Point center, double r, Color color1, Color color2, int w)
        {
            centerPoint = center;
            radius = r;
            contourColor = color1;
            fillColor = color2;
            lineWidth = w;
            shapeIdentifier = 3;
            rotation = 0;
        }

        public override Color AccessContourColor
        {
            get
            {
                return contourColor;
            }
            set
            {
                contourColor = value;
            }
        }

        public override Color AccessFillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }

        public override int AccessLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }

        }

        public override int AccessRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }


        public Point AccessCenterPoint
        {
            get
            {
                return centerPoint;
            }
            set
            {
                centerPoint = value;
            }
        }

        public double AccessRadius
        {
            get
            {
                return radius;
            }
        }

        public override void Draw(Pen pen, Graphics g, double scale, Point offset)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            g.DrawEllipse(pen, (float)centerPoint.X * (float)scale - (float)radius * (float)scale, (float)centerPoint.Y * (float)scale - (float)radius * (float)scale, (float)radius * 2 * (float)scale, (float)radius * 2 * (float)scale);
        }


        public override System.Drawing.Point[] SelectedLocations
        {
            get
            {
                if (highlighted)
                {
                    return new System.Drawing.Point[] { centerPoint };
                }
                else
                    return null;
            }

        }

        /*		public override bool Highlight(Pen pen, Graphics g, Point point)
				{
					Point circular = AccessCenterPoint;
					int abc = (int) AccessRadius;
	
					GraphicsPath areaPath;
					Pen areaPen;
					Region areaRegion;

					// Create path which contains wide line
					// for easy mouse selection
					areaPath = new GraphicsPath();
					areaPen = new Pen(Color.Red, 4);
					areaPath.AddEllipse (circular.X - abc, circular.Y - abc, abc * 2, abc*2);
					// startPoint and EndPoint are class members of type Point
					areaPath.Widen(areaPen);

					areaRegion = new Region(areaPath);

					RectangleF arect = areaRegion.GetBounds(g);
					
					if (arect.Contains(point))
					{
				
						g.FillPath(Brushes.DarkOrange, areaPath);
						
						//g.FillRectangle(Brushes.Green, arect);

						areaPath.Dispose();
						areaPen.Dispose();
						areaRegion.Dispose();

		
						return true;

					}
					return false;
				}
		
		*/

        //public override bool Highlight(Pen pen, Graphics g, Point point)
        //{
        //    Point center = AccessCenterPoint;
        //    int rad = (int)AccessRadius;

        //    int check1y = center.Y - rad;
        //    int check2y = center.Y + rad;
        //    int check3x = center.X + rad;
        //    int check4x = center.X - rad;

        //    double result = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y) - radius * radius;

        //    if (result < 0)
        //    {
        //        //pen.Color = Color.Red;


        //        //g.DrawEllipse(pen, centerPoint.X - (int) radius, centerPoint.Y - (int)radius, (int)radius*2, (int)radius*2);

        //        return true;
        //    }



        //    return false;
        //}

        public bool Highlight(Pen pen, Graphics g, Point point, double scale)
        {
            Point center = AccessCenterPoint;
            int rad = (int)AccessRadius;

            int check1y = center.Y - rad;
            int check2y = center.Y + rad;
            int check3x = center.X + rad;
            int check4x = center.X - rad;

            double result = (point.X - center.X * (float)scale) * (point.X - center.X * (float)scale) + (point.Y - center.Y * (float)scale) * (point.Y - center.Y * (float)scale) - radius * radius * (float)scale * (float)scale;

            if (result < 0)
            {
                //pen.Color = Color.Red;


                //g.DrawEllipse(pen, (float)centerPoint.X*(float)scale - (float) radius*(float)scale, (float)centerPoint.Y*(float)scale - (float)radius*(float)scale, (float)radius*2*(float)scale, (float)radius*2*(float)scale);

                return true;
            }



            return false;
        }

        public override Rectangle Bounds => throw new NotImplementedException();
    }
    #endregion



    #region Polyline Class

    public class DXFPolyline : DXFImporter.DXFShape
    {

        private double XMax = double.MinValue, XMin = double.MaxValue;
        private double YMax = double.MinValue, YMin = double.MaxValue;

        private ArrayList listOfLines;

        public DXFPolyline(Color color, int w, string layerIndex)
        {
            LayerIndex = layerIndex;
            listOfLines = new ArrayList();

            contourColor = color;
            lineWidth = w;
        }

        public override Color AccessContourColor
        {
            get
            {
                return contourColor;
            }
            set
            {
                contourColor = value;
            }
        }

        public override Color AccessFillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }


        public override int AccessLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }

        }

        public  int ItemLineWidth
        {
            get
            {
                foreach (var item in listOfLines)
                {
                    var temp = item as DXFShape;
                    if (temp!=null)
                    {
                        return temp.GetLineWidth();
                    }
                }
                return 0;
            }
            set
            {
                lineWidth = value;
            }

        }

        public override int AccessRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public override void Draw(Pen pen, Graphics g, double scale, Point offset)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            foreach (DFXLine obj in listOfLines)
            {

                obj.Draw(pen, g, scale, offset);
            }

        }

        public override System.Drawing.Point[] SelectedLocations
        {
            get
            {
                List<System.Drawing.Point> selected = new List<System.Drawing.Point>();
                foreach (DFXLine obj in listOfLines)                    //iterates through the objects
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

        public  bool HitTest(Point point)
        {
            foreach (DFXLine obj in listOfLines)
            {
                if (obj.HitTest(point))
                {
                    return true;
                }
            }

            return false;
        }


        public override bool Highlight(Point point)
        {
            foreach (DFXLine obj in listOfLines)
            {
                if (obj.Highlight(point))
                {
                    return true;
                }
            }

            return false;
        }



        public void AppendLine(DFXLine theLine)
        {
            listOfLines.Add(theLine);

            if (theLine.StartPoint.X > XMax)
                XMax = theLine.StartPoint.X;
            if (theLine.StartPoint.X < XMin)
                XMin = theLine.StartPoint.X;

            if (theLine.EndPoint.X > XMax)
                XMax = theLine.StartPoint.X;
            if (theLine.EndPoint.X < XMin)
                XMin = theLine.StartPoint.X;

            if (theLine.StartPoint.Y > YMax)
                YMax = theLine.StartPoint.Y;
            if (theLine.StartPoint.Y < YMin)
                YMin = theLine.StartPoint.Y;

            if (theLine.EndPoint.Y > YMax)
                YMax = theLine.StartPoint.Y;
            if (theLine.EndPoint.Y < YMin)
                YMin = theLine.StartPoint.Y;

        }

        public override Rectangle Bounds => new Rectangle((int)XMin, (int)YMin, (int)(XMax - XMin), (int)(YMax - YMin));
    }

    #endregion

    #region Arc Class

    public class DXFArc : DXFImporter.DXFShape
    {
        private Point centerPoint;
        private double radius;

        private double startAngle;
        private double sweepAngle;

        public DXFArc(Point center, double r, double startangle, double sweepangle, Color color1, Color color2, int w)
        {
            centerPoint = center;
            radius = r;
            startAngle = startangle;
            sweepAngle = sweepangle;
            contourColor = color1;
            fillColor = color2;
            lineWidth = w;
            shapeIdentifier = 3;
            rotation = 0;
        }

        public double AccessStartAngle
        {
            get
            {
                return startAngle;
            }

        }

        public double AccessSweepAngle
        {
            get
            {
                return sweepAngle;
            }
        }

        public override Color AccessContourColor
        {
            get
            {
                return contourColor;
            }
            set
            {
                contourColor = value;
            }
        }

        public override Color AccessFillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }

        public override int AccessLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }

        }

        public override int AccessRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }


        public Point AccessCenterPoint
        {
            get
            {
                return centerPoint;
            }
            set
            {
                centerPoint = value;
            }
        }

        public double AccessRadius
        {
            get
            {
                return radius;
            }
        }

        public override void Draw(Pen pen, Graphics g, double scale, Point offset)
        {
            //g.DrawEllipse(pen, (float) centerPoint.X* (float)scale - (float) radius* (float)scale, (float)centerPoint.Y * (float)scale - (float)radius* (float)scale, (float)radius*2* (float)scale, (float)radius*2* (float)scale);

            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            float tempAngle = 0;




            if (sweepAngle < startAngle)
            {
                tempAngle = -360 + (float)startAngle - (float)sweepAngle;

            }
            /*else if (startAngle > 180 && sweepAngle > 180)
            {
                tempAngle = startAngle - sweepAngle;


            }*/
            else
                tempAngle = (float)startAngle - (float)sweepAngle;



            g.DrawArc(pen, (float)centerPoint.X * (float)scale - (float)radius * (float)scale, (float)centerPoint.Y * (float)scale - (float)radius * (float)scale, (float)radius * 2 * (float)scale, (float)radius * 2 * (float)scale, -(float)startAngle, tempAngle);
        }


        public override System.Drawing.Point[] SelectedLocations
        {
            get
            {
                if (highlighted)
                {
                    return new System.Drawing.Point[] { centerPoint };
                }
                else
                    return null;
            }

        }

        //public override bool Highlight(Pen pen, Graphics g, Point point)
        //{
        //    Point center = AccessCenterPoint;
        //    int rad = (int)AccessRadius;

        //    int check1y = center.Y - rad;
        //    int check2y = center.Y + rad;
        //    int check3x = center.X + rad;
        //    int check4x = center.X - rad;

        //    double result = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y) - radius * radius;

        //    if (result < 0)
        //    {
        //        //pen.Color = Color.Yellow;


        //        //g.DrawEllipse(pen, centerPoint.X - (int) radius, centerPoint.Y - (int)radius, (int)radius*2, (int)radius*2);

        //        return true;
        //    }

        //    return false;
        //}

        public bool Highlight(Pen pen, Graphics g, Point point, double scale)
        {
            Point center = AccessCenterPoint;
            int rad = (int)AccessRadius;

            int check1y = center.Y - rad;
            int check2y = center.Y + rad;
            int check3x = center.X + rad;
            int check4x = center.X - rad;

            double result = (point.X - center.X * (float)scale) * (point.X - center.X * (float)scale) + (point.Y - center.Y * (float)scale) * (point.Y - center.Y * (float)scale) - radius * radius * (float)scale * (float)scale;

            if (result < 0)
            {
                //pen.Color = Color.Yellow;


                float tempAngle = 0;
                if (sweepAngle < startAngle)
                    tempAngle = -360 + (float)startAngle - (float)sweepAngle;
                else
                    tempAngle = (float)startAngle - (float)sweepAngle;


                //g.DrawArc(pen, (float)centerPoint.X*(float)scale - (float) radius*(float)scale, (float)centerPoint.Y*(float)scale - (float)radius*(float)scale, (float)radius*2*(float)scale, (float)radius*2*(float)scale, -startAngle, tempAngle);

                return true;
            }

            return false;
        }

        public override Rectangle Bounds => throw new NotImplementedException();
    }

    #endregion
}
