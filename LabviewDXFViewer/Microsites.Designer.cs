
namespace LabviewDXFViewer
{
    partial class Microsites
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.JunctionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Position = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestFunction = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Capacitance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Intercept = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TopWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BottomWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.JunctionName,
            this.Position,
            this.TestFunction,
            this.Result,
            this.Capacitance,
            this.Intercept,
            this.TopWidth,
            this.BottomWidth});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(901, 511);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseUp);
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // JunctionName
            // 
            this.JunctionName.HeaderText = "Junction Name";
            this.JunctionName.Name = "JunctionName";
            // 
            // Position
            // 
            this.Position.HeaderText = "Position";
            this.Position.Name = "Position";
            // 
            // TestFunction
            // 
            this.TestFunction.HeaderText = "Function";
            this.TestFunction.Items.AddRange(new object[] {
            "IV",
            "IVC",
            "MidBreak",
            "WideBreak",
            "Transconductance",
            "Breakdown",
            "Joule",
            "dC/dV",
            "C/F",
            "Leakage",
            "Leakage Threshold"});
            this.TestFunction.Name = "TestFunction";
            // 
            // Result
            // 
            this.Result.HeaderText = "Conductance";
            this.Result.Name = "Result";
            // 
            // Capacitance
            // 
            this.Capacitance.HeaderText = "Capacitance";
            this.Capacitance.Name = "Capacitance";
            // 
            // Intercept
            // 
            this.Intercept.HeaderText = "Intercept";
            this.Intercept.Name = "Intercept";
            // 
            // TopWidth
            // 
            this.TopWidth.HeaderText = "Test Info";
            this.TopWidth.Name = "TopWidth";
            // 
            // BottomWidth
            // 
            this.BottomWidth.HeaderText = "Bottom Width (μm)";
            this.BottomWidth.Name = "BottomWidth";
            // 
            // Microsites
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Name = "Microsites";
            this.Size = new System.Drawing.Size(901, 511);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn JunctionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Position;
        private System.Windows.Forms.DataGridViewComboBoxColumn TestFunction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Result;
        private System.Windows.Forms.DataGridViewTextBoxColumn Capacitance;
        private System.Windows.Forms.DataGridViewTextBoxColumn Intercept;
        private System.Windows.Forms.DataGridViewTextBoxColumn TopWidth;
        private System.Windows.Forms.DataGridViewTextBoxColumn BottomWidth;
    }
}
