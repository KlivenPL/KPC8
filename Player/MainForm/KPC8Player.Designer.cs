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
            mnuToolBar = new ToolStrip();
            mnuFileDrop = new ToolStripDropDownButton();
            mnuFileLoadRomBtn = new ToolStripMenuItem();
            mnuFileLoadSourceBtn = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            mnuPlayBtn = new ToolStripButton();
            mnuDbgBtn = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            mnuStopBtn = new ToolStripButton();
            mnuPauseBtn = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            canvasPnl = new Panel();
            mnuToolBar.SuspendLayout();
            SuspendLayout();
            // 
            // mnuToolBar
            // 
            mnuToolBar.AutoSize = false;
            mnuToolBar.BackColor = SystemColors.ControlLightLight;
            mnuToolBar.BackgroundImageLayout = ImageLayout.None;
            mnuToolBar.ImageScalingSize = new Size(20, 20);
            mnuToolBar.Items.AddRange(new ToolStripItem[] { mnuFileDrop, toolStripSeparator1, mnuPlayBtn, mnuDbgBtn, toolStripSeparator2, mnuStopBtn, mnuPauseBtn, toolStripSeparator3 });
            mnuToolBar.Location = new Point(0, 0);
            mnuToolBar.Name = "mnuToolBar";
            mnuToolBar.Size = new Size(1714, 51);
            mnuToolBar.TabIndex = 0;
            mnuToolBar.Text = "toolStrip1";
            // 
            // mnuFileDrop
            // 
            mnuFileDrop.DisplayStyle = ToolStripItemDisplayStyle.Text;
            mnuFileDrop.DropDownItems.AddRange(new ToolStripItem[] { mnuFileLoadRomBtn, mnuFileLoadSourceBtn });
            mnuFileDrop.Image = (Image)resources.GetObject("mnuFileDrop.Image");
            mnuFileDrop.ImageTransparentColor = Color.Magenta;
            mnuFileDrop.Name = "mnuFileDrop";
            mnuFileDrop.Size = new Size(46, 48);
            mnuFileDrop.Text = "File";
            // 
            // mnuFileLoadRomBtn
            // 
            mnuFileLoadRomBtn.Name = "mnuFileLoadRomBtn";
            mnuFileLoadRomBtn.Size = new Size(201, 26);
            mnuFileLoadRomBtn.Text = "Load ROM";
            mnuFileLoadRomBtn.Click += loadRomBtn_Click;
            // 
            // mnuFileLoadSourceBtn
            // 
            mnuFileLoadSourceBtn.Name = "mnuFileLoadSourceBtn";
            mnuFileLoadSourceBtn.Size = new Size(201, 26);
            mnuFileLoadSourceBtn.Text = "Load Source File";
            mnuFileLoadSourceBtn.Click += mnuFileLoadSourceBtn_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 51);
            // 
            // mnuPlayBtn
            // 
            mnuPlayBtn.Image = Properties.Resources.play;
            mnuPlayBtn.ImageTransparentColor = Color.Magenta;
            mnuPlayBtn.Name = "mnuPlayBtn";
            mnuPlayBtn.Size = new Size(60, 48);
            mnuPlayBtn.Text = "Play";
            mnuPlayBtn.Click += mnuPlayBtn_Click;
            // 
            // mnuDbgBtn
            // 
            mnuDbgBtn.Image = Properties.Resources.debug;
            mnuDbgBtn.ImageTransparentColor = Color.Magenta;
            mnuDbgBtn.Name = "mnuDbgBtn";
            mnuDbgBtn.Size = new Size(78, 48);
            mnuDbgBtn.Text = "Debug";
            mnuDbgBtn.Click += mnuDbgBtn_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 51);
            // 
            // mnuStopBtn
            // 
            mnuStopBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            mnuStopBtn.Enabled = false;
            mnuStopBtn.Image = Properties.Resources.stop;
            mnuStopBtn.ImageTransparentColor = Color.Magenta;
            mnuStopBtn.Name = "mnuStopBtn";
            mnuStopBtn.Size = new Size(29, 48);
            mnuStopBtn.Text = "Stop";
            mnuStopBtn.Click += mnuStopBtn_Click;
            // 
            // mnuPauseBtn
            // 
            mnuPauseBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            mnuPauseBtn.Enabled = false;
            mnuPauseBtn.Image = Properties.Resources.pause;
            mnuPauseBtn.ImageTransparentColor = Color.Magenta;
            mnuPauseBtn.Name = "mnuPauseBtn";
            mnuPauseBtn.Size = new Size(29, 48);
            mnuPauseBtn.Text = "Pause";
            mnuPauseBtn.Click += mnuPauseBtn_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 51);
            // 
            // canvasPnl
            // 
            canvasPnl.AutoSize = true;
            canvasPnl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            canvasPnl.Dock = DockStyle.Fill;
            canvasPnl.Location = new Point(0, 51);
            canvasPnl.Margin = new Padding(3, 4, 3, 4);
            canvasPnl.Name = "canvasPnl";
            canvasPnl.Size = new Size(1714, 1200);
            canvasPnl.TabIndex = 1;
            // 
            // KPC8Player
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.WindowText;
            ClientSize = new Size(1714, 1251);
            Controls.Add(canvasPnl);
            Controls.Add(mnuToolBar);
            Margin = new Padding(3, 4, 3, 4);
            Name = "KPC8Player";
            Text = "KPC8 Player";
            Load += KPC8Player_Load;
            mnuToolBar.ResumeLayout(false);
            mnuToolBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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