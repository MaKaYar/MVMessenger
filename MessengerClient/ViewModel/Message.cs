using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MessengerClient.Interop;

namespace MessengerClient.ViewModel
{
    

    public abstract class Message : ObservableObject
    {
        protected byte[] data;
        protected Interop.MessageContentType messageType;
        private bool _fromMe;
        private bool _encrypted;
        private string _id;
        private DateTime _time;
        /// <summary>
        /// True if the message is sent BY me. False if the message is sent by someone TO me
        /// </summary>
        public bool FromMe
        {
            get { return _fromMe; }
        }

        public bool Encrypted
        {
            get { return _encrypted; }
        }

        public string ID
        {
            get { return _id; }
        }

        public DateTime Time
        {
            get { return _time; }
        }

        public Interop.MessageContentType MessageType
        {
            get { return messageType; }
        }

        public virtual object getData { get; set; }

        public byte[] getDataAsBytes
        {
            get
            {
                return data;
            }
        }

        public byte[] setData
        {
            set
            {
                data = value;
                //Set<byte[]>(() => this.Data as byte[], ref data, value);
            }
        }
        protected Message(bool fromMe, string messageID, DateTime time, bool encrypted = false)
        {
            _fromMe = fromMe;
            _id = messageID;
            _time = time;
            _encrypted = encrypted;
        }
        protected Message()
        {
            
        }

    }

    public class TextMessage : Message
    {
        public override object getData
        {
            get
            {
                var temp = Encoding.UTF8.GetString(data);
                return temp;
            }
            set { Set<byte[]>(() => this.getData as byte[], ref data, value as byte[]); }
        }

        public TextMessage(bool fromMe, string messageID, DateTime time, bool encrypted = false) : base(fromMe, messageID, time, encrypted)
        {
            messageType = MessageContentType.Text;
        }
        /// <summary>
        /// Empty message for using it as message metadata
        /// </summary>
        public TextMessage()
        {
            
        }
    }
}
