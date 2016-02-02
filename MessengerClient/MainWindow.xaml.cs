using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MessengerClient.ViewModels;

namespace MessengerClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public static MainViewModel myHystory;

        public MainWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<string> Messages
        {
            get { return (ObservableCollection<string>) GetValue(MessagesProperty); }
            set { SetValue(MessagesProperty, value); }
        }

        public ObservableCollection<User> UsersList
        {
            get { return (ObservableCollection<User>) GetValue(UsersListProperty); }
            set { SetValue(UsersListProperty, value); }
        }

        public static readonly DependencyProperty UsersListProperty =
            DependencyProperty.Register("UsersList", typeof (ObservableCollection<User>), typeof (MainWindow),
                new PropertyMetadata(new ObservableCollection<User>()));

        // Using a DependencyProperty as the backing store for Messages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessagesProperty =
            DependencyProperty.Register("Messages", typeof (ObservableCollection<string>), typeof (MainWindow),
                new PropertyMetadata(new ObservableCollection<string>()));


        private OnMessageRecieved mrh;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Test message//
            /*
            
            {
                Data = Encoding.UTF8.GetBytes(message),
                DataLength = message.Length
            };*/
            MessengerInterop.Init();
            mrh = MessageRecievedHandler;
            MessengerInterop.RegisterObserver(mrh, null);
            var t = await MessengerC.Login();

            string message = "Hello World";
            var data = Encoding.UTF8.GetBytes(message + '\0');


            IntPtr dataPointer = IntPtr.Zero;
            try
            {
                var size = Marshal.SizeOf(data[0])*data.Length;
                dataPointer = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, dataPointer, data.Length);
                var bytes = new BinaryData()
                {
                    Data = dataPointer,
                    DataLength = data.Length
                };

                MessageBox.Show(t.ToString());
                MessengerInterop.SendMessageWrapper("eldar1@dordzhiev", ref bytes);
                await Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Messages.Add(message);
                }));
                
                var temp = (await MessengerC.RequestActiveUsers()).UserList;
                UsersList = new ObservableCollection<User>(temp);
            }
            finally
            {
                if (dataPointer != IntPtr.Zero) Marshal.FreeHGlobal(dataPointer);
            }


            //Init and Login

        }

        private void MessageRecievedHandler(string sender, ref Message message)
        {
            var length = message.Content.Data.DataLength;
            var data = new byte[length];
            Marshal.Copy(message.Content.Data.Data, data, 0, length);
            var temp = Encoding.UTF8.GetString(data);
            NewMethod(temp);
        }

        private async void NewMethod(string temp)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                Messages.Add(temp);
            }));
        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = writeTextBox.Text;
            var data = Encoding.UTF8.GetBytes(message + '\0');


            IntPtr dataPointer = IntPtr.Zero;
            try
            {
                var size = Marshal.SizeOf(data[0])*data.Length;
                dataPointer = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, dataPointer, data.Length);
                var bytes = new BinaryData()
                {
                    Data = dataPointer,
                    DataLength = data.Length
                };

                MessengerInterop.SendMessageWrapper(recepientTextBox.Text, ref bytes);
                await Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Messages.Add(message);
                }));
            }
            finally
            {
                if (dataPointer != IntPtr.Zero) Marshal.FreeHGlobal(dataPointer);
            }
        }
    }
}
