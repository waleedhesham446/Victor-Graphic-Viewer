using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victor_Graphic_Viewer.Models;

namespace Victor_Graphic_Viewer.Parsers
{
    internal interface IShapeParser
    {
        IEnumerable<ShapeBase> Parse(string input);
    }
}
