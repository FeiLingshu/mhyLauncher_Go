using System.Windows.Forms;

namespace mhyLauncher_Go.Watcher
{
    /// <summary>
    /// 后台窗口消息循环模拟基础类
    /// </summary>
    internal partial class WatcherForm : Form
    {
        /// <summary>
        /// WatcherForm类默认交互逻辑
        /// </summary>
        internal WatcherForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 指示窗口不能获取焦点的窗口扩展风格
        /// </summary>
        private const int WS_EX_NOACTIVATE = 0x08000000;

        /// <summary>
        /// 重写窗口CreateParams过程
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }
    }
}
