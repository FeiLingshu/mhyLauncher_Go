using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static MHYLAUNCHER_GO.Functions.Win32;

namespace MHYLAUNCHER_GO.Plugins
{
    /// <summary>
    /// 挂载提示窗口基础类
    /// </summary>
    public partial class Loading : Form
    {
        /// <summary>
        /// Loading类默认交互逻辑
        /// </summary>
        /// <param name="targate">[暂时弃用的参数传递]启动器逻辑层的窗口句柄</param>
        /// <param name="parent">启动器进程实例</param>
        public Loading(IntPtr targate, Process parent)
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
            this.Activated += Loading_Activated;
            this.MouseHover += Loading_MouseHover;
            this.MouseLeave += Loading_MouseLeave;
        }

        /// <summary>
        /// 重写窗口CreateParams过程
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
        /// 用于存储窗口裁剪区域的内部字段
        /// </summary>
        private GraphicsPath CLIP = null;

        /// <summary>
        /// 重写窗口句柄创建过程，动态设置窗口裁剪范围
        /// </summary>
        /// <param name="e">窗口过程传递的默认参数</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int curveradius = 8;
            float offset = 0.5F;
            CLIP = new GraphicsPath();
            CLIP.AddLines(new PointF[5] {
                new PointF(curveradius - 1, 0 + offset),
                new PointF(235, 0 + offset),
                new PointF(235, 27),
                new PointF(0 + offset, 27),
                new PointF(0 + offset, curveradius - 1) });
            CLIP.AddArc(new RectangleF(0 + offset, 0 + offset, curveradius * 2 - 1, curveradius * 2 - 1), 180F, 90F);
        }

        /// <summary>
        /// 鼠标按下时触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Loading_MouseDown(object sender, MouseEventArgs e)
        {
            using (Process CurrentProcess = Process.GetCurrentProcess())
            {
                AttachThreadInput((uint)CurrentProcess.Id, (uint)parent.Id, true);
                SetForegroundWindow(parent.MainWindowHandle);
                AttachThreadInput((uint)CurrentProcess.Id, (uint)parent.Id, false);
            }
        }

        /// <summary>
        /// 窗口获取焦点时触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        /// <exception cref="NotImplementedException"></exception>
        private void Loading_Activated(object sender, EventArgs e)
        {
            using (Process CurrentProcess = Process.GetCurrentProcess())
            {
                AttachThreadInput((uint)CurrentProcess.Id, (uint)parent.Id, true);
                SetForegroundWindow(parent.MainWindowHandle);
                AttachThreadInput((uint)CurrentProcess.Id, (uint)parent.Id, false);
            }
        }

        /// <summary>
        /// 用于储存提示信息组件实例的内部字段
        /// </summary>
        private Notify notify = null;

        /// <summary>
        /// 鼠标悬停时触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Loading_MouseHover(object sender, EventArgs e)
        {
            if (GetForegroundWindow() == parent.MainWindowHandle)
            {
                if (sender is Form form && form.Tag is string std)
                {
                    if (notify == null)
                    {
                        Point point = Point.Empty;
                        ClientToScreen(parent.MainWindowHandle, ref point);
                        GetClientRect(parent.MainWindowHandle, out RECT lpRect);
                        point += lpRect.ToRectangle().Size;
                        point -= this.Size;
                        int horizontal = GetSystemMetrics(SM_CXFRAME) + GetSystemMetrics(SM_CXEDGE) - GetSystemMetrics(SM_CXBORDER);
                        int vertical = GetSystemMetrics(SM_CYFRAME) + GetSystemMetrics(SM_CYEDGE) - GetSystemMetrics(SM_CYBORDER);
                        notify = new Notify(this.Width + horizontal * 2 - 5, 100)
                        {
                            Location = new Point(point.X - horizontal, point.Y - 100 + horizontal - 5)
                        };
                        notify.FormClosed += (e1, e2) =>
                        {
                            notify = null;
                        };
                        notify.Show(this);
                    }
                }
            }
        }

        /// <summary>
        /// 鼠标离开时触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Loading_MouseLeave(object sender, EventArgs e)
        {
            notify?.Close();
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
            GetClientRect(childwindow, out RECT clientrect);
            Size parentsize = clientrect.ToRectangle().Size;
            if (parentsize.Width == 0 && parentsize.Height == 0) throw new ArgumentNullException("无法获取启动器窗口相关数据。");
            long style = GetWindowLongPtr(parent.MainWindowHandle, GWL_STYLE);
            long exstyle = GetWindowLongPtr(parent.MainWindowHandle, GWL_EXSTYLE);
            if (style == 0L || exstyle == 0L)
            {
                throw new ArgumentNullException("无法对启动器窗口进行配置。");
            }
            else
            {
                SetWindowLongPtr(parent.MainWindowHandle, GWL_STYLE, style | WS_CLIPCHILDREN & ~WS_CLIPSIBLINGS);
                SetWindowLongPtr(parent.MainWindowHandle, GWL_EXSTYLE, exstyle | WS_EX_COMPOSITED);
            }
            SetParent(this.Handle, parent.MainWindowHandle);
            int offset = 0;
            this.Location = new Point(parentsize.Width - 235, parentsize.Height - 27 + offset);
            this.Size = new Size(235, 27);
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
            this.Opacity = 0.65;
            if (CLIP != null) this.Region = new Region(CLIP);
        }

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
            _path.AddLine(new PointF(1 + offset, 27), new PointF(1 + offset, curveradius - 1 + offset));
            _path.AddArc(new RectangleF(1 + offset, 1 + offset, curveradius * 2 - 1, curveradius * 2 - 1), 180F, 90F);
            _path.AddLine(new PointF(curveradius - 1 + offset, 1 + offset), new PointF(235, 1 + offset));
            _path.AddLines(new Point[3] {
                new Point(235, 0),
                new Point(0, 0),
                new Point(0, 27) });
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(this.ForeColor))
            {
                e.Graphics.FillPath(brush, _path);
            }
            // Debug.Print("Loading_Paint"); // 由于暂时未找到能响应启动器窗口最小化和还原的操作，暂时保留该行注释
        }
    }
}
