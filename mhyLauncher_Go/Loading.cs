using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static mhyLauncher_Go.Program;

namespace mhyLauncher_Go
{
    /// <summary>
    /// 挂载提示窗口基础类
    /// </summary>
    internal partial class Loading : Form
    {
        /// <summary>
        /// Loading类默认交互逻辑
        /// </summary>
        /// <param name="targate">[暂时弃用的参数传递]启动器逻辑层的窗口句柄</param>
        /// <param name="parent">启动器进程实例</param>
        internal Loading(IntPtr targate, Process parent)
        {
            InitializeComponent();
            #region 暂时弃用的参数传递
            #pragma warning disable CS0618 // 类型或成员已过时
            this.targate = targate;
            #pragma warning restore CS0618 // 类型或成员已过时
            #endregion
            this.parent = parent;
            this.Shown += Loading_Shown;
            this.Paint += Loading_Paint;
            this.MouseDown += Loading_MouseDown;
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

        /// <summary>
        /// 指示窗口通过鼠标操作激活的Windows消息
        /// </summary>
        private const int WM_MOUSEACTIVATE = 0x0021;

        /// <summary>
        /// 指示窗口不执行激活操作
        /// </summary>
        private const int MA_NOACTIVATE = 3;

        /// <summary>
        /// 重写窗口消息过程
        /// </summary>
        /// <param name="m">传递的Windows消息</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 重写窗口句柄创建过程，动态设置窗口裁剪范围
        /// </summary>
        /// <param name="e">窗口过程传递的默认参数</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int curveradius = 8;
            float offset = 0.5F;
            GraphicsPath path = new GraphicsPath();
            path.AddLines(new PointF[5] {
                new PointF(curveradius - 1, 0 + offset),
                new PointF(373, 0 + offset),
                new PointF(373, 28),
                new PointF(0 + offset, 28),
                new PointF(0 + offset, curveradius - 1) });
            path.AddArc(new RectangleF(0 + offset, 0 + offset, curveradius * 2 - 1, curveradius * 2 - 1), 180F, 90F);
            this.Region = new Region(path);
        }

        /// <summary>
        /// 用于获取桌面窗口句柄的WindowsAPI
        /// </summary>
        /// <returns>Windows桌面窗口的句柄</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// 用于将指定窗口设置为前台窗口的WindowsAPI
        /// </summary>
        /// <param name="hWnd">要设置为前台窗口的句柄</param>
        /// <returns>指示操作是否成功</returns>
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 鼠标按下时触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Loading_MouseDown(object sender, MouseEventArgs e)
        {
            SetForegroundWindow(parent.MainWindowHandle);
        }

        /// <summary>
        /// [暂时弃用的参数传递]启动器逻辑层的窗口句柄
        /// </summary>
        [Obsolete("当前参数未在代码中实际使用，但在可预见的未来有可能存在使用价值", false)]
        [SuppressMessage("Style", "IDE0052", Justification = "<挂起>")]
        private readonly IntPtr targate = IntPtr.Zero;

        /// <summary>
        /// 启动器进程实例
        /// </summary>
        private readonly Process parent = null;

        /// <summary>
        /// 用于将窗口的父级设置为指定窗口的WindowsAPI
        /// </summary>
        /// <param name="hWndChild">子窗口句柄</param>
        /// <param name="hWndNewParent">父窗口句柄</param>
        /// <returns>子窗口的原父窗口的句柄，若函数出现错误或子窗口原为顶级窗口则返回IntPtr.Zero</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// 窗口框架初始化完成后触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Loading_Shown(object sender, EventArgs e)
        {
            IntPtr childwindow = IntPtr.Zero;
            bool EnumWindows(IntPtr hWnd, IntPtr lParam)
            {
                const int nChars = 256;
                StringBuilder sb = new StringBuilder(nChars);
                if (GetClassName(hWnd, sb, nChars) > 0)
                {
                    string classname = sb.ToString();
                    Regex regex = new Regex(@"^Qt.*?QWindowIcon$");
                    if (regex.IsMatch(classname))
                    {
                        childwindow = hWnd;
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            EnumChildWindows(parent.MainWindowHandle, new EnumWindowsProc(EnumWindows), IntPtr.Zero);
            if (childwindow == IntPtr.Zero) throw new ArgumentNullException("无法获取启动器窗口相关数据。");
            bool checksize = GetClientRect(childwindow, out RECT clientrect);
            Size parentsize = clientrect.ToRectangle().Size;
            if (parentsize.Width == 0 && parentsize.Height == 0) throw new ArgumentNullException("无法获取启动器窗口相关数据。");
            int offset = 0;
            this.Location = new Point(parentsize.Width - 373, parentsize.Height - 28 + offset);
            this.Size = new Size(373, 28);
            SetParent(this.Handle, parent.MainWindowHandle);
            Thread waitforexit = new Thread(() =>
            {
                parent.WaitForExit();
                if (!this.IsHandleCreated) return;
                this.Invoke(new Action(() =>
                {
                    this.Close();
                }));
            })
            {
                IsBackground = true
            };
            waitforexit.Start();
            SetForegroundWindow(GetDesktopWindow());
            this.Opacity = 1D;
        }

        /// <summary>
        /// 遍历某窗口的子窗口
        /// </summary>
        /// <param name="hWndParent">要遍历的目标窗口</param>
        /// <param name="lpEnumFunc">委托回调</param>
        /// <param name="lParam">回调参数</param>
        /// <returns>返回执行是否成功</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// 遍历子窗口的委托回调
        /// </summary>
        /// <param name="hWnd">子窗口句柄</param>
        /// <param name="lParam">回调参数</param>
        /// <returns></returns>
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// 获取窗口类名
        /// </summary>
        /// <param name="hWnd">目标窗口的句柄</param>
        /// <param name="lpClassName">用于承载类名字符串的StringBuilder实例</param>
        /// <param name="nMaxCount">可承载的最大字符串长度</param>
        /// <returns>返回执行是否成功</returns>
        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// 窗口进行绘制时触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Loading_Paint(object sender, PaintEventArgs e)
        {
            int curveradius = 8;
            float offset = 0.5F;
            GraphicsPath _path = new GraphicsPath();
            _path.AddLine(new PointF(1 + offset, 28), new PointF(1 + offset, curveradius - 1 + offset));
            _path.AddArc(new RectangleF(1 + offset, 1 + offset, curveradius * 2 - 1, curveradius * 2 - 1), 180F, 90F);
            _path.AddLine(new PointF(curveradius - 1 + offset, 1 + offset), new PointF(373, 1 + offset));
            _path.AddLines(new Point[3] {
                new Point(373, 0),
                new Point(0, 0),
                new Point(0, 28) });
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(new SolidBrush(Color.FromArgb(0xFF, 0x40, 0x40, 0x40)), _path);
            // Debug.Print("Loading_Paint"); // 由于暂时未找到能响应启动器窗口最小化和还原的操作，暂时保留该行注释
        }
    }
}
