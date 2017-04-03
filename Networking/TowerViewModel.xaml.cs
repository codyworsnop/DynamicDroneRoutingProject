
namespace Networking
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;


    /// <summary>
    /// Interaction logic for TowerViewModel.xaml
    /// </summary>
    public partial class TowerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The canvas top
        /// </summary>
        private double _canvasTop;
        /// <summary>
        /// The canvas left
        /// </summary>
        private double _canvasLeft;
        /// <summary>
        /// The show coverage radius
        /// </summary>
        private bool _showCoverageRadius;
        /// <summary>
        /// The canvas model
        /// </summary>
        private MainCanvasModel _canvasModel;
        /// <summary>
        /// The tower model
        /// </summary>
        public TowerModel TowerModel;

        /// <summary>
        /// The connectors
        /// </summary>
        private GeometryGroup _connectors;

        /// <summary>
        /// The node identifier
        /// </summary>
        private string _nodeId;

        /// <summary>
        /// The root canvas
        /// </summary>
        private Canvas _rootCanvas;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="TowerViewModel"/> class.
        /// </summary>
        public TowerViewModel()
        {
            InitializeComponent();
            DataContext = this;

            SetupMeasurement();

            TowerModel = new TowerModel(); 

        }

        /// <summary>
        /// This sets up the viewmodel so that the actualheight and actualwidth measurements are available before
        /// the xaml actually lays them out. 
        /// </summary>
        public void SetupMeasurement()
        {
            this.Measure(new Size(300, 300));
            this.Arrange(new Rect(0, 0, 300, 300));
        }

        /// <summary>
        /// Initializes a new instance of the towermodel class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="canvasModel">The canvas model.</param>
        /// <param name="showCoverageRadius">The show coverage radius.</param>
        /// <param name="rootCanvas">The root canvas.</param>
        /// <param name="connectors">The connectors.</param>
        public TowerViewModel(Point position, MainCanvasModel canvasModel, bool showCoverageRadius, Canvas rootCanvas, GeometryGroup connectors) : this()
        {
            _canvasModel = canvasModel;
            ShowCoverageRadius = showCoverageRadius;

            SetPosition(position);

            _rootCanvas = rootCanvas;
            _connectors = connectors;
            TowerModel.NodeId = canvasModel.TowerNodeIdCount;
            NodeId = canvasModel.TowerNodeIdCount.ToString();

            //check to see if the tower placed is in a coverage zone, if it is, update the links
            TowerModel.checkCoverageCollision(_rootCanvas, this.TowerModel, _connectors);

            TowerModel.UpdateLinks(CanvasLeft, CanvasTop, 300, 300, TowerModel);

            TowerModel.checkCoverageCollision(_rootCanvas, this.TowerModel, _connectors);
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="pos">The position.</param>
        public void SetPosition(Point pos)
        {
            CanvasLeft = pos.X;
            CanvasTop = pos.Y;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show coverage radius].
        /// </summary>
        /// <value><c>true</c> if [show coverage radius]; otherwise, <c>false</c>.</value>
        public bool ShowCoverageRadius
        {
            get
            {
                return _showCoverageRadius;
            }

            set
            {
                if (value == _showCoverageRadius) return;

                _showCoverageRadius = value;
                OnPropertyChanged("ShowCoverageRadius");
            }
        }

        /// <summary>
        /// Gets or sets the canvas top.
        /// </summary>
        /// <value>The canvas top.</value>
        public double CanvasTop
        {
            get
            {
                return _canvasTop;
            }

            set
            {
                if (value == _canvasTop) return;

                TowerModel.CanvasTop = value;
                _canvasTop = value;
                OnPropertyChanged("CanvasTop");
            }
        }

        /// <summary>
        /// Gets or sets the canvas left.
        /// </summary>
        /// <value>The canvas left.</value>
        public double CanvasLeft
        {
            get
            {
                return _canvasLeft;
            }

            set
            {
                if (value == _canvasLeft) return;

                TowerModel.CanvasLeft = value; 
                _canvasLeft = value;
                OnPropertyChanged("CanvasLeft");
            }
        }

        /// <summary>
        /// Gets or sets the node identifier.
        /// </summary>
        /// <value>The node identifier.</value>
        public string NodeId
        {
            get
            {
                return _nodeId; 
            }

            set
            {
                if (value == _nodeId) return;

                _nodeId = "Tower " + value;
                OnPropertyChanged("NodeId");
            }
        }
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Drags the node.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (_canvasModel.IsAddNewLink || _canvasModel.IsDeleteActive) return;

            UIElement drone = e.Source as UIElement;
            //update the position of the node
            CanvasLeft += e.HorizontalChange;
            CanvasTop += e.VerticalChange;
            //update links and check for new coverage collision 
            TowerModel.UpdateLinks(CanvasLeft, CanvasTop, this.ActualWidth, this.ActualHeight, TowerModel);

            TowerModel.checkCoverageCollision(_rootCanvas, this.TowerModel, _connectors);
        }

    }
}
