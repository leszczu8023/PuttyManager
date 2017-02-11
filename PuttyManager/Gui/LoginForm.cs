using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PuttyManager
{
    public partial class LoginForm : Form
    {
        internal string passwd;

        public LoginForm()
        {
            InitializeComponent();
            if (File.Exists("config.dat"))
            {
                panel1.Visible = false;
                button1.Text = "Decrypt";
                this.Height = 160;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (panel1.Visible)
            {
                if (textBox1.Text != textBox2.Text)
                {
                    MessageBox.Show("Passwords doesn't match", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (string.IsNullOrEmpty(textBox1.Text) || textBox1.Text.Length < 4)
                {
                    MessageBox.Show("Password must have 4 characters or more.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            passwd = textBox1.Text;
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(null, null);
            }
        }
    }
}
