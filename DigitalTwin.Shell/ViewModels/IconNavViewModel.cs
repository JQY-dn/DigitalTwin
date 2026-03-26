using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Shell.ViewModels
{
    public class IconNavViewModel : BindableBase
    {
        private bool _hasActiveAlarm;
        public bool HasActiveAlarm
        {
            get => _hasActiveAlarm;
            set => SetProperty(ref _hasActiveAlarm, value);
        }

        public DelegateCommand<string> NavigateCommand { get; }

        public IconNavViewModel(IRegionManager rm)
        {
            NavigateCommand = new DelegateCommand<string>(page =>
                rm.RequestNavigate("LeftPanelRegion", page));
        }
    }
}
