using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Shell.ViewModels
{
    public class ViewToolbarViewModel : BindableBase
    {
        private bool _isPerspective = true;
        public bool IsPerspective { get => _isPerspective; set => SetProperty(ref _isPerspective, value); }

        private bool _isTopView;
        public bool IsTopView { get => _isTopView; set => SetProperty(ref _isTopView, value); }

        private bool _isSideView;
        public bool IsSideView { get => _isSideView; set => SetProperty(ref _isSideView, value); }

        private bool _isFollowMode;
        public bool IsFollowMode { get => _isFollowMode; set => SetProperty(ref _isFollowMode, value); }

        public DelegateCommand ZoomInCommand { get; }
        public DelegateCommand ZoomOutCommand { get; }
        public DelegateCommand ResetViewCommand { get; }

        public ViewToolbarViewModel()
        {
            ZoomInCommand = new DelegateCommand(() => { /* 发送 Unity 命令 */ });
            ZoomOutCommand = new DelegateCommand(() => { /* 发送 Unity 命令 */ });
            ResetViewCommand = new DelegateCommand(() => { /* 发送 Unity 命令 */ });
        }
    }
}
