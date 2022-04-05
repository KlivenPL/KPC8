namespace Player.Controls {
    partial class RegisterCtrl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.regLbl = new System.Windows.Forms.Label();
            this.valueBinTxt = new System.Windows.Forms.TextBox();
            this.valueHexTxt = new System.Windows.Forms.TextBox();
            this.valueDecTxt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // regLbl
            // 
            this.regLbl.ForeColor = System.Drawing.Color.White;
            this.regLbl.Location = new System.Drawing.Point(3, 2);
            this.regLbl.Name = "regLbl";
            this.regLbl.Size = new System.Drawing.Size(57, 23);
            this.regLbl.TabIndex = 0;
            this.regLbl.Text = "register";
            this.regLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // valueBinTxt
            // 
            this.valueBinTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(52)))), ((int)(((byte)(64)))));
            this.valueBinTxt.ForeColor = System.Drawing.Color.White;
            this.valueBinTxt.Location = new System.Drawing.Point(66, 3);
            this.valueBinTxt.Name = "valueBinTxt";
            this.valueBinTxt.ReadOnly = true;
            this.valueBinTxt.Size = new System.Drawing.Size(110, 23);
            this.valueBinTxt.TabIndex = 1;
            this.valueBinTxt.Text = "11111010 11110101";
            // 
            // valueHexTxt
            // 
            this.valueHexTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(52)))), ((int)(((byte)(64)))));
            this.valueHexTxt.ForeColor = System.Drawing.Color.White;
            this.valueHexTxt.Location = new System.Drawing.Point(182, 3);
            this.valueHexTxt.Name = "valueHexTxt";
            this.valueHexTxt.ReadOnly = true;
            this.valueHexTxt.Size = new System.Drawing.Size(75, 23);
            this.valueHexTxt.TabIndex = 2;
            this.valueHexTxt.Text = "0x2137";
            // 
            // valueDecTxt
            // 
            this.valueDecTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(52)))), ((int)(((byte)(64)))));
            this.valueDecTxt.ForeColor = System.Drawing.Color.White;
            this.valueDecTxt.Location = new System.Drawing.Point(263, 3);
            this.valueDecTxt.Name = "valueDecTxt";
            this.valueDecTxt.ReadOnly = true;
            this.valueDecTxt.Size = new System.Drawing.Size(75, 23);
            this.valueDecTxt.TabIndex = 3;
            this.valueDecTxt.Text = "8503";
            // 
            // RegisterCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(54)))));
            this.Controls.Add(this.valueDecTxt);
            this.Controls.Add(this.valueHexTxt);
            this.Controls.Add(this.valueBinTxt);
            this.Controls.Add(this.regLbl);
            this.Name = "RegisterCtrl";
            this.Size = new System.Drawing.Size(347, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label regLbl;
        private TextBox valueBinTxt;
        private TextBox valueHexTxt;
        private TextBox valueDecTxt;
    }
}
