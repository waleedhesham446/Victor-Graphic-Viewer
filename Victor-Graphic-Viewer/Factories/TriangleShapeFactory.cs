using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victor_Graphic_Viewer.Models;
using Victor_Graphic_Viewer.Parsers;

namespace Victor_Graphic_Viewer.Factories
{
    internal class TriangleShapeFactory : IShapeFactory
    {
        public string ShapeType => "triangle";

        public ShapeBase Create(ShapeData data)
        {
            return new TriangleShape
            {
                Color = ShapeParsingHelper.ParseColor(data.Color),
                Filled = data.Filled,
                A = ShapeParsingHelper.ParsePoint(data.Get("a")),
                B = ShapeParsingHelper.ParsePoint(data.Get("b")),
                C = ShapeParsingHelper.ParsePoint(data.Get("c"))
            };
        }
    }
}
