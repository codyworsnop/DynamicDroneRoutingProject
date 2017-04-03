/// <summary>
/// TowerModel.cs
/// </summary>
namespace Networking
{
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// The towerModel class used to model the tower object. 
    /// </summary>
    public class TowerModel : NodeModel
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public TowerModel()
        {
            StartLines = new List<LineGeometry>();
            EndLines = new List<LineGeometry>();
            connections = new List<NodeModel>();
        }
    }
}
