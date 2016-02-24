using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MessengerClient.Interop;
using MessengerClient.Interop.Helpers;

namespace MessengerClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        private User _selectedUser;
        private DispatcherTimer timer = null;

        private void TimerStartMethod()
        {
            MessengerManager.RegisterObservers(MessageRecievedHandler, MessageStatusChangedHandler);
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            //timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            timer.Start();
        }

        private async void timerTick(object sender, EventArgs e)
        {
            ObservableCollection<User> result = await GetActiveUsersList();
            foreach (var item in result)
            {
                if (UsersList.All(r => r.Identifier != item.Identifier))
                    UsersList.Add(item);
            }
        }

        //Temporary decision
        private string _currentText;
        //
        private Interop.MessageContentType _currentContentType;

        private void SendMessageMethod()
        {
            //BUG The first two characters in the first message are currupted
            //TODO Fix bug: The first two characters in the first message are currupted
            //TODO Add oportunity sending empty message
            Interop.Message temp;
            switch (CurrentContentType)
            {
                case MessageContentType.Text:
                    {
                        temp = MessengerManager.SendMessage(_selectedUser.Identifier, Encoding.UTF8.GetBytes(CurrentText),
                            CurrentContentType);
                        break;
                    }
                default:
                    return;
            }
            this.AddMessageToViewModel(_selectedUser.Identifier, temp, true);
            RaisePropertyChanged(() => SelectedUser);
        }

        private void SetTextTypeMethod()
        {
            _currentContentType = MessageContentType.Text;
            //RaisePropertyChanged(()=>SendingMessage);
        }

        public ICommand SendMessageCommand { get; private set; }
        public ICommand SetTextTypeCommand { get; private set; }
        public ICommand StartTimerCommand { get; private set; }

        public ObservableCollection<User> UsersList
        {
            get
            {
                return _users;
            }

            set
            {
                _users = value;
                RaisePropertyChanged(() => UsersList);
            }
        }

        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                RaisePropertyChanged(() => SelectedUser);
            }
        }

        public Interop.MessageContentType CurrentContentType
        {
            get { return _currentContentType; }
            set
            {
                _currentContentType = value;
                RaisePropertyChanged(() => CurrentContentType);
            }
        }

        public string CurrentText
        {
            get { return _currentText; }
            set
            {
                _currentText = value;
                RaisePropertyChanged(() => CurrentText);
            }
        }
        private void MessageRecievedHandler(string sender, Interop.Message message)
        {
            //var length = message.Content.Data.DataLength;
            //var data = new byte[length];
            //Marshal.Copy(message.Content.Data.Data, data, 0, length);
            //var temp = Encoding.UTF8.GetString(data);
            //NewMethod(temp);
            this.AddMessageToViewModel(sender, message, false);

            //Messages.Add(temp);
        }
        private void MessageStatusChangedHandler()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <param name="fromMe">True - the message from me; False - the message from someone</param>
        private void AddMessageToViewModel(string sender, Interop.Message message, bool fromMe)
        {
            //BUG I get recived message before i get sent message
            //TODO Fix bug: I get recived message before i get sent message
            ViewModel.Message viewMessage;
            var time = Interop.Helpers.DateTimeConversion.UnixTimeToDateTime(message.Time);
            switch (message.Content.Type)
            {
                case MessageContentType.Text:
                    {
                        viewMessage = new TextMessage(fromMe, message.MessageId, time); break;
                    }
                default:
                    return;
            }
            viewMessage.setData = BytesConversion.GetBytesFromBinaryData(message.Content.Data);

            Application.Current.Dispatcher.BeginInvoke(
                new Action(() => this.UsersList.First(r => r.Identifier == sender).AddMessageToTalk(viewMessage))
                );
            RaisePropertyChanged(() => SelectedUser);
        }

        bool canExecuteSendCommand()
        {
            return !String.IsNullOrWhiteSpace(CurrentText) && SelectedUser != null;
        }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            SendMessageCommand = new RelayCommand(SendMessageMethod, canExecuteSendCommand);
            SetTextTypeCommand = new RelayCommand(SetTextTypeMethod);
            StartTimerCommand = new RelayCommand(TimerStartMethod);
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
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

        public override void Cleanup()
        {
            MessengerManager.Disconnect();
            //TODO Unregister Observers and Disconnect
            base.Cleanup();
        }
    }
}