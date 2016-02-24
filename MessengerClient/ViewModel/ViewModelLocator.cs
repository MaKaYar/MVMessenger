/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:MessengerClient"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Windows.Media.TextFormatting;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MessengerClient.Interop;
using Microsoft.Practices.ServiceLocation;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MessengerClient.Interop.Helpers;

namespace MessengerClient.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<MainViewModel>();
                ConnectToMessangerManager();
            }
        }

        private void MessageRecievedHandler(string sender, Interop.Message message)
        {
            //var length = message.Content.Data.DataLength;
            //var data = new byte[length];
            //Marshal.Copy(message.Content.Data.Data, data, 0, length);
            //var temp = Encoding.UTF8.GetString(data);
            //NewMethod(temp);
            MainVM.AddMessageToViewModel(sender, message, false);
            
            //Messages.Add(temp);
        }

        

        //private void GetMessageFromInterop(Interop.Message message)
        //{
        //    ViewModel.Message viewMessage;
        //    switch (message.Content.Type)
        //    {

        //        case MessageContentType.Text:
        //            {
        //                viewMessage = new TextMessage(false, message.MessageId); break;
        //            }
        //        default:
        //            return;
        //    }
        //    viewMessage.setData = BytesConversion.GetBytesFromBinaryData(message.Content.Data);
        //}

        private void MessageStatusChangedHandler()
        {

        }

        private OnMessageRecieved _mrh;
        private OnMessageStatusChanged _msh;

        private async void ConnectToMessangerManager()
        {
            //if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
            //{
            //    return; //in Design mode
            //}
            MessengerManager.Init(MessageRecievedHandler, MessageStatusChangedHandler);
            var resultLogin = await MessengerManager.Login();
            MessageBox.Show(resultLogin.ToString());
            ObservableCollection<User> result = await GetActiveUsersList();
            MainVM.UsersList = new ObservableCollection<User>(result);
            //var temp = new TextMessage();
            //temp.Data = "Hello";
            //var str = temp.Data;
            //MessageBox.Show((string)str);

        }

        public static async Task<ObservableCollection<User>> GetActiveUsersList()
        {
            var requestActiveUsersResult = await MessengerManager.RequestActiveUsers();
            var activeUsersList = new ObservableCollection<User>();
            foreach (var activeUser in requestActiveUsersResult.UserList.OrderBy(r => r.Identifier))
            {
                activeUsersList.Add(new User
                {
                    Identifier = activeUser.Identifier
                });
            }

            //var temp = new ObservableCollection<User>(activeUsersList.OrderBy(r=>r.Identifier));
            //return temp;\
            return activeUsersList;
        }

        public MainViewModel MainVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}