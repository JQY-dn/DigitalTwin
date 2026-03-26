using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalTwin.Infrastructure.Enums;

namespace DigitalTwin.Infrastructure.Tools
{
    public class LogEntry
    {
        public DateTime Time { get; init; } = DateTime.Now;
        public LogLevel Level { get; init; }
        public string Tag { get; init; } = "";
        public string Message { get; init; } = "";

        /// <summary>格式化时间显示</summary>
        public string TimeText => Time.ToString("HH:mm:ss");

        /// <summary>级别标签</summary>
        public string LevelText => Level switch
        {
            LogLevel.Debug => "DBG",
            LogLevel.Success => "Suc",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Info => "INF",
            _ => "???"
        };

    }
}
