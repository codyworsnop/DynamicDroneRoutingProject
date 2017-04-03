/// <summary>
/// MainWindow.xaml.cs
/// </summary>
namespace Networking
{
    using MahApps.Metro.Controls.Dialogs;
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {

        /// <summary>
        /// The loading flyout is open
        /// </summary>
        private bool _loadingFlyoutIsOpen;

        /// <summary>
        /// The canvas model
        /// </summary>
        private MainCanvasModel _canvasModel;

        /// <summary>
        /// The drone view
        /// </summary>
        private DroneViewModel _droneView;

        /// <summary>
        /// The command model
        /// </summary>
        private CommandCenterViewModel _commandModel;

        /// <summary>
        /// The show coverage radius
        /// </summary>
        private bool _showCoverageRadius;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the mainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            CanvasModel = new MainCanvasModel();
            _commandModel = new CommandCenterViewModel();
            //create new drone at point 50, 50
            _droneView = new DroneViewModel(_canvasModel, new Point(50, 50), _showCoverageRadius, RootCanvas, connectors, _commandModel);

            RootCanvas.Children.Add(_droneView);
            RootCanvas.Children.Add(_commandModel);

            //subscribe to warning event if path does not exist 
            _droneView.WarningEvent += new DroneViewModel.WarningEventHandler(WarningEventHandler);
            _droneView.LoadingEvent += new DroneViewModel.LoadingEventHandler(LoadingEventHandler);

        }

