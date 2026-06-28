using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Victor_Graphic_Viewer.Models;
using Victor_Graphic_Viewer.Renderers;

namespace Victor_Graphic_Viewer
{
    internal class ShapeCanvas : FrameworkElement
    {
        public event Action<ShapeBase> ShapeSelected;

        private List<ShapeBase> _shapes = new List<ShapeBase>();
        private List<IShapeRenderer> _renderers = new List<IShapeRenderer>();
        private ShapeBase _selected = null;
        private Transform _transform = Transform.Identity;

        public void RegisterRenderers(IEnumerable<IShapeRenderer> renderers)
            => _renderers = new List<IShapeRenderer>(renderers);

        public void LoadShapes(IEnumerable<ShapeBase> shapes)
        {
            _shapes = new List<ShapeBase>(shapes);
            _selected = null;
            _transform = BuildTransform();
            InvalidateVisual();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            var clickPoint = e.GetPosition(this);

            // Iterate in reverse so topmost-drawn shape wins
            _selected = _shapes
                .AsEnumerable()
                .Reverse()
                .FirstOrDefault(s =>
                {
                    var r = _renderers.FirstOrDefault(r => r.CanRender(s));
                    return r?.HitTest(s, clickPoint, _transform) == true;
                });

            InvalidateVisual();

            // Notify subscribers (MainWindow)
            ShapeSelected?.Invoke(_selected);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (_shapes.Count == 0) return;

            _transform = BuildTransform();

            foreach (var shape in _shapes)
            {
                var renderer = _renderers.FirstOrDefault(r => r.CanRender(shape));
                if (renderer == null) continue;

                // Draw highlight ring around selected shape
                if (shape == _selected)
                {
                    var highlightPen = new Pen(Brushes.Yellow, 2);
                    // Let the renderer draw normally, then overlay the highlight
                }

                renderer.Render(shape, dc, _transform);

                if (shape == _selected)
                    DrawHighlight(dc, shape);
            }
        }

        private void DrawHighlight(DrawingContext dc, ShapeBase shape)
        {
            var pen = new Pen(Brushes.Yellow, 2) { DashStyle = DashStyles.Dash };
            if (shape is LineShape l)
            {
                dc.DrawLine(pen, _transform.Transform(l.A), _transform.Transform(l.B));
            }
            else if (shape is CircleShape c)
            {
                var center = _transform.Transform(c.Center);
                dc.DrawEllipse(null, pen, center, c.Radius * _transform.Value.M11,
                                                   c.Radius * _transform.Value.M11);
            }
            else if (shape is TriangleShape t)
            {
                var a = _transform.Transform(t.A);
                var b = _transform.Transform(t.B);
                var cv = _transform.Transform(t.C);
                var geo = new StreamGeometry();
                using (var ctx = geo.Open())
                {
                    ctx.BeginFigure(a, false, true);
                    ctx.LineTo(b, true, false);
                    ctx.LineTo(cv, true, false);
                }
                geo.Freeze();
                dc.DrawGeometry(null, pen, geo);
            }
        }

        // Compute a matrix that:
        //   1. Flips Y  (Cartesian → screen)
        //   2. Scales uniformly so the scene fits this control
        //   3. Centers the scene
        private Transform BuildTransform()
        {
            var bounds = ComputeBounds();
            if (bounds.IsEmpty) return Transform.Identity;

            double scaleX = ActualWidth / bounds.Width;
            double scaleY = ActualHeight / bounds.Height;
            double scale = Math.Min(scaleX, scaleY) * 0.9;   // 10 % padding

            double cx = bounds.X + bounds.Width / 2;
            double cy = bounds.Y + bounds.Height / 2;

            // Step 1: translate scene center to origin
            // Step 2: flip Y, apply scale
            // Step 3: move to control center
            var m = new Matrix();
            m.Translate(-cx, -cy);
            m.Scale(scale, -scale);          // negative Y = flip
            m.Translate(ActualWidth / 2, ActualHeight / 2);

            return new MatrixTransform(m);
        }

        private Rect ComputeBounds()
        {
            var pts = new List<Point>();
            foreach (var s in _shapes)
            {
                switch (s)
                {
                    case LineShape l:
                        pts.Add(l.A); pts.Add(l.B); break;
                    case CircleShape c:
                        pts.Add(new Point(c.Center.X - c.Radius, c.Center.Y - c.Radius));
                        pts.Add(new Point(c.Center.X + c.Radius, c.Center.Y + c.Radius));
                        break;
                    case TriangleShape t:
                        pts.Add(t.A); pts.Add(t.B); pts.Add(t.C); break;
                }
            }
            if (!pts.Any()) return Rect.Empty;
            double minX = pts.Min(p => p.X), maxX = pts.Max(p => p.X);
            double minY = pts.Min(p => p.Y), maxY = pts.Max(p => p.Y);
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
