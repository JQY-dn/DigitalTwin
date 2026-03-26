using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Shell.ViewModels
{
    public class RightPanelViewModel : BindableBase
    {
        // KPI
        public double OccupancyRate { get; set; } = 78.3;
        public string OccupancyDelta { get; set; } = "↑ 2.1% 较昨日";
        public int TodayThroughput { get; set; } = 1247;
        public string ThroughputDetail { get; set; } = "入库 623 · 出库 624";
        public int ActiveAlarmCount { get; set; } = 2;
        public string UnacknowledgedText { get; set; } = "1 条未确认";
        public int TotalTaskCount { get; set; } = 18;
        public string TaskQueueDetail { get; set; } = "执行 3 · 等待 15";
        public string HeatmapSubtitle { get; set; } = "12列 × 8排";

        // 热力图
        public ObservableCollection<HeatmapCellVm> HeatmapCells { get; } = new();

        // 报警列表
        public ObservableCollection<AlarmItemVm> ActiveAlarms { get; } = new();

        // 任务队列
        public ObservableCollection<TaskItemVm> TaskQueue { get; } = new();

        public RightPanelViewModel()
        {
            InitHeatmap();
            InitAlarms();
            InitTasks();
        }

        private void InitHeatmap()
        {
            var rnd = new Random(42);
            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 12; col++)
                    HeatmapCells.Add(new HeatmapCellVm
                    {
                        Column = col + 1,
                        Row = row + 1,
                        Value = rnd.Next(0, 6),
                        Tooltip = $"列{col + 1} 排{row + 1}"
                    });
        }

        private void InitAlarms()
        {
            ActiveAlarms.Add(new AlarmItemVm
            {
                Title = "SRM-02 X轴超速报警",
                Code = "E-1042",
                TimeText = "14:32:07",
                LevelBrush = System.Windows.Media.Brushes.OrangeRed,
                IsUnacknowledged = true
            });
            ActiveAlarms.Add(new AlarmItemVm
            {
                Title = "输送线B段急停",
                Code = "E-2001",
                TimeText = "14:28:53",
                LevelBrush = System.Windows.Media.Brushes.Red,
                IsUnacknowledged = false
            });
            ActiveAlarms.Add(new AlarmItemVm
            {
                Title = "输送线C段急停",
                Code = "E-2002",
                TimeText = "14:28:53",
                LevelBrush = System.Windows.Media.Brushes.Red,
                IsUnacknowledged = false
            });
            ActiveAlarms.Add(new AlarmItemVm
            {
                Title = "输送线D段急停",
                Code = "E-2004",
                TimeText = "14:28:53",
                LevelBrush = System.Windows.Media.Brushes.Red,
                IsUnacknowledged = false
            });
            ActiveAlarms.Add(new AlarmItemVm
            {
                Title = "输送线C段急停",
                Code = "E-2002",
                TimeText = "14:28:53",
                LevelBrush = System.Windows.Media.Brushes.Red,
                IsUnacknowledged = false
            });
        }

        private void InitTasks()
        {
            TaskQueue.Add(new TaskItemVm { ShortId = "#247", TaskName = "入库 SKU-8821", RouteText = "入库口 → C3-R2-L4", Status = "Executing" });
            TaskQueue.Add(new TaskItemVm { ShortId = "#248", TaskName = "出库 SKU-4413", RouteText = "C7-R5-L2 → 出库口", Status = "Waiting" });
            TaskQueue.Add(new TaskItemVm { ShortId = "#249", TaskName = "盘点 C5列", RouteText = "C5-R1 → C5-R8", Status = "Waiting" });
            TaskQueue.Add(new TaskItemVm { ShortId = "#246", TaskName = "入库 SKU-3302", RouteText = "入库口 → C1-R4-L3", Status = "Done" });
        }
    }

    public class HeatmapCellVm
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public int Value { get; set; }   // 0~5
        public string Tooltip { get; set; } = "";
    }

    public class AlarmItemVm
    {
        public string Title { get; set; } = "";
        public string Code { get; set; } = "";
        public string TimeText { get; set; } = "";
        public bool IsUnacknowledged { get; set; }
        public System.Windows.Media.SolidColorBrush LevelBrush { get; set; }
            = System.Windows.Media.Brushes.Gray;
    }

    public class TaskItemVm
    {
        public string ShortId { get; set; } = "";
        public string TaskName { get; set; } = "";
        public string RouteText { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
