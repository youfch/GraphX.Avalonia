using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GraphX.Avalonia.Demo.Controls;
using GraphX.Avalonia.Demo.Models;

namespace GraphX.Avalonia.Demo.Views
{
    public partial class MainWindow : Window
    {
        private DemoGraphArea? _graphArea;
        private ZoomControl? _zoomControl;

        public MainWindow()
        {
            InitializeComponent();
            LoadSampleGraph();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
            _zoomControl = this.FindControl<ZoomControl>("ZoomControl");
            _graphArea = this.FindControl<DemoGraphArea>("GraphArea");
        }

        private void LoadSampleGraph()
        {
            if (_graphArea == null) return;

            // Create sample graph
            var graph = DataGraph.CreateSampleGraph();
            
            // Generate visual presentation
            _graphArea.GenerateGraph(graph);

            // Apply simple layout
            ApplyRandomLayout();
        }

        private void ApplyRandomLayout()
        {
            if (_graphArea == null) return;

            var random = new System.Random();
            double width = 800;
            double height = 600;
            double padding = 50;

            foreach (var kvp in _graphArea.VertexList)
            {
                var x = padding + random.NextDouble() * (width - 2 * padding);
                var y = padding + random.NextDouble() * (height - 2 * padding);
                
                GraphAreaBase.SetX(kvp.Value, x);
                GraphAreaBase.SetY(kvp.Value, y);
            }

            _graphArea.UpdateAllEdges();
        }

        public void ZoomToFill()
        {
            _zoomControl?.ZoomToFill();
        }

        public void ResetZoom()
        {
            _zoomControl?.ResetZoom();
        }
    }
}