using DigitalTwin.Shell.ViewModels;
using DigitalTwin.Shell.Views;
using DigitalTwin.Shell.Views.Pages;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Shell
{
    /// <summary>
    /// Shell 公共模块，负责注册与主窗口 Region 绑定的所有通用视图：
    ///   NavTabsRegion      → NavTabsView
    ///   IconNavRegion      → IconNavView
    ///   ConnStatusRegion   → ConnStatusView
    ///   ViewToolbarRegion  → ViewToolbarView
    ///   ViewModeLabelRegion→ ViewModeLabelView
    ///   TaskOverlayRegion  → TaskOverlayView
    ///   StatusBarRegion    → StatusBarView
    /// </summary>
    public class ShellModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavTabsView, NavTabsViewModel>();
            containerRegistry.RegisterForNavigation<IconNavView, IconNavViewModel>();
            containerRegistry.RegisterForNavigation<ConnStatusView, ConnStatusViewModel>();
            containerRegistry.RegisterForNavigation<ViewToolbarView, ViewToolbarViewModel>();
            containerRegistry.RegisterForNavigation<StatusBarView, StatusBarViewModel>();
            containerRegistry.RegisterForNavigation<RightPanelView, RightPanelViewModel>();
            containerRegistry.RegisterForNavigation<LeftPanelView, LeftPanelViewModel>();

            containerRegistry.RegisterForNavigation<InventoryPageView>();
            containerRegistry.RegisterForNavigation<TaskPageView>();
            containerRegistry.RegisterForNavigation<ReportPageView>();
            containerRegistry.RegisterForNavigation<ConfigPageView>();
            containerRegistry.RegisterForNavigation<AlarmPageView>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var rm = containerProvider.Resolve<IRegionManager>();

            // 直接导航注入，确保 Region 已就绪
            rm.RequestNavigate("NavTabsRegion", nameof(NavTabsView));
            rm.RequestNavigate("IconNavRegion", nameof(IconNavView));
            rm.RequestNavigate("ConnStatusRegion", nameof(ConnStatusView));
            rm.RequestNavigate("ViewToolbarRegion", nameof(ViewToolbarView));
            rm.RequestNavigate("StatusBarRegion", nameof(StatusBarView));
            rm.RequestNavigate("RightPanelRegion", nameof(RightPanelView));
            rm.RequestNavigate("LeftPanelRegion", nameof(LeftPanelView));
        }
    }
}
