using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CONTROLBPA.ViewModel;

// StartupUri="MainWindow.xaml"
namespace CONTROLBPA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ControlBPAViewModel controlBPA = new ControlBPAViewModel();

            // Create an instance of the MainWindow and set the controlBPA instance 
            // as the data context of the MainWindow.     
            MainWindow win = new MainWindow();
            win.DataContext = controlBPA;

            // Set the attached GameoverBehavior.ReportResult property to true on the MainWindow.            
            // win.SetValue(GameOverBehavior.ReportResultProperty, True)

            // Show the MainWindow. 
            win.Show();
        }

    }
}
