using ErnanisRenamer.Models;
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

namespace ErnanisRenamer.Views
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private OptionsModel _opt;
        private BackgroundWorker _bgw;

        public ProgressWindow(OptionsModel opt,ref BackgroundWorker bgw)
        {
            // Pass important variables for processing
            _opt = opt;
            _bgw = bgw;

            this.Loaded += ProgressWindow_Loaded;
            InitializeComponent();
           
        }

        void ProgressWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_opt != null)
            {
                this.DataContext = _opt;
            }
            else
            {
                MessageBox.Show("Progress win didnt work.");
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_bgw != null)
            {
                // Cancel the operation
                _bgw.CancelAsync();
            }
            this.Close();
        }
    }
    
}
