using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace WPF.Assignment
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            base.OnStartup(e);
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            SynchronizationContext.Current.Post(new SendOrPostCallback((p) =>
            {
                //Show user friendly message
                MessageBox.Show("An error has occured, please check windows eventviewer for more details");
                Util.LogMessage(e.Exception);
            }), null);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SynchronizationContext.Current.Post(new SendOrPostCallback((p) =>
            {
                //Show user friendly message
                MessageBox.Show("An error has occured, please check windows eventviewer for more details");
                Util.LogMessage(e.ExceptionObject as Exception);
            }), null);
            
        }
    }
}
