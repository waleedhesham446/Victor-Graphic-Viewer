using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Victor_Graphic_Viewer.Factories
{
    internal static class ShapeParsingHelper
    {
        public static Color ParseColor(string s)
        {
            var p = s.Split(';');
            return Color.FromArgb(
                byte.Parse(p[0].Trim()),
                byte.Parse(p[1].Trim()),
                byte.Parse(p[2].Trim()),
                byte.Parse(p[3].Trim()));
        }

        public static Point ParsePoint(string s)
        {
            var p = s.Split(';');
            return new Point(
                double.Parse(p[0].Trim().Replace(',', '.'),
                    System.Globalization.CultureInfo.InvariantCulture),
                double.Parse(p[1].Trim().Replace(',', '.'),
                    System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
