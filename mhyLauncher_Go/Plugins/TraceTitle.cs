using System;
using System.Drawing;
using System.Windows.Forms;
using static MHYLAUNCHER_GO.Functions.Win32;

namespace MHYLAUNCHER_GO.Plugins
{
    /// <summary>
    /// 控制台提示信息窗口基础类
    /// </summary>
    public partial class TraceTitle : Form
    {
        /// <summary>
        /// TraceTitle类默认交互逻辑
        /// </summary>
        /// <param name="console">控制台窗口句柄</param>
        /// <param name="width">控制台窗口客户区宽度</param>
        /// <param name="height">控制台窗口客户区高度</param>
        public TraceTitle(IntPtr console, int width, int height = 32)
        {
            InitializeComponent();
            this.Load += (sender, e) =>
            {
                SetParent(this.Handle, console);
                this.Location = Point.Empty;
                this.Width = width;
                if (height > 0) this.Height = height;
            };
        }

        /// <summary>
        /// 重写CreateParams属性
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)WS_EX_NOACTIVATE;
                return cp;
            }
        }
    }
}
