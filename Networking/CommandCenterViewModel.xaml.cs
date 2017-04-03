/// <summary>
/// CommandCenterViewModal.xaml.cs
/// </summary>
namespace Networking
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for CommandCenterViewModel.xaml
    /// </summary>
    public partial class CommandCenterViewModel
    {
        /// <summary>
        /// The commandModal used to target the commandCenterModal object
        /// </summary>
        public CommandCenterModel CommandModel;

        /// <summary>
        /// CanvasLeft used to target the x canvas property
        /// </summary>
        private double _canvasLeft;

        /// <summary>
        /// CanvasTop used to target the y canvas property
        /// </summary>
        private double _canvasTop; 

        /// <summary>
        /// Default constructor. 
        /// </summary>
        public CommandCenterViewModel()
        {
            InitializeComponent();
            DataContext = this;

            CommandModel = new CommandCenterModel();
            SetPosition(new Point(470, 25));
        }     
        
        /// <summary>
        /// getter setter for canvasleft 
        /// </summary>
        public double CanvasLeft
        {
            get
            {
                return _canvasLeft; 
            }

            set
            {
                _canvasLeft = value; 
            }
        } 

        /// <summary>
        /// getter seets for canvastop
        /// </summary>
        public double CanvasTop
        {
            get
            {
                return _canvasTop;
            }

            set
            {
                _canvasTop = value; 
            }
        }

        /// <summary>
        /// Sets the coordinate values (canvasleft and canvastop)
        /// </summary>
        /// <param name="pos">the passed in position to set the canvas to</param>
        public void SetPosition(Point pos)
        {
            CanvasLeft = pos.X;
            CanvasTop = pos.Y; 
        }
    }
}
