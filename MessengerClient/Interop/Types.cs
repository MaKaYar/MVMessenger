using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MessengerClient.Interop
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LoginCallback(OperationResult result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void RequestUsersCallback([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] User[] users, int length, OperationResult result);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void OnMessageRecieved([MarshalAs(UnmanagedType.LPStr)] string sender, Message message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void OnMessageStatusChanged();
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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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
        [MarshalAs(UnmanagedType.LPStr)]
        public string identifier;

        public string Identifier => identifier;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BinaryData
    {
        public IntPtr Data;
        public int DataLength;
    };
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MessageContent
    {
        public MessageContentType Type;
        public bool Encrypted;
        public BinaryData Data;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Message
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string MessageId;
        public MessageContent Content;
        public long Time;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MessageMetaData
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string MessageId;
        public long Time;
    }
}
