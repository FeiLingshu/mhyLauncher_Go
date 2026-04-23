namespace MHYLAUNCHER_GO.Plugins
{
    partial class Notify
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Notify));
            this.label = new System.Windows.Forms.Label();
            this.copyright = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(13)))), ((int)(((byte)(15)))));
            this.label.Dock = System.Windows.Forms.DockStyle.Top;
            this.label.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(142)))), ((int)(((byte)(144)))));
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Margin = new System.Windows.Forms.Padding(0);
            this.label.Name = "label";
            this.label.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label.Size = new System.Drawing.Size(304, 76);
            this.label.TabIndex = 0;
            this.label.Tag = "　MHYLAUNCHER_GO 加载成功\r\n　按 F12 可启动 Trace 组件";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // copyright
            // 
            this.copyright.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.copyright.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(168)))), ((int)(((byte)(255)))));
            this.copyright.Location = new System.Drawing.Point(0, 76);
            this.copyright.Margin = new System.Windows.Forms.Padding(0);
            this.copyright.Name = "copyright";
            this.copyright.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.copyright.Size = new System.Drawing.Size(304, 25);
            this.copyright.TabIndex = 0;
            this.copyright.Text = "Copyright (c) 2026, FeiLingshu";
            this.copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Notify
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(37)))));
            this.ClientSize = new System.Drawing.Size(304, 101);
            this.Controls.Add(this.label);
            this.Controls.Add(this.copyright);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Notify";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label copyright;
    }
}