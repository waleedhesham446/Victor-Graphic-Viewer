using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Victor_Graphic_Viewer.Models
{
    internal class TriangleShape : ShapeBase
    {
        public Point A { get; set; }

        public Point B { get; set; }

        public Point C { get; set; }

        public override Dictionary<string, string> GetProperties()
        {
            return new Dictionary<string, string>
            {
                { "Type",    "Triangle" },
                { "A",       $"({A.X}, {A.Y})" },
                { "B",       $"({B.X}, {B.Y})" },
                { "C",       $"({C.X}, {C.Y})" },
                { "Filled",  Filled.ToString() },
                { "Color",   $"ARGB({Color.A}, {Color.R}, {Color.G}, {Color.B})" }
            };
        }
    }
}
