using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Victor_Graphic_Viewer.Parsers
{
    internal class ShapeData
    {
        public string Type { get; set; }
        public string Color { get; set; }
        public bool Filled { get; set; }

        // Generic key-value store for shape-specific fields
        // e.g. "center" -> "0; 0",  "radius" -> "15.0"
        public Dictionary<string, string> Properties { get; set; }
            = new Dictionary<string, string>();

        public string Get(string key) => Properties[key];
        public bool Has(string key) => Properties.ContainsKey(key);
    }
}
