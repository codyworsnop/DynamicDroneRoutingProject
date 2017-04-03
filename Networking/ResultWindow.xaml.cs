using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Networking
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow
    {
        /// <summary>
        /// The result text
        /// </summary>
        private string _resultText;
        /// <summary>
        /// The result
        /// </summary>
        private string _result;

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultWindow"/> class.
        /// </summary>
        public ResultWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultWindow"/> class.
        /// </summary>
        /// <param name="nodePath">The node path.</param>
        public ResultWindow(List<NodeModel> nodePath) : this()
        {
            SetupResultText(nodePath); 
        }

        /// <summary>
        /// Setups the result text.
        /// </summary>
        /// <param name="nodePath">The node path.</param>
        private void SetupResultText(List<NodeModel> nodePath)
        {
            //loop through every node in the optimized path in order to output the path
            foreach(var node in nodePath)
            {
                //Starting route is a drone
                if (node == nodePath.First())
                {
                    _result += "Starting route at: drone " + node.NodeId + "\n";
                }
                //check to see if node is drone
                else if (node.GetType() == typeof(DroneModel))
                { 
                    _result += "routing to: drone " + node.NodeId + "\n";
                }
                //check to see if node is a tower
                else if (node.GetType() == typeof(TowerModel))
                {
                    _result += "routing to: tower " + node.NodeId + "\n"; 
                }
                //output that it's at the comamand center
                else
                {
                    _result += "routing to: command center\n";
                }           
            }
            //rout compete
            _result += "Route Complete!";

            ResultText = _result;
        }

        /// <summary>
        /// Gets or sets the result text.
        /// </summary>
        /// <value>The result text.</value>
        public string ResultText
        {
            get
            {
                return _resultText;
            }
            
            set
            {
                if (value == _resultText) return; 

                _resultText = value;
                OnPropertyChanged("ResultText"); 
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
    }
}
