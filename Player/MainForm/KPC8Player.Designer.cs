namespace Player.MainForm {
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
            this.mnuFileDrop = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuFileLoadRomBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileLoadSourceBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuPlayBtn = new System.Windows.Forms.ToolStripButton();
            this.mnuDbgBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuStopBtn = new System.Windows.Forms.ToolStripButton();
            this.mnuPauseBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.canvasPnl = new System.Windows.Forms.Panel();
            this.mnuToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.AutoSize = false;
            this.mnuToolBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.mnuToolBar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mnuToolBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileDrop,
            this.toolStripSeparator1,
            this.mnuPlayBtn,
            this.mnuDbgBtn,
            this.toolStripSeparator2,
            this.mnuStopBtn,
            this.mnuPauseBtn,
            this.toolStripSeparator3});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(1500, 38);
            this.mnuToolBar.TabIndex = 0;
            this.mnuToolBar.Text = "toolStrip1";
            // 
            // mnuFileDrop
            // 
            this.mnuFileDrop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuFileDrop.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileLoadRomBtn,
            this.mnuFileLoadSourceBtn});
            this.mnuFileDrop.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileDrop.Image")));
            this.mnuFileDrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFileDrop.Name = "mnuFileDrop";
            this.mnuFileDrop.Size = new System.Drawing.Size(38, 35);
            this.mnuFileDrop.Text = "File";
            // 
            // mnuFileLoadRomBtn
            // 
            this.mnuFileLoadRomBtn.Name = "mnuFileLoadRomBtn";
            this.mnuFileLoadRomBtn.Size = new System.Drawing.Size(160, 22);
            this.mnuFileLoadRomBtn.Text = "Load ROM";
            this.mnuFileLoadRomBtn.Click += new System.EventHandler(this.loadRomBtn_Click);
            // 
            // mnuFileLoadSourceBtn
            // 
            this.mnuFileLoadSourceBtn.Name = "mnuFileLoadSourceBtn";
            this.mnuFileLoadSourceBtn.Size = new System.Drawing.Size(160, 22);
            this.mnuFileLoadSourceBtn.Text = "Load Source File";
            this.mnuFileLoadSourceBtn.Click += new System.EventHandler(this.mnuFileLoadSourceBtn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // mnuPlayBtn
            // 
            this.mnuPlayBtn.Image = global::Player.Properties.Resources.play;
            this.mnuPlayBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuPlayBtn.Name = "mnuPlayBtn";
            this.mnuPlayBtn.Size = new System.Drawing.Size(53, 35);
            this.mnuPlayBtn.Text = "Play";
            this.mnuPlayBtn.Click += new System.EventHandler(this.mnuPlayBtn_Click);
            // 
            // mnuDbgBtn
            // 
            this.mnuDbgBtn.Image = global::Player.Properties.Resources.debug;
            this.mnuDbgBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuDbgBtn.Name = "mnuDbgBtn";
            this.mnuDbgBtn.Size = new System.Drawing.Size(66, 35);
            this.mnuDbgBtn.Text = "Debug";
            this.mnuDbgBtn.Click += new System.EventHandler(this.mnuDbgBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
            // 
            // mnuStopBtn
            // 
            this.mnuStopBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuStopBtn.Enabled = false;
            this.mnuStopBtn.Image = global::Player.Properties.Resources.stop;
            this.mnuStopBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuStopBtn.Name = "mnuStopBtn";
            this.mnuStopBtn.Size = new System.Drawing.Size(24, 35);
            this.mnuStopBtn.Text = "Stop";
            this.mnuStopBtn.Click += new System.EventHandler(this.mnuStopBtn_Click);
            // 
            // mnuPauseBtn
            // 
            this.mnuPauseBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuPauseBtn.Enabled = false;
            this.mnuPauseBtn.Image = global::Player.Properties.Resources.pause;
            this.mnuPauseBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuPauseBtn.Name = "mnuPauseBtn";
            this.mnuPauseBtn.Size = new System.Drawing.Size(24, 35);
            this.mnuPauseBtn.Text = "Pause";
            this.mnuPauseBtn.Click += new System.EventHandler(this.mnuPauseBtn_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // canvasPnl
            // 
            this.canvasPnl.AutoSize = true;
            this.canvasPnl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.canvasPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasPnl.Location = new System.Drawing.Point(0, 38);
            this.canvasPnl.Name = "canvasPnl";
            this.canvasPnl.Size = new System.Drawing.Size(1500, 900);
            this.canvasPnl.TabIndex = 1;
            // 
            // KPC8Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowText;
            this.ClientSize = new System.Drawing.Size(1500, 938);
            this.Controls.Add(this.canvasPnl);
            this.Controls.Add(this.mnuToolBar);
            this.Name = "KPC8Player";
            this.Text = "KPC8 Player";
            this.Load += new System.EventHandler(this.KPC8Player_Load);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip mnuToolBar;
        private ToolStripDropDownButton mnuFileDrop;
        private ToolStripMenuItem mnuFileLoadRomBtn;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton mnuPlayBtn;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton mnuStopBtn;
        private ToolStripButton mnuPauseBtn;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton mnuDbgBtn;
        private ToolStripMenuItem mnuFileLoadSourceBtn;
        private Panel canvasPnl;
    }
}