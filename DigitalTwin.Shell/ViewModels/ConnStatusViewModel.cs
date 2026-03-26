using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Shell.ViewModels
{
    public class ConnStatusItem
    {
        public string Endpoint { get; set; } = "";
        public bool IsOnline { get; set; }
    }

    public class ConnStatusViewModel : BindableBase
    {
        public ObservableCollection<ConnStatusItem> Connections { get; } = new()
        {
            new ConnStatusItem { Endpoint = "192.168.1.100:502", IsOnline = false },
            new ConnStatusItem { Endpoint = "UnityPipe",         IsOnline = false },
        };
    }
}
