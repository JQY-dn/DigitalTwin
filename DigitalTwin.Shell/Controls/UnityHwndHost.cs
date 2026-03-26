using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DigitalTwin.Shell.Controls
{
    /// <summary>
    /// 将 Unity 独立进程的窗口嵌入为 WPF HwndHost 子窗口。
    /// 使用方式：在 XAML 中放置此控件，绑定 UnityExePath。
    /// </summary>
    public class UnityHwndHost : HwndHost
    {
        // ── Win32 ────────────────────────────────────────
        private const int GWL_STYLE   = -16;
        private const int WS_CHILD    = 0x40000000;
        private const int WS_VISIBLE  = 0x10000000;
        private const int WS_CLIPCHILDREN = 0x02000000;

        [DllImport("user32.dll")] static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")] static extern int    SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")] static extern bool   MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);
        [DllImport("user32.dll")] static extern bool   ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateWindowEx(int exStyle, string className, string windowName,
            int style, int x, int y, int w, int h,
            IntPtr parent, IntPtr menu, IntPtr instance, IntPtr param);
        [DllImport("user32.dll")] static extern bool DestroyWindow(IntPtr hWnd);

        // ── 依赖属性 ─────────────────────────────────────
        public static readonly DependencyProperty UnityExePathProperty =
            DependencyProperty.Register(nameof(UnityExePath), typeof(string),
                typeof(UnityHwndHost), new PropertyMetadata(string.Empty));

        public string UnityExePath
        {
            get => (string)GetValue(UnityExePathProperty);
            set => SetValue(UnityExePathProperty, value);
        }

        // ── 内部状态 ─────────────────────────────────────
        private Process? _unityProcess;
        private IntPtr   _unityHwnd   = IntPtr.Zero;
        private IntPtr   _placeholder = IntPtr.Zero;   // 异步等待期间的占位 HWND

        // ── HwndHost 核心方法 ────────────────────────────

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            // 1. 先创建一个占位子窗口（同步返回，不阻塞 UI 线程）
            _placeholder = CreateWindowEx(
                0, "Static", "",
                WS_CHILD | WS_VISIBLE | WS_CLIPCHILDREN,
                0, 0, (int)ActualWidth, (int)ActualHeight,
                hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            // 2. 异步启动 Unity，完成后替换占位
            _ = Task.Run(() => LaunchAndAttachUnity(hwndParent.Handle));

            return new HandleRef(this, _placeholder);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (_unityProcess is { HasExited: false })
            {
                _unityProcess.Kill();
                _unityProcess.Dispose();
            }
            if (_placeholder != IntPtr.Zero)
                DestroyWindow(_placeholder);
        }

        // ── Unity 启动 + 挂接 ────────────────────────────

        private async Task LaunchAndAttachUnity(IntPtr parentHwnd)
        {
            if (string.IsNullOrEmpty(UnityExePath) || !System.IO.File.Exists(UnityExePath))
                return;

            _unityProcess = Process.Start(new ProcessStartInfo
            {
                FileName  = UnityExePath,
                Arguments = "-popupwindow",   // 无边框独立窗口
                UseShellExecute = false
            });

            if (_unityProcess == null) return;

            // 轮询等待 Unity 主窗口句柄就绪（最多 30 秒）
            _unityHwnd = await WaitForMainWindowAsync(_unityProcess, TimeSpan.FromSeconds(30));
            if (_unityHwnd == IntPtr.Zero) return;

            // 3. 切回 UI 线程完成窗口挂接
            await Dispatcher.InvokeAsync(() =>
            {
                SetParent(_unityHwnd, parentHwnd);
                SetWindowLong(_unityHwnd, GWL_STYLE, WS_CHILD | WS_VISIBLE);
                ShowWindow(_unityHwnd, 1);
                ResizeUnity();                // 立即同步到当前尺寸
            });
        }

        private static async Task<IntPtr> WaitForMainWindowAsync(Process proc, TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < deadline)
            {
                await Task.Delay(200);
                proc.Refresh();
                if (proc.MainWindowHandle != IntPtr.Zero)
                    return proc.MainWindowHandle;
            }
            return IntPtr.Zero;
        }

        // ── 尺寸同步（DPI 感知）────────────────────────────

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ResizeUnity();
        }

        private void ResizeUnity()
        {
            if (_unityHwnd == IntPtr.Zero) return;

            // 获取 DPI 缩放比，将逻辑像素转换为物理像素
            var source = PresentationSource.FromVisual(this);
            double dpiX = source?.CompositionTarget.TransformToDevice.M11 ?? 1.0;
            double dpiY = source?.CompositionTarget.TransformToDevice.M22 ?? 1.0;

            MoveWindow(_unityHwnd, 0, 0,
                (int)(ActualWidth  * dpiX),
                (int)(ActualHeight * dpiY),
                true);
        }
    }
}
