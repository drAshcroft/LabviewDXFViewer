
namespace LabviewDXFViewer
{
    partial class ImageButton
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageButton));
            this.bLeft = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // bLeft
            // 
            this.bLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLeft.BackgroundImage =  ((System.Drawing.Image)(resources.GetObject("bLeft.Image")));
            this.bLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bLeft.Location = new System.Drawing.Point(0, 0);
            this.bLeft.Name = "bLeft";
            this.bLeft.Size = new System.Drawing.Size(45, 48);
            
            this.bLeft.TabIndex = 3;
            this.bLeft.UseVisualStyleBackColor = true;
            this.bLeft.Click += new System.EventHandler(this.bLeft_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "test";
            // 
            // LeftButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.bLeft);
            this.Name = "LeftButton";
            this.Size = new System.Drawing.Size(45, 48);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button bLeft;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
