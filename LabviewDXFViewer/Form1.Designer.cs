namespace LabviewDXFViewer
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.microsites1 = new LabviewDXFViewer.Microsites();
            this.dxfCanvas1 = new LabviewDXFViewer.DXFCanvas();
            this.joystick21 = new LabviewDXFViewer.Joystick2();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(613, 437);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load Mask File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1013, 437);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(932, 437);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Load";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(734, 438);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(192, 21);
            this.comboBox1.TabIndex = 7;
            // 
            // microsites1
            // 
            this.microsites1.Canvas = null;
            this.microsites1.Location = new System.Drawing.Point(721, 32);
            this.microsites1.Name = "microsites1";
            this.microsites1.Size = new System.Drawing.Size(367, 386);
            this.microsites1.TabIndex = 2;
            // 
            // dxfCanvas1
            // 
            this.dxfCanvas1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dxfCanvas1.Location = new System.Drawing.Point(12, 32);
            this.dxfCanvas1.Name = "dxfCanvas1";
            this.dxfCanvas1.Size = new System.Drawing.Size(703, 386);
            this.dxfCanvas1.TabIndex = 1;
            // 
            // joystick21
            // 
            this.joystick21.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.joystick21.DownMore = false;
            this.joystick21.DownMove = false;
            this.joystick21.HomeMove = false;
            this.joystick21.LeftMore = false;
            this.joystick21.LeftMove = false;
            this.joystick21.LoadMove = false;
            this.joystick21.Location = new System.Drawing.Point(218, 75);
            this.joystick21.Name = "joystick21";
            this.joystick21.RightMore = false;
            this.joystick21.RightMove = false;
            this.joystick21.Size = new System.Drawing.Size(360, 318);
            this.joystick21.TabIndex = 8;
            this.joystick21.UpMore = false;
            this.joystick21.UpMove = false;
            this.joystick21.ZeroMove = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 477);
            this.Controls.Add(this.joystick21);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.microsites1);
            this.Controls.Add(this.dxfCanvas1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Mask Labeller";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private DXFCanvas dxfCanvas1;
        private Microsites microsites1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox1;
        private Joystick2 joystick21;
    }
}

