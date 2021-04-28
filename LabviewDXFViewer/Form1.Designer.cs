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
            this.joystick1 = new LabviewDXFViewer.Joystick();
            this.webCamViewer1 = new LabviewDXFViewer.WebCamViewer();
            this.microsites1 = new LabviewDXFViewer.Microsites();
            this.dxfCanvas1 = new LabviewDXFViewer.DXFCanvas();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(713, 415);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(966, 415);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // joystick1
            // 
            this.joystick1.Location = new System.Drawing.Point(696, 452);
            this.joystick1.Name = "joystick1";
            this.joystick1.Size = new System.Drawing.Size(197, 181);
            this.joystick1.TabIndex = 4;
            // 
            // webCamViewer1
            // 
            this.webCamViewer1.Location = new System.Drawing.Point(688, 444);
            this.webCamViewer1.Name = "webCamViewer1";
            this.webCamViewer1.Size = new System.Drawing.Size(340, 222);
            this.webCamViewer1.TabIndex = 3;
            // 
            // microsites1
            // 
            this.microsites1.Canvas = null;
            this.microsites1.Location = new System.Drawing.Point(721, 12);
            this.microsites1.Name = "microsites1";
            this.microsites1.Size = new System.Drawing.Size(367, 386);
            this.microsites1.TabIndex = 2;
            // 
            // dxfCanvas1
            // 
            this.dxfCanvas1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dxfCanvas1.Location = new System.Drawing.Point(12, 2);
            this.dxfCanvas1.Name = "dxfCanvas1";
            this.dxfCanvas1.Size = new System.Drawing.Size(703, 396);
            this.dxfCanvas1.TabIndex = 1;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(885, 415);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Load";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 678);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.joystick1);
            this.Controls.Add(this.webCamViewer1);
            this.Controls.Add(this.microsites1);
            this.Controls.Add(this.dxfCanvas1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private DXFCanvas dxfCanvas1;
        private Microsites microsites1;
        private WebCamViewer webCamViewer1;
        private Joystick joystick1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

