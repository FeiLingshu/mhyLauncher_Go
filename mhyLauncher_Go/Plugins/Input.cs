using MHYLAUNCHER_GO.Functions;
using MHYLAUNCHER_GO.Functions.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static MHYLAUNCHER_GO.Functions.IO.MsgBeep;
using static MHYLAUNCHER_GO.Functions.Win32;

namespace MHYLAUNCHER_GO.Plugins
{
    /// <summary>
    /// 信息输入窗口基础类
    /// </summary>
    public partial class Input : Form
    {
        /// <summary>
        /// Input类默认交互逻辑
        /// </summary>
        /// <param name="mainbase">应用程序域实例</param>
        /// <param name="HYPHWND">所有者窗口句柄</param>
        /// <param name="HYPPID">所有者进程ID</param>
        /// <param name="is_readonly">指示配置文件是否处于只读模式</param>
        public Input(App mainbase, IntPtr HYPHWND, int HYPPID, bool is_readonly)
        {
            InitializeComponent();
            HYP = HYPHWND;
            Point offset = Point.Empty;
            if (is_readonly)
            {
                label_tip.Text = "工作线程已启动，配置文件 .bin 处于只读模式";
                panel_bottom.ForeColor = Color.FromArgb(0xFF, 0x1E, 0x20, 0x25);
                panel_bottom.BackColor = Color.FromArgb(0xFF, 0xFE, 0x69, 0x52);
                comboBox_mode.SelectedIndexChanged += (sender, e) =>
                {
                    comboBox_mode.SelectedIndex = (int)BIN.BINDATA.MODE;
                };
                numericUpDown_width.ValueChanged += (sender, e) =>
                {
                    numericUpDown_width.Value = BIN.BINDATA.SET_1;
                };
                numericUpDown_height.ValueChanged += (sender, e) =>
                {
                    numericUpDown_height.Value = BIN.BINDATA.SET_2;
                };
                this.FormClosing += (sender, e) =>
                {
                    SetWindowLongPtr(this.Handle, GWL_HWNDPARENT, 0L);
                };
            }
            else
            {
                this.button.Click += Button_Click;
                this.listBox.KeyUp += ListBox_KeyUp;
                this.FormClosing += Input_FormClosing;
                this.Activated += (ss, ee) =>
                {
                    if (win32Dialog != IntPtr.Zero)
                    {
                        SetForegroundWindow(win32Dialog);
                    }
                };
            }
            this.panel_Trace.Width = (this.panel_Trace.Width + this.panel_Trace.Right - this.label_trace.Left - this.label_tip.Right) / 2;
            this.textBox_LICENSE.Layout += (sender, e) =>
            {
                this.textBox_LICENSE.Height = this.pictureBox_github.Top - this.textBox_LICENSE.Top - this.textBox_LICENSE.Margin.Bottom;
            };
            this.Load += (sender, e) => // 配置窗口绑定
            {
                this.SetDWM(false, true, this.label_background.BackColor.TO_COLORREF(), this.ForeColor.TO_COLORREF(), true);
                if (HYPHWND != IntPtr.Zero)
                {
                    bool isHidden = !IsWindowVisible(HYPHWND);
                    bool isMinimized = IsIconic(HYPHWND);
                    if (isHidden)
                    {
                        using (Process HYP = Process.Start($"{Environment.CurrentDirectory}\\launcher.exe"))
                        {
                            HYP.WaitForExit();
                        }
                        ;
                        AutoResetEvent timer = new AutoResetEvent(false);
                        do
                        {
                            timer.WaitOne(100, false);
                            isHidden = !IsWindowVisible(HYPHWND);
                        } while (isHidden);
                    }
                    else if (isMinimized)
                    {
                        ShowWindow(HYPHWND, SW_SHOW);
                    }
                    else
                    {
                        GetWindowThreadProcessId(HYPHWND, out uint pid);
                        if (pid != HYPPID)
                        {
                            AttachThreadInput(pid, (uint)HYPPID, true);
                            SetForegroundWindow(HYPHWND);
                            AttachThreadInput(pid, (uint)HYPPID, false);
                        }
                        else
                        {
                            SetForegroundWindow(HYPHWND);
                        }
                    }
                    SetWindowPos(HYPHWND, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
                    bool result = GetWindowRect(HYPHWND, out RECT rect);
                    if (result)
                    {
                        SetWindowLongPtr(this.Handle, GWL_HWNDPARENT, HYPHWND.ToInt64());
                        Rectangle rectangle = rect.ToRectangle();
                        Point location = rectangle.Location;
                        offset = new Point(
                            (rectangle.Width - this.Width) / 2,
                            (rectangle.Height - this.Height) / 2);
                        location.Offset(offset);
                        this.Location = location;
                    }
                    else
                    {
                        this.StartPosition = FormStartPosition.CenterScreen;
                    }
                }
            };
            this.Shown += (sender, e) => // 强制更新窗口
            {
                this.Refresh();
                this.Activate();
                CACHE.IS_READY = true;
            };
            this.button.MouseUp += Button_MouseUp;
            this.label_github.MouseEnter += Labels_MouseEnter;
            this.label_bilibili.MouseEnter += Labels_MouseEnter;
            this.label_trace.MouseEnter += Labels_MouseEnter;
            this.label_github.MouseLeave += Labels_MouseLeave;
            this.label_bilibili.MouseLeave += Labels_MouseLeave;
            this.label_trace.MouseLeave += Labels_MouseLeave;
            this.pictureBox1.MouseLeave += PictureBox1_MouseLeave;
            this.label_github.Click += Label_github_Click;
            this.label_bilibili.Click += Label_bilibili_Click;
            this.label_trace.Click += Label_trace_Click;
            this.label_github.MouseHover += ToolTip1_MouseHover;
            this.label_bilibili.MouseHover += ToolTip1_MouseHover;
            this.label_trace.MouseHover += ToolTip1_MouseHover;
            this.pictureBox1.MouseHover += ToolTip1_MouseHover;
            this.toolTip1.Draw += ToolTip1_Draw;
            this.toolTip1.Popup += ToolTip1_Popup;
            this.comboBox_mode.SelectedIndex = (int)BIN.BINDATA.MODE;
            this.numericUpDown_width.Value = BIN.BINDATA.SET_1;
            this.numericUpDown_height.Value = BIN.BINDATA.SET_2;
            if (BIN.PATHS != null && BIN.PATHS.Length != 0)
            {
                foreach (string item in BIN.PATHS)
                {
                    listBox.Items.Add(item);
                }
            }
            WM_POS_NOTIFY = mainbase.GUIDHASH;
            this.mainbase = mainbase;
        }

        /// <summary>
        /// 按钮点击后重置焦点
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            this.label_background.Focus();
        }

