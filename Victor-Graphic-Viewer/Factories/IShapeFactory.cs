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
    internal interface IShapeFactory
    {
        string ShapeType { get; }

        ShapeBase Create(ShapeData data);
    }
}
