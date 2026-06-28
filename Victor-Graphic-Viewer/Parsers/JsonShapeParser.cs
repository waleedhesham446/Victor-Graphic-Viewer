using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Victor_Graphic_Viewer.Factories;
using Victor_Graphic_Viewer.Models;

namespace Victor_Graphic_Viewer.Parsers
{
    internal class JsonShapeParser : IShapeParser
    {
        private readonly Dictionary<string, IShapeFactory> _factories;

        public JsonShapeParser(IEnumerable<IShapeFactory> factories)
        {
            _factories = new Dictionary<string, IShapeFactory>();
            foreach (var f in factories)
                _factories[f.ShapeType] = f;
        }

        public IEnumerable<ShapeBase> Parse(string input)
        {
            var array = JArray.Parse(input);
            foreach (var doc in array)
            {
                var data = ExtractShapeData(doc);

                if (!_factories.TryGetValue(data.Type, out var factory))
                    throw new System.NotSupportedException("Unknown shape type: " + data.Type);

                yield return factory.Create(data);
            }
        }

        private static ShapeData ExtractShapeData(JToken doc)
        {
            var data = new ShapeData
            {
                Type = doc["type"].Value<string>(),
                Color = doc["color"].Value<string>(),
                Filled = doc["filled"]?.Value<bool>() ?? false
            };

            foreach (var prop in ((JObject)doc).Properties())
            {
                if (prop.Name == "type" || prop.Name == "color" || prop.Name == "filled")
                    continue;
                data.Properties[prop.Name] = prop.Value.ToString();
            }

            return data;
        }
    }
}
