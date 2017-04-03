/// <summary>
/// DroneModel.cs
/// </summary>
namespace Networking
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls.Primitives;
    using System.Windows.Controls;
    
    /// <summary>
    /// Dronemodel class used to model the drone object 
    /// </summary>
    public class DroneModel : NodeModel
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DroneModel()
        {
            StartLines = new List<LineGeometry>();
            EndLines = new List<LineGeometry>();
            connections = new List<NodeModel>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="title">The title.</param>
        /// <param name="source">The source.</param>
        public DroneModel(ControlTemplate template, string title, string source) : this()
        {
            this.Template = template;
            this.Title = title;
            this.ImageSource = source;

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="title">The title.</param>
        /// <param name="imageSource">The image source.</param>
        /// <param name="position">The position.</param>
        /// <param name="dragDelta">The drag delta.</param>
        public DroneModel(ControlTemplate template, string title, string imageSource, Point position, DragDeltaEventHandler dragDelta) : this(template, title, imageSource)
        {
            this.DragDelta += dragDelta;
        }
    }
}
