using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MessengerClient.ViewModel
{
   

    public class User : ObservableObject
    {

        private string identifier;

        private ObservableCollection<Message> talk = new ObservableCollection<Message>();
        /// <summary>
        /// Get identifier of current user
        /// </summary>
        public string Identifier
        {
            get { return identifier; }
            set { Set<string>(() => this.Identifier, ref identifier, value); }
        }
        
        /// <summary>
        /// Get collection of all messages between me and current user
        /// </summary>
        public ObservableCollection<Message> Talk
        {
            get { return talk; }
            private set
            {
                talk = value;
                RaisePropertyChanged(() => Talk);
            }
        }
        /// <summary>
        /// Add Message to talk
        /// </summary>
        /// <param name="message"></param>
        public void AddMessageToTalk(Message message)
        {
            try
            {
                Talk.Add(message);
                RaisePropertyChanged(() => Talk);
            }
            catch (Exception e)
            {
                
            }
            
        }
        public static User GetUserFromInterop(Interop.User user)
        {
            return new User
            {
                Identifier = user.Identifier
            };
        }
        //public int CompareTo(object obj)
        //{
        //    User person = obj as User;
        //    if (person == null)
        //    {
        //        throw new ArgumentException("Object is not User");
        //    }
        //    return this.Identifier.CompareTo(person.Identifier);
        //}
        
    }
}
