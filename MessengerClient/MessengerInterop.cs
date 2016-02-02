using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MessengerClient
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LoginCallback(OperationResult result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void RequestUsersCallback([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] User[] users, int length, OperationResult result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void OnMessageRecieved([MarshalAs(UnmanagedType.LPStr)] string sender, ref Message message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void OnMessageStatusChange();
    public enum OperationResult : int
    {
        Ok,
        AuthError,
        NetworkError,
        InternalError
    }

    public enum MessageContentType : int
    {
        Text,
        Image,
        Video
    }

    [StructLayout(LayoutKind.Sequential,Pack = 1)]
    public struct RequestActiveUsersResult
    {
        public OperationResult RequestResult;
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
        public User[] UserList;
        public int Length;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct User
    {
        [MarshalAs(UnmanagedType.LPStr)] public string identifier;

        public string Identifier => identifier;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct BinaryData
    {
        public IntPtr Data;
        public int DataLength;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct MessageContent
    {
        public MessageContentType Type;
        public bool Encrypted;
        public BinaryData Data;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Message
    {
        public User Recepient;
        public MessageContent Content;
    }

    public static class MessengerInterop
    {
        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Login(IntPtr loginCallback, [MarshalAs(UnmanagedType.LPStr)]string login, [MarshalAs(UnmanagedType.LPStr)]string pass);

        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init();

        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Disconnect();

        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMessageWrapper([MarshalAs(UnmanagedType.LPStr)] string recepient, ref BinaryData message);

        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RequestActiveUsersWrapper(IntPtr requestUsersCalback);

        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterObserver(OnMessageRecieved onMessageRecieved, OnMessageStatusChange onMessageStatusChange);
    }

    public static class MessengerC
    {
        private static LoginCallback logBack;
        private static RequestUsersCallback recBack;
        private static OnMessageRecieved onMesRecieved;
        public static List<User> activeUsersList; 
        public static async Task<OperationResult> Login()
        {
            var task = new TaskCompletionSource<OperationResult>();
            logBack = new LoginCallback(task.SetResult);
            var loginCallbackPtr = Marshal.GetFunctionPointerForDelegate(
                logBack);
            MessengerInterop.Login(loginCallbackPtr, "makar@gmail.com", "12345");
            return await task.Task;
            
        }

        public static void MessageRecievedHandler(string sender, ref Message message)
        {
            
        }

        public static async Task<RequestActiveUsersResult> RequestActiveUsers()
        {
            var task = new TaskCompletionSource<RequestActiveUsersResult>();
            recBack = new RequestUsersCallback((users, length, recResult) =>
            {
                var result  = new RequestActiveUsersResult();
                result.UserList = users;
                result.Length = length;
                result.RequestResult = recResult;

                task.SetResult(result);
            });
            var requestActiveUsersCallbackPtr = Marshal.GetFunctionPointerForDelegate(
                recBack);
            MessengerInterop.RequestActiveUsersWrapper(requestActiveUsersCallbackPtr);
            return await task.Task;
        }
        //public static async Task<RequestActiveUsersResult> RequestActiveUsers()
        //{
        //    return await Task.Run(() =>
        //    {
        //        RequestActiveUsersResult temp = null;
        //        bool f = true;


        //        recBack = (a, b, c) =>
        //        {
        //            temp = new RequestActiveUsersResult
        //            {
        //                RequestResult = a,
        //                UserList = b.ToList(),
        //                Length = c
        //            };
        //            f = false;
        //        };

        //        MessengerInterop.RequestActiveUsersWrapper(Marshal.GetFunctionPointerForDelegate(recBack));
        //        while (temp==null)
        //        {

        //        }
        //        return temp;
        //    });
        //}
    }
}
