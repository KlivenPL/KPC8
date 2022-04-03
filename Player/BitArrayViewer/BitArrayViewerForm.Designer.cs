namespace Player.BitArrayViewer {
    partial class BitArrayViewerForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.memoryView = new System.Windows.Forms.DataGridView();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.memoryView)).BeginInit();
            this.SuspendLayout();
            // 
            // memoryView
            // 
            this.memoryView.AllowUserToAddRows = false;
            this.memoryView.AllowUserToDeleteRows = false;
            this.memoryView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.memoryView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.memoryView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.memoryView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Address});
            this.memoryView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoryView.Location = new System.Drawing.Point(0, 0);
            this.memoryView.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.memoryView.Name = "memoryView";
            this.memoryView.ReadOnly = true;
            this.memoryView.RowHeadersVisible = false;
            this.memoryView.RowTemplate.Height = 25;
            this.memoryView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.memoryView.Size = new System.Drawing.Size(604, 961);
            this.memoryView.TabIndex = 0;
            this.memoryView.VirtualMode = true;
            // 
            // Address
            // 
            this.Address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Address.DividerWidth = 1;
            this.Address.HeaderText = "Address";
            this.Address.Name = "Address";
            this.Address.ReadOnly = true;
            this.Address.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Address.Width = 60;
            // 
            // BitArrayViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 961);
            this.Controls.Add(this.memoryView);
            this.MaximumSize = new System.Drawing.Size(630, 2000);
            this.Name = "BitArrayViewerForm";
            this.Text = "BitArrayViewerForm";
            ((System.ComponentModel.ISupportInitialize)(this.memoryView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView memoryView;
        private DataGridViewTextBoxColumn Address;
    }
}