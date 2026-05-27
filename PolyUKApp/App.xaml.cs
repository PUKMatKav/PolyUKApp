using System.Configuration;
using System.Data;
using System.Windows;

namespace PolyUKApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            // Handle exceptions on the UI thread
            DispatcherUnhandledException += (sender, args) =>
            {
                System.Windows.MessageBox.Show(
                    $"An unexpected error occurred:\n\n{args.Exception.Message}",
                    "Application Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                args.Handled = true; // Prevents crash; remove this line if you want the app to close
            };

            // Handle exceptions from background threads
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                System.Windows.MessageBox.Show(
                    $"A fatal error occurred:\n\n{exception?.Message}",
                    "Fatal Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            };

            // Handle exceptions from async Task operations
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                System.Windows.MessageBox.Show(
                    $"An async error occurred:\n\n{args.Exception.Message}",
                    "Async Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                args.SetObserved(); // Prevents process termination
            };

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var comException = e.Exception as System.Runtime.InteropServices.COMException;

            if (comException != null && comException.ErrorCode == -2147221040)
                e.Handled = true;
        }
    }

}
