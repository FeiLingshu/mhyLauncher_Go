using MHYLAUNCHER_GO.Functions.Font;
using MHYLAUNCHER_GO.Plugins;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using static MHYLAUNCHER_GO.Functions.Win32;

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
            SetWindowLongPtr(consolehwnd, GWL_STYLE,
                GetWindowLongPtr(consolehwnd, GWL_STYLE) & ~WS_MAXIMIZEBOX);
            IntPtr menuhwnd = GetSystemMenu(consolehwnd, false);
            if (menuhwnd != IntPtr.Zero)
            {
                RemoveMenu(menuhwnd, SC_SIZE, MF_BYCOMMAND);
                RemoveMenu(menuhwnd, SC_CLOSE, MF_BYCOMMAND);
                RemoveMenu(menuhwnd, SC_DOSA, MF_BYCOMMAND);
                RemoveMenu(menuhwnd, SC_DOSB, MF_BYCOMMAND);
            }
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
                    Console.CursorVisible = false; // 关闭光标显示
                    IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
                    if (hConsole != IntPtr.Zero && hConsole != INVALID_HANDLE_VALUE)
                    {
                        // 配置控制台代码页
                        SetConsoleOutputCP(65001U);
                        BugFix.SetCodePage(65001U);
                        // 通过win32API修改控制台(及缓冲区)大小
                        // 使用Console类内置方法依然会抛出异常(句柄无效)，具体原因未知
                        // 经测试：Console类中打印字符串的方法不受影响
                        (short console_w, short console_h) = (120, 27);
                        COORD BSIZE = new COORD
                        {
                            X = short.MaxValue - 1,
                            Y = short.MaxValue - 1,
                        };
                        SMALL_RECT CRECT = new SMALL_RECT
                        {
                            Left = 0,
                            Top = 0,
                            Right = (short)(console_w - 1),
                            Bottom = (short)(console_h - 1)
                        };
                        SetConsoleScreenBufferSize(hConsole, BSIZE);
                        SetConsoleWindowInfo(hConsole, true, ref CRECT);
                        BSIZE.X = console_w;
                        SetConsoleScreenBufferSize(hConsole, BSIZE);
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
            // 植入提示信息窗口
            long style = GetWindowLongPtr(GetConsoleWindow(), GWL_STYLE);
            long exstyle = GetWindowLongPtr(GetConsoleWindow(), GWL_EXSTYLE);
            if (GetClientRect(consolehwnd, out RECT lpRect) && style != 0L && exstyle != 0L)
            {
                SetWindowLongPtr(GetConsoleWindow(), GWL_STYLE, style | WS_CLIPCHILDREN & ~WS_CLIPSIBLINGS);
                //SetWindowLongPtr(GetConsoleWindow(), GWL_EXSTYLE, exstyle | WS_EX_COMPOSITED); // 会导致闪烁，不要使用
                if (CCS)
                {
                    TraceTitle tt = new TraceTitle(consolehwnd, lpRect.ToRectangle().Width, _CCS)
                    {
                        BackColor = Color.FromArgb(0xFF, 0x20, 0x20, 0x20),
                        Height = _CCS
                    };
                    tt.Show();
                    Console.Write("\x1b[2E");
                }
                else
                {
                    TraceTitle tt = new TraceTitle(consolehwnd, lpRect.ToRectangle().Width, 0)
                    {
                        Height = 32
                    };
                    using (RegistryKey consoleKey = Registry.CurrentUser.OpenSubKey("Console", false))
                    {
                        if (consoleKey == null)
                        {
                            tt.BackColor = Color.FromArgb(0xFF, 0x20, 0x20, 0x20);
                        }
                        else
                        {
                            // Windows11/Windows10/else
                            object colorValue = (consoleKey.GetValue("ColorTable00")
                                ?? consoleKey.GetValue("ColorTable0"))
                                ?? consoleKey.GetValue("BackgroundColor");
                            if (colorValue != null && colorValue is int rawColor)
                            {
                                tt.BackColor = rawColor.FROM_COLORREF();
                            }
                            else
                            {
                                tt.BackColor = Color.FromArgb(0xFF, 0x20, 0x20, 0x20);
                            }
                            // Height
                            object fontsizeValue = consoleKey.GetValue("FontSize");
                            if (fontsizeValue != null && fontsizeValue is int rawSize)
                            {
                                int height = (rawSize >> 16) & 0xFFFF;
                                if (height * 2 < 32 - 5)
                                {
                                    tt.label_break.Visible = false;
                                    tt.label.Height = height * 2;
                                }
                                tt.Height = height * 2;
                            }
                        }
                    }
                    tt.Show();
                    Console.Write(Environment.NewLine + Environment.NewLine);
                }
            }
            else
            {
                Console.Title += " | Ctrl+C / Ctrl+Break(Pause) 退出 | Copyright (c) 2026, FeiLingshu";
            }

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
