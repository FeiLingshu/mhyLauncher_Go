using MHYLAUNCHER_GO.Functions.Font;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using static MHYLAUNCHER_GO.Functions.Win32;
using Screen = System.Windows.Forms.Screen;
using Size = System.Drawing.Size;

namespace MHYLAUNCHER_GO.Functions
{
    /// <summary>
    /// 增强型Trace跟踪器
    /// </summary>
    public class TraceListenerEx : ConsoleTraceListener
    {
        /// <summary>
        /// 初始化增强型Trace跟踪器
        /// </summary>
        /// <param name="consolehwnd">控制台窗口句柄</param>
        /// <exception cref="ArgumentException">传递参数不正确</exception>
        /// <exception cref="Win32Exception">发生win32异常</exception>
        public TraceListenerEx(IntPtr consolehwnd)
        {
            // 检查控制台窗口句柄
            if (consolehwnd == IntPtr.Zero)
            {
                throw new ArgumentException("未识别到正确的控制台窗口句柄。");
            }
            // 配置控制台窗口
            consolehwnd.SetDWM(
                false,
                true,
                Color.FromArgb(0xFF, 0x20, 0x20, 0x20).TO_COLORREF(),
                Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0).TO_COLORREF(),
                true);
            Console.Title = "MHYLAUNCHER_GO Trace 组件";
            IntPtr menuhwnd = GetSystemMenu(consolehwnd, false);
            if (menuhwnd != IntPtr.Zero)
            {
                RemoveMenu(menuhwnd, SC_SIZE, MF_BYCOMMAND);
                RemoveMenu(menuhwnd, SC_MAXIMIZE, MF_BYCOMMAND);
                RemoveMenu(menuhwnd, SC_CLOSE, MF_BYCOMMAND);
                RemoveMenu(menuhwnd, SC_DOSA, MF_BYCOMMAND);
                RemoveMenu(menuhwnd, SC_DOSB, MF_BYCOMMAND);
            }
            SetWindowLongPtr(consolehwnd, GWL_STYLE,
                GetWindowLongPtr(consolehwnd, GWL_STYLE) & ~WS_MAXIMIZEBOX);
            bool CCS = false;
            int _CCS = 0;
            if (/*!Debugger.IsAttached*/true)
            {
                try
                {
                    Console.Write(string.Empty); // 前置易引发异常的代码，预测后续代码是否能够成功执行，防止执行中意外中断
                    Console.CursorLeft = 0;
                    Console.CursorTop = 0;
                    Console.Clear();
                    IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
                    if (hConsole != IntPtr.Zero && hConsole != INVALID_HANDLE_VALUE)
                    {
                        // 配置控制台代码页
                        SetConsoleOutputCP(65001U);
                        BugFix.SetCodePage(65001U);
                        // 读取版本信息
                        Version sysver = Environment.OSVersion.Version;
                        // 根据版本决定是否进行额外操作（Windows Vista +）
                        if (sysver >= new Version(6, 0, 6000))
                        {
                            // 配置控制台字体
                            var (fontname, size) = GetFontName();
                            _CCS = size * 2;
                            CONSOLE_FONT_INFO_EX info = new CONSOLE_FONT_INFO_EX()
                            {
                                cbSize = Marshal.SizeOf(typeof(CONSOLE_FONT_INFO_EX)),
                                nFont = 0U,
                                dwFontSize = new COORD { X = 0, Y = size },
                                FontFamily = 0,
                                FontWeight = 400,
                                FaceName = fontname
                            };
                            _ = SetCurrentConsoleFontEx(hConsole, true, ref info);
                            _ = SetCurrentConsoleFontEx(hConsole, false, ref info);
                            // 调整控制台模式，启用扩展模式
                            if (GetConsoleMode(hConsole, out uint mode))
                            {
                                mode |= ENABLE_PROCESSED_OUTPUT;
                                mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                                SetConsoleMode(hConsole, mode);
                            }
                            // 修改控制台默认颜色（0=Black, 7=White）
                            CONSOLE_SCREEN_BUFFER_INFOEX csbi = new CONSOLE_SCREEN_BUFFER_INFOEX()
                            {
                                cbSize = (uint)Marshal.SizeOf(typeof(CONSOLE_SCREEN_BUFFER_INFOEX)),
                                ColorTable = new uint[16]
                            };
                            GetConsoleScreenBufferInfoEx(hConsole, ref csbi);
                            csbi.ColorTable[0] = 0x00202020; // Black
                            csbi.ColorTable[7] = 0x00808080; // Gray
                            csbi.ColorTable[8] = 0x00404040; // DarkGray
                            csbi.ColorTable[9] = 0x00EFAE00; // Blue
                            csbi.ColorTable[10] = 0x003FC68D; // Green
                            csbi.ColorTable[12] = 0x002265F2; // Red
                            csbi.ColorTable[14] = 0x000EC2FF; // Yellow
                            csbi.ColorTable[15] = 0x00F0F0F0; // White
                            SetConsoleScreenBufferInfoEx(hConsole, ref csbi);
                            CCS = true;
                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        // 统计屏幕空间数据
                        var screens = Screen.AllScreens;
                        List<int> widths = new List<int>(screens.Length);
                        List<int> heights = new List<int>(screens.Length);
                        List<uint> dpis = new List<uint>(screens.Length * 2);
                        IntPtr hdc = GetDC(IntPtr.Zero);
                        int dpiX = GetDeviceCaps(hdc, LOGPIXELSX);
                        int dpiY = GetDeviceCaps(hdc, LOGPIXELSY);
                        ReleaseDC(IntPtr.Zero, hdc);
                        int[] screenarea = new int[2];
                        bool[] screenstatus = new bool[2];
                        foreach (var screen in screens)
                        {
                            screenarea[0] = screen.Bounds.Width;
                            screenarea[1] = screen.Bounds.Height;
                            if (screenarea.Max() >= 800 && screenarea.Min() >= 600)
                            {
                                widths.Add(screen.WorkingArea.Width);
                                heights.Add(screen.WorkingArea.Height);
                            }
                            else
                            {
                                screenstatus[0] |= true;
                            }
                        }
                        if (widths.Count == 0 || heights.Count == 0)
                        {
                            throw new ArgumentException("无法识别到有效的屏幕空间。");
                        }
                        if (GetProcAddress(GetModuleHandle("shcore.dll"), "GetDpiForMonitor") == IntPtr.Zero)
                        {
                            if (dpiX >= 72)
                            {
                                dpis.Add((uint)dpiX);
                            }
                            else
                            {
                                screenstatus[1] |= true;
                            }
                            if (dpiY >= 72)
                            {
                                dpis.Add((uint)dpiY);
                            }
                            else
                            {
                                screenstatus[1] |= true;
                            }
                        }
                        else
                        {
                            bool MonitorEnum(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
                            {
                                int _result = GetDpiForMonitor(hMonitor, MDT_EFFECTIVE_DPI, out uint _dpiX, out uint _dpiY);
                                if (_result == 0)
                                {
                                    if (_dpiX >= 72)
                                    {
                                        dpis.Add(_dpiX);
                                    }
                                    else
                                    {
                                        screenstatus[1] |= true;
                                    }
                                    if (_dpiY >= 72)
                                    {
                                        dpis.Add(_dpiY);
                                    }
                                    else
                                    {
                                        screenstatus[1] |= true;
                                    }
                                }
                                return true;
                            }
                            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnum, IntPtr.Zero);
                        }
                        if (dpis.Count == 0)
                        {
                            throw new ArgumentException("无法识别到有效的屏幕空间。");
                        }
                        short widthlimit = (short)widths.Min();
                        short heightlimit = (short)heights.Min();
                        uint dpilimit = dpis.Max();
                        widths.Clear();
                        heights.Clear();
                        // 计算窗口框架
                        AutoResetEvent wait = new AutoResetEvent(false);
                        Size window_s = Size.Empty;
                        Size client_s = Size.Empty;
                        bool result = false;
                        int retry = 0;
                        do
                        {
                            result = GetWindowRect(consolehwnd, out RECT windowrect);
                            if (result) window_s = windowrect.ToRectangle().Size;
                            result = GetClientRect(consolehwnd, out RECT clientrect);
                            if (result) client_s = clientrect.ToRectangle().Size;
                            if (!window_s.IsEmpty && !client_s.IsEmpty)
                            {
                                break;
                            }
                            retry++;
                            if (retry == 60)
                            {
                                throw new Win32Exception("等待控制台窗口超时。");
                            }
                            wait.WaitOne(TimeSpan.FromMilliseconds(1000 / 60));
                        } while (true);
                        Size offset = window_s - client_s;
                        // 获取控制台字符单位
                        Size single = Size.Empty;
                        CONSOLE_FONT_INFO_EX _info = new CONSOLE_FONT_INFO_EX
                        {
                            cbSize = Marshal.SizeOf(typeof(CONSOLE_FONT_INFO_EX))
                        };
                        result = GetCurrentConsoleFontEx(GetStdHandle(STD_OUTPUT_HANDLE), false, ref _info);
                        if (!result)
                        {
                            throw new Win32Exception("无法获取控制台属性。");
                        }
                        single.Width = (int)Math.Ceiling(_info.dwFontSize.X / 96D * (double)dpilimit);
                        single.Height = (int)Math.Ceiling(_info.dwFontSize.Y / 96D * (double)dpilimit);
                        if (Debugger.IsAttached) // 测试用代码
                        {
                            Debug.Print($"{widthlimit},{heightlimit} | {offset} | {single} | {dpilimit}");
                        }
                        // 计算合理窗口大小
                        short cwidth = (short)Math.Floor((widthlimit - offset.Width) / (double)single.Width);
                        short cheight = (short)Math.Floor((heightlimit - offset.Height) / (double)single.Height);
                        if (cwidth > 100) cwidth = 100;
                        if (cheight > 60) cheight = 60;
                        // 配置控制台属性
                        COORD BSIZE = new COORD
                        {
                            X = short.MaxValue - 1,
                            Y = short.MaxValue - 1,
                        };
                        SMALL_RECT CRECT = new SMALL_RECT
                        {
                            Left = 0,
                            Top = 0,
                            Right = (short)(cwidth - 1),
                            Bottom = (short)(cheight - 1)
                        };
                        SetConsoleScreenBufferSize(hConsole, BSIZE);
                        BSIZE.X = cwidth;
                        if (!SetConsoleWindowInfo(hConsole, true, ref CRECT))
                        {
                            BSIZE.X = (short)Console.WindowWidth;
                        }
                        SetConsoleScreenBufferSize(hConsole, BSIZE);
                        // 适时弹出提示信息
                        if (screenstatus[0] || screenstatus[1])
                        {
                            string msg = "当前系统中部分显示器存在异常参数：" +
                                $"{(screenstatus[0] ? "\n- 分辨率" : string.Empty)}" +
                                $"{(screenstatus[1] ? "\n- DPI" : string.Empty)}" +
                                "\n可能导致Trace窗口在多显示器间移动时出现异常";
                            Task.Run(() =>
                            {
                                MessageBox.Show(
                                    msg,
                                    "Trace组件提示...",
                                    MessageBoxButton.OK, MessageBoxImage.Warning,
                                    MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                            });
                        }
                    }
                }
                catch (Exception exp)
                {
                    Console.WriteLine(
                        "ERROR: 发生意外错误" +
                        $"控制台校验失败，原因未知，调用Console相关函数时运行时抛出异常：{exp.Message}\n" +
                        "使用 Ctrl+C / Ctrl+Break(Pause) 退出");
                    MessageBox.Show(
                        "控制台校验失败，原因未知，调用Console相关函数时运行时抛出异常：" +
                        $"\n\n{exp.Message}\n{exp.GetType()}\n{exp.StackTrace}",
                        "控制台校验出现异常...",
                        MessageBoxButton.OK, MessageBoxImage.Error,
                        MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            // 绑定自释放逻辑
            int currentpid = 0;
            int consolepid = 0;
            using (Process CurrentProcess = Process.GetCurrentProcess())
            {
                currentpid = CurrentProcess.Id;
            }
            foreach (Process conhost in Process.GetProcessesByName("conhost"))
            {
                using (conhost)
                {
                    PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
                    int status = NtQueryInformationProcess(
                        conhost.Handle,
                        0,/*ProcessBasicInformation*/
                        ref pbi,
                        (uint)Marshal.SizeOf(pbi),
                        out _);
                    if (status < 0)
                    {
                        continue;
                    }
                    if (pbi.InheritedFromUniqueProcessId.ToInt32() == currentpid)
                    {
                        consolepid = conhost.Id;
                        break;
                    }
                }
            }
            if (consolepid == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    $"调用底层函数NtQueryInformationProcess时出现未知问题：未能获取到控制台后台服务进程。");
            }
            IntPtr hInput = GetStdHandle(STD_INPUT_HANDLE);
            if (hInput != IntPtr.Zero && hInput != INVALID_HANDLE_VALUE && GetConsoleMode(hInput, out uint originalMode))
            {
                originalMode |= ENABLE_PROCESSED_INPUT;
                originalMode &= ~ENABLE_QUICK_EDIT_MODE;
                SetConsoleMode(hInput, originalMode);
            }
            bool ConsoleEvent(int ctrlType)
            {
                switch (ctrlType)
                {
                    case CTRL_C_EVENT:
                        SetConsoleCtrlHandler(CCD, false);
                        Trace.Listeners.Remove(this);
                        BugFix.CloseConsole();
                        GC.KeepAlive(CCD);
                        return true;
                    case CTRL_BREAK_EVENT:
                        goto case CTRL_C_EVENT;
                    default:
                        break;
                }
                return false;
            }
            CCD = ConsoleEvent;
            SetConsoleCtrlHandler(CCD, true);
            // 调整控制台窗口配置
            long style = GetWindowLongPtr(consolehwnd, GWL_STYLE);
            if (style != 0L)
            {
                SetWindowLongPtr(consolehwnd, GWL_STYLE, style & ~WS_THICKFRAME);
                SetWindowPos(consolehwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
            }
            if (CCS)
            {
                Console.Write("\x1b[2E");
                Console.Out.Flush();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.CursorVisible = false;
            Console.Title += " | 按下 Ctrl+C / Ctrl+Break(Pause) 退出";
            // 配置Trace自动更新
            Trace.AutoFlush = true;
        }

        /// <summary>
        /// 获取特定字体是否在系统中可用
        /// </summary>
        /// <returns>返回经回退处理的字体名称及对应的字体大小</returns>
        private static (string fontname, short size) GetFontName()
        {
            if (FontInstall.Exists)
            {
                return ("GiteeGit", 19);
            }
            else
            {
                return ("Consolas", 16);
            }
        }


        /// <summary>
        /// 用于保存控制台事件处理程序实例的内部字段
        /// </summary>
        private readonly ConsoleCtrlDelegate CCD;

        /// <summary>
        /// 显式重写Write方法(直接调用基类中的方法)
        /// </summary>
        /// <param name="message">消息字符串</param>
        public override void Write(string message) => base.Write(message);

        /// <summary>
        /// 显式重写WriteLine方法(直接调用基类中的方法)
        /// </summary>
        /// <param name="message">消息字符串</param>
        public override void WriteLine(string message) => base.WriteLine(message);
    }
}
