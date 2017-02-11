using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuttyManager
{
    public partial class Downloader : Form
    {
        public Downloader()
        {
            InitializeComponent();
        }

        public void setProgress(int val)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => setProgress(val)));
            }
            else
            {
                progressBar1.Value = val;
            }
        }
    }
}
