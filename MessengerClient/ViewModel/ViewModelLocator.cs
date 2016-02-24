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
using System.Windows.Markup;
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
                //MessengerManager.Init("127.0.0.1", 5222);
                //MessengerManager.RegisterObservers(MessageRecievedHandler, MessageStatusChangedHandler);
                //Login("makar2@gmail.com", "12345");
                //MessageBox.Show(resultLogin.ToString());
                SimpleIoc.Default.Register<MainViewModel>();
                SimpleIoc.Default.Register<LoginViewModel>();
                

            }
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

        

        private OnMessageRecieved _mrh;
        private OnMessageStatusChanged _msh;

        private async void Login(string user, string password)
        {
            //if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
            //{
            //    return; //in Design mode
            //}
            //MessengerManager.Init(MessageRecievedHandler, MessageStatusChangedHandler, url, port);
            //var resultLogin = await MessengerManager.Login();
            //MessageBox.Show(resultLogin.ToString());
            //ObservableCollection<User> result = await GetActiveUsersList();
            //MainVM.UsersList = new ObservableCollection<User>(result);
            //var temp = new TextMessage();
            //temp.Data = "Hello";
            //var str = temp.Data;
            //MessageBox.Show((string)str);
            var resultLogin = await MessengerManager.Login(user, password);
            MessageBox.Show(resultLogin.ToString());
        }



        public MainViewModel MainVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public LoginViewModel LoginVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}