using DigitalTwin.Shell.Views.Pages;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using DigitalTwin.Infrastructure.Events;
using DigitalTwin.Infrastructure.Enums;

namespace DigitalTwin.Shell.ViewModels
{
    public class NavTabsViewModel : BindableBase
    {
        private readonly IRegionManager _rm;
        private readonly IEventAggregator _ea;

        private bool _hasActiveAlarm;
        public bool HasActiveAlarm
        {
            get => _hasActiveAlarm;
            set => SetProperty(ref _hasActiveAlarm, value);
        }

        public DelegateCommand<string> NavigateCommand { get; }

        public NavTabsViewModel(IRegionManager rm, IEventAggregator ea)
        {
            _rm = rm;
            _ea = ea;
            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string page)
        {
            if (!Enum.TryParse<AppPage>(page, out var p)) return;

            _ea.GetEvent<PageChangedEvent>().Publish(p);

            switch (p)
            {
                case AppPage.Dashboard:
                    if (_rm.Regions.ContainsRegionWithName("MainContentRegion"))
                        _rm.Regions["MainContentRegion"].RemoveAll();
                    break;
                case AppPage.Inventory:
                    _rm.RequestNavigate("MainContentRegion", nameof(InventoryPageView));
                    break;
                case AppPage.Task:
                    _rm.RequestNavigate("MainContentRegion", nameof(TaskPageView));
                    break;
                case AppPage.Report:
                    _rm.RequestNavigate("MainContentRegion", nameof(ReportPageView));
                    break;
                case AppPage.Config:
                    _rm.RequestNavigate("MainContentRegion", nameof(ConfigPageView));
                    break;
                case AppPage.Alarm:
                    _rm.RequestNavigate("MainContentRegion", nameof(AlarmPageView));
                    break;
            }
        }
    }
}
