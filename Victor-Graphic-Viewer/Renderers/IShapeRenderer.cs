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
    internal interface IShapeRenderer
    {
        // Returns true if this renderer handles the given shape type
        bool CanRender(ShapeBase shape);

        // Appends WPF geometry to the DrawingContext.
        // transform converts Cartesian → screen coords (Y-flip + scale + pan)
        void Render(ShapeBase shape, DrawingContext dc, Transform transform);

        // Returns true if the screen-space point hits this shape
        bool HitTest(ShapeBase shape, Point screenPoint, Transform transform);
    }
}
