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
    internal class LineShapeFactory : IShapeFactory
    {
        public string ShapeType => "line";

        public ShapeBase Create(ShapeData data)
        {
            return new LineShape
            {
                Color = ShapeParsingHelper.ParseColor(data.Color),
                A = ShapeParsingHelper.ParsePoint(data.Get("a")),
                B = ShapeParsingHelper.ParsePoint(data.Get("b"))
            };
        }
    }
}
