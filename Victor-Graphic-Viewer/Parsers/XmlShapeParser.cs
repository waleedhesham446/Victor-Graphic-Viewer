using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Victor_Graphic_Viewer.Factories;
using Victor_Graphic_Viewer.Models;

namespace Victor_Graphic_Viewer.Parsers
{
    internal class XmlShapeParser : IShapeParser
    {
        private readonly Dictionary<string, IShapeFactory> _factories;

        public XmlShapeParser(IEnumerable<IShapeFactory> factories)
        {
            _factories = new Dictionary<string, IShapeFactory>();
            foreach (var f in factories)
                _factories[f.ShapeType] = f;
        }

        public IEnumerable<ShapeBase> Parse(string input)
        {
            var doc = XDocument.Parse(input);
            foreach (var el in doc.Root.Elements("shape"))
            {
                var data = ExtractShapeData(el);

                if (!_factories.TryGetValue(data.Type, out var factory))
                    throw new System.NotSupportedException("Unknown shape type: " + data.Type);

                yield return factory.Create(data);
            }
        }

        private static ShapeData ExtractShapeData(XElement el)
        {
            var data = new ShapeData
            {
                Type = el.Element("type")?.Value,
                Color = el.Element("color")?.Value,
                Filled = bool.Parse(el.Element("filled")?.Value ?? "false")
            };

            foreach (var child in el.Elements())
                data.Properties[child.Name.LocalName] = child.Value;

            return data;
        }
    }
}
