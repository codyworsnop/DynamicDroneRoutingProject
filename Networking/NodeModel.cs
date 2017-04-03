/// <summary>
/// NodeModel.cs
/// </summary>
namespace Networking
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// NodeModel is a generic node used to inheritance in other objects added to the canvas
    /// </summary>
    /// 
    public class NodeModel : Thumb
    {
        /// <summary>
        /// The title property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(DroneModel), new UIPropertyMetadata(""));
        /// <summary>
        /// The image source property
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(string), typeof(DroneModel), new UIPropertyMetadata(""));

        /// <summary>
        /// Gets or sets the end lines.
        /// </summary>
        /// <value>The end lines.</value>
        public List<LineGeometry> EndLines { get; set; }
        /// <summary>
        /// Gets or sets the start lines.
        /// </summary>
        /// <value>The start lines.</value>
        public List<LineGeometry> StartLines { get; set; }
        /// <summary>
        /// Gets or sets the connections.
        /// </summary>
        /// <value>The connections.</value>
        public List<NodeModel> connections { get; set; }
        /// <summary>
        /// Gets or sets the congestion.
        /// </summary>
        /// <value>The congestion.</value>
        public double Congestion { get; set; }
        /// <summary>
        /// Gets or sets the node identifier.
        /// </summary>
        /// <value>The node identifier.</value>
        public int NodeId { get; set; }

        /// <summary>
        /// canvas top value for y coordinate value 
        /// </summary>
        public double CanvasTop { get; set; }

        /// <summary>
        /// canvasleft used 
        /// </summary>
        public double CanvasLeft { get; set; }

        /// <summary>
        /// gets or sets the title property
        /// </summary>
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, Title);
            }
        }

        /// <summary>
        /// gets or sets the imagesource property
        /// </summary>
        /// <value>The image source.</value>
        public string ImageSource
        {
            get
            {
                return (string)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, ImageSource);
            }
        }

        /// <summary>
        /// sets the dependency properties
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.Title != string.Empty)
            {
                TextBlock text = this.Template.FindName("Text", this) as TextBlock;
                if (text != null)
                {
                    text.Text = Title;
                }
            }

            if (this.ImageSource != string.Empty)
            {
                Image img = this.Template.FindName("Image", this) as Image;
                if (img != null)
                {
                    img.Source = new BitmapImage(new Uri(this.ImageSource, UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// UpdateLinks updates the start and end positions of the lines connecting to a nodel object 
        /// </summary>
        /// <param name="canvasLeft">The x coordinate for the canvas</param>
        /// <param name="canvasTop">The y coordinate for the canvas</param>
        /// <param name="width">The width of the viewModel</param>
        /// <param name="height">The height of the viewmodel</param>
        /// <param name="target">The object being dragged</param>
        /// TODO: this code can be optimized
        public void UpdateLinks(double canvasLeft, double canvasTop, double width, double height, UIElement target)
        {
            if (target.GetType() == typeof(DroneModel))
            {
                var node = target as DroneModel;

                if (node.StartLines == null || node.EndLines == null) return;

                foreach (LineGeometry startLine in node.StartLines)
                {
                    var val = new Point(canvasLeft + width / 2, canvasTop + height / 2);
                    startLine.StartPoint = val;
                }

                foreach (LineGeometry endLine in node.EndLines)
                {
                    var val = new Point(canvasLeft + width / 2, canvasTop + height / 2);
                    endLine.EndPoint = val;
                }
            }
            else
            {
                var node = target as TowerModel;

                if (node.StartLines == null || node.EndLines == null) return;

                foreach (LineGeometry startLine in node.StartLines)
                {
                    var val = new Point(canvasLeft + width / 2, canvasTop + height / 2);
                    startLine.StartPoint = val;
                }

                foreach (LineGeometry endLine in node.EndLines)
                {
                    var val = new Point(canvasLeft + width / 2, canvasTop + height / 2);
                    endLine.EndPoint = val;
                }
            }
        }

        /// <summary>
        /// Ares the nodes linked.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if nodes are linked, <c>false</c> otherwise.</returns>
        public bool AreNodesLinked(NodeModel target)
        {
            bool isLinked = false;

            foreach (LineGeometry line in this.StartLines)
            {
                if (target.EndLines.Exists(x => x.Bounds == line.Bounds))
                {
                    isLinked = true;
                }
            }

            foreach (LineGeometry line in this.EndLines)
            {
                if (target.StartLines.Exists(x => x.Bounds == line.Bounds))
                {
                    isLinked = true;
                }
            }

            return isLinked;
        }

        /// <summary>
        /// Checks the coverage collision.
        /// </summary>
        /// <param name="rootCanvas">The root canvas.</param>
        /// <param name="selectedDrone">The selected drone.</param>
        /// <param name="connectors">The connectors.</param>
        public void checkCoverageCollision(Canvas rootCanvas, NodeModel selectedDrone, GeometryGroup connectors)
        {
            // determine if collision has happened
            foreach (var child in rootCanvas.Children)
            {
                NodeModel targetChild = null;
                if (child.GetType() == typeof(DroneViewModel) || child.GetType() == typeof(TowerViewModel))
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


                    if (!targetChild.Equals(selectedDrone))
                    {
                        double distance = Math.Sqrt(Math.Pow(targetChild.CanvasLeft - selectedDrone.CanvasLeft, 2) + Math.Pow(targetChild.CanvasTop - selectedDrone.CanvasTop, 2));
                        if (distance < Constants.LTE_RADIUS && !selectedDrone.AreNodesLinked(targetChild))
                        {
                            //setup new line 
                            var link = new LineModel();
                            selectedDrone.LinkToNode(selectedDrone, targetChild, ref link.Line, Constants.LTE_RADIUS, Constants.LTE_RADIUS, CanvasLeft, CanvasTop,
                                Constants.LTE_RADIUS, Constants.LTE_RADIUS, targetChild.CanvasLeft, targetChild.CanvasTop);
                            connectors.Children.Add(link.Line);
                            connections.Add(targetChild);
                            targetChild.connections.Add(this);

                        }
                        //checks to see if the distance of the node is greater than the lte distance. If it is, un link the nodes
                        else if (distance >= Constants.LTE_RADIUS && selectedDrone.AreNodesLinked(targetChild))
                        {
                            Geometry lineToRemove = null;

                            //remove start lines from list and canvas
                            foreach (LineGeometry line in selectedDrone.StartLines.ToArray())
                            {
                                List<Geometry> lines = new List<Geometry>(targetChild.EndLines);
                                lineToRemove = lines.Find(x => x.Bounds == line.Bounds);

                                if (lineToRemove != null)
                                {
                                    connectors.Children.Remove(lineToRemove);
                                }

                                var lineData = lineToRemove as LineGeometry;
                                selectedDrone.StartLines.Remove(lineData);
                                selectedDrone.EndLines.Remove(lineData);
                                targetChild.StartLines.Remove(lineData);
                                targetChild.EndLines.Remove(lineData);
                                selectedDrone.connections.Remove(targetChild);
                                targetChild.connections.Remove(selectedDrone);
                            }
                            //remove end lines from list and canvas
                            foreach (LineGeometry line in selectedDrone.EndLines.ToArray())
                            {
                                List<Geometry> lines = new List<Geometry>(targetChild.StartLines);
                                lineToRemove = lines.Find(x => x.Bounds == line.Bounds);

                                if (lineToRemove != null)
                                {
                                    connectors.Children.Remove(lineToRemove);
                                }

                                var lineData = lineToRemove as LineGeometry;
                                selectedDrone.StartLines.Remove(lineData);
                                selectedDrone.EndLines.Remove(lineData);
                                targetChild.StartLines.Remove(lineData);
                                targetChild.EndLines.Remove(lineData);
                                selectedDrone.connections.Remove(targetChild);
                                targetChild.connections.Remove(selectedDrone);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// LinkToNode links a node object to another node object visually. 
        /// </summary>
        /// <param name="node">The starting connection node</param>
        /// <param name="target">The ending connection node</param>
        /// <param name="line">The line connecting the nodes</param>
        /// <param name="width">The width of the node object</param>
        /// <param name="height">The height of the node object</param>
        /// <param name="canvasLeft">The x coordinate of the node object</param>
        /// <param name="canvasTop">The y coordinate of the node object</param>
        /// <param name="targetWidth">The width of the target node object</param>
        /// <param name="targetHeight">The height of the target node object</param>
        /// <param name="targetCanvasLeft">The x coordinate of the target node object</param>
        /// <param name="targetCanvasTop">The y coordinate of the target node object</param>
        public void LinkToNode(NodeModel node, NodeModel target, ref LineGeometry line, double width, double height, double canvasLeft, double canvasTop,
            double targetWidth, double targetHeight, double targetCanvasLeft, double targetCanvasTop)
        {
            node.StartLines.Add(line);
            target.EndLines.Add(line);

            node.UpdateLayout();
            target.UpdateLayout();
            //start the line point at the center of the node
            line.StartPoint = new Point(canvasLeft + width / 2, canvasTop + height / 2);
            //end the line point at the cneter of the target node.
            line.EndPoint = new Point(targetCanvasLeft + targetWidth / 2, targetCanvasTop + targetHeight / 2);
        }
    }
}
