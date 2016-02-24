using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MessengerClient.Interop
{
    public static class MessengerInterop
    {
        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Login(IntPtr loginCallback, [MarshalAs(UnmanagedType.LPStr)]string login, [MarshalAs(UnmanagedType.LPStr)]string pass);

        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Init(ushort port,[MarshalAs(UnmanagedType.LPStr)]string address);

        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Disconnect();

        [DllImport("libmessenger.interop.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SendMessageWrapper([MarshalAs(UnmanagedType.LPStr)] string recepient, MessageContent message);

        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RequestActiveUsersWrapper(IntPtr requestUsersCalback);

        [DllImport("libmessenger.interop.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterObserver(IntPtr onMessageRecieved, IntPtr onMessageStatusChange);
    }
}
