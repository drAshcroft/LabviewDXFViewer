namespace LabviewDXFViewer
{
    partial class DXFCanvas
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
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbLayerSelect = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSelectionMirror = new System.Windows.Forms.CheckBox();
            this.cbSelectionRotate = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Layers";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(136, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(668, 491);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // lbLayerSelect
            // 
            this.lbLayerSelect.CheckOnClick = true;
            this.lbLayerSelect.FormattingEnabled = true;
            this.lbLayerSelect.Location = new System.Drawing.Point(6, 29);
            this.lbLayerSelect.Name = "lbLayerSelect";
            this.lbLayerSelect.Size = new System.Drawing.Size(120, 169);
            this.lbLayerSelect.TabIndex = 3;
            this.lbLayerSelect.SelectedIndexChanged += new System.EventHandler(this.lbLayerSelect_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 234);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Selection";
            // 
            // cbSelectionMirror
            // 
            this.cbSelectionMirror.AutoSize = true;
            this.cbSelectionMirror.Location = new System.Drawing.Point(8, 250);
            this.cbSelectionMirror.Name = "cbSelectionMirror";
            this.cbSelectionMirror.Size = new System.Drawing.Size(52, 17);
            this.cbSelectionMirror.TabIndex = 5;
            this.cbSelectionMirror.Text = "Mirror";
            this.cbSelectionMirror.UseVisualStyleBackColor = true;
            // 
            // cbSelectionRotate
            // 
            this.cbSelectionRotate.AutoSize = true;
            this.cbSelectionRotate.Location = new System.Drawing.Point(8, 273);
            this.cbSelectionRotate.Name = "cbSelectionRotate";
            this.cbSelectionRotate.Size = new System.Drawing.Size(58, 17);
            this.cbSelectionRotate.TabIndex = 6;
            this.cbSelectionRotate.Text = "Rotate";
            this.cbSelectionRotate.UseVisualStyleBackColor = true;
            // 
            // DXFCanvas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbSelectionRotate);
            this.Controls.Add(this.cbSelectionMirror);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbLayerSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "DXFCanvas";
            this.Size = new System.Drawing.Size(807, 492);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckedListBox lbLayerSelect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbSelectionMirror;
        private System.Windows.Forms.CheckBox cbSelectionRotate;
    }
}
