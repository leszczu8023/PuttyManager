using FastColoredTextBoxNS;
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
    public partial class AddProfileDialog : Form
    {
        private MainWindow _parent;
        private bool editMode = false;

        public AddProfileDialog(MainWindow parent)
        {
            InitializeComponent();
            this.mscript.DescriptionFile = "style.xml";

            editMode = false;
            _parent = parent;
            this.Text = "Adding new profile.";

            if (!checkBox1.Checked)
            {
                this.Width = 305;
            }
            else
            {
                this.Width = 806;
            }

            this.mscript.Text = @"#!/bin/bash

# *** EXAMPLE SCRIPT ***

while true
do
    echo Example
    sleep 1
done
";
            this.mscript.Visible = checkBox1.Checked;
        }

        public AddProfileDialog(PuttyManagerProfile profile, MainWindow parent)
        {
            InitializeComponent();
            this.mscript.DescriptionFile = "style.xml";

            button1.Text = "Save changes";
            this.profile = profile;
            editMode = true;
            this.Text = "Editing profile \"" + profile.name + "\".";
            _parent = parent;
            mProfileName.Text = profile.name;
            mHost.Text = profile.hostname;
            mPort.Value = profile.port;
            mUsername.Text = profile.user;
            mPassword.Text = profile.pass;
            mComment.Text = profile.comment;
            mscript.Text = profile.script;
            checkBox1.Checked = profile.useScript;

            if (!checkBox1.Checked)
            {
                this.Width = 305;
            }
            else
            {
                this.Width = 806;
            }

            this.mscript.Visible = checkBox1.Checked;
        }

        public PuttyManagerProfile profile;

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(mProfileName.Text))
            {
                MessageBox.Show("Profile name cannot be empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (_parent.puttyManager.containsName(mProfileName.Text) && !editMode)
            {
                MessageBox.Show("Profile name aready exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(mHost.Text))
            {
                MessageBox.Show("Hostname cannot be empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (mPort.Value == 0)
            {
                MessageBox.Show("Port must be greather than 0", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(mUsername.Text))
            {
                MessageBox.Show("Username cannot be empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (string.IsNullOrEmpty(mPassword.Text))
            {
                MessageBox.Show("Password cannot be empty", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            profile = new PuttyManagerProfile();
            profile.name = mProfileName.Text;
            profile.hostname = mHost.Text;
            profile.port = int.Parse(mPort.Value.ToString());
            profile.user = mUsername.Text;
            profile.pass = mPassword.Text;
            profile.comment = mComment.Text;
            profile.script = mscript.Text;
            profile.useScript = checkBox1.Checked;
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void mProfileName_TextChanged(object sender, EventArgs e)
        {
            if (editMode)
            {
                this.Text = "Editing profile \"" + mProfileName.Text + "\".";
            }
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(null, null);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                this.Width = 305;
                this.Location = new Point(this.Location.X + 250, this.Location.Y);               
            }
            else
            {
                this.Width = 806;
                this.Location = new Point(this.Location.X - 250, this.Location.Y);
            }
            this.mscript.Visible = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            mPassword.UseSystemPasswordChar = !checkBox2.Checked;
        }
    }
}
