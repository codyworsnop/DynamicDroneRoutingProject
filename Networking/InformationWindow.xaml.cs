using System;
using System.Collections.Generic;
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

/// <summary>
/// The Networking namespace.
/// </summary>
namespace Networking
{
    /// <summary>
    /// Interaction logic for InformationWindow.xaml
    /// </summary>
    public partial class InformationWindow
    {
        public InformationWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Closes the button handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void CloseButtonHandler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
