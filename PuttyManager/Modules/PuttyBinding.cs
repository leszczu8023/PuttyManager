using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PuttyManager
{

    public class PuttyBinding
    {
        public readonly string executableName = "bin\\putty.exe";
        public readonly string downloadUrl = "https://the.earth.li/~sgtatham/putty/latest/x86/putty.exe";
        private PuttyManagerProfile run_afterdl;
        private Downloader dwl;

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

                var tmp = "";
                if (profile.useScript)
                {
                    tmp = Path.GetTempFileName();
                    System.IO.File.WriteAllText(tmp, replaceDOStoUNIX(profile.script), new UTF8Encoding(false));
                }

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.EnvironmentVariables["LocalAppData"] = Environment.CurrentDirectory + "\\bin";
                p.StartInfo.FileName = executableName;
                p.StartInfo.Arguments = "-load \"PTTM Temporary Profile\" -ssh " + profile.user + "@" + profile.hostname + " -P " + profile.port + " -pw " + profile.pass + ((profile.useScript) ? " -m \"" + tmp + "\"" : "");
                p.Start();

                Thread.Sleep(1000);
                removeTmpKey();
            }
            else
            {
                run_afterdl = profile;
                downloadPutty(parent);
            }
        }

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
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadFileAsync(new Uri(downloadUrl), executableName);
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
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
