using DigitalTwin.Infrastructure.Enums;
using DigitalTwin.Infrastructure.Interface;
using DigitalTwin.Infrastructure.Services;
using DigitalTwin.Infrastructure.Tools;
using DigitalTwin.Shell.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Threading.Tasks;
using System.Windows;
using DigitalTwin.Infrastructure.Services;

namespace DigitalTwin.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        // 程序启动
        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);
            var log = Container.Resolve<ILogService>();
            log.Success("程序启动成功！");

            DispatcherUnhandledException += (s, ex) =>
            {
                log.Error("UI线程未处理异常", ex.Exception, tag: "GlobalHandler");
                ex.Handled = true; // 阻止程序崩溃
            };

            TaskScheduler.UnobservedTaskException += (s, ex) =>
            {
                log.Error("后台Task未处理异常", ex.Exception, tag: "GlobalHandler");
                ex.SetObserved();
            };
        }
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ShellModule>();         // ← 第一个
           
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<ILogService>(new LogService()
            {
                DedupWindow = TimeSpan.FromSeconds(10),
                DedupThreshold = 3,
                MinimumLevel = LogLevel.Debug,
                MaxFileSizeBytes = 10 * 1024 * 1024,
                RetainDays = 30
            });

            // ── EF Core（单例）───────────────────────────────────────────────
            var connectionString = ConnectionStringProvider.Get(); ;

            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            var dbFactory = new PooledDbContextFactory<AppDbContext>(dbOptions);
            var logService = Container.Resolve<ILogService>();

            containerRegistry.RegisterInstance<IDbContextFactory<AppDbContext>>(dbFactory);
            containerRegistry.RegisterInstance<IDatabaseService>(new DatabaseService(dbFactory, logService));
            
            

        }
        // 程序退出
        protected override void OnExit(ExitEventArgs e)
        {

            base.OnExit(e);
        }
    }
}
