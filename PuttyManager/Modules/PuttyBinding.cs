using PuttyManager.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PuttyManager
{

    public class PuttyBinding
    {
        private readonly string executableName = "bin\\putty.exe";
        private readonly string downloadUrl = "https://the.earth.li/~sgtatham/putty/latest/x86/putty.exe";
        private PuttyManagerProfile run_afterdl;
        private Downloader dwl;

        [DllImport("User32")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOP = new IntPtr(0);
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_SHOWWINDOW = 0x0040;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        private void createTmpKey(string profileName)
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\SimonTatham\\PuTTY\\Sessions\\PTTM%20Temporary%20Profile");
            key.SetValue("LFImpliesCR", 0x1);
            key.SetValue("WinTitle", "PTTM - " + profileName);
            key.Close();
        }

        private void removeTmpKey()
        {
            Microsoft.Win32.Registry.CurrentUser.DeleteSubKey("SOFTWARE\\SimonTatham\\PuTTY\\Sessions\\PTTM%20Temporary%20Profile");
        }

        public void Start(PuttyManagerProfile profile, MainWindow parent)
        {
            if (File.Exists(executableName))
            {
                createTmpKey(profile.name + " [" + profile.user + "@" + profile.hostname + ":" + profile.port + "]");
                Program.index++;

                var tmp = "";
                if (profile.useScript)
                {
                    tmp = Path.GetTempFileName();
                    System.IO.File.WriteAllText(tmp, replaceDOStoUNIX(profile.script), new UTF8Encoding(false));
                }

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.EnvironmentVariables["LocalAppData"] = Environment.CurrentDirectory + "\\bin";
                p.StartInfo.FileName = executableName;
                p.StartInfo.Arguments = "-load \"PTTM Temporary Profile\" -ssh " + profile.user + "@" + profile.hostname + " -P " + profile.port + " -pw \"" + profile.pass + "\"" + ((profile.useScript) ? " -m \"" + tmp + "\"" : "");
                p.Start();

                p.WaitForInputIdle(-1);
                
                removeTmpKey();
                var a = parent.createTabPageAndGetHandle(profile.name);
                SetParent(p.MainWindowHandle, a.pointer);
                AssignProcessToJobObject(p.MainWindowHandle, a.pointer);
                parent.clients.Add(p.MainWindowHandle, a.host);
                string addd = "prs" + Program.index;

                WindowsReStyle(p.MainWindowHandle, a.host);
                WindowsReStyle(p.MainWindowHandle, a.host);

                SetForegroundWindow(p.MainWindowHandle);

                p.WaitForExit();
                

                foreach (TabPage ad in parent.tabControl1.TabPages)
                {
                    if (ad.Name == addd)
                    {
                        if (!parent.IsDisposed)
                        parent.Invoke(new MethodInvoker(() => {
                            parent.tabControl1.TabPages.Remove(ad);
                            ad.Dispose();
                        }));
                    }
                }
                parent.clients.Remove(p.MainWindowHandle);
            }
            else
            {
                run_afterdl = profile;
                downloadPutty(parent);
            }
        }

        public static void focus(IntPtr p)
        {
            SetForegroundWindow(p);
        }

        #region handles
        #region Constants
        //Finds a window by class name
        [DllImport("USER32.DLL")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //Gets window attributes
        [DllImport("USER32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        //assorted constants needed
        public static uint MF_BYPOSITION = 0x400;
        public static uint MF_REMOVE = 0x1000;
        public static int GWL_STYLE = -16;
        public static int WS_CHILD = 0x40000000; //child window
        public static int WS_BORDER = 0x00800000; //window with border
        public static int WS_DLGFRAME = 0x00400000; //window with double border but no title
        public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar 
        public static int WS_SYSMENU = 0x00080000; //window menu  
        private const int WS_MINIMIZEBOX = 0x00020000;
        private const int WS_MINIMIZE = 0x20000000;
        private const int WS_THICKFRAME = 0x00040000;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        #endregion

        public static void WindowsReStyle(IntPtr window, PuttyHost Panel1)
        {
            int style = GetWindowLong(window, GWL_STYLE);
            SetWindowLongPtr(new HandleRef(null, window), GWL_STYLE, new IntPtr(style & ~WS_CAPTION & ~WS_SYSMENU & ~WS_THICKFRAME & ~WS_MINIMIZE & ~WS_MAXIMIZEBOX));
            ShowWindow(window, SW_SHOWMAXIMIZED);
            SetWindowPos(window, HWND_TOP,
                    Panel1.ClientRectangle.Left,
                    Panel1.ClientRectangle.Top,
                    Panel1.ClientRectangle.Width,
                    Panel1.ClientRectangle.Height,
                    SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }
        #endregion

        MainWindow p = null;

        public string replaceDOStoUNIX(string data)
        {
            return data.Replace("\r\n", "\n");
        }

        public void downloadPutty(MainWindow parent)
        {
            if (!Directory.Exists("bin")) Directory.CreateDirectory("bin");
            WebClient wc = new WebClient();
            dwl = new Downloader();
            Thread t = new Thread(() => { parent.Invoke(new MethodInvoker(() => dwl.ShowDialog(parent))); });
            t.SetApartmentState(ApartmentState.MTA);
            t.Start();
            p = parent;
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            wc.DownloadFileAsync(new Uri(downloadUrl), executableName);
            while (wc.IsBusy)
            {
                Application.DoEvents();
            }
            dwl.Invoke(new MethodInvoker(() => dwl.Close()));
            Start(run_afterdl, p);
            run_afterdl = null;
        }

        private void Wc_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            dwl.setProgress(e.ProgressPercentage);
        }
    }
}
