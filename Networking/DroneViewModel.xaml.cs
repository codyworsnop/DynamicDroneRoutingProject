/// <summary>
/// DroneViewModel.xaml.cs
/// </summary>
namespace Networking
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;


    /// <summary>
    /// Interaction logic for DroneViewModel.xaml
    /// </summary>
    public partial class DroneViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// canvas model used to get canvas model constants
        /// </summary>
        private MainCanvasModel _canvasModel;

        /// <summary>
        /// bool to determine if the coverage map should be on 
        /// </summary>
        private bool _showCoverageRadius;

        /// <summary>
        /// canvas top value for y coordinate value 
        /// </summary>
        private double _canvasTop;

        /// <summary>
        /// canvasleft used 
        /// </summary>
        private double _canvasLeft;

        /// <summary>
        /// drone model used to target the drone object 
        /// </summary>
        public DroneModel DroneModel;

        /// <summary>
        /// propertychanged event used to update xaml elements
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The root canvas
        /// </summary>
        private Canvas _rootCanvas;

        /// <summary>
        /// The connectors
        /// </summary>
        private GeometryGroup _connectors;

        /// <summary>
        /// The command model
        /// </summary>
        private CommandCenterViewModel _commandModel;

        /// <summary>
        /// The node identifier
        /// </summary>
        private string _nodeId;

        /// <summary>
        /// Occurs when [warning event].
        /// </summary>
        public event WarningEventHandler WarningEvent;

        /// <summary>
        /// Occurs when [loading event].
        /// </summary>
        public event LoadingEventHandler LoadingEvent;

        /// <summary>
        /// Occurs when [end asynchronous event].
        /// </summary>
        public event AsyncCallback EndAsyncEvent;

        /// <summary>
        /// Delegate WarningEventHandler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public delegate void WarningEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Delegate LoadingEventHandler
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        public delegate void LoadingEventHandler(bool isEnabled); 

        /// <summary>
        /// Default constructor. Do not use. 
        /// </summary>
        public DroneViewModel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// constructor. 
        /// </summary>
        /// <param name="canvasModel">used to update the local canvas object</param>
        /// <param name="position">used to position the drone in the canvas</param>
        /// <param name="showCoverage">used to set if the coverage map should be enabled</param>
        public DroneViewModel(MainCanvasModel canvasModel, Point position, bool showCoverage, Canvas rootCanvas, GeometryGroup connectors, CommandCenterViewModel commandModel)
        {
            //init component, set data conext for xaml
            InitializeComponent();
            DataContext = this;

            
            //update the canvas element, create new drone
            _canvasModel = canvasModel;
            DroneModel = new DroneModel();
            SetPosition(position);
            _showCoverageRadius = showCoverage;
            _rootCanvas = rootCanvas;
            _connectors = connectors;
            _commandModel = commandModel;

            DroneModel.NodeId = canvasModel.NodeIdCount;
            NodeId = canvasModel.NodeIdCount.ToString(); 
            
            //check if the new drone is in range of another Node, and if it is, link the two nodes
            DroneModel.checkCoverageCollision(_rootCanvas, this.DroneModel, _connectors);

            DroneModel.UpdateLinks(CanvasLeft, CanvasTop, 300, 300, DroneModel);

            DroneModel.checkCoverageCollision(_rootCanvas, this.DroneModel, _connectors);

        }

        /// <summary>
        /// Updates the target Drone being dragged by the user.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The Node being dragged</param>
        private void Drone_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (_canvasModel.IsAddNewLink || _canvasModel.IsDeleteActive) return;
            //saves new position
            CanvasLeft += e.HorizontalChange;
            CanvasTop += e.VerticalChange;
            //Update links and checks if the Node is in a new collision zone
            UpdateLinks();

            DroneModel.checkCoverageCollision(_rootCanvas, this.DroneModel, _connectors);
        }

        /// <summary>
        ///Updates links of the drone being placed or dragged
        /// </summary>
        public void UpdateLinks()
        {
            DroneModel.UpdateLinks(CanvasLeft, CanvasTop, this.ActualWidth, this.ActualHeight, DroneModel);
        }

        /// <summary>
        /// Links the drone to the command center
        /// </summary>
        /// <param name="target"></param>
        /// <param name="line"></param>
        public void LinkToCommand(CommandCenterViewModel target, LineGeometry line)
        {
            DroneModel.LinkToNode(DroneModel, target.CommandModel, ref line, this.ActualWidth, this.ActualHeight, CanvasLeft, CanvasTop,
                target.ActualWidth, target.ActualHeight, target.CanvasLeft, target.CanvasTop);
        }
        /// <summary>
        /// Links to tower.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="line">The line.</param>
        public void LinkToTower(TowerViewModel target, LineGeometry line)
        {
            DroneModel.LinkToNode(DroneModel, target.TowerModel, ref line, this.ActualWidth, this.ActualHeight, CanvasLeft, CanvasTop,
                target.ActualWidth, target.ActualHeight, target.CanvasLeft, target.CanvasTop);

        }

        /// <summary>
        /// Links to drone.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="line">The line.</param>
        public void LinkToDrone(DroneViewModel target, ref LineGeometry line)
        {
            DroneModel.LinkToNode(DroneModel, target.DroneModel, ref line, this.ActualWidth, this.ActualHeight, CanvasLeft, CanvasTop,
                target.ActualWidth, target.ActualHeight, target.CanvasLeft, target.CanvasTop);

            DroneModel.connections.Add(target.DroneModel);
            target.DroneModel.connections.Add(this.DroneModel);
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

                DroneModel.CanvasTop = value; 
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

                DroneModel.CanvasLeft = value;
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

                _nodeId = "Drone " + value;
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
        /// Sets the position.
        /// </summary>
        /// <param name="pos">The position.</param>
        private void SetPosition(Point pos)
        {
            CanvasLeft = pos.X;
            CanvasTop = pos.Y;        
        }

        /// <summary>
        /// Routes the event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void RouteEventHandler(object sender, RoutedEventArgs e)
        {
            var obj = e.Source as MenuItem;
            var droneModel = obj.DataContext as DroneViewModel;
            var model = droneModel.DroneModel;

            var childIndexToFind = _rootCanvas.Children.IndexOf(_commandModel);
            var childToFindView = _rootCanvas.Children[childIndexToFind] as CommandCenterViewModel;
            var childToFind = childToFindView.CommandModel;

            //this may take awhile
            var bestPath = RoutePlanner.PlanPath(DroneModel, childToFind);

            //if no path exists
            if (bestPath == null)
            {
                WarningEvent(sender, e);
                return;
            }

            var resultWindow = new ResultWindow(bestPath);

            resultWindow.Show();
        }


    }
}

