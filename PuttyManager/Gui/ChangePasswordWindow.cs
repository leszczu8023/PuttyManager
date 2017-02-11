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
    public partial class ChangePasswordWindow : Form
    {
        private MainWindow parent;
        public string newPassword
        {
            get { return textBox2.Text; }
        }

        public ChangePasswordWindow(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!parent.isPasswordCorrect(textBox1.Text)) { ms("Incorrect old password!"); return; }
            if (string.IsNullOrEmpty(textBox2.Text))  { ms("Passwords cannot be empty!"); return; }
            if (textBox2.Text != textBox3.Text) { ms("Passwords aren't same!"); return; }
            if (textBox2.Text.Length < 3) { ms("Minimal password lenght is 4!"); return; }
            DialogResult = DialogResult.OK;
        }

        private void ms(string par)
        {
            MessageBox.Show(par, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
