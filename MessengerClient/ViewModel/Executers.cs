using System;
using System.Windows;
using System.Windows.Threading;

namespace MessengerClient.ViewModel
{
    public static class Executers
    {
        public static void ExecuteInUiThread(
        Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(action), priority);
        }
    }
}