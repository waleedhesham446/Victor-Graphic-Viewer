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
    internal class TriangleRenderer : IShapeRenderer
    {
        public bool CanRender(ShapeBase shape) => shape is TriangleShape;

        public bool HitTest(ShapeBase shape, Point screenPoint, Transform transform)
        {
            var tri = (TriangleShape)shape;
            var a = transform.Transform(tri.A);
            var b = transform.Transform(tri.B);
            var c = transform.Transform(tri.C);
            return PointInTriangle(screenPoint, a, b, c);
        }

        public void Render(ShapeBase shape, DrawingContext dc, Transform transform)
        {
            var tri = (TriangleShape)shape;
            var pen = new Pen(new SolidColorBrush(tri.Color), 1);
            Brush fill = tri.Filled ? new SolidColorBrush(tri.Color) : null;
            var a = transform.Transform(tri.A);
            var b = transform.Transform(tri.B);
            var c = transform.Transform(tri.C);
            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                ctx.BeginFigure(a, tri.Filled, true);
                ctx.LineTo(b, true, false);
                ctx.LineTo(c, true, false);
            }
            geo.Freeze();
            dc.DrawGeometry(fill, pen, geo);
        }

        // Barycentric method — works for both filled and unfilled triangles
        private static bool PointInTriangle(Point p, Point a, Point b, Point c)
        {
            double d1 = Sign(p, a, b);
            double d2 = Sign(p, b, c);
            double d3 = Sign(p, c, a);
            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
            return !(hasNeg && hasPos);
        }

        private static double Sign(Point p1, Point p2, Point p3)
            => (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
    }
}
