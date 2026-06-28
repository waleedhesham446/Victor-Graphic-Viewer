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
    internal class CircleRenderer : IShapeRenderer
    {
        public bool CanRender(ShapeBase shape) => shape is CircleShape;

        public bool HitTest(ShapeBase shape, Point screenPoint, Transform transform)
        {
            var circle = (CircleShape)shape;
            var center = transform.Transform(circle.Center);
            var scale = transform.Value.M11;
            var scaledRadius = circle.Radius * scale;
            return (screenPoint - center).Length <= scaledRadius;
        }

        public void Render(ShapeBase shape, DrawingContext dc, Transform transform)
        {
            var circle = (CircleShape)shape;
            var pen = new Pen(new SolidColorBrush(circle.Color), 1);
            Brush fill = circle.Filled ? new SolidColorBrush(circle.Color) : null;
            var center = transform.Transform(circle.Center);
            var scale = transform.Value.M11;
            dc.DrawEllipse(fill, pen, center, circle.Radius * scale, circle.Radius * scale);
        }
    }
}
