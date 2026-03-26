using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Infrastructure.Enums
{
    public enum ToastType
    {
        Loading,   // 蓝色转圈，无自动关闭
        Success,   // 绿色勾，默认 4s 后关闭
        Error,     // 红色叉，默认 6s 后关闭
    }
}
