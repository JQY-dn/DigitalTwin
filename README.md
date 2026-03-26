# 🏭 AS/RS Digital Twin · 智能立体库数字孪生平台（后端功能待添加）

> 基于 WPF + Prism + EF Core + Modbus TCP 的工业级自动化立体仓库数字孪生监控系统

---

## 📸 界面预览

```
┌─────────────────────────────────────────────────────────────────┐
│  ⬡ AS/RS TWIN  │ 总览 │ 库存管理 │ 任务调度 │ 报表 │ 配置 │ 报警  │
├──────┬──────────────┬──────────────────────────┬────────────────┤
│      │  设备列表    │                          │  KPI 四宫格    │
│ 图标 │  SRM-01 ●   │                          │  库存热力图    │
│ 导航 │  SRM-02 ●   │     Unity 3D 数字孪生     │  活跃报警      │
│      │  三轴实时    │        可视化区域          │  任务队列      │
│      │  OEE 数据   │                          │                │
│      │  实时日志    │                          │                │
└──────┴──────────────┴──────────────────────────┴────────────────┘
```

---

## ✨ 功能特性

- **实时设备监控**：通过 Modbus TCP 100ms 轮询堆垛机三轴位置、速度、状态字、温度、电机电流
- **心跳检测**：3秒心跳间隔，9秒超时自动触发重连，最多重试5次
- **丢包防护**：请求级重试（3次）、串行化锁（SemaphoreSlim）、数据完整性校验、物理范围校验、写入回读验证
- **差分推送**：数据无变化时不触发 UI 更新，减少无效渲染
- **3D 数字孪生**：Unity3D 实时驱动堆垛机动画，通过命名管道与 WPF 通信
- **库存热力图**：12列×8排货位占用可视化
- **报警管理**：实时报警列表，支持确认操作
- **任务调度**：任务队列展示，执行状态追踪
- **实时日志**：分级日志（DBG/INF/WRN/ERR/FTL），支持 Tag 过滤，最多保留200条
- **数据持久化**：EF Core + SQL Server，支持设备档案、库存、报警、任务记录

---

## 🏗️ 项目架构

```
DigitalTwin.sln
├── DigitalTwin.Shell/                  WPF 主程序（界面层）
│   ├── Views/                          所有 XAML 界面
│   │   ├── ShellWindow.xaml            主窗口（4列布局）
│   │   ├── LeftPanelView.xaml          左面板（设备+实时数据+OEE+日志）
│   │   ├── RightPanelView.xaml         右面板（KPI+热力图+报警+任务）
│   │   ├── NavTabsView.xaml            顶部导航 Tab
│   │   ├── UnityOverlayWindow.xaml     Unity 悬浮卡片层
│   │   └── Pages/                      各功能页面
│   ├── ViewModels/                     MVVM ViewModel 层
│   ├── Converters/                     值转换器
│   ├── Controls/                       自定义控件（UnityHwndHost）
│   ├── Themes/                         工业暗色主题（IndustrialDark.xaml）
│   └── AppEvents.cs                    Prism 事件定义
│
└── DigitalTwin.Infrastructure/         类库（业务逻辑 + 数据层）
    ├── Services/
    │   ├── ModbusCommService.cs        Modbus TCP 通信服务
    │   ├── DatabaseService.cs          EF Core 泛型数据库服务
    │   └── AppDbContext.cs             数据库上下文
    ├── Models/                         数据模型（SrmDeviceData 等）
    ├── Tools/                          工具类（LogEntry、ConnectionStringProvider）
    └── Enums/                          枚举定义（LogLevel 等）
```

### 架构分层原则

```
Shell（界面层）
    ↓ 依赖
Infrastructure（服务层）
    ↓ 依赖
数据库 / PLC 设备
```

`Infrastructure` 不依赖任何 WPF 命名空间，保持纯净可复用。

---

## 🛠️ 技术栈

| 类别 | 技术 |
|------|------|
| UI 框架 | WPF (.NET 8) + Prism 8.1.97 (DryIoc) |
| 3D 引擎 | Unity3D 2022 LTS (URP) |
| 通信协议 | Modbus TCP (EasyModbusTCP 5.6.0) |
| WPF↔Unity IPC | 命名管道 NamedPipe |
| 数据库 ORM | EF Core 8.0 + SQL Server |
| 编程语言 | C# 12 / .NET 8 |

---

## 📦 依赖包

### DigitalTwin.Shell

```xml
<PackageReference Include="Prism.DryIoc" Version="8.1.97" />
```

### DigitalTwin.Infrastructure

```xml
<PackageReference Include="EasyModbusTCP" Version="5.6.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.25" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.25" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.25" />
<PackageReference Include="Prism.DryIoc" Version="8.1.97" />
<PackageReference Include="System.Configuration.ConfigurationManager" Version="8.0.0" />
```

---

## ⚡ 快速开始

### 环境要求

- Visual Studio 2022+
- .NET 8 SDK
- SQL Server 2019+（或 SQL Server Express）

### 1. 克隆仓库

```bash
git clone https://github.com/your-username/DigitalTwin.git
cd DigitalTwin
```

### 2. 配置数据库连接字符串

编辑 `DigitalTwin.Shell/App.config`：

```xml
<connectionStrings>
    <add name="AppDb"
         connectionString="Server=localhost;Database=DigitalTwinDb;
                           Trusted_Connection=True;TrustServerCertificate=True;"
         providerName="System.Data.SqlClient"/>
</connectionStrings>
```

### 3. 初始化数据库

应用启动时会自动调用 `EnsureCreated()` 建表，无需手动迁移。

如需手动执行迁移：

