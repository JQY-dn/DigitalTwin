using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Timers;
using System.Windows;

namespace DigitalTwin.Shell.ViewModels
{
    public class DeviceStatusItem
    {
        public string DeviceName { get; set; } = "";
        public string StatusLabel { get; set; } = "";
        public SolidColorBrush StatusBrush { get; set; } = new(Colors.Gray);
    }

    public class StatusBarViewModel : BindableBase
    {
        private string _pollIntervalText = "100 ms";
        private string _modbusLatencyText = "— ms";
        private string _unityLatencyText = "— ms";
        private string _dateText = DateTime.Now.ToString("yyyy-MM-dd");

        public string PollIntervalText { get => _pollIntervalText; set => SetProperty(ref _pollIntervalText, value); }
        public string ModbusLatencyText { get => _modbusLatencyText; set => SetProperty(ref _modbusLatencyText, value); }
        public string UnityLatencyText { get => _unityLatencyText; set => SetProperty(ref _unityLatencyText, value); }
        public string DateText { get => _dateText; set => SetProperty(ref _dateText, value); }

        public ObservableCollection<DeviceStatusItem> DeviceStatusItems { get; } = new()
        {
            new DeviceStatusItem { DeviceName = "SRM-01", StatusLabel = "离线",
                                   StatusBrush = new SolidColorBrush(Color.FromRgb(0x3D,0x55,0x70)) },
            new DeviceStatusItem { DeviceName = "SRM-02", StatusLabel = "离线",
                                   StatusBrush = new SolidColorBrush(Color.FromRgb(0x3D,0x55,0x70)) },
            new DeviceStatusItem { DeviceName = "SRM-02", StatusLabel = "离线",
                                   StatusBrush = new SolidColorBrush(Color.FromRgb(0x3D,0x55,0x70)) },
        };

        public StatusBarViewModel()
        {
            var timer = new Timer(60_000) { AutoReset = true };
            timer.Elapsed += (_, _) =>
                Application.Current?.Dispatcher.Invoke(
                    () => DateText = DateTime.Now.ToString("yyyy-MM-dd"));
            timer.Start();
        }
    }
}
