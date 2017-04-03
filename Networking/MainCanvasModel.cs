
/// <summary>
/// The Networking namespace.
/// </summary>
namespace Networking
{
    /// <summary>
    /// Class MainCanvasModel.
    /// </summary>
    public class MainCanvasModel
    {
        public bool IsAddNewLink;
        public bool IsLinkStarted;
        public bool IsAddNewDrone;
        public bool IsDeleteActive;
        public bool IsAddTowerActive; 

        private bool _LTEChecked;
        private bool _bluetoothChecked;
        private bool _WifiChecked;

        private DroneViewModel _linkedDrone;
        private LineModel _link;
        private CommandCenterViewModel _linkedCommandCenter;

        public int NodeIdCount = 1;
        public int TowerNodeIdCount = 1;

        /// <summary>
        /// default constructor
        /// </summary>
        public MainCanvasModel()
        {

        }

        /// <summary>
        /// Gets or sets the linked drone.
        /// </summary>
        /// <value>The linked drone.</value>
        public DroneViewModel LinkedDrone
        {
            get
            {
                return _linkedDrone;
            }

            set
            {
                _linkedDrone = value;
            }
        }

        /// <summary>
        /// Gets or sets the linked command center.
        /// </summary>
        /// <value>The linked command center.</value>
        public CommandCenterViewModel LinkedCommandCenter
        {
            get
            {
                return _linkedCommandCenter;
            }

            set
            {
                _linkedCommandCenter = value; 
            }
        }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        /// <value>The link.</value>
        public LineModel Link
        {
            get
            {
                return _link;
            }

            set
            {
                _link = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [wifi checked].
        /// </summary>
        /// <value><c>true</c> if [wifi checked]; otherwise, <c>false</c>.</value>
        public bool WifiChecked
        {
            get
            {
                return _WifiChecked;
            }

            set
            {
                _WifiChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [bluetooth checked].
        /// </summary>
        /// <value><c>true</c> if [bluetooth checked]; otherwise, <c>false</c>.</value>
        public bool BluetoothChecked
        {
            get
            {
                return _bluetoothChecked;
            }

            set
            {
                _bluetoothChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [lte checked].
        /// </summary>
        /// <value><c>true</c> if [lte checked]; otherwise, <c>false</c>.</value>
        public bool LTEChecked
        {
            get
            {
                return _LTEChecked;
            }

            set
            {
                _LTEChecked = value;
            }
        }
    }
}
