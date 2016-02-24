using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MessengerClient.Interop.Helpers
{
    public static class BytesConversion
    {
        public static byte[] GetBytesFromBinaryData(BinaryData binaryData)
        {
            var length = binaryData.DataLength;
            var bytes = new byte[length];
            Marshal.Copy(binaryData.Data, bytes, 0, length);
            return bytes;
        }
    }
}
