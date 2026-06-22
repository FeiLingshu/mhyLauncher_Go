namespace online.UI
{
    partial class Download
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Download));
            this.checkBox_source = new System.Windows.Forms.CheckBox();
            this.label_source = new System.Windows.Forms.Label();
            this.label_break = new System.Windows.Forms.Label();
            this.radioButton_dll = new System.Windows.Forms.RadioButton();
            this.radioButton_mp4 = new System.Windows.Forms.RadioButton();
            this.radioButton_otf = new System.Windows.Forms.RadioButton();
            this.button_downlaod = new System.Windows.Forms.Button();
            this.checkBox_self = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBox_source
            // 
            this.checkBox_source.AutoSize = true;
            this.checkBox_source.Checked = true;
            this.checkBox_source.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_source.Font = new System.Drawing.Font("微软雅黑", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_source.Location = new System.Drawing.Point(12, 12);
            this.checkBox_source.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.checkBox_source.Name = "checkBox_source";
            this.checkBox_source.Size = new System.Drawing.Size(156, 21);
            this.checkBox_source.TabIndex = 0;
            this.checkBox_source.TabStop = false;
            this.checkBox_source.Text = "使用国区镜像源 (gitee)";
            this.checkBox_source.UseVisualStyleBackColor = true;
            // 
            // label_source
            // 
            this.label_source.AutoSize = true;
            this.label_source.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.label_source.ForeColor = System.Drawing.Color.Gray;
            this.label_source.Location = new System.Drawing.Point(9, 33);
            this.label_source.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.label_source.Name = "label_source";
            this.label_source.Size = new System.Drawing.Size(186, 17);
            this.label_source.TabIndex = 0;
            this.label_source.Text = "否则将使用使用原始源 (GitHub)";
            // 
            // label_break
            // 
            this.label_break.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_break.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_break.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.label_break.Location = new System.Drawing.Point(9, 56);
            this.label_break.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.label_break.Name = "label_break";
            this.label_break.Size = new System.Drawing.Size(366, 2);
            this.label_break.TabIndex = 0;
            // 
            // radioButton_dll
            // 
            this.radioButton_dll.AutoCheck = false;
            this.radioButton_dll.AutoSize = true;
            this.radioButton_dll.Location = new System.Drawing.Point(12, 64);
            this.radioButton_dll.Name = "radioButton_dll";
            this.radioButton_dll.Size = new System.Drawing.Size(139, 21);
            this.radioButton_dll.TabIndex = 0;
            this.radioButton_dll.Text = "EfficiencyMode.dll";
            this.radioButton_dll.UseVisualStyleBackColor = true;
            // 
            // radioButton_mp4
            // 
            this.radioButton_mp4.AutoCheck = false;
            this.radioButton_mp4.AutoSize = true;
            this.radioButton_mp4.Location = new System.Drawing.Point(12, 91);
            this.radioButton_mp4.Name = "radioButton_mp4";
            this.radioButton_mp4.Size = new System.Drawing.Size(90, 21);
            this.radioButton_mp4.TabIndex = 0;
            this.radioButton_mp4.Text = "video.mp4";
            this.radioButton_mp4.UseVisualStyleBackColor = true;
            // 
            // radioButton_otf
            // 
            this.radioButton_otf.AutoCheck = false;
            this.radioButton_otf.AutoSize = true;
            this.radioButton_otf.Location = new System.Drawing.Point(12, 118);
            this.radioButton_otf.Name = "radioButton_otf";
            this.radioButton_otf.Size = new System.Drawing.Size(97, 21);
            this.radioButton_otf.TabIndex = 0;
            this.radioButton_otf.Text = "GiteeGit.otf";
            this.radioButton_otf.UseVisualStyleBackColor = true;
            // 
            // button_downlaod
            // 
            this.button_downlaod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_downlaod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(205)))), ((int)(((byte)(255)))));
            this.button_downlaod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.button_downlaod.Location = new System.Drawing.Point(282, 114);
            this.button_downlaod.Name = "button_downlaod";
            this.button_downlaod.Size = new System.Drawing.Size(90, 25);
            this.button_downlaod.TabIndex = 4;
            this.button_downlaod.Text = "开始下载";
            this.button_downlaod.UseVisualStyleBackColor = false;
            // 
            // checkBox_self
            // 
            this.checkBox_self.AutoSize = true;
            this.checkBox_self.Location = new System.Drawing.Point(273, 12);
            this.checkBox_self.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.checkBox_self.Name = "checkBox_self";
            this.checkBox_self.Size = new System.Drawing.Size(99, 21);
            this.checkBox_self.TabIndex = 5;
            this.checkBox_self.Text = "同时下载本体";
            this.checkBox_self.UseVisualStyleBackColor = true;
            // 
            // Download
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(384, 151);
            this.Controls.Add(this.checkBox_self);
            this.Controls.Add(this.button_downlaod);
            this.Controls.Add(this.radioButton_otf);
            this.Controls.Add(this.radioButton_mp4);
            this.Controls.Add(this.radioButton_dll);
            this.Controls.Add(this.label_break);
            this.Controls.Add(this.checkBox_source);
            this.Controls.Add(this.label_source);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.MaximizeBox = false;
            this.Name = "Download";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_source;
        private System.Windows.Forms.Label label_source;
        private System.Windows.Forms.Label label_break;
        private System.Windows.Forms.Button button_downlaod;
        public System.Windows.Forms.RadioButton radioButton_dll;
        public System.Windows.Forms.RadioButton radioButton_mp4;
        public System.Windows.Forms.RadioButton radioButton_otf;
        public System.Windows.Forms.CheckBox checkBox_self;
    }
}