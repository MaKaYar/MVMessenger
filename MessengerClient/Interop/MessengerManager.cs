using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO.Packaging;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MessengerClient.Interop
{
    public delegate void OnMessageRecievedForViewModel();

    public static class MessengerManager
    {
        private static LoginCallback _logBack;
        private static RequestUsersCallback _recBack;
        private static OnMessageRecieved _onMesRecieved;
        private static OnMessageStatusChanged _onMesStatusChanged;

        public static List<User> activeUsersList;


        public static void Init(string url, ushort port)
        {
            MessengerInterop.Init(port, url);
        }

        public static void Disconnect()
        {
            MessengerInterop.Disconnect();
        }

        public static async Task<OperationResult> Login(string login, string password)
        {
            var task = new TaskCompletionSource<OperationResult>();
            _logBack = new LoginCallback(task.SetResult);
            var loginCallbackPtr = Marshal.GetFunctionPointerForDelegate(
                _logBack);
            MessengerInterop.Login(loginCallbackPtr, login, password);
            return await task.Task;
        }

        public static void RegisterObservers(OnMessageRecieved onMessageRecieved, OnMessageStatusChanged onMesStatusChanged)
        {
            _onMesRecieved = onMessageRecieved;
            var mrhPtr = Marshal.GetFunctionPointerForDelegate(_onMesRecieved);
            _onMesStatusChanged = onMesStatusChanged;
            var mshPtr = Marshal.GetFunctionPointerForDelegate(_onMesStatusChanged);
            MessengerInterop.RegisterObserver(mrhPtr, mshPtr);
        }

        //public static void MessageRecievedHandler(string sender, Message message)
        //{

        //}

        //public static void MessageStatusChangedHandler()
        //{
            
        //}

        public static async Task<RequestActiveUsersResult> RequestActiveUsers()
        {
            var task = new TaskCompletionSource<RequestActiveUsersResult>();
            _recBack = new RequestUsersCallback((users, length, recResult) =>
            {
                var result = new RequestActiveUsersResult();
                result.UserList = users;
                result.Length = length;
                result.RequestResult = recResult;

                task.SetResult(result);
            });
            var requestActiveUsersCallbackPtr = Marshal.GetFunctionPointerForDelegate(
                _recBack);
            MessengerInterop.RequestActiveUsersWrapper(requestActiveUsersCallbackPtr);
            return await task.Task;
        }

        public static Message SendMessage(string recepient, byte[] messageContent, MessageContentType type, bool encrypted = false)
        {
            //BUG It can send only english symbols
            //TODO: Workout the problem that this function send only english symbols
            //var data = Encoding.UTF8.GetBytes(textMessage + '\0');
            IntPtr dataPointer = IntPtr.Zero;
            try
            {
                var size = Marshal.SizeOf(messageContent[0]) * messageContent.Length;
                dataPointer = Marshal.AllocHGlobal(size);
                Marshal.Copy(messageContent, 0, dataPointer, messageContent.Length);
                var bytes = new BinaryData()
                {
                    Data = dataPointer,
                    DataLength = messageContent.Length
                };

                MessageContent content = new MessageContent();
                content.Data = bytes;
                content.Type = type;
                content.Encrypted = encrypted;
                var tempPtr = MessengerInterop.SendMessageWrapper(recepient, content);
                var temp = Marshal.PtrToStructure<Message>(tempPtr);
                return temp;
            }
            finally
            {
                if (dataPointer != IntPtr.Zero) Marshal.FreeHGlobal(dataPointer);
            }
        }
        
    }
}
