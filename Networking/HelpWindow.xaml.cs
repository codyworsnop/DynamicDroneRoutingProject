using System.Windows;


namespace Networking
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelpWindow"/> class.
        /// </summary>
        public HelpWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Closes the button handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButtonHandler(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
