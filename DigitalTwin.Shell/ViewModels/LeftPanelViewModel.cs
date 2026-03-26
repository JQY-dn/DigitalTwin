using DigitalTwin.Infrastructure.Interface;
using DigitalTwin.Infrastructure.Models;
using DigitalTwin.Infrastructure.Tools;
using DigitalTwin.Shell.PageServices;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DigitalTwin.Infrastructure.Enums;
namespace DigitalTwin.Shell.ViewModels
{
    public enum DeviceStatus { Running, Idle, Warning, Error, Offline }

    // 单设备数据
    public class DeviceViewModel : BindableBase
    {
        public string Id { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string IpAddress { get; set; } = "";

        

        private DeviceStatus _status;
        public DeviceStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        // 三轴最大值
        public int XMax { get; set; } = 50000;
        public int YMax { get; set; } = 20000;
        public int ZMax { get; set; } = 2000;

        // 三轴位置
        private int _x, _y, _z;
        public int XPosition
        {
            get => _x;
            set
            {
                SetProperty(ref _x, value);
                RaisePropertyChanged(nameof(XPositionText));
                RaisePropertyChanged(nameof(XPercent));
            }
        }
        public int YPosition
        {
            get => _y;
            set
            {
                SetProperty(ref _y, value);
                RaisePropertyChanged(nameof(YPositionText));
                RaisePropertyChanged(nameof(YPercent));
            }
        }
        public int ZPosition
        {
            get => _z;
            set
            {
                SetProperty(ref _z, value);
                RaisePropertyChanged(nameof(ZPositionText));
                RaisePropertyChanged(nameof(ZPercent));
            }
        }

        public string XPositionText => $"{XPosition:N0} mm";
        public string YPositionText => $"{YPosition:N0} mm";
        public string ZPositionText => $"{ZPosition:N0} mm";

        public double XPercent => XMax > 0 ? (double)XPosition / XMax : 0;
        public double YPercent => YMax > 0 ? (double)YPosition / YMax : 0;
        public double ZPercent => ZMax > 0 ? (double)ZPosition / ZMax : 0;

        // 温度
        private double _temperature;
        public double Temperature
        {
            get => _temperature;
            set
            {
                SetProperty(ref _temperature, value);
                RaisePropertyChanged(nameof(TemperatureText));
                RaisePropertyChanged(nameof(TempPercent));
            }
        }
        public string TemperatureText => $"{Temperature:F0} ℃";
        public double TempPercent => Math.Clamp(Temperature / 120.0, 0, 1);

        public double MotorCurrent { get; set; }
        public int StatusWord { get; set; }
        public int CurrentTaskId { get; set; }
    }

    // OEE / 寄存器读出行
    public class ReadoutItem
    {
        public string Label { get; set; } = "";
        public string ValueText { get; set; } = "";

        // 颜色：直接给 Brush 供绑定
        public SolidColorBrush ValueBrush { get; set; } =
            new SolidColorBrush(Color.FromRgb(0x00, 0xD8, 0xFF));
    }

    // LeftPanelViewModel
    public class LeftPanelViewModel : BindableBase
    {
        // 设备列表
        public ObservableCollection<DeviceViewModel> Devices { get; } = new();

        public ObservableCollection<LogEntry> Logs { get; } = new();

        /// <summary>UI 最多显示的日志条数，超出后自动移除最旧的</summary>
        private const int MaxLogCount = 200;


        private readonly ILogService _log;
        private readonly IDatabaseService _db;

        private DeviceViewModel? _selectedDevice;
        public DeviceViewModel? SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public string OnlineCountText =>
            $"{Devices.Count(d => d.Status != DeviceStatus.Offline)} / {Devices.Count} 在线";

        // OEE 数据行
        public ObservableCollection<ReadoutItem> OeeItems { get; } = new()
        {
            new ReadoutItem
            {
                Label     = "可用率",
                ValueText = "96.2%",
                ValueBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xE5, 0xA0))
            },
            new ReadoutItem
            {
                Label     = "性能率",
                ValueText = "88.7%",
                ValueBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xD8, 0xFF))
            },
            new ReadoutItem
            {
                Label     = "质量率",
                ValueText = "99.1%",
                ValueBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xE5, 0xA0))
            },
            new ReadoutItem
            {
                Label     = "综合 OEE",
                ValueText = "84.5%",
                ValueBrush = new SolidColorBrush(Color.FromRgb(0x00, 0xD8, 0xFF))
            },
        };

        public LeftPanelViewModel(ILogService log,IDatabaseService db)
        {

            _db = db;
            _ = InitDevices();

            _log = log;
            

            _log.OnLogEntry += OnLogReceived;
            _log.Debug("开始执行！");
            _log.Success("系统启动成功！");
            _log.Warning("SRM-02 号机温度过高！");
            _log.Error("系统通信异常");

            

            


        }

        private void OnLogReceived(LogEntry entry)
        {
            Application.Current?.Dispatcher.BeginInvoke(() =>
            {
                Logs.Add(entry);

                // 超出上限时移除最旧的一条
                if (Logs.Count > MaxLogCount)
                    Logs.RemoveAt(0);
            });
        }




        private async Task InitDevices()
        {
            Devices.Add(new DeviceViewModel
            {
                Id = "SRM-01",
                DisplayName = "SRM-01 号机",
                IpAddress = "192.168.1.100",
                Status = DeviceStatus.Running,
                XPosition = 12500,
                YPosition = 8000,
                ZPosition = 500,
                Temperature = 42,
                MotorCurrent = 8.3,
            });
            Devices.Add(new DeviceViewModel
            {
                Id = "SRM-02",
                DisplayName = "SRM-02 号机",
                IpAddress = "192.168.1.101",
                Status = DeviceStatus.Warning,
                XPosition = 30000,
                YPosition = 4000,
                ZPosition = 0,
                Temperature = 67,
                MotorCurrent = 12.1,
            });
            Devices.Add(new DeviceViewModel
            {
                Id = "CONV-A",
                DisplayName = "输送线 A 段",
                IpAddress = "192.168.1.103",
                Status = DeviceStatus.Idle,
            });
            Devices.Add(new DeviceViewModel
            {
                Id = "CONV-B",
                DisplayName = "输送线 B 段",
                IpAddress = "192.168.1.102",
                Status = DeviceStatus.Error,
            });

            
            SelectedDevice = Devices[0];


            
        }
    }
}
