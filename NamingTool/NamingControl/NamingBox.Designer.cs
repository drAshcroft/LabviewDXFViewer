
namespace NamingControl
{
    partial class NamingBox
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
            this.tbWaferChip = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTags = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.bNextStep = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbNotes = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbBuffer = new System.Windows.Forms.ComboBox();
            this.tbExperiment = new System.Windows.Forms.ComboBox();
            this.tbControl = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbReference = new System.Windows.Forms.ComboBox();
            this.tbBias = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.b48 = new System.Windows.Forms.Button();
            this.b37 = new System.Windows.Forms.Button();
            this.b26 = new System.Windows.Forms.Button();
            this.b15 = new System.Windows.Forms.Button();
            this.tbChannelNames = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Wafer, Chip";
            // 
            // tbWaferChip
            // 
            this.tbWaferChip.Location = new System.Drawing.Point(6, 30);
            this.tbWaferChip.Name = "tbWaferChip";
            this.tbWaferChip.Size = new System.Drawing.Size(178, 20);
            this.tbWaferChip.TabIndex = 2;
            this.tbWaferChip.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbWaferChip_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tags (Optional)";
            // 
            // tbTags
            // 
            this.tbTags.Location = new System.Drawing.Point(6, 77);
            this.tbTags.Name = "tbTags";
            this.tbTags.Size = new System.Drawing.Size(178, 20);
            this.tbTags.TabIndex = 4;
            this.tbTags.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbTags_KeyUp);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Comparison Control";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Experiment";
            // 
            // bNextStep
            // 
            this.bNextStep.Location = new System.Drawing.Point(92, 56);
            this.bNextStep.Name = "bNextStep";
            this.bNextStep.Size = new System.Drawing.Size(79, 26);
            this.bNextStep.TabIndex = 8;
            this.bNextStep.Text = "Next Step";
            this.bNextStep.UseVisualStyleBackColor = true;
            this.bNextStep.Click += new System.EventHandler(this.bNextStep_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Bias Voltage";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Reference Voltage";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Buffer (Optional)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 149);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Notes (Optional)";
            // 
            // tbNotes
            // 
            this.tbNotes.Location = new System.Drawing.Point(6, 169);
            this.tbNotes.Name = "tbNotes";
            this.tbNotes.Size = new System.Drawing.Size(178, 20);
            this.tbNotes.TabIndex = 16;
            this.tbNotes.TextChanged += new System.EventHandler(this.tbNotes_TextChanged);
            this.tbNotes.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbWaferChip_KeyUp);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Channel Names";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbWaferChip);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbTags);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(192, 105);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbBuffer);
            this.groupBox2.Controls.Add(this.tbExperiment);
            this.groupBox2.Controls.Add(this.tbControl);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbNotes);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.bNextStep);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(3, 114);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 198);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            // 
            // tbBuffer
            // 
            this.tbBuffer.AutoCompleteCustomSource.AddRange(new string[] {
            "1mmPB",
            "DIW"});
            this.tbBuffer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbBuffer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbBuffer.FormattingEnabled = true;
            this.tbBuffer.Location = new System.Drawing.Point(6, 128);
            this.tbBuffer.Name = "tbBuffer";
            this.tbBuffer.Size = new System.Drawing.Size(176, 21);
            this.tbBuffer.TabIndex = 19;
            this.tbBuffer.SelectedIndexChanged += new System.EventHandler(this.tbControl_SelectedIndexChanged);
            this.tbBuffer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbControl_KeyUp);
            // 
            // tbExperiment
            // 
            this.tbExperiment.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbExperiment.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbExperiment.FormattingEnabled = true;
            this.tbExperiment.Location = new System.Drawing.Point(6, 85);
            this.tbExperiment.Name = "tbExperiment";
            this.tbExperiment.Size = new System.Drawing.Size(177, 21);
            this.tbExperiment.TabIndex = 18;
            this.tbExperiment.SelectedIndexChanged += new System.EventHandler(this.tbControl_SelectedIndexChanged);
            this.tbExperiment.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbControl_KeyUp);
            // 
            // tbControl
            // 
            this.tbControl.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbControl.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbControl.FormattingEnabled = true;
            this.tbControl.Location = new System.Drawing.Point(6, 29);
            this.tbControl.Name = "tbControl";
            this.tbControl.Size = new System.Drawing.Size(177, 21);
            this.tbControl.TabIndex = 17;
            this.tbControl.SelectedIndexChanged += new System.EventHandler(this.tbControl_SelectedIndexChanged);
            this.tbControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbControl_KeyUp);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbReference);
            this.groupBox3.Controls.Add(this.tbBias);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(3, 318);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(191, 117);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            // 
            // tbReference
            // 
            this.tbReference.AutoCompleteCustomSource.AddRange(new string[] {
            "0 mV",
            "100 mV",
            "200 mV",
            "-100 mV",
            "-200 mV"});
            this.tbReference.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbReference.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbReference.FormattingEnabled = true;
            this.tbReference.Items.AddRange(new object[] {
            "0 mV",
            "100 mV",
            "200 mV",
            "-100 mV",
            "-200 mV"});
            this.tbReference.Location = new System.Drawing.Point(6, 90);
            this.tbReference.Name = "tbReference";
            this.tbReference.Size = new System.Drawing.Size(174, 21);
            this.tbReference.TabIndex = 14;
            this.tbReference.SelectedIndexChanged += new System.EventHandler(this.tbControl_SelectedIndexChanged);
            this.tbReference.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbControl_KeyUp);
            // 
            // tbBias
            // 
            this.tbBias.AutoCompleteCustomSource.AddRange(new string[] {
            "0 mV",
            "100 mV",
            "200 mV",
            "-100 mV",
            "-200 mV"});
            this.tbBias.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tbBias.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbBias.FormattingEnabled = true;
            this.tbBias.Items.AddRange(new object[] {
            "0 mV",
            "100 mV",
            "200 mV",
            "-100 mV",
            "-200 mV"});
            this.tbBias.Location = new System.Drawing.Point(6, 40);
            this.tbBias.Name = "tbBias";
            this.tbBias.Size = new System.Drawing.Size(175, 21);
            this.tbBias.TabIndex = 13;
            this.tbBias.SelectedIndexChanged += new System.EventHandler(this.tbControl_SelectedIndexChanged);
            this.tbBias.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbControl_KeyUp);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.b48);
            this.groupBox4.Controls.Add(this.b37);
            this.groupBox4.Controls.Add(this.b26);
            this.groupBox4.Controls.Add(this.b15);
            this.groupBox4.Controls.Add(this.tbChannelNames);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(3, 441);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(192, 92);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            // 
            // b48
            // 
            this.b48.Location = new System.Drawing.Point(137, 58);
            this.b48.Name = "b48";
            this.b48.Size = new System.Drawing.Size(33, 21);
            this.b48.TabIndex = 25;
            this.b48.Text = "48";
            this.b48.UseVisualStyleBackColor = true;
            this.b48.Click += new System.EventHandler(this.b48_Click);
            // 
            // b37
            // 
            this.b37.Location = new System.Drawing.Point(98, 58);
            this.b37.Name = "b37";
            this.b37.Size = new System.Drawing.Size(33, 21);
            this.b37.TabIndex = 24;
            this.b37.Text = "37";
            this.b37.UseVisualStyleBackColor = true;
            this.b37.Click += new System.EventHandler(this.b37_Click);
            // 
            // b26
            // 
            this.b26.Location = new System.Drawing.Point(59, 58);
            this.b26.Name = "b26";
            this.b26.Size = new System.Drawing.Size(33, 21);
            this.b26.TabIndex = 23;
            this.b26.Text = "26";
            this.b26.UseVisualStyleBackColor = true;
            this.b26.Click += new System.EventHandler(this.b26_Click);
            // 
            // b15
            // 
            this.b15.Location = new System.Drawing.Point(20, 58);
            this.b15.Name = "b15";
            this.b15.Size = new System.Drawing.Size(33, 21);
            this.b15.TabIndex = 22;
            this.b15.Text = "15";
            this.b15.UseVisualStyleBackColor = true;
            this.b15.Click += new System.EventHandler(this.b15_Click);
            // 
            // tbChannelNames
            // 
            this.tbChannelNames.Location = new System.Drawing.Point(6, 32);
            this.tbChannelNames.Name = "tbChannelNames";
            this.tbChannelNames.Size = new System.Drawing.Size(178, 20);
            this.tbChannelNames.TabIndex = 22;
            this.tbChannelNames.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbChannelNames_KeyUp);
            // 
            // NamingBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "NamingBox";
            this.Size = new System.Drawing.Size(200, 612);
            this.Load += new System.EventHandler(this.NamingBox_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbWaferChip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbTags;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bNextStep;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbNotes;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button b48;
        private System.Windows.Forms.Button b37;
        private System.Windows.Forms.Button b26;
        private System.Windows.Forms.Button b15;
        private System.Windows.Forms.TextBox tbChannelNames;
        private System.Windows.Forms.ComboBox tbBuffer;
        private System.Windows.Forms.ComboBox tbExperiment;
        private System.Windows.Forms.ComboBox tbControl;
        private System.Windows.Forms.ComboBox tbReference;
        private System.Windows.Forms.ComboBox tbBias;
    }
}
