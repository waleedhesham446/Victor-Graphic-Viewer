using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Victor_Graphic_Viewer.Models
{
    internal class CircleShape : ShapeBase
    {
        public Point Center { get; set; }

        public double Radius { get; set; }

        public override Dictionary<string, string> GetProperties()
        {
            return new Dictionary<string, string>
            {
                { "Type",    "Circle" },
                { "Center",  $"({Center.X}, {Center.Y})" },
                { "Radius",  Radius.ToString() },
                { "Filled",  Filled.ToString() },
                { "Color",   $"ARGB({Color.A}, {Color.R}, {Color.G}, {Color.B})" }
            };
        }
    }
}
