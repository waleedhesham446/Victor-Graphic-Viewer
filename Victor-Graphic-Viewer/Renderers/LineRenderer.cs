using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Victor_Graphic_Viewer.Models;

namespace Victor_Graphic_Viewer.Renderers
{
    internal class LineRenderer : IShapeRenderer
    {
        private const double HitTolerance = 6.0; // pixels

        public bool CanRender(ShapeBase shape) => shape is LineShape;

        public bool HitTest(ShapeBase shape, Point screenPoint, Transform transform)
        {
            var line = (LineShape)shape;
            var a = transform.Transform(line.A);
            var b = transform.Transform(line.B);
            return DistanceToSegment(screenPoint, a, b) <= HitTolerance;
        }

        public void Render(ShapeBase shape, DrawingContext dc, Transform transform)
        {
            var line = (LineShape)shape;
            var pen = new Pen(new SolidColorBrush(line.Color), 1);
            dc.DrawLine(pen, transform.Transform(line.A), transform.Transform(line.B));
        }

        private static double DistanceToSegment(Point p, Point a, Point b)
        {
            var ab = new Vector(b.X - a.X, b.Y - a.Y);
            var ap = new Vector(p.X - a.X, p.Y - a.Y);
            double t = Vector.Multiply(ap, ab) / Vector.Multiply(ab, ab);
            t = Math.Max(0, Math.Min(1, t));
            var closest = new Point(a.X + t * ab.X, a.Y + t * ab.Y);
            return (p - closest).Length;
        }
    }
}
