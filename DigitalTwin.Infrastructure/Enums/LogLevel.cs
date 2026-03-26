using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Infrastructure.Enums
{
    public enum LogLevel
    {
        /// <summary>调试信息（仅开发阶段）</summary>
        Debug = 0,

        /// <summary>成功信息</summary>
        Success = 1,

        /// <summary>警告信息</summary>
        Warning = 2,

        /// <summary>错误信息</summary>
        Error = 3,

        /// <summary>严重错误（致命异常）</summary>
        Info = 4
    }
}
