using DigitalTwin.Infrastructure.Enums;
using DigitalTwin.Infrastructure.Models;
using Prism.Events;


namespace DigitalTwin.Infrastructure.Events
{
    public class PageChangedEvent : PubSubEvent<AppPage> { }

    /// <summary>Modbus 连接状态变化（true=已连接）</summary>
    public class ModbusConnectionChangedEvent : PubSubEvent<bool> { }

    /// <summary>设备数据更新</summary>
    public class SrmDataUpdatedEvent : PubSubEvent<SrmDeviceData> { }

    /// <summary>超过最大重连次数，payload=IP地址</summary>
    public class ModbusMaxReconnectFailedEvent : PubSubEvent<string> { }

    /// <summary>丢包率超阈值警告，payload=丢包率%</summary>
    public class PacketLossWarningEvent : PubSubEvent<double> { }
}
