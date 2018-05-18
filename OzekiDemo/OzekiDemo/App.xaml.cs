using OzekiDemo.Model;
using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace OzekiDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

                SoftphoneEngine model = new SoftphoneEngine();

                Phone window = new Phone(model);
                window.Show();
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Could not initialize softphone: \r\n");
                sb.Append(ex.Message);
                sb.Append("\r\n");
                sb.Append(ex.InnerException);
                sb.Append(ex.StackTrace);
                MessageBox.Show(sb.ToString(), "Reliance Ozeki", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            base.OnStartup(e);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.Exception;
            MessageBox.Show(string.Format("Unexpected error occurred: {0}\r\n{1}\r\n{2}\r\nHandled:{3}", ex.Message, ex.InnerException, ex.StackTrace, e.Handled));
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            MessageBox.Show(string.Format("Unexpected error occurred: {0}\r\n{1}\r\n{2}\r\nIsTerminating:{3}", ex.Message, ex.InnerException, ex.StackTrace, e.IsTerminating));
        }
    }
}
