using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Victor_Graphic_Viewer.Factories;
using Victor_Graphic_Viewer.Models;
using Victor_Graphic_Viewer.Parsers;
using Victor_Graphic_Viewer.Renderers;

namespace Victor_Graphic_Viewer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IShapeParser _jsonParser;
        private readonly IShapeParser _xmlParser;
        // Add parsers for new formats

        private readonly List<IShapeFactory> _factories = new List<IShapeFactory>
        {
            new LineShapeFactory(),
            new CircleShapeFactory(),
            new TriangleShapeFactory()
            // Add factories for new shape
        };

        public MainWindow()
        {
            InitializeComponent();

            _jsonParser = new JsonShapeParser(_factories);
            _xmlParser = new XmlShapeParser(_factories);
            // Add parsers for new formats

            shapeCanvas.ShapeSelected += OnShapeSelected;

            shapeCanvas.RegisterRenderers(new List<IShapeRenderer>
            {
                new LineRenderer(),
                new CircleRenderer(),
                new TriangleRenderer()
                // Register renderers for new shapes
            });
        }

        private void OnShapeSelected(ShapeBase shape)
        {
            if (shape == null)
            {
                PropertyList.ItemsSource = null;
                return;
            }

            // Shape reports its own properties — no instanceof checks here
            PropertyList.ItemsSource = shape.GetProperties();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files|*.json|XML files|*.xml"
            };
            if (dlg.ShowDialog() != true) return;

            IShapeParser parser = dlg.FilterIndex == 1
                ? _jsonParser : _xmlParser;

            var shapes = parser.Parse(File.ReadAllText(dlg.FileName));
            shapeCanvas.LoadShapes(shapes);
        }
    }
}
