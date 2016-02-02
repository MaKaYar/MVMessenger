using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MessengerClient.ViewModels
{
    struct Hystory
    {
        public User user;
        public List<Message> MessageHystory;
    }
    class MainViewModel
    {
        public List<Hystory> MessangerHystor;
    }
}
