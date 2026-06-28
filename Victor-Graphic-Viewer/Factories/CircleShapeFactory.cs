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
    internal class CircleShapeFactory : IShapeFactory
    {
        public string ShapeType => "circle";

        public ShapeBase Create(ShapeData data)
        {
            return new CircleShape
            {
                Color = ShapeParsingHelper.ParseColor(data.Color),
                Filled = data.Filled,
                Center = ShapeParsingHelper.ParsePoint(data.Get("center")),
                Radius = double.Parse(data.Get("radius"),
                             System.Globalization.CultureInfo.InvariantCulture)
            };
        }
    }
}
