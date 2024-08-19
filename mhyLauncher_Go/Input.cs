using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace mhyLauncher_Go
{
    /// <summary>
    /// 信息输入窗口基础类
    /// </summary>
    internal partial class Input : Form
    {
        /// <summary>
        /// Input类默认交互逻辑
        /// </summary>
        internal Input()
        {
            InitializeComponent();
            button1.Click += Button_Click;
            button2.Click += Button_Click;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            skip.Click += Skip_Click;
            if (string.IsNullOrEmpty(INI.YuanShenPath))
            {
                button1.BackColor = Color.Crimson;
                check1 = false;
            }
            else
            {
                button1.BackColor = Color.MediumSeaGreen;
                check1 = true;
            }
            if (string.IsNullOrEmpty(INI.StarRailPath))
            {
                button2.BackColor = Color.Crimson;
                check2 = false;
            }
            else
            {
                button2.BackColor = Color.MediumSeaGreen;
                check2 = true;
            }
        }

        /// <summary>
        /// 文件浏览组件启动时触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // 识别触发文件选择的来源
            skip.Tag = ((ContextMenuStrip)sender).SourceControl;
        }

        /// <summary>
        /// 点击右键菜单项后触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件数据</param>
        private void Skip_Click(object sender, EventArgs e)
        {
            if ((Button)skip.Tag == button1)
            {
                button1.BackColor = Color.DimGray;
                INI.Skip_YuanShen = true;
                INI.Write("Skip", "YuanShen", "true");
                INI.YuanShenPath = string.Empty;
                INI.Write("GamePaths", "YuanShen", string.Empty);
            }
            if ((Button)skip.Tag == button2)
            {
                button2.BackColor = Color.DimGray;
                INI.Skip_StarRail = true;
                INI.Write("Skip", "StarRail", "true");
                INI.StarRailPath = string.Empty;
                INI.Write("GamePaths", "StarRail", string.Empty);
            }
            if ((INI.Skip_YuanShen | check1) && (INI.Skip_StarRail | check2)) this.Close();
        }

        /// <summary>
        /// 指示数据#1是否正确填充
        /// </summary>
        private bool check1 = false;

        /// <summary>
        /// 指示数据#2是否正确填充
        /// </summary>
        private bool check2 = false;

        /// <summary>
        /// 点击按钮后触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件数据</param>
        private void Button_Click(object sender, System.EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                if (((Button)sender) == button1 && path.EndsWith("YuanShen.exe") && File.Exists(path))
                {
                    button1.BackColor = Color.MediumSeaGreen;
                    check1 = true;
                    INI.Skip_YuanShen = false;
                    INI.Write("Skip", "YuanShen", "false");
                    INI.YuanShenPath = path;
                    INI.Write("GamePaths", "YuanShen", path);
                }
                if (((Button)sender) == button2 && path.EndsWith("StarRail.exe") && File.Exists(path))
                {
                    button2.BackColor = Color.MediumSeaGreen;
                    check2 = true;
                    INI.Skip_StarRail = false;
                    INI.Write("Skip", "StarRail", "false");
                    INI.StarRailPath = path;
                    INI.Write("GamePaths", "StarRail", path);
                }
                if ((INI.Skip_YuanShen | check1) && (INI.Skip_StarRail | check2)) this.Close();
            }
        }
    }
}
