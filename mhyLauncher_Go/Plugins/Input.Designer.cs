namespace MHYLAUNCHER_GO.Plugins
{
    partial class Input
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Input));
            this.label = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.listBox = new System.Windows.Forms.ListBox();
            this.button = new System.Windows.Forms.Button();
            this.label_del = new System.Windows.Forms.Label();
            this.comboBox_mode = new System.Windows.Forms.ComboBox();
            this.label_mode = new System.Windows.Forms.Label();
            this.label_width = new System.Windows.Forms.Label();
            this.label_height = new System.Windows.Forms.Label();
            this.numericUpDown_width = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_height = new System.Windows.Forms.NumericUpDown();
            this.breakline = new System.Windows.Forms.Label();
            this.label_LOGO = new System.Windows.Forms.Label();
            this.label_ME = new System.Windows.Forms.Label();
            this.textBox_LICENSE = new System.Windows.Forms.TextBox();
            this.label_github = new System.Windows.Forms.Label();
            this.label_bilibili = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label_trace = new System.Windows.Forms.Label();
            this.panel_bottom = new System.Windows.Forms.Panel();
            this.panel_Trace = new System.Windows.Forms.Panel();
            this.label_tip = new System.Windows.Forms.Label();
            this.label_split = new System.Windows.Forms.Label();
            this.label_name = new System.Windows.Forms.Label();
            this.label_background = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox_bilibili = new System.Windows.Forms.PictureBox();
            this.pictureBox_github = new System.Windows.Forms.PictureBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_width)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_height)).BeginInit();
            this.panel_bottom.SuspendLayout();
            this.panel_Trace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_bilibili)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_github)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label.Location = new System.Drawing.Point(12, 12);
            this.label.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(460, 50);
            this.label.TabIndex = 0;
            this.label.Text = "程序支持添加任意游戏，添加前请先确认游戏能够以窗口化模式运行\r\n不建议添加过多程序，以防止过度的性能消耗";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.AddExtension = false;
            this.openFileDialog1.Filter = "可执行文件|*.exe";
            this.openFileDialog1.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            this.openFileDialog1.RestoreDirectory = true;
            this.openFileDialog1.Title = "选择相关游戏的.exe文件...";
            // 
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(36)))), ((int)(((byte)(41)))));
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(144)))), ((int)(((byte)(146)))));
            this.listBox.FormattingEnabled = true;
            this.listBox.HorizontalScrollbar = true;
            this.listBox.ItemHeight = 19;
            this.listBox.Location = new System.Drawing.Point(12, 95);
            this.listBox.Name = "listBox";
            this.listBox.ScrollAlwaysVisible = true;
            this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox.Size = new System.Drawing.Size(460, 116);
            this.listBox.Sorted = true;
            this.listBox.TabIndex = 0;
            this.listBox.TabStop = false;
            // 
            // button
            // 
            this.button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(168)))), ((int)(((byte)(255)))));
            this.button.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(37)))));
            this.button.Location = new System.Drawing.Point(12, 259);
            this.button.Name = "button";
            this.button.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.button.Size = new System.Drawing.Size(460, 30);
            this.button.TabIndex = 0;
            this.button.TabStop = false;
            this.button.Text = "添加游戏路径";
            this.button.UseVisualStyleBackColor = false;
            // 
            // label_del
            // 
            this.label_del.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_del.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(144)))), ((int)(((byte)(146)))));
            this.label_del.Location = new System.Drawing.Point(12, 64);
            this.label_del.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.label_del.Name = "label_del";
            this.label_del.Size = new System.Drawing.Size(460, 25);
            this.label_del.TabIndex = 0;
            this.label_del.Text = "—  选中项后按 Del 键可以删除项  —";
            this.label_del.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBox_mode
            // 
            this.comboBox_mode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(37)))));
            this.comboBox_mode.FormattingEnabled = true;
            this.comboBox_mode.Items.AddRange(new object[] {
            "PHONE",
            "PC",
            "CUSTOM"});
            this.comboBox_mode.Location = new System.Drawing.Point(73, 219);
            this.comboBox_mode.Name = "comboBox_mode";
            this.comboBox_mode.Size = new System.Drawing.Size(85, 25);
            this.comboBox_mode.TabIndex = 0;
            this.comboBox_mode.TabStop = false;
            this.comboBox_mode.Text = "PHONE";
            // 
            // label_mode
            // 
            this.label_mode.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_mode.Location = new System.Drawing.Point(12, 220);
            this.label_mode.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.label_mode.Name = "label_mode";
            this.label_mode.Size = new System.Drawing.Size(55, 25);
            this.label_mode.TabIndex = 0;
            this.label_mode.Text = "◪ 模式";
            this.label_mode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_width
            // 
            this.label_width.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.label_width.Location = new System.Drawing.Point(164, 220);
            this.label_width.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.label_width.Name = "label_width";
            this.label_width.Size = new System.Drawing.Size(85, 25);
            this.label_width.TabIndex = 0;
            this.label_width.Text = "◪ 窗口宽度";
            this.label_width.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_height
            // 
            this.label_height.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_height.Location = new System.Drawing.Point(321, 220);
            this.label_height.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.label_height.Name = "label_height";
            this.label_height.Size = new System.Drawing.Size(85, 25);
            this.label_height.TabIndex = 0;
            this.label_height.Text = "◪ 窗口高度";
            this.label_height.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDown_width
            // 
            this.numericUpDown_width.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(37)))));
            this.numericUpDown_width.Location = new System.Drawing.Point(255, 220);
            this.numericUpDown_width.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown_width.Minimum = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.numericUpDown_width.Name = "numericUpDown_width";
            this.numericUpDown_width.Size = new System.Drawing.Size(60, 23);
            this.numericUpDown_width.TabIndex = 0;
            this.numericUpDown_width.TabStop = false;
            this.numericUpDown_width.Value = new decimal(new int[] {
            640,
            0,
            0,
            0});
            // 
            // numericUpDown_height
            // 
            this.numericUpDown_height.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(37)))));
            this.numericUpDown_height.Location = new System.Drawing.Point(412, 220);
            this.numericUpDown_height.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown_height.Minimum = new decimal(new int[] {
            480,
            0,
            0,
            0});
            this.numericUpDown_height.Name = "numericUpDown_height";
            this.numericUpDown_height.Size = new System.Drawing.Size(60, 23);
            this.numericUpDown_height.TabIndex = 0;
            this.numericUpDown_height.TabStop = false;
            this.numericUpDown_height.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
            // 
            // breakline
            // 
            this.breakline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.breakline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(56)))), ((int)(((byte)(60)))));
            this.breakline.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.breakline.Location = new System.Drawing.Point(483, 11);
            this.breakline.Margin = new System.Windows.Forms.Padding(8, 2, 8, 8);
            this.breakline.Name = "breakline";
            this.breakline.Size = new System.Drawing.Size(2, 282);
            this.breakline.TabIndex = 0;
            // 
            // label_LOGO
            // 
            this.label_LOGO.AutoSize = true;
            this.label_LOGO.Font = new System.Drawing.Font("Microsoft YaHei UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_LOGO.Location = new System.Drawing.Point(566, 26);
            this.label_LOGO.Name = "label_LOGO";
            this.label_LOGO.Size = new System.Drawing.Size(226, 28);
            this.label_LOGO.TabIndex = 0;
            this.label_LOGO.Text = "MHYLAUNCHER_GO";
            this.label_LOGO.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_ME
            // 
            this.label_ME.AutoSize = true;
            this.label_ME.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_ME.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(144)))), ((int)(((byte)(146)))));
            this.label_ME.Location = new System.Drawing.Point(567, 54);
            this.label_ME.Name = "label_ME";
            this.label_ME.Size = new System.Drawing.Size(112, 22);
            this.label_ME.TabIndex = 2;
            this.label_ME.Text = "@FeiLingshu";
            this.label_ME.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_LICENSE
            // 
            this.textBox_LICENSE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(36)))), ((int)(((byte)(41)))));
            this.textBox_LICENSE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox_LICENSE.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(144)))), ((int)(((byte)(146)))));
            this.textBox_LICENSE.Location = new System.Drawing.Point(496, 87);
            this.textBox_LICENSE.Margin = new System.Windows.Forms.Padding(0, 8, 3, 8);
            this.textBox_LICENSE.Multiline = true;
            this.textBox_LICENSE.Name = "textBox_LICENSE";
            this.textBox_LICENSE.ReadOnly = true;
            this.textBox_LICENSE.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_LICENSE.Size = new System.Drawing.Size(296, 138);
            this.textBox_LICENSE.TabIndex = 0;
            this.textBox_LICENSE.TabStop = false;
            this.textBox_LICENSE.Text = resources.GetString("textBox_LICENSE.Text");
            // 
            // label_github
            // 
            this.label_github.AutoSize = true;
            this.label_github.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_github.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(168)))), ((int)(((byte)(255)))));
            this.label_github.Location = new System.Drawing.Point(528, 238);
            this.label_github.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.label_github.Name = "label_github";
            this.label_github.Size = new System.Drawing.Size(100, 17);
            this.label_github.TabIndex = 0;
            this.label_github.Tag = "https://github.com/FeiLingshu/MHYLAUNCHER_GO";
            this.label_github.Text = "GitHub项目仓库";
            this.label_github.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_bilibili
            // 
            this.label_bilibili.AutoSize = true;
            this.label_bilibili.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_bilibili.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(168)))), ((int)(((byte)(255)))));
            this.label_bilibili.Location = new System.Drawing.Point(528, 270);
            this.label_bilibili.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.label_bilibili.Name = "label_bilibili";
            this.label_bilibili.Size = new System.Drawing.Size(72, 17);
            this.label_bilibili.TabIndex = 0;
            this.label_bilibili.Tag = "https://space.bilibili.com/483822869";
            this.label_bilibili.Text = "bilibili视频";
            this.label_bilibili.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolTip1
            // 
            this.toolTip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.toolTip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.toolTip1.OwnerDraw = true;
            // 
            // label_trace
            // 
            this.label_trace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label_trace.AutoSize = true;
            this.label_trace.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_trace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(168)))), ((int)(((byte)(255)))));
            this.label_trace.Location = new System.Drawing.Point(129, 2);
            this.label_trace.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.label_trace.Name = "label_trace";
            this.label_trace.Size = new System.Drawing.Size(110, 17);
            this.label_trace.TabIndex = 0;
            this.label_trace.Tag = "启动控制台窗口对日志信息进行追踪";
            this.label_trace.Text = "启动 Trace 组件 ...";
            this.label_trace.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel_bottom
            // 
            this.panel_bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(48)))));
            this.panel_bottom.Controls.Add(this.panel_Trace);
            this.panel_bottom.Controls.Add(this.label_tip);
            this.panel_bottom.Controls.Add(this.label_split);
            this.panel_bottom.Controls.Add(this.label_name);
            this.panel_bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_bottom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(144)))), ((int)(((byte)(146)))));
            this.panel_bottom.Location = new System.Drawing.Point(0, 301);
            this.panel_bottom.Margin = new System.Windows.Forms.Padding(0);
            this.panel_bottom.Name = "panel_bottom";
            this.panel_bottom.Size = new System.Drawing.Size(804, 25);
            this.panel_bottom.TabIndex = 0;
            // 
            // panel_Trace
            // 
            this.panel_Trace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(48)))));
            this.panel_Trace.Controls.Add(this.label_trace);
            this.panel_Trace.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_Trace.Location = new System.Drawing.Point(552, 0);
            this.panel_Trace.Margin = new System.Windows.Forms.Padding(0);
            this.panel_Trace.Name = "panel_Trace";
            this.panel_Trace.Size = new System.Drawing.Size(252, 25);
            this.panel_Trace.TabIndex = 0;
            // 
            // label_tip
            // 
            this.label_tip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_tip.AutoSize = true;
            this.label_tip.Location = new System.Drawing.Point(147, 3);
            this.label_tip.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.label_tip.Name = "label_tip";
            this.label_tip.Size = new System.Drawing.Size(401, 17);
            this.label_tip.TabIndex = 0;
            this.label_tip.Text = "程序通过外部函数实现相关功能，未对游戏进程进行任何形式的注入操作...";
            this.label_tip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_split
            // 
            this.label_split.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_split.AutoSize = true;
            this.label_split.Location = new System.Drawing.Point(135, 3);
            this.label_split.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.label_split.Name = "label_split";
            this.label_split.Size = new System.Drawing.Size(12, 17);
            this.label_split.TabIndex = 0;
            this.label_split.Text = "I";
            this.label_split.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_name
            // 
            this.label_name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_name.AutoSize = true;
            this.label_name.Location = new System.Drawing.Point(4, 2);
            this.label_name.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(130, 17);
            this.label_name.TabIndex = 0;
            this.label_name.Text = "MHYLAUNCHER_GO";
            this.label_name.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_background
            // 
            this.label_background.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_background.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(37)))));
            this.label_background.Location = new System.Drawing.Point(0, 0);
            this.label_background.Name = "label_background";
            this.label_background.Size = new System.Drawing.Size(804, 326);
            this.label_background.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = global::MHYLAUNCHER_GO.Properties.Resources.watermask;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(634, 269);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(158, 20);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "米哈游启动器窗口右下角出现该图标时，指示程序已开始正常运行";
            // 
            // pictureBox_bilibili
            // 
            this.pictureBox_bilibili.ErrorImage = null;
            this.pictureBox_bilibili.Image = global::MHYLAUNCHER_GO.Properties.Resources.bilibili;
            this.pictureBox_bilibili.InitialImage = null;
            this.pictureBox_bilibili.Location = new System.Drawing.Point(496, 265);
            this.pictureBox_bilibili.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox_bilibili.Name = "pictureBox_bilibili";
            this.pictureBox_bilibili.Size = new System.Drawing.Size(24, 24);
            this.pictureBox_bilibili.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_bilibili.TabIndex = 4;
            this.pictureBox_bilibili.TabStop = false;
            // 
            // pictureBox_github
            // 
            this.pictureBox_github.ErrorImage = null;
            this.pictureBox_github.Image = global::MHYLAUNCHER_GO.Properties.Resources.github_mark_white;
            this.pictureBox_github.InitialImage = null;
            this.pictureBox_github.Location = new System.Drawing.Point(496, 233);
            this.pictureBox_github.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.pictureBox_github.Name = "pictureBox_github";
            this.pictureBox_github.Size = new System.Drawing.Size(24, 24);
            this.pictureBox_github.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_github.TabIndex = 3;
            this.pictureBox_github.TabStop = false;
            // 
            // pictureBox
            // 
            this.pictureBox.ErrorImage = null;
            this.pictureBox.Image = global::MHYLAUNCHER_GO.Properties.Resources.ICON;
            this.pictureBox.InitialImage = null;
            this.pictureBox.Location = new System.Drawing.Point(496, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(64, 64);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // Input
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(37)))));
            this.ClientSize = new System.Drawing.Size(804, 326);
            this.Controls.Add(this.label_LOGO);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label_bilibili);
            this.Controls.Add(this.pictureBox_bilibili);
            this.Controls.Add(this.label_github);
            this.Controls.Add(this.pictureBox_github);
            this.Controls.Add(this.textBox_LICENSE);
            this.Controls.Add(this.label_ME);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.breakline);
            this.Controls.Add(this.panel_bottom);
            this.Controls.Add(this.button);
            this.Controls.Add(this.numericUpDown_height);
            this.Controls.Add(this.numericUpDown_width);
            this.Controls.Add(this.label_height);
            this.Controls.Add(this.label_width);
            this.Controls.Add(this.comboBox_mode);
            this.Controls.Add(this.label_mode);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.label_del);
            this.Controls.Add(this.label);
            this.Controls.Add(this.label_background);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(229)))), ((int)(((byte)(229)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Input";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MHYLAUNCHER_GO V2";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_width)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_height)).EndInit();
            this.panel_bottom.ResumeLayout(false);
            this.panel_bottom.PerformLayout();
            this.panel_Trace.ResumeLayout(false);
            this.panel_Trace.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_bilibili)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_github)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button button;
        private System.Windows.Forms.Label label_del;
        private System.Windows.Forms.ComboBox comboBox_mode;
        private System.Windows.Forms.Label label_mode;
        private System.Windows.Forms.Label label_width;
        private System.Windows.Forms.Label label_height;
        private System.Windows.Forms.NumericUpDown numericUpDown_width;
        private System.Windows.Forms.NumericUpDown numericUpDown_height;
        private System.Windows.Forms.Label breakline;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label label_LOGO;
        private System.Windows.Forms.Label label_ME;
        private System.Windows.Forms.TextBox textBox_LICENSE;
        private System.Windows.Forms.PictureBox pictureBox_github;
        private System.Windows.Forms.Label label_github;
        private System.Windows.Forms.PictureBox pictureBox_bilibili;
        private System.Windows.Forms.Label label_bilibili;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel_bottom;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.Label label_tip;
        private System.Windows.Forms.Label label_split;
        private System.Windows.Forms.Label label_trace;
        private System.Windows.Forms.Label label_background;
        private System.Windows.Forms.Panel panel_Trace;
    }
}