```powershell
# 在包管理器控制台执行
Add-Migration InitialCreate -Project DigitalTwin.Infrastructure -StartupProject DigitalTwin.Shell
Update-Database -Project DigitalTwin.Infrastructure -StartupProject DigitalTwin.Shell
```

### 4. 配置 Modbus 设备地址

编辑 `DigitalTwin.Infrastructure/Services/ModbusCommService.cs`：

```csharp
private readonly Dictionary<string, byte> _deviceSlaveMap = new()
{
    { "SRM-01", 1 },   // 设备ID → Modbus从站地址
    { "SRM-02", 2 },
    { "SRM-03", 3 },
};
```

在 `App.xaml.cs` 里修改 PLC IP 地址：

```csharp
await modbus.ConnectAsync("192.168.1.100", 502);
```

### 5. 编译运行

```
Ctrl+Shift+B  →  重新生成解决方案
F5            →  启动调试
```

---

## 📡 Modbus 寄存器映射

| 地址 | 功能码 | 数据名称 | 说明 |
|------|--------|----------|------|
| 0x0000 | FC03 | X 轴位置 | 0 ~ 50000 mm |
| 0x0001 | FC03 | Y 轴位置 | 0 ~ 20000 mm |
| 0x0002 | FC03 | Z 轴伸出量 | 0 ~ 2000 mm |
| 0x0003 | FC03 | X 轴速度 | mm/s |
| 0x000A | FC03 | 设备状态字 | Bit0=运行 Bit1=故障 Bit2=有货 Bit3=门开 |
| 0x000B | FC03 | 运行模式 | 0手动 1半自动 2全自动 3维护 |
| 0x0014 | FC03 | 当前任务号高位 | UINT32 高16位 |
| 0x0015 | FC03 | 当前任务号低位 | UINT32 低16位 |
| 0x001E | FC04 | 报警代码 | 0 = 无报警 |
| 0x001F | FC04 | X 轴电机电流 | × 0.1 A |
| 0x0020 | FC04 | 控制器温度 | -40 ~ 120 ℃ |

---

## 🔌 通信可靠性设计

```
请求重试      → 单次失败自动重试3次，间隔50ms
请求串行化    → SemaphoreSlim 防止并发冲突
心跳检测      → 每3秒探测，超过9秒无响应触发重连
自动重连      → 每5秒重试，最多5次，超过后通知用户
数据校验      → 长度校验 + 物理范围校验（X/Y/Z/温度/电流）
写入验证      → 写完回读对比，防止写入丢包
丢包统计      → 实时计算丢包率，超10%触发告警
```

---

## 📂 项目文件结构

```
DigitalTwin/
├── DigitalTwin.Shell/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── App.config                      ← 数据库连接字符串
│   ├── AppEvents.cs                    ← Prism 事件定义
│   ├── ShellModule.cs
│   ├── Controls/
│   │   └── UnityHwndHost.cs
│   ├── Converters/
│   │   └── Converters.cs
│   ├── Themes/
│   │   └── IndustrialDark.xaml         ← 工业暗色主题
│   ├── ViewModels/
│   │   ├── ShellViewModel.cs
│   │   ├── LeftPanelViewModel.cs
│   │   ├── RightPanelViewModel.cs
│   │   ├── NavTabsViewModel.cs
│   │   ├── ConnStatusViewModel.cs
│   │   ├── StatusBarViewModel.cs
│   │   ├── ViewToolbarViewModel.cs
│   │   └── OverlayViewModels.cs
│   └── Views/
│       ├── ShellWindow.xaml
│       ├── LeftPanelView.xaml
│       ├── RightPanelView.xaml
│       ├── NavTabsView.xaml
│       ├── UnityOverlayWindow.xaml
│       ├── ShellSubViews.xaml
│       └── Pages/
│           ├── InventoryPageView.xaml
│           ├── TaskPageView.xaml
│           ├── ReportPageView.xaml
│           ├── ConfigPageView.xaml
│           └── AlarmPageView.xaml
│
├── DigitalTwin.Infrastructure/
│   ├── Enums/
│   │   └── LogLevel.cs
│   ├── Models/
│   │   ├── SrmDeviceData.cs
│   │   ├── Equipment.cs
│   │   └── LogEntry.cs
│   ├── Services/
│   │   ├── IModbusCommService.cs
│   │   ├── ModbusCommService.cs
│   │   ├── IDatabaseService.cs
│   │   ├── DatabaseService.cs
│   │   ├── AppDbContext.cs
│   │   └── AppDbContextFactory.cs
│   └── Tools/
│       └── ConnectionStringProvider.cs
│
└── README.md
```

---

## 🎨 主题色彩

| 变量 | 色值 | 用途 |
|------|------|------|
| Bg0 | `#07090F` | 最深背景 |
| Bg1 | `#0B0F1A` | 面板背景 |
| Cyan | `#00D8FF` | 主强调色、数值 |
| Green | `#00E5A0` | 运行状态、正常 |
| Amber | `#FFB020` | 警告、温度 |
| Red | `#FF3D5A` | 故障、报警 |
| Blue | `#3D7FFF` | 次要强调 |

---

## 📋 开发路线

- [x] WPF Prism 框架骨架
- [x] 工业暗色主题系统
- [x] 左面板（设备列表 + 实时数据 + OEE + 日志）
- [x] 右面板（KPI + 热力图 + 报警 + 任务队列）
- [x] 顶部导航 Tab 切换
- [x] Modbus TCP 通信服务（心跳 + 重连 + 丢包防护）
- [x] EF Core + SQL Server 数据持久化
- [ ] Unity3D 3D 可视化场景
- [ ] WPF ↔ Unity 命名管道通信
- [ ] 库存管理页面
- [ ] 任务调度页面
- [ ] 报表统计页面
- [ ] 系统配置页面
- [ ] 报警日志页面



## 📄 许可证

MIT License
