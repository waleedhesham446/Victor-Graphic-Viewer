using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Victor_Graphic_Viewer.Models
{
    internal class LineShape : ShapeBase
    {
        public Point A { get; set; }

        public Point B { get; set; }

        public override Dictionary<string, string> GetProperties()
        {
            return new Dictionary<string, string>
            {
                { "Type",    "Line" },
                { "A",       $"({A.X}, {A.Y})" },
                { "B",       $"({B.X}, {B.Y})" },
                { "Color",   $"ARGB({Color.A}, {Color.R}, {Color.G}, {Color.B})" }
            };
        }
    }
}
