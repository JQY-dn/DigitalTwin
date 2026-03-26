using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Timers;
using System.Windows;
using DigitalTwin.Infrastructure.Enums;
using DigitalTwin.Infrastructure.Events;

namespace DigitalTwin.Shell.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        // ── 时钟 ──────────────────────────────────────────
        private string _currentTime = DateTime.Now.ToString("HH:mm:ss");
        public string CurrentTime
        {
            get => _currentTime;
            private set => SetProperty(ref _currentTime, value);
        }

        private bool _isNonDashboardPage;
        public bool IsNonDashboardPage
        {
            get => _isNonDashboardPage;
            set
            {
                SetProperty(ref _isNonDashboardPage, value);
                RaisePropertyChanged(nameof(IsDashboardPage));
            }
        }

       
        public bool IsDashboardPage => !_isNonDashboardPage;

        public string UnityExePath { get; } =
            System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                @"Unity\DigitalTwinUnity.exe");

        public DelegateCommand MinimizeCommand { get; }
        public DelegateCommand MaximizeCommand { get; }
        public DelegateCommand CloseCommand { get; }

        public MainWindowViewModel(IEventAggregator ea)
        {
            ea.GetEvent<PageChangedEvent>()
              .Subscribe(page =>
              {
                  IsNonDashboardPage = (page != AppPage.Dashboard);
              }, ThreadOption.UIThread);

            var timer = new Timer(1000) { AutoReset = true };
            timer.Elapsed += (_, _) =>
                Application.Current?.Dispatcher.Invoke(
                    () => CurrentTime = DateTime.Now.ToString("HH:mm:ss"));
            timer.Start();

            MinimizeCommand = new DelegateCommand(
                () => Application.Current.MainWindow!.WindowState = WindowState.Minimized);

            MaximizeCommand = new DelegateCommand(() =>
            {
                var w = Application.Current.MainWindow!;
                w.WindowState = w.WindowState == WindowState.Maximized
                    ? WindowState.Normal : WindowState.Maximized;
            });

            CloseCommand = new DelegateCommand(
                () => Application.Current.MainWindow!.Close());
        }
    }
}