        /// <summary>
        /// 点击按钮后触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Button_Click(object sender, System.EventArgs e)
        {
            // 初始化win32组件
            int pid = 0;
            int tid = 0;
            using (Process CurrentProcess = Process.GetCurrentProcess())
            {
                pid = CurrentProcess.Id;
                tid = Thread.CurrentThread.ID();
            }
            TraceExtensions.Print(TraceExtensions.FormatMessage(
                DateTime.Now,
                "Win32数据",
                9,
                new string[2]
                {
                    $"进程ID: 0x{pid:X8}",
                    $"线程ID: 0x{tid:X8}"
                }), ConsoleColor.Yellow);
            IntPtr handle = this.Handle;
            WinEventDelegate EventDelegate = (
                hWinEventHook, eventType,
                hwnd, idObject, idChild,
                dwEventThread, dwmsEventTime) =>
            {
                if (idObject == OBJID_WINDOW && idChild == CHILDID_SELF)
                {
                    if (win32Dialog != IntPtr.Zero && hwnd == win32Dialog && GetWindowRect(win32Dialog, out RECT rect))
                    {
                        var win32bindresult = Win32Bind(win32Dialog, this.Handle, rect.ToRectangle());
                        TraceExtensions.Print(TraceExtensions.FormatMessage(
                            DateTime.Now,
                            "Win32数据",
                            9,
                            new string[6]
                            {
                                $"消息常量：EVENT_SYSTEM_MOVESIZEEND = 0x{EVENT_SYSTEM_MOVESIZEEND:X4}",
                                $"RECT结构：Left   = {win32bindresult.rect.Left,4}",
                                $"#          Top    = {win32bindresult.rect.Top,4}",
                                $"#          Right  = {win32bindresult.rect.Right,4}",
                                $"#          Bottom = {win32bindresult.rect.Bottom,4}",
                                $"屏幕空间溢出：{(win32bindresult.overflow ? "已处理" : "已跳过")}"
                            }), ConsoleColor.Yellow);
                    }
                }
            };
            IntPtr win32hook = SetWinEventHook(
                EVENT_SYSTEM_MOVESIZEEND,
                EVENT_SYSTEM_MOVESIZEEND,
                IntPtr.Zero,
                EventDelegate,
                (uint)pid,
                (uint)tid,
                WINEVENT_OUTOFCONTEXT);
            if (win32hook == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
            }
            // 调整窗口状态
            CACHE.IS_WAITING = true;
            CACHE.IS_READY = false;
            this.Opacity = 0;
            if (HYP != IntPtr.Zero) ShowWindow(HYP, SW_MINIMIZE);
            // 启动win32窗口
            DialogResult result = openFileDialog1.ShowDialog(this);
            // 卸载win32组件
            win32Dialog = IntPtr.Zero;
            win32preset = false;
            if (!UnhookWinEvent(win32hook))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
            }
            ;
            GC.KeepAlive(EventDelegate);
            // 恢复窗口状态
            if (HYP != IntPtr.Zero)
            {
                if (IsWindowVisible(HYP))
                {
                    ShowWindow(HYP, SW_SHOWNOACTIVATE);
                }
                else
                {
                    bool isHidden = true;
                    using (Process HYP = Process.Start($"{Environment.CurrentDirectory}\\launcher.exe"))
                    {
                        HYP.WaitForExit();
                    }
                    ;
                    AutoResetEvent timer = new AutoResetEvent(false);
                    do
                    {
                        timer.WaitOne(100, false);
                        isHidden = !IsWindowVisible(HYP);
                    } while (isHidden);
                }
            }
            if (IsIconic(this.Handle))
            {
                CACHE.IS_VISIBLE = true;
                CACHE.IS_ICONIC = false;
                CACHE.STATE = 0x0001;
                ShowWindow(this.Handle, SW_SHOW);
            }
            this.Opacity = 1;
            CACHE.IS_READY = true;
            // 读取win32返回信息
            if (result == DialogResult.OK)
            {
                if (listBox.Items.Contains(openFileDialog1.FileName))
                {
                    return;
                }
                listBox.Items.Add(openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// 捕获按键后触发
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void ListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && listBox.SelectedIndex != -1)
            {
                foreach (int index in listBox.SelectedIndices.Cast<int>().OrderByDescending(i => i))
                {
                    listBox.Items.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// 指示警告窗口实例是否存在
        /// </summary>
        public bool wform = false;

        /// <summary>
        /// 窗口关闭标志
        /// </summary>
        public bool signal = false;

        /// <summary>
        /// 窗口关闭时自动保存
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Input_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason) // 过滤非标准关闭动作
            {
                case CloseReason.None:
                    e.Cancel = true;
                    return;
                case CloseReason.WindowsShutDown:
                    goto default;
                case CloseReason.MdiFormClosing:
                    goto default;
                case CloseReason.UserClosing:
                    break;
                case CloseReason.TaskManagerClosing:
                    goto default;
                case CloseReason.FormOwnerClosing:
                    goto default;
                case CloseReason.ApplicationExitCall:
                    goto default;
                default:
                    return;
            }
            if (signal) // 匹配正确关闭行为
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            if (listBox.Items.Count == 0) // 自适应启动警告窗口
            {
                if (!wform)
                {
                    wform = true;
                    new Warning(this).Show(this);
                    Beep(UType.MB_OK);
                }
                e.Cancel = true;
                return;
            }
            SetWindowLongPtr(this.Handle, GWL_HWNDPARENT, 0L); // 关闭前操作
            List<string> stringList = listBox.Items
                .Cast<object>()
                .Select(item => item?.ToString() ?? string.Empty)
                .ToList();
            bool result = BIN.SetPaths(
                (BIN.MODE)comboBox_mode.SelectedIndex,
                ((int)numericUpDown_width.Value, (int)numericUpDown_height.Value),
                stringList);
            if (!result) // 匹配操作结果
            {
                this.DialogResult = DialogResult.Abort;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// 用于存储米哈游启动器窗口句柄的内部字段
        /// </summary>
        private readonly IntPtr HYP = IntPtr.Zero;

        /// <summary>
        /// 用于存储win32对话框句柄的内部字段
        /// </summary>
        private IntPtr win32Dialog = IntPtr.Zero;

        /// <summary>
        /// 用于存储win32窗口创建后首次配置状态的内部字段
        /// </summary>
        private bool win32preset = false;

        /// <summary>
        /// 指示窗口进入空闲状态的win32消息
        /// </summary>
        private const int WM_ENTERIDLE = 0x0121;
        /// <summary>
        /// 指示由于显示win32对话框，导致拥有者窗口进入空闲状态
        /// </summary>
        private const int MSGF_DIALOGBOX = 0;

        /// <summary>
        /// 指示窗口正在进行显示相关操作的win32消息
        /// </summary>
        private const int WM_SHOWWINDOW = 0x0018;
        /// <summary>
        /// 指示窗口正因所有者窗口最小化而进行响应
        /// </summary>
        private const int SW_PARENTCLOSING = 1;

        /// <summary>
        /// 指示窗口正在接收系统命令的win32消息
        /// </summary>
        private const int WM_SYSCOMMAND = 0x0112;
        /// <summary>
        /// 指示窗口正在最小化
        /// </summary>
        private const int SC_MINIMIZE = 0xF020;
        /// <summary>
        /// 指示窗口正在还原
        /// </summary>
        private const int SC_RESTORE = 0xF120;

        /// <summary>
        /// 指示窗口属性已被调整的win32消息
        /// </summary>
        private const int WM_WINDOWPOSCHANGED = 0x0047;

        /// <summary>
        /// 声明win32结构WINDOWPOS
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            /// <summary>
            /// 窗口句柄
            /// </summary>
            public IntPtr hWnd;
            /// <summary>
            /// 窗口Z序定位
            /// </summary>
            public IntPtr hWndInsertAfter;
            /// <summary>
            /// 窗口横坐标
            /// </summary>
            public int x;
            /// <summary>
            /// 窗口纵坐标
            /// </summary>
            public int y;
            /// <summary>
            /// 窗口宽度
            /// </summary>
            public int cx;
            /// <summary>
            /// 窗口高度
            /// </summary>
            public int cy;
            /// <summary>
            /// 状态标志
            /// </summary>
            public uint flags;
        }

        /// <summary>
        /// 窗口操作状态缓存
        /// <para>
        /// 参数 IS_VISIBLE ：指示启动器窗口是否可见<br/>
        /// 参数 IS_ICONIC ：指示启动器窗口是否最小化<br/>
        /// 参数 STATE ：指示后代窗口当前的显示状态（0x0000 = 最小化 / 0x0001 = 正常 / 0xFFFF = 隐藏）<br/>
        /// 参数 IS_WAITING ：指示是否正在等待win32Dialog<br/>
        /// 参数 IS_READY ：指示窗口是否就绪
        /// </para>
        /// </summary>
        private (bool IS_VISIBLE, bool IS_ICONIC, int STATE, bool IS_WAITING, bool IS_READY) CACHE =
            (true, false, 0x0001, false, false);

        /// <summary>
        /// 由于存储特殊窗口消息值的内部只读字段
        /// </summary>
        private readonly int WM_POS_NOTIFY = 0x0;

        /// <summary>
        /// 重写WndProc过程，捕获窗口消息循环
        /// </summary>
        /// <param name="m">窗口消息数据</param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ENTERIDLE: // 捕获win32对话框句柄
                    if ((int)m.WParam == MSGF_DIALOGBOX)
                    {
                        if (win32Dialog == IntPtr.Zero || win32Dialog != m.LParam)
                        {
                            win32Dialog = m.LParam;
                            CACHE.IS_WAITING = false;
                        }
                        else
                        {
                            if (win32preset) break;
                            if (GetWindowRect(win32Dialog, out RECT rect))
                            {
                                Rectangle rectangle = new Rectangle(this.Location, rect.ToRectangle().Size);
                                var win32bindresult = Win32Bind(win32Dialog, this.Handle, rectangle);
                                TraceExtensions.Print(TraceExtensions.FormatMessage(
                                    DateTime.Now,
                                    "Win32数据",
                                    9,
                                    new string[6]
                                    {
                                        $"消息常量：EVENT_SYSTEM_MOVESIZEEND = 0x{EVENT_SYSTEM_MOVESIZEEND:X4}",
                                        $"RECT结构：Left   = {win32bindresult.rect.Left,4}",
                                        $"#          Top    = {win32bindresult.rect.Top,4}",
                                        $"#          Right  = {win32bindresult.rect.Right,4}",
                                        $"#          Bottom = {win32bindresult.rect.Bottom,4}",
                                        $"屏幕空间溢出：{(win32bindresult.overflow ? "已处理" : "已跳过")}"
                                    }), ConsoleColor.Yellow);
                                win32preset = true;
                            }
                        }
                    }
                    break;
                case WM_SHOWWINDOW: // 侦测由米哈游启动器窗口最小化产生的win32消息（拦截该消息以避免应用程序退出）
                    if (m.WParam.ToInt32() == 0 && m.LParam.ToInt32() == SW_PARENTCLOSING)
                    {
                        if (!CACHE.IS_WAITING)
                        {
                            if (CACHE.IS_READY) // 通过WM_WINDOWPOSCHANGED消息可以正确相应，但窗口在最小化途中将不会在启动器上方显示，原因未知
                            {
                                bool isHidden = !IsWindowVisible(HYP);
                                bool isMinimized = IsIconic(HYP);
                                TraceExtensions.Print(TraceExtensions.FormatMessage(
                                    DateTime.Now,
                                    "临时数据（实验功能）",
                                    20,
                                    new string[8]
                                    {
                                        $"CACHE: {CACHE.IS_VISIBLE}",
                                        $"#       {CACHE.IS_ICONIC}",
                                        $"#       0x{CACHE.STATE:X4}",
                                        $"TO:    {!isHidden}",
                                        $"#       {isMinimized}",
                                        $"#       0x{0:X4}",
                                        "模式：强制",
                                        "代码尚处实验阶段，相应调试信息予以保留..."
                                    }), ConsoleColor.Yellow);
                                CACHE.IS_VISIBLE = !isHidden;
                                CACHE.IS_ICONIC = isMinimized;
                                CACHE.STATE = 0x0000;
                                if (!IsIconic(this.Handle)) ShowWindow(this.Handle, SW_MINIMIZE);
                            }
                            if (win32Dialog != IntPtr.Zero)
                            {
                                // 匹配使用win32Dialog时米哈游启动器窗口最小化行为
                                if (IsWindowVisible(HYP) && !IsIconic(HYP))
                                {
                                    ShowWindow(win32Dialog, SW_SHOW);
                                    SetForegroundWindow(win32Dialog);
                                }
                            }
                        }
                        m.Result = IntPtr.Zero;
                        return;
                    }
                    if (m.WParam.ToInt32() == 0 && m.LParam.ToInt32() == 0) // 通过代码隐藏窗口也会导致应用程序退出，原因未知，需拦截
                    {
                        if (!IsWindowVisible(HYP))
                        {
                            m.Result = IntPtr.Zero;
                            return;
                        }
                    }
                    break;
                case WM_SYSCOMMAND: // 侦测由窗口自身系统命令产生的win32消息
                    if (HYP != IntPtr.Zero)
                    {
                        int command = m.WParam.ToInt32() & 0xFFF0;
                        switch (command)
                        {
                            case SC_MINIMIZE:
                                TraceExtensions.Print(TraceExtensions.FormatMessage(
                                    DateTime.Now,
                                    "临时数据（实验功能）",
                                    20,
                                    new string[8]
                                    {
                                        $"CACHE: {CACHE.IS_VISIBLE}",
                                        $"#       {CACHE.IS_ICONIC}",
                                        $"#       0x{CACHE.STATE:X4}",
                                        $"TO:    {true}",
                                        $"#       {true}",
                                        $"#       0x{0:X4}",
                                        "模式：主动",
                                        "代码尚处实验阶段，相应调试信息予以保留..."
                                    }), ConsoleColor.Yellow);
                                CACHE.IS_VISIBLE = true;
                                CACHE.IS_ICONIC = true;
                                CACHE.STATE = 0x0000;
                                ShowWindow(HYP, SW_MINIMIZE);
                                break;
                            case SC_RESTORE:
                                TraceExtensions.Print(TraceExtensions.FormatMessage(
                                    DateTime.Now,
                                    "临时数据（实验功能）",
                                    20,
                                    new string[8]
                                    {
                                        $"CACHE: {CACHE.IS_VISIBLE}",
                                        $"#       {CACHE.IS_ICONIC}",
                                        $"#       0x{CACHE.STATE:X4}",
                                        $"TO:    {true}",
                                        $"#       {false}",
                                        $"#       0x{1:X4}",
                                        "模式：主动",
                                        "代码尚处实验阶段，相应调试信息予以保留..."
                                    }), ConsoleColor.Yellow);
                                CACHE.IS_VISIBLE = true;
                                CACHE.IS_ICONIC = false;
                                CACHE.STATE = 0x0001;
                                if (IsWindowVisible(HYP))
                                {
                                    ShowWindow(HYP, SW_SHOWNOACTIVATE);
                                }
                                else // 必须使用此方法，否则会导致米哈游启动器窗口的关闭按钮失效
                                {
                                    bool hidden;
                                    using (Process HYP = Process.Start($"{Environment.CurrentDirectory}\\launcher.exe"))
                                    {
                                        HYP.WaitForExit();
                                    }
                                    ;
                                    AutoResetEvent timer = new AutoResetEvent(false);
                                    do
                                    {
                                        timer.WaitOne(100, false);
                                        hidden = !IsWindowVisible(HYP);
                                    } while (hidden);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case WM_WINDOWPOSCHANGED: // 侦测由米哈游启动器窗口还原产生的win32消息（同时响应米哈游启动器窗口隐藏的动作）
                    if (CACHE.IS_READY)
                    {
                        bool isHidden = !IsWindowVisible(HYP);
                        bool isMinimized = IsIconic(HYP);
                        if (CACHE.IS_VISIBLE == !isHidden && CACHE.IS_ICONIC == isMinimized)
                        {
                            WINDOWPOS pos = Marshal.PtrToStructure<WINDOWPOS>(m.LParam);
                            //Debug.Print(
                            //    $"hWnd=0x{pos.hWnd.ToInt64():X8}" +
                            //    $"\nhWndInsertAfter=0x{pos.hWndInsertAfter.ToInt64():X8}" +
                            //    $"\nx={pos.x,4}" +
                            //    $"\ny={pos.y,4}" +
                            //    $"\ncx={pos.cx,4}" +
                            //    $"\ncy={pos.cy,4}" +
                            //    $"\nflags={pos.flags}");
                            if (CACHE.IS_VISIBLE && CACHE.IS_ICONIC
                                && IsIconic(this.Handle)
                                && (pos.x == -32000 || pos.x == this.Location.X)
                                && (pos.y == -32000 || pos.y == this.Location.Y)
                                && (pos.flags & (SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE)) == (SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE))
                            {
                                if (WM_POS_NOTIFY != 0x0)
                                {
                                    this.Activate(); // 必须手动激活窗口，否则窗口自行激活前不会执行相应动作
                                    // 由于此时启动器窗口接到还原指令但尚未还原
                                    // 在此处以同步方式还原配置窗口会造成还原失败
                                    // 故采取发送Windows消息的方式在消息队列中插入消息，并在下次消息泵执行时同步执行
                                    if (WM_POS_NOTIFY != 0x0) PostMessage(
                                        this.Handle,
                                        (uint)WM_POS_NOTIFY,
                                        (IntPtr)((!isHidden ? 0x0001 : 0x0000) << 16 | (isMinimized ? 0x0001 : 0x0000)),
                                        (IntPtr)0x0001);
                                }
                            }
                            else
                            {
                                int ns = 0x0001;
                                if (IsWindowVisible(this.Handle))
                                {
                                    if (IsIconic(this.Handle)) ns = 0x0000;
                                }
                                else
                                {
                                    ns = 0xFFFF;
                                }
                                if (ns != CACHE.STATE)
                                {
                                    switch (CACHE.STATE)
                                    {
                                        case 0x0000:
                                            ShowWindow(this.Handle, SW_MINIMIZE);
                                            break;
                                        case 0x0001:
                                            ShowWindow(this.Handle, SW_SHOW);
                                            break;
                                        case 0xFFFF:
                                            ShowWindow(this.Handle, SW_HIDE);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (isHidden)
                            {
                                if (CACHE.IS_VISIBLE != !isHidden)
                                {
                                    if (WM_POS_NOTIFY != 0x0) PostMessage(
                                        this.Handle,
                                        (uint)WM_POS_NOTIFY,
                                        (IntPtr)((!isHidden ? 0x0001 : 0x0000) << 16 | (isMinimized ? 0x0001 : 0x0000)),
                                        (IntPtr)0xFFFF);
                                }
                            }
                            else
                            {
                                if (isMinimized)
                                {
                                    if (WM_POS_NOTIFY != 0x0) PostMessage(
                                        this.Handle,
                                        (uint)WM_POS_NOTIFY,
                                        (IntPtr)((!isHidden ? 0x0001 : 0x0000) << 16 | (isMinimized ? 0x0001 : 0x0000)),
                                        (IntPtr)0x0000);
                                }
                                else
                                {
                                    if (WM_POS_NOTIFY != 0x0) PostMessage(
                                        this.Handle,
                                        (uint)WM_POS_NOTIFY,
                                        (IntPtr)((!isHidden ? 0x0001 : 0x0000) << 16 | (isMinimized ? 0x0001 : 0x0000)),
                                        (IntPtr)0x0001);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            if (WM_POS_NOTIFY != 0x0 && m.Msg == WM_POS_NOTIFY)
            {
                int p1 = (m.WParam.ToInt32() >> 16) & 0xFFFF;
                int p2 = m.WParam.ToInt32() & 0xFFFF;
                int p3 = m.LParam.ToInt32();
                TraceExtensions.Print(TraceExtensions.FormatMessage(
                    DateTime.Now,
                    "临时数据（实验功能）",
                    20,
                    new string[8]
                    {
                        $"CACHE: {CACHE.IS_VISIBLE}",
                        $"#       {CACHE.IS_ICONIC}",
                        $"#       0x{CACHE.STATE:X4}",
                        $"NOW:   {IsWindowVisible(HYP)}",
                        $"#       {IsIconic(HYP)}",
                        $"#       0x{p3:X4}",
                        "模式：被动",
                        "代码尚处实验阶段，相应调试信息予以保留..."
                    }), ConsoleColor.Yellow);
                CACHE.IS_VISIBLE = p1 != 0;
                CACHE.IS_ICONIC = p2 != 0;
                CACHE.STATE = p3;
                switch (p3)
                {
                    case 0x0000:
                        if (!IsIconic(this.Handle)) ShowWindow(this.Handle, SW_MINIMIZE);
                        break;
                    case 0x0001:
                        if (!IsWindowVisible(this.Handle) || IsIconic(this.Handle)) ShowWindow(this.Handle, SW_SHOW);
                        break;
                    case 0xFFFF:
                        if (IsWindowVisible(this.Handle)) ShowWindow(this.Handle, SW_HIDE);
                        break;
                    default:
                        break;
                }
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 自动处理win32窗口位置并控制屏幕空间溢出
        /// </summary>
        /// <param name="win32hwnd">win32窗口句柄</param>
        /// <param name="hwnd">所有者窗口句柄</param>
        /// <param name="win32rect">win32窗口原始数据</param>
        /// <returns>返回一个元组(overflow=是否处理屏幕空间溢出，rect=最终窗口位置)</returns>
        private (bool overflow, Rectangle rect) Win32Bind(IntPtr win32hwnd, IntPtr hwnd, Rectangle win32rect)
        {
            Point location = win32rect.Location;
            win32rect.Height += 4;
            Screen screen = Screen.FromRectangle(win32rect);
            bool is_overflow = false;
            if (!screen.WorkingArea.Contains(win32rect))
            {
                if (win32rect.Bottom > screen.WorkingArea.Bottom)
                {
                    location.Y = win32rect.Location.Y - (win32rect.Bottom - screen.WorkingArea.Bottom);
                }
                if (win32rect.Right > screen.WorkingArea.Right)
                {
                    location.X = win32rect.Location.X - (win32rect.Right - screen.WorkingArea.Right);
                }
                if (win32rect.Top < screen.WorkingArea.Top)
                {
                    location.Y = win32rect.Location.Y + (screen.WorkingArea.Top - win32rect.Top);
                }
                if (win32rect.Left < screen.WorkingArea.Left)
                {
                    location.X = win32rect.Location.X + (screen.WorkingArea.Left - win32rect.Left);
                }
                is_overflow = true;
            }
            SetWindowPos(
                win32hwnd,
                IntPtr.Zero,
                location.X,
                location.Y,
                0,
                0,
                SWP_NOSIZE | SWP_NOZORDER);
            SetWindowPos(
                hwnd,
                IntPtr.Zero,
                location.X,
                location.Y,
                0,
                0,
                SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
            win32rect.Height -= 4;
            return (is_overflow, win32rect);
        }

        /// <summary>
        /// 配置鼠标进入时超链接颜色
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Labels_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.FromArgb(0xFF, 0xFF, 0xDB, 0x29);
        }

        /// <summary>
        /// 配置鼠标离开时超链接颜色
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Labels_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.FromArgb(0xFF, 0x2D, 0xA8, 0xFF);
            toolTip1.Hide((Control)sender);
        }

        /// <summary>
        /// 配置鼠标离开图像控件的行为
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide((Control)sender);
        }

        /// <summary>
        /// GitHub伪超链接
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Label_github_Click(object sender, EventArgs e)
        {
            using (Process.Start(toolTip1.GetToolTip((Label)sender))) { }
        }

        /// <summary>
        /// bilibili伪超链接
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Label_bilibili_Click(object sender, EventArgs e)
        {
            using (Process.Start(toolTip1.GetToolTip((Label)sender))) { }
        }

        /// <summary>
        /// 实现在鼠标悬停时触发ToolTip显示
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void ToolTip1_MouseHover(object sender, EventArgs e)
        {
            if (sender is Control control && control.Tag is string std)
            {
                Point point = Point.Empty;
                Size size = TextRenderer.MeasureText(std, this.Font) + new Size(8, 8);
                point.X -= size.Width - control.Width;
                point.Y -= size.Height + 5;
                if (control is PictureBox)
                {
                    point.Y -= 6;
                }
                toolTip1.Show(std, control, point);
            }
        }

        /// <summary>
        /// 自绘ToolTip组件
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void ToolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(toolTip1.BackColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            e.DrawBorder();
            Rectangle bound_offset = e.Bounds;
            bound_offset.X += 4;
            bound_offset.Width -= 8;
            TextRenderer.DrawText(
                e.Graphics,
                e.ToolTipText,
                this.Font,
                bound_offset,
                toolTip1.ForeColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
        }

        /// <summary>
        /// 自适应自绘ToolTip大小
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void ToolTip1_Popup(object sender, PopupEventArgs e)
        {
            if (e.AssociatedControl != null && e.AssociatedControl.Tag is string std)
            {
                e.ToolTipSize = TextRenderer.MeasureText(std, this.Font) + new Size(8, 8);
            }
        }

        /// <summary>
        /// 储存应用程序域实例
        /// </summary>
        private readonly App mainbase = null;

        /// <summary>
        /// 启动Trace窗口
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        /// <exception cref="Win32Exception">发生win32异常</exception>
        private void Label_trace_Click(object sender, EventArgs e)
        {
            TraceExtensions.Set(mainbase);
        }
    }
}
