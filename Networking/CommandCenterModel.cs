/// <summary>
/// CommandCenterModal.cs
/// </summary>
namespace Networking
{
    using System.Collections.Generic;
    using System.Windows.Media;

    /// <summary>
    /// This class model the command center modal
    /// </summary>
    public class CommandCenterModel : NodeModel
    {
        /// <summary>
        /// Default constructor. Initializes inherited start and end lines. 
        /// </summary>
        public CommandCenterModel()
        {
            StartLines = new List<LineGeometry>();
            EndLines = new List<LineGeometry>();
        }
    }
}
