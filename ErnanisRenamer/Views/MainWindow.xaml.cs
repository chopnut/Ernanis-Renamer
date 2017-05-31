using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// My own object below
using ErnanisRenamer.ViewModels;
using ErnanisRenamer.AttachedProperties;
using Prism.Commands;
using Microsoft.Practices.Unity;

namespace ErnanisRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        private UnityContainer ioc;
        private MainWindowViewModel _mainWindowVM;
        public MainWindow()
        {
           
            this.Loaded += MainWindow_Loaded;

            // This UnityContainer is use by FilterTypeConverter, MainWindowViewModel;

            ioc = new UnityContainer();
            Application.Current.Resources.Add("UContainer",ioc);


            // Add resource before initialize component 
            InitializeComponent();

        }

        // Use this for initialisation
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Data Binding ViewModel
            _mainWindowVM = new MainWindowViewModel();
          
            gridMain.DataContext    = _mainWindowVM;
            gridOptions.DataContext = _mainWindowVM.Option;
            LoadFrontInstruction();

            // throw new NotImplementedException();
        }
        void LoadFrontInstruction()
        {
      
            wbInstruction.NavigateToString(@"<html><body scroll='auto' style='margin:0px;padding: 5px;'>
            <font size=1 face=arial>
<strong>Filetype</strong><br>
*.* all files, -.txt exclude txt file.<br>
<strong>Prefix and Suffix</strong><br>
[d][m][y] day,month,year
[i] 0,01,001 increment
</font></body></html>");

        }
    }
 
}
