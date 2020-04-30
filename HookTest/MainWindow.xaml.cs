using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace HookTest {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        // API calls to give us a bit more information about the data we get from the hook
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);
        [DllImport("user32.dll")]
        private static extern uint RealGetWindowClass(IntPtr hWnd, StringBuilder pszType, uint cchType);

        private Wilsons.GlobalHooks globalHook;

        public MainWindow() {
            InitializeComponent();

            this.Loaded += (s, e) => {

                globalHook = new Wilsons.GlobalHooks((new WindowInteropHelper(this)).Handle);

                globalHook.CBT.Activate += (IntPtr ptr) => { listCBT.Items.Add("Activate: " + GetWindowName(ptr)); };
                globalHook.CBT.CreateWindow += (IntPtr ptr) => { listCBT.Items.Add("CreateWindow: " + GetWindowName(ptr)); };
                globalHook.CBT.DestroyWindow += (IntPtr ptr) => { listCBT.Items.Add("DestroyWindow: " + GetWindowName(ptr)); };
                globalHook.CBT.MinMax += (IntPtr ptr) => { listCBT.Items.Add("MinMax: " + GetWindowName(ptr)); };
                globalHook.CBT.SysCommand += (int SysCommand, int lParam) => { listCBT.Items.Add("SysCommand: " + SysCommand.ToString() + " " + lParam.ToString()); };
                globalHook.CBT.SetFocus += (IntPtr ptr) => { listCBT.Items.Add("SetFocus: " + GetWindowName(ptr)); };
                globalHook.CBT.MoveSize += (IntPtr ptr) => { listCBT.Items.Add("MoveSize: " + GetWindowName(ptr)); };

                globalHook.Shell.ActivateShellWindow += () => { listShell.Items.Add("ActivateShellWindow"); };
                globalHook.Shell.GetMinRect += (IntPtr ptr) => { listShell.Items.Add("GetMinRect: " + GetWindowName(ptr)); };
                globalHook.Shell.Language += (IntPtr ptr) => { listShell.Items.Add("Language: " + GetWindowName(ptr)); };
                globalHook.Shell.Redraw += (IntPtr ptr) => { listShell.Items.Add("Redraw: " + GetWindowName(ptr)); };
                globalHook.Shell.Taskman += () => { listShell.Items.Add("Taskman"); };
                globalHook.Shell.WindowActivated += (IntPtr ptr) => { listShell.Items.Add("WindowActivated: " + GetWindowName(ptr)); };
                globalHook.Shell.WindowCreated += (IntPtr ptr) => { listShell.Items.Add("WindowCreated: " + GetWindowName(ptr)); };
                globalHook.Shell.WindowDestroyed += (IntPtr ptr) => { listShell.Items.Add("WindowDestroyed: " + GetWindowName(ptr)); };

            };
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            if(sender.Equals(hookCBT)){
                globalHook.CBT.Start();
            }
            else if(sender.Equals(unhookCBT) ){
                globalHook.CBT.Stop();
            }
            else if(sender.Equals(hookShell)) {
                globalHook.Shell.Start();
            }
            else if(sender.Equals(unhookShell)){
                globalHook.Shell.Start();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            base.OnClosing(e);
            globalHook.CBT.Stop();
            globalHook.Shell.Stop();
        }

        #region Windows API Helper Functions

        private string GetWindowName(IntPtr Hwnd) {
            // This function gets the name of a window from its handle
            StringBuilder Title = new StringBuilder(256);
            GetWindowText(Hwnd, Title, 256);

            return Title.ToString().Trim();
        }

        private string GetWindowClass(IntPtr Hwnd) {
            // This function gets the name of a window class from a window handle
            StringBuilder Title = new StringBuilder(256);
            RealGetWindowClass(Hwnd, Title, 256);

            return Title.ToString().Trim();
        }

        #endregion
    }
}
