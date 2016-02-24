using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MessengerClient.ViewModel
{
    class MessagesHistory : ObservableObject
    {
        private User collocutor;
        private ObservableCollection<Message> talk;

        public User Collocutor
        {
            get
            {
                return collocutor;
            }
        }

        public ObservableCollection<Message> Talk
        {
            get { return talk; }
            set
            {
                talk = value;
                RaisePropertyChanged(() => Talk);
            }
        }

        public void AddMessageToTalk(Message message)
        {
            Talk.Add(message);
            RaisePropertyChanged(() => Talk);
        }
    }
}
