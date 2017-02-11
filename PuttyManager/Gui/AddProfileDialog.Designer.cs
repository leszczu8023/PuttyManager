using FastColoredTextBoxNS;
using System.ComponentModel;

namespace PuttyManager
{
    partial class AddProfileDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddProfileDialog));
            this.mProfileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mHost = new System.Windows.Forms.TextBox();
            this.mPort = new System.Windows.Forms.NumericUpDown();
            this.mUsername = new System.Windows.Forms.TextBox();
            this.mPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.mComment = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.mscript = new FastColoredTextBoxNS.FastColoredTextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.mPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mscript)).BeginInit();
            this.SuspendLayout();
            // 
            // mProfileName
            // 
            this.mProfileName.Location = new System.Drawing.Point(12, 25);
            this.mProfileName.Name = "mProfileName";
            this.mProfileName.Size = new System.Drawing.Size(263, 20);
            this.mProfileName.TabIndex = 0;
            this.mProfileName.TextChanged += new System.EventHandler(this.mProfileName_TextChanged);
            this.mProfileName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 99;
            this.label1.Text = "Profile name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 99;
            this.label2.Text = "Host";
            // 
            // mHost
            // 
            this.mHost.Location = new System.Drawing.Point(12, 64);
            this.mHost.Name = "mHost";
            this.mHost.Size = new System.Drawing.Size(185, 20);
            this.mHost.TabIndex = 1;
            this.mHost.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
            // 
            // mPort
            // 
            this.mPort.Location = new System.Drawing.Point(203, 64);
            this.mPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.mPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mPort.Name = "mPort";
            this.mPort.Size = new System.Drawing.Size(72, 20);
            this.mPort.TabIndex = 2;
            this.mPort.Value = new decimal(new int[] {
            22,
            0,
            0,
            0});
            this.mPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
            // 
            // mUsername
            // 
            this.mUsername.Location = new System.Drawing.Point(12, 115);
            this.mUsername.Name = "mUsername";
            this.mUsername.Size = new System.Drawing.Size(263, 20);
            this.mUsername.TabIndex = 3;
            this.mUsername.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
            // 
            // mPassword
            // 
            this.mPassword.Location = new System.Drawing.Point(12, 154);
            this.mPassword.Name = "mPassword";
            this.mPassword.PasswordChar = '*';
            this.mPassword.Size = new System.Drawing.Size(263, 20);
            this.mPassword.TabIndex = 4;
            this.mPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 99;
            this.label3.Text = "Username";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 99;
            this.label4.Text = "Password";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(200, 280);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Create";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 280);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // mComment
            // 
            this.mComment.Location = new System.Drawing.Point(12, 225);
            this.mComment.Multiline = true;
            this.mComment.Name = "mComment";
            this.mComment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mComment.Size = new System.Drawing.Size(263, 49);
            this.mComment.TabIndex = 6;
            this.mComment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 209);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 99;
            this.label5.Text = "Comment";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(302, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 99;
            this.label6.Text = "Script:";
            // 
            // mscript
            // 
            this.mscript.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.mscript.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.mscript.BackBrush = null;
            this.mscript.CharHeight = 14;
            this.mscript.CharWidth = 8;
            this.mscript.CommentPrefix = "#";
            this.mscript.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mscript.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.mscript.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.mscript.IsReplaceMode = false;
            this.mscript.Location = new System.Drawing.Point(302, 25);
            this.mscript.Name = "mscript";
            this.mscript.Paddings = new System.Windows.Forms.Padding(0);
            this.mscript.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.mscript.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("mscript.ServiceColors")));
            this.mscript.Size = new System.Drawing.Size(481, 278);
            this.mscript.TabIndex = 9;
            this.mscript.Zoom = 100;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 183);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(142, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Execute script after login";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // AddProfileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 355);
            this.ControlBox = false;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.mscript);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.mComment);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mPassword);
            this.Controls.Add(this.mUsername);
            this.Controls.Add(this.mPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mHost);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mProfileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddProfileDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddProfileDialog";
            ((System.ComponentModel.ISupportInitialize)(this.mPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mscript)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mProfileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mHost;
        private System.Windows.Forms.NumericUpDown mPort;
        private System.Windows.Forms.TextBox mUsername;
        private System.Windows.Forms.TextBox mPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox mComment;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private FastColoredTextBox mscript;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}