        /// <summary>
        /// Gets or sets the canvas model.
        /// </summary>
        /// <value>The canvas model.</value>
        public MainCanvasModel CanvasModel
        {
            get
            {
                return _canvasModel;
            }

            set
            {
                _canvasModel = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [loading flyout is open].
        /// </summary>
        /// <value><c>true</c> if [loading flyout is open]; otherwise, <c>false</c>.</value>
        public bool LoadingFlyoutIsOpen
        {
            get
            {
                return _loadingFlyoutIsOpen;           
            }

            set
            {
                if (value == _loadingFlyoutIsOpen) return;

                _loadingFlyoutIsOpen = value;
                OnPropertyChanged("LoadingFlyoutIsOpen"); 
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
        /// BTNs the new drone click handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The  instance containing the event data.</param>
        private void BtnNewDroneClickHandler(object sender, RoutedEventArgs e)
        {
            CanvasModel.IsAddNewDrone = true;
            SetCursorAndButtons(Cursors.Cross);
            
        }

        /// <summary>
        /// BTNs the drone link click handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void BtnDroneLinkClickHandler(object sender, RoutedEventArgs e)
        {
            CanvasModel.IsAddNewLink = true;
            SetCursorAndButtons(Cursors.Pen);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanvasModel.IsAddNewDrone)
            {
                //place a node at the position of the mouse
                var position = e.GetPosition(this);
                position.X -= _droneView.ActualWidth / 2;
                position.Y -= _droneView.ActualHeight / 2 + 30;
                //increase the amount of nodes that are in the window
                CanvasModel.NodeIdCount++;
                DroneViewModel drone = new DroneViewModel(_canvasModel, position, _showCoverageRadius, RootCanvas, connectors, _commandModel);

                //subscribe to warning event if path does not exist 
                drone.WarningEvent += new DroneViewModel.WarningEventHandler(WarningEventHandler);
                drone.LoadingEvent += new DroneViewModel.LoadingEventHandler(LoadingEventHandler);

                //add the drone
                RootCanvas.Children.Add(drone);
                CanvasModel.IsAddNewDrone = false;
                ResetCursorAndButtons();
                e.Handled = true;
                

            }
            //check to see if tower is being placed
            if (CanvasModel.IsAddTowerActive)
            {
                //get position of click
                var position = e.GetPosition(this);
                position.X -= _droneView.ActualWidth / 2;
                position.Y -= _droneView.ActualHeight / 2 + 30;

                //setup new line 
                CanvasModel.Link = new LineModel();

                
                TowerViewModel tower = new TowerViewModel(position, _canvasModel, _showCoverageRadius, RootCanvas, connectors);

                //link the new tower to the command 
                tower.TowerModel.LinkToNode(tower.TowerModel, _commandModel.CommandModel, ref _canvasModel.Link.Line, tower.ActualWidth, tower.ActualHeight, tower.CanvasLeft, tower.CanvasTop,
                    _commandModel.ActualWidth, _commandModel.ActualHeight, _commandModel.CanvasLeft, _commandModel.CanvasTop);


                tower.TowerModel.connections.Add(_commandModel.CommandModel); 

                //add the children to the canvas
                RootCanvas.Children.Add(tower);
                connectors.Children.Add(_canvasModel.Link.Line);

                //reset cursor 
                CanvasModel.IsAddTowerActive = false;
                ResetCursorAndButtons();
                CanvasModel.TowerNodeIdCount++;

                e.Handled = true;
            }

            //chck to see if the node is a drone or command ceneter
            if (CanvasModel.IsAddNewLink && e.Source.GetType() == typeof(DroneViewModel) || e.Source.GetType() == typeof(CommandCenterViewModel))
            {

                if (!CanvasModel.IsLinkStarted)
                {
                    if (CanvasModel.Link == null || CanvasModel.Link.Line.EndPoint != CanvasModel.Link.Line.StartPoint)
                    {
                        //get the position of the node
                        Point position = e.GetPosition(this);
                        position.Y -= 30;
                        //create a new link
                        CanvasModel.Link = new LineModel(position, position);
                        connectors.Children.Add(CanvasModel.Link.Line);
                        CanvasModel.IsLinkStarted = true;
                        //check to see if it is a drone
                        if (e.Source.GetType() == typeof(DroneViewModel))
                        {
                            CanvasModel.LinkedDrone = e.Source as DroneViewModel;
                            CanvasModel.LinkedCommandCenter = null;
                        }
                        //check to see if tis a command center
                        else
                        {
                            CanvasModel.LinkedCommandCenter = e.Source as CommandCenterViewModel;
                            CanvasModel.LinkedDrone = null;
                        }

                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonUp event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The  instance containing the event data.</param>
        private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //check to see if a new link is being placed
            if (CanvasModel.IsAddNewLink && CanvasModel.IsLinkStarted)
            {
                bool linked = false;
                //check to see if the source is the command center
                if (e.Source.GetType() == typeof(CommandCenterViewModel))
                {
                    CommandCenterViewModel targetNode = e.Source as CommandCenterViewModel;

                    CanvasModel.Link.Line.EndPoint = e.GetPosition(this);

                    if (CanvasModel.Link.Line.EndPoint != CanvasModel.Link.Line.StartPoint && CanvasModel.LinkedCommandCenter != targetNode)
                    {
                        CanvasModel.LinkedDrone.LinkToCommand(targetNode, CanvasModel.Link.Line);
                        linked = true;
                    }
                }

                //check to see if the item being placed is a drone model
                if (e.Source.GetType() == typeof(DroneViewModel))
                {
                    DroneViewModel targetNode = e.Source as DroneViewModel;

                    CanvasModel.Link.Line.EndPoint = e.GetPosition(this);
                    //place the drone on the canvas
                    if (CanvasModel.Link.Line.EndPoint != CanvasModel.Link.Line.StartPoint && CanvasModel.LinkedDrone != targetNode)
                    {
                        CanvasModel.LinkedDrone.LinkToDrone(targetNode, ref CanvasModel.Link.Line);
                      
                        linked = true;
                    }
                }
                //link up the new node 
                if (!linked)
                {
                    connectors.Children.Remove(CanvasModel.Link.Line);
                    CanvasModel.Link = null;
                }

                CanvasModel.IsLinkStarted = CanvasModel.IsAddNewLink = false;
                ResetCursorAndButtons();
                e.Handled = true;

            }

            //if delete is active
            if (CanvasModel.IsDeleteActive)
            {
                //check if it is a drone model
                if (e.Source.GetType() == typeof(DroneViewModel))
                {
                    var selectedDrone  = e.Source as DroneViewModel;
                    //loop through everything on canvas starting at root
                    foreach (var child in RootCanvas.Children)
                    {
                        NodeModel targetChild = null;
                        //checks to see the child is a drone or tower
                        if (child.GetType() == typeof(DroneViewModel))
                        {
                            if (child.GetType() == typeof(DroneViewModel))
                            {
                                var view = child as DroneViewModel;
                                targetChild = view.DroneModel as NodeModel;
                            }
                            else
                            {
                                var view = child as TowerViewModel;
                                targetChild = view.TowerModel as NodeModel;
                            }
                            Geometry lineToRemove = null;

                            //remove the start lines from the  list
                            foreach (LineGeometry line in selectedDrone.DroneModel.StartLines.ToArray())
                            {
                                List<Geometry> lines = new List<Geometry>(targetChild.EndLines);
                                lineToRemove = lines.Find(x => x.Bounds == line.Bounds);

                                if (lineToRemove != null)
                                {
                                    connectors.Children.Remove(lineToRemove);
                                }

                                var lineData = lineToRemove as LineGeometry;
                                selectedDrone.DroneModel.StartLines.Remove(lineData);
                                selectedDrone.DroneModel.EndLines.Remove(lineData);
                                targetChild.StartLines.Remove(lineData);
                                targetChild.EndLines.Remove(lineData);
                                selectedDrone.DroneModel.connections.Remove(targetChild);
                                targetChild.connections.Remove(selectedDrone.DroneModel);
                            }
                            //remove end lines from the list
                            foreach (LineGeometry line in selectedDrone.DroneModel.EndLines.ToArray())
                            {
                                List<Geometry> lines = new List<Geometry>(targetChild.StartLines);
                                lineToRemove = lines.Find(x => x.Bounds == line.Bounds);

                                if (lineToRemove != null)
                                {
                                    connectors.Children.Remove(lineToRemove);
                                }

                                var lineData = lineToRemove as LineGeometry;
                                selectedDrone.DroneModel.StartLines.Remove(lineData);
                                selectedDrone.DroneModel.EndLines.Remove(lineData);
                                targetChild.StartLines.Remove(lineData);
                                targetChild.EndLines.Remove(lineData);
                                selectedDrone.DroneModel.connections.Remove(targetChild);
                                targetChild.connections.Remove(selectedDrone.DroneModel);
                            }
                        }
                    }
                    //remove the start lines from the canvas
                    foreach (LineGeometry line in selectedDrone.DroneModel.StartLines)
                    {
                        var lineToRemove = connectors.Children.Where(x => x.Bounds == line.Bounds);
                        connectors.Children.Remove(lineToRemove.FirstOrDefault());
                    }
                    //remove the end lines from the canvas
                    foreach (LineGeometry line in selectedDrone.DroneModel.EndLines)
                    {
                        var lineToRemove = connectors.Children.Where(x => x.Bounds == line.Bounds);
                        connectors.Children.Remove(lineToRemove.FirstOrDefault());
                    }
                    RootCanvas.Children.Remove(selectedDrone);
                    selectedDrone.DroneModel.connections.Clear();
                    selectedDrone.DroneModel.StartLines.Clear();
                    selectedDrone.DroneModel.EndLines.Clear();
                 }
                //check to see if it's a tower
                else if (e.Source.GetType() == typeof(TowerViewModel))
                {
                    var selectedDrone = e.Source as TowerViewModel;
                    //loop through all nodes on canvas starting at root
                    foreach (var child in RootCanvas.Children)
                    {
                        NodeModel targetChild = null;
                        //checks to seee if the child is a drone or a tower
                        if (child.GetType() == typeof(DroneViewModel))
                        {
                            if (child.GetType() == typeof(DroneViewModel))
                            {
                                var view = child as DroneViewModel;
                                targetChild = view.DroneModel as NodeModel;
                            }
                            else
                            {
                                var view = child as TowerViewModel;
                                targetChild = view.TowerModel as NodeModel;
                            }
                            Geometry lineToRemove = null;

                            //remove start lines from the list
                            foreach (LineGeometry line in selectedDrone.TowerModel.StartLines.ToArray())
                            {
                                List<Geometry> lines = new List<Geometry>(targetChild.EndLines);
                                lineToRemove = lines.Find(x => x.Bounds == line.Bounds);

                                if (lineToRemove != null)
                                {
                                    connectors.Children.Remove(lineToRemove);
                                }

                                var lineData = lineToRemove as LineGeometry;
                                selectedDrone.TowerModel.StartLines.Remove(lineData);
                                selectedDrone.TowerModel.EndLines.Remove(lineData);
                                targetChild.StartLines.Remove(lineData);
                                targetChild.EndLines.Remove(lineData);
                                selectedDrone.TowerModel.connections.Remove(targetChild);
                                targetChild.connections.Remove(selectedDrone.TowerModel);
                            }
                            //remove end lines from the list
                            foreach (LineGeometry line in selectedDrone.TowerModel.EndLines.ToArray())
                            {
                                List<Geometry> lines = new List<Geometry>(targetChild.StartLines);
                                lineToRemove = lines.Find(x => x.Bounds == line.Bounds);

                                if (lineToRemove != null)
                                {
                                    connectors.Children.Remove(lineToRemove);
                                }

                                var lineData = lineToRemove as LineGeometry;
                                selectedDrone.TowerModel.StartLines.Remove(lineData);
                                selectedDrone.TowerModel.EndLines.Remove(lineData);
                                targetChild.StartLines.Remove(lineData);
                                targetChild.EndLines.Remove(lineData);
                                selectedDrone.TowerModel.connections.Remove(targetChild);
                                targetChild.connections.Remove(selectedDrone.TowerModel);
                            }
                        }
                    }
                    //remove the start lines from the canvas
                    foreach (LineGeometry line in selectedDrone.TowerModel.StartLines)
                    {
                        var lineToRemove = connectors.Children.Where(x => x.Bounds == line.Bounds);
                        connectors.Children.Remove(lineToRemove.FirstOrDefault());
                    }
                    //remove the end lines from the canvas
                    foreach (LineGeometry line in selectedDrone.TowerModel.EndLines)
                    {
                        var lineToRemove = connectors.Children.Where(x => x.Bounds == line.Bounds);
                        connectors.Children.Remove(lineToRemove.FirstOrDefault());
                    }
                    RootCanvas.Children.Remove(selectedDrone);
                    selectedDrone.TowerModel.connections.Clear();
                    selectedDrone.TowerModel.StartLines.Clear();
                    selectedDrone.TowerModel.EndLines.Clear();
                }

                CanvasModel.IsDeleteActive = false;
            }

            ResetCursorAndButtons();
        }

        /// <summary>
        /// Handles the PreviewMouseMove event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (CanvasModel.IsAddNewLink && CanvasModel.IsLinkStarted)
            {
                var pos = e.GetPosition(this);
                pos.Y -= 30;
                CanvasModel.Link.Line.EndPoint = pos;
                
                e.Handled = true;
                
            }            
        }

        /// <summary>
        /// BTNs the delete handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The  instance containing the event data.</param>
        private void BtnDeleteHandler(object sender, RoutedEventArgs e)
        {
            CanvasModel.IsDeleteActive = true;
            Mouse.OverrideCursor = Cursors.Hand;
            BtnnNewDrone.IsEnabled = BtnNewLink.IsEnabled = BtnDelete.IsEnabled = BtnNewTower.IsEnabled = false;
        }

        /// <summary>
        /// Informations the BTN handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void InfoBtnHandler(object sender, RoutedEventArgs e)
        {
            InformationWindow informationWindow = new InformationWindow();
            informationWindow.Show();
        }

        /// <summary>
        /// Helps the BTN handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void HelpBtnHandler(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        /// <summary>
        /// Sets the cursor and buttons.
        /// </summary>
        /// <param name="type">The type.</param>
        private void SetCursorAndButtons(Cursor type)
        {
            Mouse.OverrideCursor = type;
            BtnnNewDrone.IsEnabled = BtnNewLink.IsEnabled = BtnDelete.IsEnabled = BtnNewTower.IsEnabled = false;
        }

        /// <summary>
        /// Resets the cursor and buttons.
        /// </summary>
        private void ResetCursorAndButtons()
        {
            Mouse.OverrideCursor = null;
            BtnnNewDrone.IsEnabled = BtnNewLink.IsEnabled = BtnDelete.IsEnabled = BtnNewTower.IsEnabled = true;
        }

        /// <summary>
        /// Connections the map checked handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void ConnectionMapCheckedHandler(object sender, RoutedEventArgs e)
        {
            //update the existing children
            foreach (var child in RootCanvas.Children)
            {
                if (child.GetType() == typeof(DroneViewModel))
                {
                    var drone = child as DroneViewModel;
                    var val = (ConnectionMapCheckBox.IsChecked == true) ? true : false;
                    drone.ShowCoverageRadius = _showCoverageRadius = val;

                }
                else if (child.GetType() == typeof(TowerViewModel))
                {
                    var tower = child as TowerViewModel;
                    var val = (ConnectionMapCheckBox.IsChecked == true) ? true : false;
                    tower.ShowCoverageRadius = _showCoverageRadius = val;
                }
            }
        }

        /// <summary>
        /// Adds the tower click event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void AddTowerClickEventHandler(object sender, RoutedEventArgs e)
        {
            CanvasModel.IsAddTowerActive = true;
            Mouse.OverrideCursor = Cursors.Cross;
            BtnnNewDrone.IsEnabled = BtnNewLink.IsEnabled = BtnDelete.IsEnabled = BtnNewTower.IsEnabled = false;
        }

        /// <summary>
        /// Warnings the event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void WarningEventHandler(object sender, EventArgs e)
        {
            alertNoPath();
        }

        /// <summary>
        /// Loadings the event handler.
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        private void LoadingEventHandler(bool isEnabled)
        {           
            LoadingFlyoutIsOpen = IsEnabled; 
        }

        /// <summary>
        /// Alerts the no path.
        /// </summary>
        private async void alertNoPath()
        {
            await this.ShowMessageAsync("Warning", "The selected drone to find a route for does not have a possible path to the command center");
        }
    }
}

