namespace Player {
    partial class KPC8Player {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KPC8Player));
            this.mnuToolBar = new System.Windows.Forms.ToolStrip();
            this.FileMnuDrop = new System.Windows.Forms.ToolStripDropDownButton();
            this.loadRomBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMnuDrop});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(1584, 25);
            this.mnuToolBar.TabIndex = 0;
            this.mnuToolBar.Text = "toolStrip1";
            // 
            // FileMnuDrop
            // 
            this.FileMnuDrop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.FileMnuDrop.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadRomBtn});
            this.FileMnuDrop.Image = ((System.Drawing.Image)(resources.GetObject("FileMnuDrop.Image")));
            this.FileMnuDrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FileMnuDrop.Name = "FileMnuDrop";
            this.FileMnuDrop.Size = new System.Drawing.Size(38, 22);
            this.FileMnuDrop.Text = "File";
            // 
            // loadRomBtn
            // 
            this.loadRomBtn.Name = "loadRomBtn";
            this.loadRomBtn.Size = new System.Drawing.Size(180, 22);
            this.loadRomBtn.Text = "Load ROM";
            this.loadRomBtn.Click += new System.EventHandler(this.loadRomBtn_Click);
            // 
            // KPC8Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1584, 861);
            this.Controls.Add(this.mnuToolBar);
            this.Name = "KPC8Player";
            this.Text = "KPC8 Player";
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip mnuToolBar;
        private ToolStripDropDownButton FileMnuDrop;
        private ToolStripMenuItem loadRomBtn;
    }
}