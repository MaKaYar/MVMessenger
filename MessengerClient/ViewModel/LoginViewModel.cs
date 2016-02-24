using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MessengerClient.Interop;

namespace MessengerClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _login;
        private string _password;
        private string _address;
        private ushort _port;
        private bool _isBusy;

        public string Login
        {
            get { return _login; }
            set { Set<string>(() => this.Login, ref _login, value); }
        }
        public string Password
        {
            get { return _password; }
            set { Set<string>(() => this.Password, ref _password, value); }
        }
        public string Address
        {
            get { return _address; }
            set { Set<string>(() => this.Address, ref _address, value); }
        }
        public ushort Port
        {
            get { return _port; }
            set { Set<ushort>(() => this.Port, ref _port, value); }
        }
        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set<bool>(() => this.IsBusy, ref _isBusy, value); }
        }

        private async void LoginMethod()
        {
            IsBusy = true;
            MessengerManager.Init(Address, Port);
            var loginResult = await MessengerManager.Login(Login, Password);
            switch (loginResult)
            {
                case OperationResult.Ok:
                    {
                        Executers.ExecuteInUiThread(() =>
                        {
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();
                            //TODO Closing LoginWindow
                        });
                        break;
                    }
                default:
                    {
                        MessageBox.Show(loginResult.ToString());
                        break;
                    }

            }
        }

        public ICommand LoginCommand { get; private set; }


        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(LoginMethod);
            SetDefaultConnection();
        }

        public void SetDefaultConnection()
        {
            Address = "127.0.0.1";
            Port = 5222;
            Login = "makar2@gmail.com";
            Password = "12345";
        }
    }
}