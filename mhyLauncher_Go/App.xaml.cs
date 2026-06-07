using MHYLAUNCHER_GO.Functions;
using MHYLAUNCHER_GO.Functions.Config;
using MHYLAUNCHER_GO.Functions.Core;
using MHYLAUNCHER_GO.Functions.Dump;
using MHYLAUNCHER_GO.Functions.Font;
using MHYLAUNCHER_GO.Functions.IO;
using MHYLAUNCHER_GO.MainFrame;
using MHYLAUNCHER_GO.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static MHYLAUNCHER_GO.Functions.ProcessManager;
using static MHYLAUNCHER_GO.Functions.Win32;
using DialogResult = System.Windows.Forms.DialogResult;
using Padding = System.Windows.Forms.Padding;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Screen = System.Windows.Forms.Screen;
using Size = System.Drawing.Size;

namespace MHYLAUNCHER_GO
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// App.xaml 的默认启动逻辑
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Main(e.Args, this);
        }

        /// <summary>
        /// 用于存储欢迎窗口实例的内部字段
        /// </summary>
        private Hello hello = null;

        /// <summary>
        /// 全局GUID常数
        /// </summary>
        public readonly string GUID = "1B1FBA27-60A8-4F8B-B107-73EA7DA0CE56";

        /// <summary>
        /// 用于根据GUID的哈希值生成消息值（消息值范围：0x1000~0x7FFF / WM_USER可用范围：0x0400~0x7FFF）
        /// </summary>
        public short GUIDHASH
        {
            get
            {
                int hash = GUID.GetHashCode();
                byte[] bytes = new byte[8]
                {
                    (byte)((hash >> 28) & 0xF),
                    (byte)((hash >> 24) & 0xF),
                    (byte)((hash >> 20) & 0xF),
                    (byte)((hash >> 16) & 0xF),
                    (byte)((hash >> 12) & 0xF),
                    (byte)((hash >> 8) & 0xF),
                    (byte)(hash >> 4 & 0xF),
                    (byte)(hash & 0xF)
                };
                byte[] result = new byte[4]
                {
                    (byte)Math.Floor((bytes[0] + bytes[1]) / 2D),
                    (byte)Math.Floor((bytes[0] + bytes[1]) / 2D),
                    (byte)Math.Floor((bytes[0] + bytes[1]) / 2D),
                    (byte)Math.Floor((bytes[0] + bytes[1]) / 2D)
                };
                result[0] = (byte)Math.Round(result[0] / (double)0xF * (0x7 - 0x1) + 0x1, MidpointRounding.AwayFromZero);
                return (short)(result[0] << 12 | result[1] << 8 | result[2] << 4 | result[3]);
            }
        }

        /// <summary>
        /// 全局线程同步事件
        /// </summary>
        private EventWaitHandle ProgramStarted;

        /// <summary>
        /// 全局线程同步事件
        /// </summary>
        private EventWaitHandle SettingMode;

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        /// <param name="args">应用程序启动参数</param>
        /// <param name="source"></param>
        [STAThread]
        private void Main(string[] args, App source)
        {
            // 绑定异常处理程序
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            source.DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            System.Windows.Forms.Application.ThreadException += Application_ThreadException;
            // 验证DUMP模式
            if (args.Length == 1 && args[0].ToUpper() == "DUMP")
            {
                Thread t = new Thread(() =>
                {
                    Thread tt = new Thread(() =>
                    {
                        try
                        {
                            throw new EntryPointNotFoundException(
                                $"0x{Thread.CurrentThread.ID():X8}+0x{Thread.CurrentThread.ManagedThreadId:X}");
                        }
                        catch (Exception e)
                        {
                            Thread ttt = new Thread(() =>
                            {
                                Task _t = Task.Run(() =>
                                {
                                    throw e;
                                });
                                try
                                {
                                    _t.Wait();
                                }
                                catch (Exception _e)
                                {
                                    if (_e.GetType() == typeof(AggregateException)
                                        && _e.InnerException.GetType() == typeof(EntryPointNotFoundException)
                                        && Regex.IsMatch(_e.InnerException.Message, @"^0x[0-9A-Fa-f]{8}\+0x[0-9A-Fa-f]+$"))
                                    {
                                        throw new AggregateException(
                                            $"Thread {_e.InnerException.Message} - 触发Dump文件导出测试。",
                                            _e.InnerException);
                                    }
                                    else
                                    {
                                        throw new AggregateException(
                                            "未捕获到原始异常 - 触发Dump文件导出测试。",
                                            _e.InnerException);
                                    }
                                }
                                finally
                                {
                                    _t.Dispose();
                                }
                            })
                            { IsBackground = true };
                            ttt.Start();
                            ttt.Join();
                        }
                    })
                    { IsBackground = true };
                    tt.Start();
                    tt.Join();
                })
                { IsBackground = true };
                t.Start();
                t.Join();
            }
            // 尝试创建一个命名事件
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, GUID, out bool createnew);
            SettingMode = new EventWaitHandle(false, EventResetMode.AutoReset, $"{GUID}_BIN", out bool state);
            // 验证事件状态
            if (createnew != state)
            {
                throw new NotImplementedException("尝试初始化\"应用层单实例组件\"时出现逻辑验证错误。");
            }
            // 如果该命名事件已经存在(存在有前一个运行实例)，则发事件通知并退出
            if (createnew)
            {
                // 为命名事件添加回调委托
                ThreadPool.RegisterWaitForSingleObject(ProgramStarted, OnProgramStarted, null, -1, false);
                ThreadPool.RegisterWaitForSingleObject(SettingMode, OnSettingMode, null, -1, false);
            }
            else
            {
                using (Process CurrentProcess = Process.GetCurrentProcess())
                {
                    string FILE =
                        $"{Path.GetDirectoryName(CurrentProcess.MainModule.FileName)}\\" +
                        $"{Path.GetFileNameWithoutExtension(CurrentProcess.MainModule.FileName)}.bin";
                    if (args.Length == 1 && args[0] == FILE)
                    {
                        SettingMode.Set();
                        Environment.Exit(0);
                    }
                    else
                    {
                        ProgramStarted.Set();
                        Environment.Exit(0);
                    }
                }
            }
            // 读取运行环境
            string PE = string.Empty;
            using (Process CurrentProcess = Process.GetCurrentProcess())
            {
                PE = Path.GetDirectoryName(CurrentProcess.MainModule.FileName);
            }
            bool checkout = File.Exists($"{PE}\\launcher.exe")
                && PE.EndsWith("miHoYo Launcher");
            if (!checkout)
            {
                if (Debugger.IsAttached)
                {
                    PE = Environment.CurrentDirectory;
                }
                else
                {
                    throw new ArithmeticException(
                        "程序所在目录不正确。\n备注：请将程序置于米哈游启动器(miHoYo Launcher\\launcher.exe)相同目录下。");
                }
            }
            // 挂载性能配置DLL
            string PATH_DLL = $"{PE}\\EfficiencyMode.dll";
            string MD5DATA_DLL = "7B329DC875EBE75972B5AFE43468CACE";
            if (File.Exists(PATH_DLL))
            {
                try
                {
                    if (MD5.Get(PATH_DLL) != MD5DATA_DLL)
                    {
                        throw new FileFormatException("文件EfficiencyMode.dll的MD5校验未通过，文件未加载，指示程序文件可能遭到篡改。");
                    }
                    Load.Cache(PATH_DLL);
                    byte[] hash = Load.SHA512HASH(PATH_DLL);
                    string standard =
                        "091D1CEE7253BDF2E818409E54E5C8DE" +
                        "E83552956F8B59AFD95D1B8040DD73F8" +
                        "D63B21E2BE81912B48ED1544191B06C9" +
                        "95D547C365C6AF5A5883C111B1A91E8E";
                    if (string.Join(string.Empty, hash.Select(b => b.ToString("X2"))) != standard)
                    {
                        throw new FileFormatException("文件EfficiencyMode.dll的SHA512校验未通过，文件未加载，指示程序文件可能遭到篡改。");
                    }
                }
                catch (Exception excp)
                {
                    Load.Clear();
                    if (excp.GetType() == typeof(FileFormatException) &&
                        (excp.Message == "文件EfficiencyMode.dll的SHA512校验未通过，文件未加载，指示程序文件可能遭到篡改。"
                        || excp.Message == "文件EfficiencyMode.dll的MD5校验未通过，文件未加载，指示程序文件可能遭到篡改。"))
                    {
                        Task.Run(() =>
                        {
                            MessageBox.Show(
                                $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] 部分代码触发数据安全保护！\n- {excp.Message}",
                                "MHYLAUNCHER_GO V2",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning,
                                MessageBoxResult.OK,
                                MessageBoxOptions.DefaultDesktopOnly);
                        });
                    }
                    else
                    {
                        throw excp;
                    }
                }
            }
            // 载入资源文件
            string PATH = $"{PE}\\video.mp4";
            string MD5DATA = "599202058B4EA4AA2B0507DFDF3D2125";
            bool STATE = false;
            if (File.Exists(PATH))
            {
                try
                {
                    if (MD5.Get(PATH) == MD5DATA)
                    {
                        STATE = true;
                    }
                    else
                    {
                        throw new FileFormatException("文件video.mp4的MD5校验未通过，文件未加载，指示文件可能遭到篡改。");
                    }
                }
                catch (Exception excp)
                {
                    if (excp.GetType() == typeof(FileFormatException) &&
                        excp.Message == "文件video.mp4的MD5校验未通过，文件未加载，指示文件可能遭到篡改。")
                    {
                        Task.Run(() =>
                        {
                            MessageBox.Show(
                                $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] 部分代码触发数据安全保护！\n- {excp.Message}",
                                "MHYLAUNCHER_GO V2",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning,
                                MessageBoxResult.OK,
                                MessageBoxOptions.DefaultDesktopOnly);
                        });
                    }
                    else
                    {
                        throw excp;
                    }
                }
            }
            // 载入控制台数据缓存
            ConsoleCharLength.LoadResource();
            // 检测并尝试安装本地字体
            FontInstall.Exists = FontInstall.CheckFont(PE);
            // 初始化局部变量
            AutoResetEvent timer = new AutoResetEvent(false);
            bool binstate = false;
            Process mhyLauncher()
            {
                Process p = null;
                Process[] ppool = Process.GetProcessesByName("HYP");
                if (ppool != null)
                {
                    foreach (Process e in ppool)
                    {
                        using (e)
                        {
                            try
                            {
                                if (e.MainModule.FileName.Contains(PE))
                                {
                                    p = Process.GetProcessById(e.Id);
                                    break;
                                }
                                continue;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }
                return p;
            }
            Process process = null;
            // 启动欢迎窗口
            hello = new Hello(this, STATE ? PE : string.Empty, () =>
            {
                // 准备动画参数
                TimeSpan time = TimeSpan.Zero;
                TimeSpan wait = TimeSpan.FromMilliseconds(500);
                hello.StepSignal.WaitOne(wait, false);
                // 获取更新(异步)
                hello.Log("获取更新(异步)");
                time = TimeSpan.FromMilliseconds(500);
                hello.PreStep(0.2, time);
                hello.StepSignal.WaitOne(time.Add(wait), false);
                // 加载必要资源
                hello.Log("加载必要资源");
                time = TimeSpan.FromMilliseconds(1000);
                hello.PreStep(0.4, time);
                BugFix.Initialize();
                hello.StepSignal.WaitOne(time.Add(wait), false);
                // 识别运行环境
                hello.Log("识别运行环境");
                time = TimeSpan.FromMilliseconds(2000);
                hello.PreStep(0.6, time);
                Load.CoreType = Load.GetCoreMap();
                hello.StepSignal.WaitOne(time.Add(wait), false);
                // 读取用户配置文件
                hello.Log("挂载用户配置文件");
                time = TimeSpan.FromMilliseconds(1000);
                hello.PreStep(0.8, time);
                binstate = BIN.Check() && BIN.GetPaths();
                hello.StepSignal.WaitOne(time.Add(wait), false);
                // 识别启动器进程
                hello.Log("预读游戏启动器进程");
                time = TimeSpan.FromMilliseconds(500);
                hello.PreStep(1.0, time);
                process = mhyLauncher();
                hello.StepSignal.WaitOne(time.Add(wait), false);
                // 等待用户输入
                hello.Log("程序加载完毕");
            });
            bool? hello_report = hello.ShowDialog();
            if (!hello_report.HasValue || !hello_report.Value)
            {
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
            // 主进程启用性能配置
            using (Process p = Process.GetCurrentProcess())
            {
                if (Load.CheckVersion())
                {
                    _ = Load.Set(p);
                }
                if (Load.CoreType)
                {
                    _ = Load.SetProcess_Pro(p, ProcessPriorityClass.Idle, false);
                }
            }
            // 运行应用程序主功能方法
            using (Process HYP = Process.Start($"{PE}\\launcher.exe"))
            {
                HYP.WaitForExit();
            }
            if (process == null) // 配置米哈游启动器
            {
                do
                {
                    process = mhyLauncher();
                    if (process != null) break;
                    timer.WaitOne(100, false);
                } while (process == null);
            }
            int startcount = 0;
            do
            {
                timer.WaitOne(100, false);
                startcount++;
                if (process.MainWindowHandle != IntPtr.Zero && IsWindowVisible(process.MainWindowHandle))
                {
                    break;
                }
                startcount++;
                if (startcount >= 300)
                {
                    throw new InvalidOperationException("无法捕获米哈游启动器窗口。");
                }
            } while (true);
            if (Load.CoreType) // 额外配置：用于修正性能配置
            {
                _ = Load.SetProcess_Pro(process, ProcessPriorityClass.Normal, true);
            }
            HYPHWND = process.MainWindowHandle;
            HYPPID = process.Id;
            if (binstate == false || (args.Length == 1 && args[0] == BIN.FILE)) // 加载配置文件
            {
                DialogResult dr = new Input(this, HYPHWND, HYPPID, false).ShowDialog();
                if (dr == DialogResult.OK)
                {
                    BIN.GetPaths();
                }
                else
                {
                    if (dr == DialogResult.Abort)
                    {
                        throw new ArgumentNullException("尝试写入配置数据时出现错误，数据写入失败。");
                    }
                    else
                    {
                        Application.Current.Shutdown();
                        Environment.Exit(0);
                        return;
                    }
                }
            }
            if (process == null || process.HasExited) // 检查启动器状态(冗余)
            {
                Application.Current.Shutdown();
                Environment.Exit(0);
                return;
            }
            // 配置Trace组件状态标志
            Trace.UseGlobalLock = true; // 显式启用全局锁，避免线程安全问题
            TraceReady = true;
            // 加载ProcessManager组件
            pmcs = new Conditions(BIN.PATHS, BIN.BINDATA.MODE, (BIN.BINDATA.SET_1, BIN.BINDATA.SET_2), MainFunction);
            hds = new HookData[BIN.PATHS.Length];
            for (int i = 0; i < hds.Length; i++)
            {
                hds[i] = new HookData(null);
            }
            using (process)
            {
                pm = new ProcessManager(pmcs, hds, process, this);
                new Loading(IntPtr.Zero, process).ShowDialog();
                TraceReady = false; // 执行资源释放前调整标志量
            }
            pm.EndProcessManager(); // 释放相关资源
            // 由于WPF窗口会随游戏窗口启动和关闭，需配置OnExplicitShutdown以防止AppDomain被释放，故需手动终止应用程序
            Application.Current.Shutdown();
            Environment.Exit(0);
            return;
        }

        /// <summary>
        /// 用于保存ProcessManager组件实例的内部字段
        /// </summary>
        private ProcessManager pm = null;

        /// <summary>
        /// 用于保存ProcessManager组件运行参数的内部字段
        /// </summary>
        private Conditions pmcs = new Conditions();

        /// <summary>
        /// 用于保存Hook相关数据的内部字段
        /// </summary>
        private HookData[] hds = null;

        /// <summary>
        /// 应用程序主方法体
        /// </summary>
        /// <param name="game">游戏进程实例</param>
        /// <param name="mode">配置模式</param>
        /// <param name="sizeset">配置参数</param>
        /// <param name="index">索引</param>
        /// <param name="dwmstate">指示DWM操作是否执行</param>
        /// <exception cref="Win32Exception">发生win32错误</exception>
        /// <exception cref="ArithmeticException">发生参数错误</exception>
        /// <exception cref="NotSupportedException">发生不支持错误</exception>
        public void MainFunction(Process game, BIN.MODE mode, (int, int) sizeset, int index, bool dwmstate)
        {
            #region 应用程序主功能

            // 获取窗口win32信息
            bool report = true;
            report &= GetWindowRect(game.MainWindowHandle, out RECT window_rect);
            report &= GetClientRect(game.MainWindowHandle, out RECT client_rect);
            Point client_location = Point.Empty;
            report &= ClientToScreen(game.MainWindowHandle, ref client_location);
            if (!report)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(),
                        $"获取窗口属性过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                });
            }
            Padding client_pad = new Padding()
            {
                Left = client_location.X - window_rect.ToRectangle().Location.X,
                Top = client_location.Y - window_rect.ToRectangle().Location.Y,
                Right = window_rect.ToRectangle().Width
                    - client_rect.ToRectangle().Width
                    - (client_location.X - window_rect.ToRectangle().Location.X),
                Bottom = window_rect.ToRectangle().Height
                    - client_rect.ToRectangle().Height
                    - (client_location.Y - window_rect.ToRectangle().Location.Y)
            };

            // 判断命令行参数选择执行模式
            if (mode == BIN.MODE.PHONE || mode == BIN.MODE.PC || mode == BIN.MODE.CUSTOM)
            {
                (Rectangle, Size, Padding) param = GetParam(mode, sizeset, window_rect.ToRectangle(), client_pad); // 获取标准显示参数
                Rectangle rectangle = param.Item1;
                Size windowsize = param.Item2;
                bool movestate = true; // 修改窗口菜单选项
                void ChangeMenu(bool restore)
                {
                    if (!restore)
                    {
                        IntPtr menuhwnd = GetSystemMenu(game.MainWindowHandle, false);
                        if (menuhwnd != IntPtr.Zero)
                        {
                            RemoveMenu(menuhwnd, SC_MOVE, MF_BYCOMMAND);
                            ChangeTitle(" | MHYLAUNCHER_GO: 已加载 | F12: 位置锁定已激活");
                            TraceExtensions.Print(TraceExtensions.FormatMessage(
                                DateTime.Now,
                                "窗口管理器",
                                $"事件源：\r窗口位置锁定模块\n" +
                                $"状态：\r已激活"),
                                ConsoleColor.Green);
                            movestate = false;
                        }
                    }
                    else
                    {
                        IntPtr menuhwnd = GetSystemMenu(game.MainWindowHandle, true);
                        ChangeTitle(" | MHYLAUNCHER_GO: 已加载 | F12: 位置锁定未激活");
                        TraceExtensions.Print(TraceExtensions.FormatMessage(
                            DateTime.Now,
                            "窗口管理器",
                            $"事件源：\r窗口位置锁定模块\n" +
                            $"状态：\r未激活"),
                            ConsoleColor.Red);
                        movestate = true;
                    }
                }
                bool SizeMove(bool forceupdate) // 调整窗口属性
                {
                    if (!GetWindowRect(game.MainWindowHandle, out RECT checkout))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error(),
                                $"调整窗口属性过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                        });
                    };
                    Rectangle checkout_r = checkout.ToRectangle();
                    bool location = movestate || checkout_r.Location == rectangle.Location;
                    bool size = checkout_r.Size == rectangle.Size;
                    switch (location && size)
                    {
                        case true:
                            break;
                        case false:
                            bool final_in = SetWindowPos(game.MainWindowHandle, IntPtr.Zero,
                                rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                                SWP_DEFERERASE | SWP_NOCOPYBITS | SWP_NOZORDER | (location ? SWP_NOMOVE : 0) | (size ? SWP_NOSIZE : 0));
                            if (!final_in)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    throw new Win32Exception(Marshal.GetLastWin32Error(),
                                        $"调整窗口属性过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                                });
                            };
                            if (forceupdate || !size)
                            {
                                long style = GetWindowLongPtr(game.MainWindowHandle, GWL_STYLE);
                                SetWindowLongPtr(game.MainWindowHandle, GWL_STYLE, style & ~WS_CAPTION);
                                SetWindowLongPtr(game.MainWindowHandle, GWL_STYLE, style);
                                SendMessage(game.MainWindowHandle, WM_SIZE, IntPtr.Zero, (IntPtr)(windowsize.Height << 16 | windowsize.Width));
                            }
                            TraceExtensions.Print(TraceExtensions.FormatMessage(
                                DateTime.Now,
                                "窗口管理器",
                                $"事件源：\r窗口位置/大小变化事件\n" +
                                $"类型：\r位置{(size ? string.Empty : " + 大小")}\n" +
                                $"Bound：\rX：{checkout_r.Left,4} -> {rectangle.X,4}\n" +
                                $"\rY：{checkout_r.Top,4} -> {rectangle.Y,4}\n" +
                                $"\rWidth： {checkout_r.Width,4} {(size ? "(已忽略)" : $"-> {rectangle.Width,4}")}\n" +
                                $"\rHeight：{checkout_r.Height,4} {(size ? "(已忽略)" : $"-> {rectangle.Height,4}")}\n" +
                                $"强制更新窗口框架：\r{forceupdate}"),
                                ConsoleColor.Blue);
                            return true;
                    }
                    return false;
                }
                string originaltitle = string.Empty; // 修改程序标题
                string addupstd = string.Empty;
                object _initLock = new object();
                bool ChangeTitle(string str)
                {
                    lock (_initLock) // 方法体可能被多个线程调用，方法体内部同时涉及读写，需要锁定
                    {
                        StringBuilder title = new StringBuilder(GetWindowTextLength(game.MainWindowHandle) + 1);
                        GetWindowText(game.MainWindowHandle, title, title.Capacity);
                        if (string.IsNullOrEmpty(originaltitle))
                        {
                            string std = title.ToString();
                            string std_1 = " | MHYLAUNCHER_GO: 已加载 | F12: 位置锁定未激活";
                            string std_2 = " | MHYLAUNCHER_GO: 已加载 | F12: 位置锁定已激活";
                            if (std.Contains(std_1)
                                || std.Contains(std_2))
                            {
                                if (std.StartsWith(std_1)
                                    || std.StartsWith(std_2))
                                {
                                    std = "UNKNOWN";
                                    IntPtr phandle = OpenProcess(
                                        ProcessAccessFlags.PROCESS_QUERY_INFORMATION | ProcessAccessFlags.PROCESS_VM_READ,
                                        false, game.Id);
                                    if (phandle != IntPtr.Zero)
                                    {
                                        StringBuilder path = new StringBuilder(32768);
                                        uint scount = (uint)path.Capacity;
                                        QueryFullProcessImageName(phandle, 0, path, ref scount);
                                        string p = path.ToString().Trim();
                                        path.Clear();
                                        if (!string.IsNullOrEmpty(p))
                                        {
                                            std = p;
                                        }
                                        CloseHandle(phandle);
                                    }
                                }
                                else
                                {
                                    std = std.Replace(std_1, string.Empty);
                                    std = std.Replace(std_2, string.Empty);
                                }
                            }
                            originaltitle = std;
                        }
                        if (!string.IsNullOrEmpty(str) && addupstd != str)
                        {
                            addupstd = str;
                        }
                        if (title.ToString() != $"{originaltitle}{addupstd}")
                        {
                            string titlestr = $"{originaltitle}{addupstd}";
                            SendMessage(game.MainWindowHandle, WM_SETTEXT, IntPtr.Zero, titlestr);
                            TraceExtensions.Print(TraceExtensions.FormatMessage(
                                DateTime.Now,
                                "窗口管理器",
                                $"事件源：\r窗口标题变化事件\n" +
                                $"原始：\r{originaltitle}\n" +
                                $"当前：\r{originaltitle}{addupstd}"),
                                ConsoleColor.Yellow);
                            return true;
                        }
                        return false;
                    }
                }
                Action<IntPtr> KeyHook = hwnd =>
                {
                    ChangeMenu(!movestate);
                    if (!movestate) SizeMove(false);
                };
                Action<IntPtr> WindowLocationHook = hwnd =>
                {
                    if (GetWindowRect(hwnd, out RECT rect))
                    {
                        if (rect.left <= -32000 || IsIconic(hwnd)) // 过滤无效坐标（如最小化状态）
                        {
                            return;
                        }
                        else
                        {
                            Rectangle _rect = rect.ToRectangle();
                            SizeMove(false);
                        }
                    }
                };
                Action<IntPtr> WindowTitleHook = hwnd =>
                {
                    ChangeTitle(string.Empty);
                };
                ChangeMenu(false); // 上传挂钩过程
                SizeMove(dwmstate);
                ChangeTitle(" | MHYLAUNCHER_GO: 已加载 | F12: 位置锁定已激活");
                if (!game.HasExited)
                {
                    pm.Write(index, new HookData(
                        true,
                        game.MainWindowHandle,
                        new Action<IntPtr>[3]
                        {
                            KeyHook,
                            WindowLocationHook,
                            WindowTitleHook
                        }));
                    ChangeTitle(string.Empty); // 冗余操作，避免出现异常行为
                    game.WaitForExit();
                    pm.Write(index, new HookData(null));
                }
                GC.KeepAlive(KeyHook); // 固定相关Win32委托内存，防止垃圾回收机制回收方法体导致抛出【未将对象引用设置到对象的实例】异常
                GC.KeepAlive(WindowLocationHook);
                GC.KeepAlive(WindowTitleHook);
                GC.Collect();
            } // 执行窗口化宽屏模式
            else
            {
                return;
            } // 匹配无效的启动参数

            #endregion
        }

        /// <summary>
        /// 用于存储游戏窗口所在显示器的内部缓存字段
        /// </summary>
        private Screen cache_screen = Screen.PrimaryScreen;

        /// <summary>
        /// 用于存储游戏窗口目标大小的内部缓存字段
        /// </summary>
        private Size cache_csize = Size.Empty;

        /// <summary>
        /// 用于存储游戏窗口框架尺寸的内部缓存字段
        /// </summary>
        private Padding cache_cpad = Padding.Empty;

        /// <summary>
        /// 获取游戏窗口相关配置参数
        /// </summary>
        /// <param name="mode">配置模式</param>
        /// <param name="sizeset">配置参数</param>
        /// <param name="window_rect">游戏窗口大小</param>
        /// <param name="client_pad">游戏窗口客户区边距</param>
        /// <returns>返回游戏窗口相关配置参数</returns>
        /// <exception cref="ArgumentException">指示参数不合法</exception>
        /// <exception cref="ArithmeticException">指示算数运算出现异常</exception>
        /// <exception cref="NotSupportedException">指示特定操作不受支持</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private (Rectangle, Size, Padding) GetParam(BIN.MODE mode, (int, int) sizeset, Rectangle window_rect, Padding client_pad)
        {
            Screen main_screen = Screen.FromRectangle(window_rect); // 进行屏幕检测
            Size screensize = main_screen.Bounds.Size;
            Size workingareasize = main_screen.WorkingArea.Size;
            (Rectangle, Size, Padding) Param = (Rectangle.Empty, Size.Empty, Padding.Empty); // 准备返回值
            List<string> log = new List<string> // 准备日志字符串
            {
                "事件源：\r窗口框架检测模块",
                $"目标显示器：\r{main_screen.DeviceName}"
            };
            bool screen_check = cache_screen.DeviceName == main_screen.DeviceName // 按需执行运算操作
                && cache_screen.Bounds == main_screen.Bounds
                && cache_screen.WorkingArea == main_screen.WorkingArea;
            if (screen_check && cache_csize != Size.Empty && cache_cpad == client_pad)
            {
                log.Add("是否从缓存中读取：\rtrue");
                Param.Item2 = cache_csize;
                Param.Item3 = cache_cpad;
            }
            else
            {
                log.Add("是否从缓存中读取：\rfalse");
                Size targatesize = Size.Empty; // 对两种预置显示模式进行匹配
                if (mode == BIN.MODE.PC) // 21:9
                {
                    targatesize = new Size(2520, 1080);
                }
                if (mode == BIN.MODE.PHONE) // 20:9
                {
                    targatesize = new Size(2400, 1080);
                }
                bool iscustomsize = false;
                if (mode == BIN.MODE.CUSTOM) // 匹配用户配置
                {
                    targatesize = new Size(sizeset.Item1, sizeset.Item2);
                    iscustomsize = true;
                }
                if (targatesize == Size.Empty) // 检查参数
                {
                    throw new ArgumentException("配置参数错误：参数不合法。");
                };
                Size windowsize = Size.Empty; // 自动计算用户配置并进行合法化调整
                if (iscustomsize)
                {
                    int maxwidth = workingareasize.Width - client_pad.Horizontal;
                    int maxheight = workingareasize.Height - client_pad.Vertical - client_pad.Bottom;
                    if (targatesize.Width > maxwidth)
                    {
                        windowsize.Width = maxwidth;
                    }
                    else
                    {
                        windowsize.Width = targatesize.Width;
                    }
                    if (targatesize.Height > maxheight)
                    {
                        windowsize.Height = maxheight;
                    }
                    else
                    {
                        windowsize.Height = targatesize.Height;
                    }
                }
                else // 对各种纵横比的屏幕区域进行相关检测
                {
                    int screenw = screensize.Width;
                    int screenh = screensize.Height;
                    int? pick = null;
                    if ((double)screensize.Width / screensize.Height > 16D / 9D)
                    {
                        pick = screenh; // 21:9...
                    }
                    if ((double)screensize.Width / screensize.Height < 16D / 9D)
                    {
                        pick = screenw; // 4:3...
                    }
                    if (!pick.HasValue) // 适配并计算实际的显示区域大小
                    {
                        double zoomw = screenw / 2560D;
                        double zoomh = screenh / 1440D;
                        if (zoomw == zoomh)
                        {
                            windowsize = new Size()
                            {
                                Width = (int)Math.Round(targatesize.Width * zoomw, MidpointRounding.AwayFromZero),
                                Height = (int)Math.Round(targatesize.Height * zoomh, MidpointRounding.AwayFromZero)
                            };
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                throw new ArithmeticException("算数运算出现严重的未知错误。");
                            });
                        }
                    }
                    if (pick.GetValueOrDefault() == screenw)
                    {
                        windowsize = new Size()
                        {
                            Width = (int)Math.Round(targatesize.Width * (screenw / 2560D), MidpointRounding.AwayFromZero),
                            Height = 0
                        };
                        windowsize.Height =
                            (int)Math.Round((double)windowsize.Width / targatesize.Width * targatesize.Height, MidpointRounding.AwayFromZero);
                    }
                    if (pick.GetValueOrDefault() == screenh)
                    {
                        windowsize = new Size()
                        {
                            Width = 0,
                            Height = (int)Math.Round(targatesize.Height * (screenh / 1440D), MidpointRounding.AwayFromZero)
                        };
                        windowsize.Width =
                            (int)Math.Round((double)windowsize.Height / targatesize.Height * targatesize.Width, MidpointRounding.AwayFromZero);
                    }
                }
                int[] _ = new int[2] { windowsize.Width, windowsize.Height }; // 检查最小尺寸
                if (_.Max() < 640 || _.Min() < 480)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        throw new NotSupportedException("当前窗口配置不受支持，窗口最小空间应确保为640*480(横向/纵向不限)。");
                    });
                }
                Param.Item2 = windowsize;
                Param.Item3 = client_pad;

                // 保存内部参数
                cache_screen = main_screen;
                cache_csize = windowsize;
                cache_cpad = client_pad;
            }

            // 计算新的窗口坐标
            Point lction_p = main_screen.WorkingArea.Location;
            int height_s = main_screen.WorkingArea.Height;
            int height_w = Param.Item2.Height + cache_cpad.Vertical + cache_cpad.Bottom;
            lction_p.Offset(new Point()
            {
                X = (main_screen.WorkingArea.Width - Param.Item2.Width) / 2 - cache_cpad.Left,
                Y = (height_w > height_s / 2)
                    ? (int)(Math.Pow(height_s, 2) - Math.Pow(height_w, 2)) / (height_s * 3) + cache_cpad.Bottom
                    : (height_s - height_w) / 2 + cache_cpad.Bottom
            });
            Param.Item1 = new Rectangle(lction_p, Param.Item2 + cache_cpad.Size);

            // 打印日志
            log.Insert(2, $"窗口目标位置：\rX：{Param.Item1.Left,4}");
            log.Insert(3, $"\rY：{Param.Item1.Top,4}");
            log.Insert(4, $"\rWidth： {Param.Item1.Width,4}");
            log.Insert(5, $"\rHeight：{Param.Item1.Height,4}");
            log.Insert(6, $"窗口客户区大小：\rWidth： {Param.Item2.Width,4}");
            log.Insert(7, $"\rHeight：{Param.Item2.Height,4}");
            log.Insert(8, $"窗口框架大小：\rTop：   {Param.Item3.Top,4}");
            log.Insert(9, $"\rBottom：{Param.Item3.Bottom,4}");
            log.Insert(10, $"\rLeft：  {Param.Item3.Left,4}");
            log.Insert(11, $"\rRight： {Param.Item3.Right,4}");
            TraceExtensions.Print(TraceExtensions.FormatMessage(
                DateTime.Now,
                "窗口管理器",
                string.Join("\n", log)),
                ConsoleColor.Blue);
            return Param;
        }

        /// <summary>
        /// 用于指示Trace组件是否已准备好运行的全局字段
        /// </summary>
        public bool TraceReady = false;

        /// <summary>
        /// 用于储存Trace组件启动序列的全局字段
        /// </summary>
        public Task TraceInitialize = null;

        /// <summary>
        /// 用于指示二次启动窗口是否已经呈现
        /// </summary>
        private bool second = false;

        /// <summary>
        /// 用于存储米哈游启动器主窗口句柄的内部字段
        /// </summary>
        private IntPtr HYPHWND = IntPtr.Zero;

        /// <summary>
        /// 用于存储米哈游启动器进程ID的内部字段
        /// </summary>
        private int HYPPID = 0;

        /// <summary>
        /// 当收到第二进程的通知时，响应消息
        /// </summary>
        /// <param name="state">消息参数</param>
        /// <param name="timeout">超时时间</param>
        private void OnProgramStarted(object state, bool timeout)
        {
            Task.Run(() =>
            {
                MessageBox.Show(
                    $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] 已有应用程序实例正在运行！",
                    "MHYLAUNCHER_GO V2",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning,
                    MessageBoxResult.OK,
                    MessageBoxOptions.DefaultDesktopOnly);
            });
        }

        /// <summary>
        /// 当收到第二进程的通知时，响应消息
        /// </summary>
        /// <param name="state">消息参数</param>
        /// <param name="timeout">超时时间</param>
        private void OnSettingMode(object state, bool timeout)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (TraceReady)
                {
                    if (second)
                    {
                        Task.Run(() =>
                        {
                            MessageBox.Show(
                                $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] 应用程序已处于配置模式(只读)！",
                                "MHYLAUNCHER_GO V2",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning,
                                MessageBoxResult.OK,
                                MessageBoxOptions.DefaultDesktopOnly);
                        });
                    }
                    else
                    {
                        second = true;
                        Thread t = new Thread(() =>
                        {
                            new Input(this, HYPHWND, HYPPID, true).ShowDialog();
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                second = false;
                            });
                        })
                        { IsBackground = true };
                        t.Start();
                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        MessageBox.Show(
                            $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] 应用程序尚未完成加载！",
                            "MHYLAUNCHER_GO V2",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning,
                            MessageBoxResult.OK,
                            MessageBoxOptions.DefaultDesktopOnly);
                    });
                }
            });
        }

        /// <summary>
        /// 捕获非UI线程所有异常
        /// </summary>
        /// <param name="sender">抛出异常的?(object)</param>
        /// <param name="e">异常相关信息</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception_throw(e.ExceptionObject as Exception);
            if (!e.IsTerminating)
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 捕获UI线程所有异常
        /// </summary>
        /// <param name="sender">抛出异常的?(object)</param>
        /// <param name="e">异常相关信息</param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception_throw(e.Exception);
            e.Handled = true;
        }

        /// <summary>
        /// 捕获异步任务所有异常
        /// </summary>
        /// <param name="sender">抛出异常的?(object)</param>
        /// <param name="e">异常相关信息</param>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Exception_throw(e.Exception);
            e.SetObserved();
        }

        /// <summary>
        /// 捕获线程中产生的所有异常
        /// </summary>
        /// <param name="sender">抛出异常的?(object)</param>
        /// <param name="e">异常相关信息</param>
        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception_throw(e.Exception);
        }

        /// <summary>
        /// 表示程序运行中出现了[级别·轻微]等级的异常
        /// </summary>
        [Obsolete("该类型暂时没有使用需求", false)]
        private class Lowlevel_Exception : AggregateException
        {
            public Lowlevel_Exception() : base() { }
            public Lowlevel_Exception(string message) : base(message) { }
            public Lowlevel_Exception(string message, Exception innerException) : base(message, innerException) { }
            public Lowlevel_Exception(IEnumerable<Exception> innerExceptions) : base(innerExceptions) { }
            public Lowlevel_Exception(params Exception[] innerExceptions) : base(innerExceptions) { }
            public Lowlevel_Exception(string message, IEnumerable<Exception> innerExceptions) : base(message, innerExceptions) { }
            public Lowlevel_Exception(string message, params Exception[] innerExceptions) : base(message, innerExceptions) { }
            public Lowlevel_Exception(string message, IList<Exception> innerExceptions) : base(message, innerExceptions) { }
        }

        /// <summary>
        /// 用于保存未导致应用程序运行错误的异常信息
        /// </summary>
        [Obsolete("该字段暂时没有使用需求", false)]
        public List<string> ExceptionList = new List<string> { };

        /// <summary>
        /// 封装异常处理程序
        /// </summary>
        /// <param name="e">异常相关信息</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Exception_throw(Exception e)
        {
            // 播放提示音
            // Beep(UType.MB_OK);
            // 匹配未知异常
            string estd =
                $"[应用程序内部异常] [{DateTime.Now:yyyy/MM/dd HH:mm:ss}]"
                + $"\n\n根命名空间:{e.Source}"
                + $"\n方法体:{e.TargetSite}";
            if (e is AggregateException ae && ae.InnerException != null)
            {
                estd +=
                    $"\nInnerException:{e.InnerException.GetType().Name}"
                    + $"\n    根命名空间:{e.InnerException.Source}"
                    + $"\n    方法体:{e.InnerException.TargetSite}"
                    + $"\n    详细信息:\n        {e.InnerException.Message}"
                    + $"{(Regex.IsMatch(e.InnerException.Message, @"\n\z") ? string.Empty : "\n")}"
                    + $"    位置:";
                if (string.IsNullOrEmpty(e.InnerException.StackTrace)
                    || !e.InnerException.StackTrace.Contains("\n"))
                {
                    estd += $"\n        {e.InnerException.StackTrace.Trim()}";
                }
                else
                {
                    foreach (string st in e.InnerException.StackTrace.Split('\n'))
                    {
                        estd += $"\n        {st.Trim()}";
                    }
                }
                estd +=
                    "\n\nMHYLAUNCHER_GO V2 - Exceptions Processed By FeiLingshu"
                    + "\n----------"
                    + "\n是否导出Dump文件？";
            }
            else
            {
                estd +=
                    $"\n详细信息:{e.GetType().Name}\n    {e.Message}"
                    + $"{(Regex.IsMatch(e.Message, @"\n\z") ? string.Empty : "\n")}"
                    + $"位置:";
                if (string.IsNullOrEmpty(e.StackTrace)
                    || !e.StackTrace.Contains("\n"))
                {
                    estd += $"\n    {e.StackTrace.Trim()}";
                }
                else
                {
                    foreach (string st in e.StackTrace.Split('\n'))
                    {
                        estd += $"\n    {st.Trim()}";
                    }
                }
                estd +=
                    "\n\nMHYLAUNCHER_GO V2 - Exceptions Processed By FeiLingshu"
                    + "\n----------"
                    + "\n是否导出Dump文件？";
            }
            MessageBoxResult mbr = MessageBox.Show(
                estd,
                $"MHYLAUNCHER_GO V2 - Thread 0x{Thread.CurrentThread.ID():X8}+0x{Thread.CurrentThread.ManagedThreadId:X}",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error,
                MessageBoxResult.No,
                MessageBoxOptions.DefaultDesktopOnly);
            // 按需导出Dump文件
            if (mbr == MessageBoxResult.Yes)
            {
                string BASE = string.Empty;
                using (Process CurrentProcess = Process.GetCurrentProcess())
                {
                    BASE = Path.GetDirectoryName(CurrentProcess.MainModule.FileName);
                }
                string SERIAL = string.Empty;
                if (File.Exists($"{BASE}\\crash.dmp"))
                {
                    try
                    {
                        File.Delete($"{BASE}\\crash.dmp");
                    }
                    catch (Exception)
                    {
                        SERIAL = $"(0x{DateTime.Now.GetHashCode():X8})";
                    }
                    finally
                    {
                        Regex regex = new Regex(@"^crash\(0x[0-9A-Fa-f]{8}\)\.dmp$", RegexOptions.IgnoreCase);
                        string[] dumpFiles = Directory.GetFiles(BASE, "*.dmp", SearchOption.TopDirectoryOnly);
                        foreach (string filePath in dumpFiles)
                        {
                            string fileName = Path.GetFileName(filePath);
                            if (regex.IsMatch(fileName))
                            {
                                try
                                {
                                    File.Delete(filePath);
                                }
                                catch (Exception)
                                {
                                    if (fileName == $"crash{SERIAL}.dmp")
                                    {
                                        SERIAL = $"(0x{DateTime.Now.AddMilliseconds(1D).GetHashCode():X8})";
                                    }
                                }
                            }
                        }
                    }
                }
                DumpGenerator.GenerateDump($"{BASE}\\crash{SERIAL}.dmp");
            }
            // 异常处理过程开始前首先尝试中止游戏进程监视组件
            pm?.EndProcessManager();
            // 强制退出程序进程
            Environment.Exit(0);
        }
    }
}
