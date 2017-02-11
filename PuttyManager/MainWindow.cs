using PTM.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PuttyManager
{
    public partial class MainWindow : Form
    {
        public PuttyManager puttyManager;
        public ImageList imageListLarge = new ImageList();
        public SimpleEncryption enc;

        public MainWindow()
        {
            InitializeComponent();
            bool passok = false; ;
            while (!passok)
            {

                LoginForm f = new LoginForm();
                if (f.ShowDialog() == DialogResult.OK)
                {
                    enc = new SimpleEncryption(f.passwd);
                    f.passwd = "";
                }

                try
                {
                    deserializeConfig();
                    passok = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                }
            }

            imageListLarge.ImageSize = new Size(32, 32);
            imageListLarge.Images.Add(Properties.Resources.icon);
            listView1.LargeImageList = imageListLarge;

            puttyManager.version = Application.ProductVersion;

            refreshProfileList();
        }

        public void refreshProfileList()
        {
            refreshProfileList(false);
        }

        public void refreshProfileList(bool setLast)
        {
            int index;
            try
            {
                index = listView1.SelectedIndices[0];
            }
            catch (Exception e)
            {
                index = 0;
            }

            listView1.Items.Clear();
            if (puttyManager.profiles.Count > 0)
            {
                lockControls(false);
                foreach (PuttyManagerProfile profile in puttyManager.profiles)
                {
                    listView1.Items.Add(profile.name, 0);
                }

                if (listView1.Items.Count > 0)
                {
                    while (listView1.Items.Count - 1 < index && index != 0) index--;

                    if (setLast)
                    {
                        index = listView1.Items.Count - 1;
                    }

                    listView1.Items[index].Selected = true;
                    listView1.Select();
                }
            }
            else
            {
                lockControls(true);
            }
        }

        public static Stream strToStream(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private readonly byte[] MAGIC_HEADER = Encoding.UTF8.GetBytes("PTTMENC!");

        public bool startsWith(byte[] pattern, byte[] target)
        {
            var index = 0;
            foreach (char a in pattern)
            {
                if (a != target[index]) return false;
                index++;
            }
            return true;
        }

        public void deserializeConfig()
        {

            if (!File.Exists("config.dat"))
            {
                if (puttyManager == null) puttyManager = new PuttyManager();
                saveConfig();
            }
            XmlSerializer xmlser = new XmlSerializer(typeof(PuttyManager));
            var t = File.ReadAllBytes("config.dat");

            if (!startsWith(MAGIC_HEADER, t)) throw new Exception("Config file corrupted.");

            var str = enc.Decrypt(t.Skip(MAGIC_HEADER.Count()).ToArray());

            if (!str.StartsWith("<?xml")) throw new Exception("Invalid password or file corrupted.");

            using (TextReader reader = new StringReader(str))
            {
                puttyManager = (PuttyManager)xmlser.Deserialize(reader);
            }
        }

        public void saveConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PuttyManager));
            var data = "";
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, puttyManager);
                data = textWriter.ToString();
            }

            var a = new List<byte>();
            a.AddRange(MAGIC_HEADER);
            a.AddRange(enc.EncryptBytes(data));

            File.WriteAllBytes("config.dat", a.ToArray<byte>());
        }

        private void lockControls(bool hm)
        {
            toolStripButton2.Enabled = !hm;
            toolStripButton3.Enabled = !hm;
            label3.Text = (hm) ? "Please, add some items to connect..." : "";
            label3.Text = "";
            pname.Text = "";
            paddr.Text = "";
            pusr.Text = "";
            ppsswd.Text = "";
            stat.Text = "";
            stat.Image = null;
            splitContainer3.Enabled = !hm;
        }

        public bool isPasswordCorrect(string passwd)
        {
            return enc.isValidPass(passwd);
        }

        private Thread thr;

        private void asyncCheckStatus(string host, int port)
        {
            stat.Text = "Checking...";
            stat.Image = Properties.Resources.wait;

            if (thr != null)
            {
                if (thr.IsAlive)
                {
                    thr.Abort();
                }
            }

            thr = new Thread(() =>
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var status = checkStatus(host, port);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                if (status)
                {
                    Bitmap bars;
                    if (elapsedMs < 60)
                    {
                        bars = Properties.Resources.max;
                    }
                    else if (elapsedMs < 240)
                    {
                        bars = Properties.Resources.med;
                    }
                    else
                    {
                        bars = Properties.Resources.low;
                    }
                    this.Invoke(new MethodInvoker(() => {
                        stat.Text = "Online (" + elapsedMs + "ms)";
                        stat.Image = bars;
                    }));
                }
                else
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        stat.Text = "Offline\n\n(click here to refresh)";
                        stat.Image = Properties.Resources.none;
                    }));
                }
            });
            thr.SetApartmentState(ApartmentState.MTA);
            thr.IsBackground = true;
            thr.Start();      
        }

        private bool checkStatus(string host, int port)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(host, port);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3_Click_1(null, null);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                toolStripButton2_Click(null, null);
            }
            else if (e.KeyCode == Keys.F5)
            {
                stat_Click(null, null);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var senderList = (MyListView)sender;
            var clickedItem = senderList.HitTest(e.Location).Item;
            if (clickedItem != null)
            {
                //MessageBox.Show(clickedItem.Index + "");
                new Putty().Start(puttyManager.profiles[clickedItem.Index], this);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                var item = puttyManager.profiles[listView1.SelectedIndices[0]];
                label3.Text = item.comment;
                pname.Text = item.name;
                paddr.Text = item.hostname + ":" + item.port;
                pusr.Text = item.user;
                ppsswd.Text = "";
                foreach (char a in item.pass) ppsswd.Text += "*";
                asyncCheckStatus(item.hostname, item.port);
            }
            catch
            {

            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AddProfileDialog a = new AddProfileDialog(this);
            if (a.ShowDialog() == DialogResult.OK)
            {
                puttyManager.profiles.Add(a.profile);
                saveConfig();
                refreshProfileList(true);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var item = puttyManager.profiles[listView1.SelectedIndices[0]];
            if (MessageBox.Show("Are you sure to remove item \"" + item.name + "\"?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                puttyManager.profiles.Remove(item);
                saveConfig();
                refreshProfileList();
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            AddProfileDialog a = new AddProfileDialog(puttyManager.profiles[listView1.SelectedIndices[0]], this);
            if (a.ShowDialog() == DialogResult.OK)
            {
                puttyManager.profiles[listView1.SelectedIndices[0]] = a.profile;
                saveConfig();
                refreshProfileList();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            new Putty().Start(puttyManager.profiles[listView1.SelectedIndices[0]], this);
        }

        private void stat_Click(object sender, EventArgs e)
        {
            var item = puttyManager.profiles[listView1.SelectedIndices[0]];
            asyncCheckStatus(item.hostname, item.port);
        }

        private void MainWindow_Close(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var a = new ChangePasswordWindow(this);
            if (a.ShowDialog() == DialogResult.OK)
            {
                enc = new SimpleEncryption(a.newPassword);
                a = null;
                saveConfig();
                MessageBox.Show("Password changed successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }


    public class Putty
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

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            dwl.setProgress(e.ProgressPercentage);
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class PuttyManager
    {

        private List<PuttyManagerProfile> profilesField;

        public bool containsName(string name)
        {
            foreach (PuttyManagerProfile p in profilesField)
            {
                if (p.name.ToLower() == name.ToLower()) return true;
            }
            return false;
        }

        private string versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("profile", IsNullable = false)]
        public List<PuttyManagerProfile> profiles
        {
            get
            {
                return this.profilesField;
            }
            set
            {
                this.profilesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PuttyManagerProfile
    {

        private string nameField;

        private string hostnameField;

        private string userField;

        private string passField;

        private string commentField;

        private string scriptField;

        private bool useScriptField;

        private int portField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string hostname
        {
            get
            {
                return this.hostnameField;
            }
            set
            {
                this.hostnameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string user
        {
            get
            {
                return this.userField;
            }
            set
            {
                this.userField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string pass
        {
            get
            {
                return this.passField;
            }
            set
            {
                this.passField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string comment
        {
            get
            {
                return this.commentField;
            }
            set
            {
                this.commentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string script
        {
            get
            {
                return this.scriptField;
            }
            set
            {
                this.scriptField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool useScript
        {
            get
            {
                return this.useScriptField;
            }
            set
            {
                this.useScriptField = value;
            }
        }
    }

    public class SimpleEncryption
    {
        #region Constructor
        public SimpleEncryption(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltValueBytes = Encoding.UTF8.GetBytes(SaltValue);

            hash = CalculateMD5Hash(SaltValue + password + SaltValue.Reverse());

            _DeriveBytes = new Rfc2898DeriveBytes(passwordBytes, saltValueBytes, PasswordIterations);
            _InitVectorBytes = Encoding.UTF8.GetBytes(InitVector);
            _KeyBytes = _DeriveBytes.GetBytes(32);
        }
        #endregion

        private string hash;

        public bool isValidPass (string passwd)
        {
            return hash == CalculateMD5Hash(SaltValue + passwd + SaltValue.Reverse());
        }

        public string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        #region Private Fields
        private readonly Rfc2898DeriveBytes _DeriveBytes;
        private readonly byte[] _InitVectorBytes;
        private readonly byte[] _KeyBytes;
        #endregion

        private const string InitVector = "T=A4rAzu94ez-dra";
        private const int PasswordIterations = 1000; //2;
        private const string SaltValue = "TSE^%#%AER#QRTACc3fq23c2a3443t@#%$@#$@#$1323!@REFA$A#5AT$#!@$@#sFfzxv";

        public string Decrypt(string encryptedText)
        {
            byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);
            string plainText;

            RijndaelManaged rijndaelManaged = new RijndaelManaged { Mode = CipherMode.CBC };

            try
            {
                using (ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(_KeyBytes, _InitVectorBytes))
                {
                    using (MemoryStream memoryStream = new MemoryStream(encryptedTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            //TODO: Need to look into this more. Assuming encrypted text is longer than plain but there is probably a better way
                            byte[] plainTextBytes = new byte[encryptedTextBytes.Length];

                            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
            catch (CryptographicException exception)
            {
                plainText = string.Empty; // Assume the error is caused by an invalid password
            }

            return plainText;
        }

        public string Decrypt(byte[] encryptedTextBytes)
        {
            string plainText;

            RijndaelManaged rijndaelManaged = new RijndaelManaged { Mode = CipherMode.CBC };

            try
            {
                using (ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(_KeyBytes, _InitVectorBytes))
                {
                    using (MemoryStream memoryStream = new MemoryStream(encryptedTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            //TODO: Need to look into this more. Assuming encrypted text is longer than plain but there is probably a better way
                            byte[] plainTextBytes = new byte[encryptedTextBytes.Length];

                            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
            catch (CryptographicException exception)
            {
                plainText = string.Empty; // Assume the error is caused by an invalid password
            }

            return plainText;
        }

        public string Encrypt(string plainText)
        {
            string encryptedText;
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            RijndaelManaged rijndaelManaged = new RijndaelManaged { Mode = CipherMode.CBC };

            using (ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(_KeyBytes, _InitVectorBytes))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        byte[] cipherTextBytes = memoryStream.ToArray();
                        encryptedText = Convert.ToBase64String(cipherTextBytes);
                    }
                }
            }

            return encryptedText;
        }

        public byte[] EncryptBytes(string plainText)
        {

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            RijndaelManaged rijndaelManaged = new RijndaelManaged { Mode = CipherMode.CBC };

            using (ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(_KeyBytes, _InitVectorBytes))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        byte[] cipherTextBytes = memoryStream.ToArray();
                        return cipherTextBytes;
                    }
                }
            }
        }
    }
}

namespace PTM.Controls
{
    public class MyListView : ListView
    {
        protected override void WndProc(ref Message m)
        {
            // Swallow mouse messages that are not in the client area
            if (m.Msg >= 0x201 && m.Msg <= 0x209)
            {
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                var hit = this.HitTest(pos);
                switch (hit.Location)
                {
                    case ListViewHitTestLocations.AboveClientArea:
                    case ListViewHitTestLocations.BelowClientArea:
                    case ListViewHitTestLocations.LeftOfClientArea:
                    case ListViewHitTestLocations.RightOfClientArea:
                    case ListViewHitTestLocations.None:
                        return;
                }
            }
            base.WndProc(ref m);
        }
    }


}
