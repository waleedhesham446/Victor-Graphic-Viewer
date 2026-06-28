using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Victor_Graphic_Viewer.Models
{
    internal abstract class ShapeBase
    {
        public Color Color { get; set; }
        
        public bool Filled { get; set; }

        public abstract Dictionary<string, string> GetProperties();
    }
}
