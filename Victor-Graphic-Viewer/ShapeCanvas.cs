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

        private double _zoom = 1.0;
        private double _panX = 0.0;
        private double _panY = 0.0;
        private const double ZoomMin = 0.1;
        private const double ZoomMax = 20.0;
        private const double ZoomStep = 1.15;

        private bool _isPanning;
        private Point _panStart;
        
        private static readonly Brush BackgroundBrush = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x2E));

        public ShapeCanvas()
        {
            Focusable = true;
            Focus();
        }

        public void RegisterRenderers(IEnumerable<IShapeRenderer> renderers)
            => _renderers = new List<IShapeRenderer>(renderers);

        public void LoadShapes(IEnumerable<ShapeBase> shapes)
        {
            _shapes = new List<ShapeBase>(shapes);
            _selected = null;
            _zoom = 1.0;
            _panX = 0.0;
            _panY = 0.0;
            InvalidateVisual();
            Focus();
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle ||
                (e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.Alt))
            {
                _isPanning = true;
                _panStart = e.GetPosition(this);
                CaptureMouse();
                e.Handled = true;
                return;
            }
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_isPanning) return;

            var pos = e.GetPosition(this);
            _panX += pos.X - _panStart.X;
            _panY += pos.Y - _panStart.Y;
            _panStart = pos;
            InvalidateVisual();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isPanning)
            {
                _isPanning = false;
                ReleaseMouseCapture();
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            var mousePos = e.GetPosition(this);
            double factor = e.Delta > 0 ? ZoomStep : 1.0 / ZoomStep;
            ZoomAround(mousePos, factor);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            bool ctrl = Keyboard.Modifiers == ModifierKeys.Control;
            Console.WriteLine($"CTRL: {ctrl}");
            if (!ctrl) return;

            var center = new Point(ActualWidth / 2, ActualHeight / 2);

            if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                ZoomAround(center, ZoomStep);
                e.Handled = true;
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                ZoomAround(center, 1.0 / ZoomStep);
                e.Handled = true;
            }
            else if (e.Key == Key.D0 || e.Key == Key.NumPad0)
            {
                // Ctrl+0 — reset zoom and pan
                _zoom = 1.0;
                _panX = 0.0;
                _panY = 0.0;
                InvalidateVisual();
                e.Handled = true;
            }
        }

        public void ZoomIn() => ZoomAround(new Point(ActualWidth / 2, ActualHeight / 2), ZoomStep);
        public void ZoomOut() => ZoomAround(new Point(ActualWidth / 2, ActualHeight / 2), 1.0 / ZoomStep);
        public void ZoomReset() { _zoom = 1.0; _panX = 0; _panY = 0; InvalidateVisual(); }

        private void ZoomAround(Point anchor, double factor)
        {
            double newZoom = Math.Max(ZoomMin, Math.Min(ZoomMax, _zoom * factor));
            if (newZoom == _zoom) return;

            // Adjust pan so the point under the cursor stays fixed
            // anchor = canvas center + panX + shape_offset * zoom
            // We want anchor to remain fixed after zoom changes
            _panX = anchor.X - (anchor.X - (ActualWidth / 2 + _panX)) * (newZoom / _zoom) - ActualWidth / 2;
            _panY = anchor.Y - (anchor.Y - (ActualHeight / 2 + _panY)) * (newZoom / _zoom) - ActualHeight / 2;
            _zoom = newZoom;

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Draw background explicitly — required for HitTestCore to cover the full area
            dc.DrawRectangle(BackgroundBrush, null, new Rect(0, 0, ActualWidth, ActualHeight));

            if (_shapes.Count == 0) return;

            _transform = BuildTransform();
            foreach (var shape in _shapes)
            {
                var renderer = _renderers.FirstOrDefault(r => r.CanRender(shape));
                if (renderer == null) continue;
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
            if (_shapes.Count == 0) return Transform.Identity;

            var bounds = ComputeBounds();
            if (bounds.IsEmpty) return Transform.Identity;

            // Base scale to fit the scene in the window at zoom = 1
            double scaleX = ActualWidth / bounds.Width;
            double scaleY = ActualHeight / bounds.Height;
            double baseScale = Math.Min(scaleX, scaleY) * 0.9;

            double cx = bounds.X + bounds.Width / 2;
            double cy = bounds.Y + bounds.Height / 2;

            var m = new Matrix();

            // 1. Center the scene at origin
            m.Translate(-cx, -cy);

            // 2. Flip Y + apply base scale + user zoom
            double totalScale = baseScale * _zoom;
            m.Scale(totalScale, -totalScale);

            // 3. Move to canvas center + apply pan offset
            m.Translate(ActualWidth / 2 + _panX, ActualHeight / 2 + _panY);

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
