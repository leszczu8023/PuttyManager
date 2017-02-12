using PuttyManager.Gui;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PuttyManager
{
    public partial class MainWindow : Form
    {
        public ConfigManager puttyManager;
        public ImageList imageListLarge = new ImageList();
        public SimpleEncryption enc;
        public Dictionary<IntPtr, string> clients;
        

        public MainWindow()
        {
            InitializeComponent();
            clients = new Dictionary<IntPtr, string>();
            button1.Visible = false;
            bool passok = false;
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
                if (puttyManager == null) puttyManager = new ConfigManager();
                saveConfig();
            }
            XmlSerializer xmlser = new XmlSerializer(typeof(ConfigManager));
            var t = File.ReadAllBytes("config.dat");

            if (!startsWith(MAGIC_HEADER, t)) throw new Exception("Config file corrupted.");

            var str = enc.Decrypt(t.Skip(MAGIC_HEADER.Count()).ToArray());

            if (!str.StartsWith("<?xml")) throw new Exception("Invalid password or file corrupted.");

            using (TextReader reader = new StringReader(str))
            {
                puttyManager = (ConfigManager)xmlser.Deserialize(reader);
            }
        }

        public void saveConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigManager));
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
            var senderList = (Gui.MyListView)sender;
            var clickedItem = senderList.HitTest(e.Location).Item;
            if (clickedItem != null)
            {
                startputty(puttyManager.profiles[clickedItem.Index], this);
            }
        }

        private void startputty(PuttyManagerProfile profile, MainWindow parent)
        {
            Thread t = new Thread(() => new PuttyBinding().Start(profile, parent));
            t.SetApartmentState(ApartmentState.MTA);
            t.Start();
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


        FormWindowState LastWindowState = FormWindowState.Minimized;
        private void Form1_Resize(object sender, EventArgs e)
        {

            // When window state changes
            if (WindowState != LastWindowState)
            {
                LastWindowState = WindowState;
                if (WindowState == FormWindowState.Maximized)
                {

                    MainWindow_ResizeEnd(null, null);
                }
                if (WindowState == FormWindowState.Normal)
                {

                    MainWindow_ResizeEnd(null, null);
                }
            }

        }

        public IntPtr createTabPageAndGetHandle(string name)
        {
            IntPtr handle = new IntPtr(0);
            this.Invoke(new MethodInvoker(() => {
                TabPage a = new TabPage(name);
                

                a.Name = "prs" + Program.index;
                tabControl1.TabPages.Add(a);
                tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
                
                PuttyHost h = new PuttyHost();
                tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(h);
                h.Dock = DockStyle.Fill;
                handle = ((PuttyHost)tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls[0]).getObj().Handle;
            }));
            
            return handle;
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
            startputty(puttyManager.profiles[listView1.SelectedIndices[0]], this);
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 0)
            {
                if (MessageBox.Show("Are you sure to close a connection?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var tb = tabControl1.SelectedTab;

                    tabControl1.TabPages.Remove(tb);
                    tb.Dispose();
                    tb = null;
                }
            }       
        }

        private void MainWindow_ResizeEnd(object sender, EventArgs e)
        {
            if (clients == null) return;
            foreach (var cl in clients)
            {
                PuttyBinding.WindowsReStyle(cl.Key);
            }
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            button1.Visible = tabControl1.SelectedIndex != 0;
        }
    }
}

