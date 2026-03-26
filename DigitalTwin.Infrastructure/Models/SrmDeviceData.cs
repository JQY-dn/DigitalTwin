using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Infrastructure.Models
{
    public class SrmDeviceData
    {
        public string DeviceId { get; set; } = "";
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // ── 位置 ──────────────────────────────────────────
        public int XPosition { get; set; }   // mm  0x0000
        public int YPosition { get; set; }   // mm  0x0001
        public int ZPosition { get; set; }   // mm  0x0002

        // ── 速度 ──────────────────────────────────────────
        public int XSpeed { get; set; }   // mm/s 0x0003

        // ── 状态字 ────────────────────────────────────────
        public int StatusWord { get; set; }  // 0x000A
        public bool IsRunning => (StatusWord & 0x01) != 0;
        public bool IsFault => (StatusWord & 0x02) != 0;
        public bool HasCargo => (StatusWord & 0x04) != 0;
        public bool IsDoorOpen => (StatusWord & 0x08) != 0;

        // ── 运行模式 ──────────────────────────────────────
        public int RunMode { get; set; }      // 0x000B
        public string RunModeText => RunMode switch
        {
            0 => "手动",
            1 => "半自动",
            2 => "全自动",
            3 => "维护",
            _ => "未知"
        };

        // ── 任务号 ────────────────────────────────────────
        public int CurrentTaskId { get; set; } // 0x0014~0x0015

        // ── 报警 + 电流 + 温度 ────────────────────────────
        public int AlarmCode { get; set; } // FC04 0x001E
        public double MotorCurrent { get; set; } // ×0.1A
        public int Temperature { get; set; } // ℃
    }
}
