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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuPlayBtn = new System.Windows.Forms.ToolStripButton();
            this.mnuDbgBtn = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuDbgPlayBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDbgStepIntoBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuStopBtn = new System.Windows.Forms.ToolStripButton();
            this.mnuPauseBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuStepOverBtn = new System.Windows.Forms.ToolStripButton();
            this.mnuStepIntoBtn = new System.Windows.Forms.ToolStripButton();
            this.rightPnl = new System.Windows.Forms.Panel();
            this.regsPnl = new System.Windows.Forms.FlowLayoutPanel();
            this.reg1 = new Player.Controls.Register.RegisterCtrl();
            this.reg2 = new Player.Controls.Register.RegisterCtrl();
            this.reg3 = new Player.Controls.Register.RegisterCtrl();
            this.reg4 = new Player.Controls.Register.RegisterCtrl();
            this.reg5 = new Player.Controls.Register.RegisterCtrl();
            this.reg6 = new Player.Controls.Register.RegisterCtrl();
            this.reg7 = new Player.Controls.Register.RegisterCtrl();
            this.reg8 = new Player.Controls.Register.RegisterCtrl();
            this.reg9 = new Player.Controls.Register.RegisterCtrl();
            this.reg10 = new Player.Controls.Register.RegisterCtrl();
            this.reg11 = new Player.Controls.Register.RegisterCtrl();
            this.reg12 = new Player.Controls.Register.RegisterCtrl();
            this.reg13 = new Player.Controls.Register.RegisterCtrl();
            this.reg14 = new Player.Controls.Register.RegisterCtrl();
            this.reg15 = new Player.Controls.Register.RegisterCtrl();
            this.reg16 = new Player.Controls.Register.RegisterCtrl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.regsLbl = new System.Windows.Forms.Label();
            this.mnuToolBar.SuspendLayout();
            this.rightPnl.SuspendLayout();
            this.regsPnl.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.AutoSize = false;
            this.mnuToolBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.mnuToolBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileDrop,
            this.toolStripSeparator1,
            this.mnuPlayBtn,
            this.mnuDbgBtn,
            this.toolStripSeparator2,
            this.mnuStopBtn,
            this.mnuPauseBtn,
            this.toolStripSeparator3,
            this.mnuStepOverBtn,
            this.mnuStepIntoBtn});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(1584, 38);
            this.mnuToolBar.TabIndex = 0;
            this.mnuToolBar.Text = "toolStrip1";
            // 
            // mnuFileDrop
            // 
            this.mnuFileDrop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuFileDrop.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileLoadRomBtn});
            this.mnuFileDrop.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileDrop.Image")));
            this.mnuFileDrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFileDrop.Name = "mnuFileDrop";
            this.mnuFileDrop.Size = new System.Drawing.Size(38, 35);
            this.mnuFileDrop.Text = "File";
            // 
            // mnuFileLoadRomBtn
            // 
            this.mnuFileLoadRomBtn.Name = "mnuFileLoadRomBtn";
            this.mnuFileLoadRomBtn.Size = new System.Drawing.Size(130, 22);
            this.mnuFileLoadRomBtn.Text = "Load ROM";
            this.mnuFileLoadRomBtn.Click += new System.EventHandler(this.loadRomBtn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // mnuPlayBtn
            // 
            this.mnuPlayBtn.Image = ((System.Drawing.Image)(resources.GetObject("mnuPlayBtn.Image")));
            this.mnuPlayBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuPlayBtn.Name = "mnuPlayBtn";
            this.mnuPlayBtn.Size = new System.Drawing.Size(53, 35);
            this.mnuPlayBtn.Text = "Play";
            this.mnuPlayBtn.Click += new System.EventHandler(this.mnuPlayBtn_Click);
            // 
            // mnuDbgBtn
            // 
            this.mnuDbgBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDbgPlayBtn,
            this.mnuDbgStepIntoBtn});
            this.mnuDbgBtn.Image = ((System.Drawing.Image)(resources.GetObject("mnuDbgBtn.Image")));
            this.mnuDbgBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuDbgBtn.Name = "mnuDbgBtn";
            this.mnuDbgBtn.Size = new System.Drawing.Size(75, 35);
            this.mnuDbgBtn.Text = "Debug";
            // 
            // mnuDbgPlayBtn
            // 
            this.mnuDbgPlayBtn.Image = ((System.Drawing.Image)(resources.GetObject("mnuDbgPlayBtn.Image")));
            this.mnuDbgPlayBtn.Name = "mnuDbgPlayBtn";
            this.mnuDbgPlayBtn.Size = new System.Drawing.Size(121, 22);
            this.mnuDbgPlayBtn.Text = "Play";
            // 
            // mnuDbgStepIntoBtn
            // 
            this.mnuDbgStepIntoBtn.Image = ((System.Drawing.Image)(resources.GetObject("mnuDbgStepIntoBtn.Image")));
            this.mnuDbgStepIntoBtn.Name = "mnuDbgStepIntoBtn";
            this.mnuDbgStepIntoBtn.Size = new System.Drawing.Size(121, 22);
            this.mnuDbgStepIntoBtn.Text = "Step into";
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
            this.mnuStopBtn.Image = ((System.Drawing.Image)(resources.GetObject("mnuStopBtn.Image")));
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
            this.mnuPauseBtn.Image = ((System.Drawing.Image)(resources.GetObject("mnuPauseBtn.Image")));
            this.mnuPauseBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuPauseBtn.Name = "mnuPauseBtn";
            this.mnuPauseBtn.Size = new System.Drawing.Size(24, 35);
            this.mnuPauseBtn.Text = "Pause";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 38);
            // 
            // mnuStepOverBtn
            // 
            this.mnuStepOverBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuStepOverBtn.Enabled = false;
            this.mnuStepOverBtn.Image = ((System.Drawing.Image)(resources.GetObject("mnuStepOverBtn.Image")));
            this.mnuStepOverBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuStepOverBtn.Name = "mnuStepOverBtn";
            this.mnuStepOverBtn.Size = new System.Drawing.Size(24, 35);
            this.mnuStepOverBtn.Text = "Step over";
            // 
            // mnuStepIntoBtn
            // 
            this.mnuStepIntoBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuStepIntoBtn.Enabled = false;
            this.mnuStepIntoBtn.Image = ((System.Drawing.Image)(resources.GetObject("mnuStepIntoBtn.Image")));
            this.mnuStepIntoBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuStepIntoBtn.Name = "mnuStepIntoBtn";
            this.mnuStepIntoBtn.Size = new System.Drawing.Size(24, 35);
            this.mnuStepIntoBtn.Text = "Step into";
            // 
            // rightPnl
            // 
            this.rightPnl.BackColor = System.Drawing.SystemColors.Control;
            this.rightPnl.Controls.Add(this.regsPnl);
            this.rightPnl.Controls.Add(this.panel1);
            this.rightPnl.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPnl.Location = new System.Drawing.Point(1231, 38);
            this.rightPnl.Name = "rightPnl";
            this.rightPnl.Size = new System.Drawing.Size(353, 823);
            this.rightPnl.TabIndex = 1;
            // 
            // regsPnl
            // 
            this.regsPnl.Controls.Add(this.reg1);
            this.regsPnl.Controls.Add(this.reg2);
            this.regsPnl.Controls.Add(this.reg3);
            this.regsPnl.Controls.Add(this.reg4);
            this.regsPnl.Controls.Add(this.reg5);
            this.regsPnl.Controls.Add(this.reg6);
            this.regsPnl.Controls.Add(this.reg7);
            this.regsPnl.Controls.Add(this.reg8);
            this.regsPnl.Controls.Add(this.reg9);
            this.regsPnl.Controls.Add(this.reg10);
            this.regsPnl.Controls.Add(this.reg11);
            this.regsPnl.Controls.Add(this.reg12);
            this.regsPnl.Controls.Add(this.reg13);
            this.regsPnl.Controls.Add(this.reg14);
            this.regsPnl.Controls.Add(this.reg15);
            this.regsPnl.Controls.Add(this.reg16);
            this.regsPnl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.regsPnl.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.regsPnl.Location = new System.Drawing.Point(0, 26);
            this.regsPnl.Name = "regsPnl";
            this.regsPnl.Size = new System.Drawing.Size(353, 797);
            this.regsPnl.TabIndex = 1;
            // 
            // reg1
            // 
            this.reg1.Location = new System.Drawing.Point(3, 3);
            this.reg1.Name = "reg1";
            this.reg1.Size = new System.Drawing.Size(392, 29);
            this.reg1.TabIndex = 0;
            // 
            // reg2
            // 
            this.reg2.Location = new System.Drawing.Point(3, 38);
            this.reg2.Name = "reg2";
            this.reg2.Size = new System.Drawing.Size(392, 29);
            this.reg2.TabIndex = 1;
            // 
            // reg3
            // 
            this.reg3.Location = new System.Drawing.Point(3, 73);
            this.reg3.Name = "reg3";
            this.reg3.Size = new System.Drawing.Size(392, 29);
            this.reg3.TabIndex = 2;
            // 
            // reg4
            // 
            this.reg4.Location = new System.Drawing.Point(3, 108);
            this.reg4.Name = "reg4";
            this.reg4.Size = new System.Drawing.Size(392, 29);
            this.reg4.TabIndex = 3;
            // 
            // reg5
            // 
            this.reg5.Location = new System.Drawing.Point(3, 143);
            this.reg5.Name = "reg5";
            this.reg5.Size = new System.Drawing.Size(392, 29);
            this.reg5.TabIndex = 4;
            // 
            // reg6
            // 
            this.reg6.Location = new System.Drawing.Point(3, 178);
            this.reg6.Name = "reg6";
            this.reg6.Size = new System.Drawing.Size(392, 29);
            this.reg6.TabIndex = 5;
            // 
            // reg7
            // 
            this.reg7.Location = new System.Drawing.Point(3, 213);
            this.reg7.Name = "reg7";
            this.reg7.Size = new System.Drawing.Size(392, 29);
            this.reg7.TabIndex = 6;
            // 
            // reg8
            // 
            this.reg8.Location = new System.Drawing.Point(3, 248);
            this.reg8.Name = "reg8";
            this.reg8.Size = new System.Drawing.Size(392, 29);
            this.reg8.TabIndex = 7;
            // 
            // reg9
            // 
            this.reg9.Location = new System.Drawing.Point(3, 283);
            this.reg9.Name = "reg9";
            this.reg9.Size = new System.Drawing.Size(392, 29);
            this.reg9.TabIndex = 8;
            // 
            // reg10
            // 
            this.reg10.Location = new System.Drawing.Point(3, 318);
            this.reg10.Name = "reg10";
            this.reg10.Size = new System.Drawing.Size(392, 29);
            this.reg10.TabIndex = 9;
            // 
            // reg11
            // 
            this.reg11.Location = new System.Drawing.Point(3, 353);
            this.reg11.Name = "reg11";
            this.reg11.Size = new System.Drawing.Size(392, 29);
            this.reg11.TabIndex = 10;
            // 
            // reg12
            // 
            this.reg12.Location = new System.Drawing.Point(3, 388);
            this.reg12.Name = "reg12";
            this.reg12.Size = new System.Drawing.Size(392, 29);
            this.reg12.TabIndex = 11;
            // 
            // reg13
            // 
            this.reg13.Location = new System.Drawing.Point(3, 423);
            this.reg13.Name = "reg13";
            this.reg13.Size = new System.Drawing.Size(392, 29);
            this.reg13.TabIndex = 12;
            // 
            // reg14
            // 
            this.reg14.Location = new System.Drawing.Point(3, 458);
            this.reg14.Name = "reg14";
            this.reg14.Size = new System.Drawing.Size(392, 29);
            this.reg14.TabIndex = 13;
            // 
            // reg15
            // 
            this.reg15.Location = new System.Drawing.Point(3, 493);
            this.reg15.Name = "reg15";
            this.reg15.Size = new System.Drawing.Size(392, 29);
            this.reg15.TabIndex = 14;
            // 
            // reg16
            // 
            this.reg16.Location = new System.Drawing.Point(3, 528);
            this.reg16.Name = "reg16";
            this.reg16.Size = new System.Drawing.Size(392, 29);
            this.reg16.TabIndex = 15;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.regsLbl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(353, 26);
            this.panel1.TabIndex = 0;
            // 
            // regsLbl
            // 
            this.regsLbl.AutoSize = true;
            this.regsLbl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.regsLbl.Location = new System.Drawing.Point(3, 4);
            this.regsLbl.Name = "regsLbl";
            this.regsLbl.Size = new System.Drawing.Size(64, 19);
            this.regsLbl.TabIndex = 0;
            this.regsLbl.Text = "Registers";
            this.regsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // KPC8Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1584, 861);
            this.Controls.Add(this.rightPnl);
            this.Controls.Add(this.mnuToolBar);
            this.Name = "KPC8Player";
            this.Text = "KPC8 Player";
            this.Load += new System.EventHandler(this.KPC8Player_Load);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.rightPnl.ResumeLayout(false);
            this.regsPnl.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ToolStrip mnuToolBar;
        private ToolStripDropDownButton mnuFileDrop;
        private ToolStripMenuItem mnuFileLoadRomBtn;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton mnuPlayBtn;
        private ToolStripDropDownButton mnuDbgBtn;
        private ToolStripMenuItem mnuDbgStepIntoBtn;
        private ToolStripMenuItem mnuDbgPlayBtn;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton mnuStopBtn;
        private ToolStripButton mnuPauseBtn;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton mnuStepOverBtn;
        private ToolStripButton mnuStepIntoBtn;
        private Panel rightPnl;
        private FlowLayoutPanel regsPnl;
        private Controls.Register.RegisterCtrl reg1;
        private Controls.Register.RegisterCtrl reg2;
        private Controls.Register.RegisterCtrl reg3;
        private Controls.Register.RegisterCtrl reg4;
        private Controls.Register.RegisterCtrl reg5;
        private Controls.Register.RegisterCtrl reg6;
        private Controls.Register.RegisterCtrl reg7;
        private Controls.Register.RegisterCtrl reg8;
        private Controls.Register.RegisterCtrl reg9;
        private Controls.Register.RegisterCtrl reg10;
        private Controls.Register.RegisterCtrl reg11;
        private Controls.Register.RegisterCtrl reg12;
        private Controls.Register.RegisterCtrl reg13;
        private Controls.Register.RegisterCtrl reg14;
        private Controls.Register.RegisterCtrl reg15;
        private Controls.Register.RegisterCtrl reg16;
        private Panel panel1;
        private Label regsLbl;
    }
}