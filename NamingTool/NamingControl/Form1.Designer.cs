
namespace NamingControl
{
    partial class Form1
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.fileNamer2 = new NamingControl.FileNamer();
            this.fileNamer1 = new NamingControl.FileNamer();
            this.namingBox1 = new NamingControl.NamingBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(222, 101);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(790, 20);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(222, 170);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(790, 20);
            this.textBox2.TabIndex = 2;
            // 
            // fileNamer2
            // 
            this.fileNamer2.FileExtention = ".tdms";
            this.fileNamer2.Filename = "_0.tdms";
            this.fileNamer2.IVCurves = false;
            this.fileNamer2.Location = new System.Drawing.Point(231, 317);
            this.fileNamer2.Name = "fileNamer2";
            this.fileNamer2.Size = new System.Drawing.Size(779, 47);
            this.fileNamer2.TabIndex = 4;
            // 
            // fileNamer1
            // 
            this.fileNamer1.FileExtention = ".tdms";
            this.fileNamer1.Filename = "_0.tdms";
            this.fileNamer1.IVCurves = false;
            this.fileNamer1.Location = new System.Drawing.Point(228, 243);
            this.fileNamer1.Name = "fileNamer1";
            this.fileNamer1.Size = new System.Drawing.Size(783, 45);
            this.fileNamer1.TabIndex = 3;
            // 
            // namingBox1
            // 
            this.namingBox1.BaseDirectory = "c:\\DataStore";
            this.namingBox1.ChannelNames = " ";
            this.namingBox1.DisplayMode = NamingControl.DisplayMode.IV;
            this.namingBox1.FileBoxIV = null;
            this.namingBox1.FileBoxRT = null;
            this.namingBox1.Filename_IV = null;
            this.namingBox1.Filename_RT = null;
            this.namingBox1.Location = new System.Drawing.Point(12, 12);
            this.namingBox1.Name = "namingBox1";
            this.namingBox1.NamingInfos = null;
            this.namingBox1.Size = new System.Drawing.Size(204, 646);
            this.namingBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(349, 432);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 77);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 579);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.fileNamer2);
            this.Controls.Add(this.fileNamer1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.namingBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private NamingBox namingBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private FileNamer fileNamer1;
        private FileNamer fileNamer2;
        private System.Windows.Forms.Button button1;
    }
}

