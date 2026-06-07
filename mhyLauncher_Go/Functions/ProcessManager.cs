using MHYLAUNCHER_GO.Functions.Config;
using MHYLAUNCHER_GO.Functions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static MHYLAUNCHER_GO.Functions.Win32;

namespace MHYLAUNCHER_GO.Functions
{
    /// <summary>
    /// 实现游戏进程捕获功能的公开类
    /// </summary>
    public class ProcessManager
    {
        /// <summary>
        /// 包含游戏进程监视组件所需信息的结构实例
        /// </summary>
        private readonly Conditions infos = new Conditions();

        /// <summary>
        /// 包含游戏进程监视组件所需信息的结构
        /// </summary>
        public struct Conditions
        {
            /// <summary>
            /// 目标路径
            /// </summary>
            public string[] paths;
            /// <summary>
            /// 程序模式参数传递
            /// </summary>
            public BIN.MODE mode;
            /// <summary>
            /// 程序配置参数传递
            /// </summary>
            public (int, int) size;
            /// <summary>
            /// 响应函数应触发的方法体
            /// </summary>
            public Action<Process, BIN.MODE, (int, int), int, bool> _void;

            /// <summary>
            /// 初始化Conditions结构
            /// </summary>
            /// <param name="name"></param>
            /// <param name="path"></param>
            /// <param name="mode"></param>
            /// <param name="_void"></param>
            public Conditions(string[] paths, BIN.MODE mode, (int, int) size, Action<Process, BIN.MODE, (int, int), int, bool> _void)
            {
                this.paths = paths;
                this.mode = mode;
                this.size = size;
                this._void = _void;
            }

            /// <summary>
            /// 以指定参数执行结构中的方法体
            /// </summary>
            /// <param name="game">运行方法体所需参数</param>
            /// <param name="index">运行方法体的实例索引</param>
            /// <param name="dwmstate">指示DWM操作是否执行</param>
            public void Run(Process game, int index, bool dwmstate)
            {
                _void(game, mode, size, index, dwmstate);
            }
        }

        /// <summary>
        /// 初始化游戏进程监视组件
        /// </summary>
        /// <param name="infos">包含游戏进程监视组件所需信息的结构</param>
        /// <param name="hds">包含win32钩子信息的结构体数组</param>
        /// <param name="HYP">米哈游启动器进程实例</param>
        /// <param name="appbase">主应用程序域实例</param>
        public ProcessManager(Conditions infos, HookData[] hds, Process HYP, App AppBase)
        {
            this.infos = infos;
            this.hds = hds;
            this.HYP = HYP;
            this.AppBase = AppBase;
            StartProcessManager();
        }

        /// <summary>
        /// 游戏进程监视组件线程实例
        /// </summary>
        private Thread ProcessManagerloop = null;

        /// <summary>
        /// 启动游戏进程监视组件
        /// </summary>
        private void StartProcessManager()
        {
            BeginEventHook();
            ProcessManagerloop = new Thread(() =>
            {
                AutoResetEvent timer = new AutoResetEvent(false);
                Process[] games = new Process[infos.paths.Length];
                Task<int>[] ts = new Task<int>[games.Length];
                do // 监视游戏进程的启动操作
                {
                    if (games.Contains(null))
                    {
                        Process[] ProcessPool = Process.GetProcesses();
                        ILookup<string, int> processcache = ProcessPool
                            .Where(p => !string.IsNullOrEmpty(p.ProcessName))
                            .ToLookup(
                                keySelector: p => p.ProcessName.ToUpper(),
                                elementSelector: p => p.Id
                            );
                        Array.ForEach(ProcessPool, p => { p?.Dispose(); p = null; });
                        for (int i_path = 0; i_path < infos.paths.Length; i_path++)
                        {
                            if (games[i_path] != null)
                            {
                                continue;
                            }
                            IEnumerable<int> pids = processcache[Path.GetFileNameWithoutExtension(infos.paths[i_path]).ToUpper()];
                            foreach (int pid in pids)
                            {
                                try
                                {
                                    using (Process process = Process.GetProcessById(pid))
                                    {
                                        if (process != null && !process.HasExited)
                                        {
                                            // 由于游戏进程受到保护，process.MainModule.FileName会产生拒绝访问的win32异常
                                            IntPtr phandle = OpenProcess(
                                                ProcessAccessFlags.PROCESS_QUERY_INFORMATION | ProcessAccessFlags.PROCESS_VM_READ,
                                                false, pid);
                                            if (phandle != IntPtr.Zero)
                                            {
                                                StringBuilder path = new StringBuilder(32768);
                                                uint scount = (uint)path.Capacity;
                                                QueryFullProcessImageName(phandle, 0, path, ref scount);
                                                string p = path.ToString().Trim();
                                                path.Clear();
                                                if (!string.IsNullOrEmpty(p) && p.Length == infos.paths[i_path].Length && p == infos.paths[i_path])
                                                {
                                                    games[i_path] = Process.GetProcessById(pid);
                                                    CloseHandle(phandle);
                                                    break;
                                                }
                                                CloseHandle(phandle);
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                        }
                        DateTime timepoint = DateTime.Now;
                        List<string> std_false = new List<string>();
                        List<string> std_true = new List<string>();
                        List<string> std_task = new List<string>();
                        List<string> std_exp = new List<string>();
                        for (int i_path = 0; i_path < infos.paths.Length; i_path++)
                        {
                            if (games[i_path] == null)
                            {
                                std_false.Add($"进程名：\r{Path.GetFileName(infos.paths[i_path])}");
                                std_false.Add($"· 路径：\r{infos.paths[i_path]}");
                            }
                            else
                            {
                                if (ts[i_path] == null)
                                {
                                    std_true.Add($"进程名：\r{Path.GetFileName(infos.paths[i_path])}");
                                    std_true.Add($"· 路径：\r{infos.paths[i_path]}");
                                    std_true.Add($"· 进程ID：\r{games[i_path].Id}");
                                    int index = i_path;
                                    ts[index] = Task.Run(RunFunc); // 执行游戏进程处理相关操作
                                    int RunFunc()
                                    {
                                        int timeout = 0;
                                        IntPtr hwnd = IntPtr.Zero;
                                        do
                                        {
                                            if (games[index].HasExited || timeout == 300)
                                            {
                                                timeout = 0;
                                                int taskresult = 0;
                                                using (games[index])
                                                {
                                                    taskresult = games[index].HasExited ? -1 : int.MinValue;
                                                }
                                                games[index] = null;
                                                return taskresult;
                                            }
                                            timer.WaitOne(100, false);
                                            timeout++;
                                            try
                                            {
                                                hwnd = games[index].MainWindowHandle;
                                            }
                                            catch (Exception) { }
                                        } while (hwnd == IntPtr.Zero);
                                        timer.WaitOne(1000, false); // 为了防止游戏窗口已初始化但未呈现 OR 呈现后极短时间内被销毁，以下为冗余代码
                                        if (games[index].HasExited || games[index].MainWindowHandle == IntPtr.Zero)
                                        {
                                            int taskresult = 0;
                                            using (games[index])
                                            {
                                                taskresult = games[index].HasExited ? -1 : int.MinValue;
                                            }
                                            games[index] = null;
                                            return taskresult;
                                        }
                                        using (games[index])
                                        {
                                            if (!games[index].HasExited)
                                            {
                                                bool dwm = games[index].MainWindowHandle.SetDWM(
                                                    false,
                                                    true,
                                                    Color.FromArgb(0xFF, 0x1E, 0x20, 0x25).TO_COLORREF(),
                                                    Color.FromArgb(0xFF, 0xE5, 0xE5, 0xE5).TO_COLORREF(),
                                                    true);
                                                try
                                                {
                                                    IntPtr processhandle = OpenProcess(
                                                        ProcessAccessFlags.PROCESS_QUERY_INFORMATION | ProcessAccessFlags.PROCESS_VM_READ,
                                                        false,
                                                        games[index].Id);
                                                    int ppid = 0;
                                                    if (processhandle != IntPtr.Zero)
                                                    {
                                                        PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
                                                        if (NtQueryInformationProcess(
                                                            processhandle,
                                                            0, // ProcessBasicInformation
                                                            ref pbi,
                                                            (uint)Marshal.SizeOf<PROCESS_BASIC_INFORMATION>(),
                                                            out _) >= 0)
                                                        {
                                                            ppid = pbi.InheritedFromUniqueProcessId.ToInt32();
                                                        }
                                                        CloseHandle(processhandle);
                                                    }
                                                    if (ppid == 0 || ppid != HYP.Id)
                                                    {
                                                        string pathtitle = "UNKNOWN";
                                                        IntPtr phandle = OpenProcess(
                                                            ProcessAccessFlags.PROCESS_QUERY_INFORMATION | ProcessAccessFlags.PROCESS_VM_READ,
                                                            false, games[index].Id);
                                                        if (phandle != IntPtr.Zero)
                                                        {
                                                            StringBuilder path = new StringBuilder(32768);
                                                            uint scount = (uint)path.Capacity;
                                                            QueryFullProcessImageName(phandle, 0, path, ref scount);
                                                            string p = path.ToString().Trim();
                                                            path.Clear();
                                                            if (!string.IsNullOrEmpty(p))
                                                            {
                                                                pathtitle = p;
                                                            }
                                                            CloseHandle(phandle);
                                                        }
                                                        string std = $"{Path.GetDirectoryName(pathtitle)}" +
                                                            $"\\0x{"MHYLAUNCHER_GO".GetHashCode():X8}.MHYLG";
                                                        if (File.Exists(std))
                                                        {
                                                            std_exp.Add("操作：\r已执行CPU亲和性配置");
                                                            std_exp.Add($"进程ID：\r0x{games[index].Id:X8}");
                                                            std_exp.Add($"状态：\r跳过");
                                                        }
                                                        else
                                                        {
                                                            if (Load.CoreType)
                                                            {
                                                                if (Load.SetProcess_Pro(games[index], ProcessPriorityClass.Normal, true))
                                                                {
                                                                    std_exp.Add("操作：\r已执行CPU亲和性配置");
                                                                    std_exp.Add($"进程ID：\r0x{games[index].Id:X8}");
                                                                    std_exp.Add($"状态：\r成功");
                                                                }
                                                                else
                                                                {
                                                                    std_exp.Add("操作：\r已执行CPU亲和性配置");
                                                                    std_exp.Add($"进程ID：\r0x{games[index].Id:X8}");
                                                                    std_exp.Add($"状态：\r失败");
                                                                    void SetFlag(int _pid, string _path)
                                                                    {
                                                                        Task.Run(() =>
                                                                        {
                                                                            MessageBoxResult mbr = MessageBox.Show(
                                                                                $"对游戏进程 0x{_pid:X8} CPU亲和性的配置出现错误，是否从此忽略该游戏？",
                                                                                $"EfficiencyMode.dll",
                                                                                MessageBoxButton.YesNo,
                                                                                MessageBoxImage.Error,
                                                                                MessageBoxResult.No,
                                                                                MessageBoxOptions.DefaultDesktopOnly);
                                                                            if (mbr == MessageBoxResult.Yes)
                                                                            {
                                                                                File.Create(_path);
                                                                            }
                                                                        }).ContinueWith(t =>
                                                                        {
                                                                            if (t.Status == TaskStatus.Faulted)
                                                                            {
                                                                                _ = t.Exception;
                                                                            }
                                                                        });
                                                                    }
                                                                    SetFlag(games[index].Id, std);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                std_exp.Add("操作：\r已执行CPU亲和性配置");
                                                                std_exp.Add($"进程ID：\r0x{games[index].Id:X8}");
                                                                std_exp.Add($"状态：\r忽略");
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        std_exp.Add("操作：\r已执行CPU亲和性配置");
                                                        std_exp.Add($"进程ID：\r0x{games[index].Id:X8}");
                                                        std_exp.Add($"状态：\r跳过(已配置)");
                                                    }
                                                    // Experiment ... ⬆️
                                                    infos.Run(games[index], index, dwm);
                                                }
                                                catch (Exception exp)
                                                {
                                                    if (!games[index].HasExited && games[index].MainWindowHandle != IntPtr.Zero)
                                                    {
                                                        games[index] = null;
                                                        throw exp;
                                                    }
                                                }
                                            }
                                        }
                                        games[index] = null;
                                        return 0;
                                    }
                                    std_task.Add($"任务已启动: \rID = {ts[index].Id}");
                                    ts[index].ContinueWith(t =>
                                    {
                                        if (t.Status == TaskStatus.RanToCompletion && t.Result < 0)
                                        {
                                            List<string> logs = new List<string>
                                            {
                                                $"任务ID：\r{t.Id}"
                                            };
                                            switch (t.Result)
                                            {
                                                case -1:
                                                    logs.Add("错误原因：\r游戏进程已退出");
                                                    break;
                                                case int.MinValue:
                                                    logs.Add("错误原因：\r未能识别到游戏窗口");
                                                    break;
                                                default:
                                                    logs.Add("错误原因：\r未知");
                                                    break;
                                            }
                                            TraceExtensions.Print(TraceExtensions.FormatMessage(
                                                timepoint,
                                                "任务执行错误",
                                                string.Join("\n", logs)),
                                                ConsoleColor.Red);
                                        }
                                        ts[index] = null;
                                        if (t.Status == TaskStatus.Faulted)
                                        {
                                            throw t.Exception;
                                        }
                                    });
                                }
                            }
                        }
                        if (std_false.Count > 0)
                        {
                            std_false.Add($"状态：\r未捕获");
                            TraceExtensions.Print(TraceExtensions.FormatMessage(
                                timepoint,
                                "进程管理器",
                                string.Join("\n", std_false)),
                                ConsoleColor.Gray);
                        }
                        if (std_true.Count > 0)
                        {
                            std_true.Add($"状态：\r已捕获");
                            TraceExtensions.Print(TraceExtensions.FormatMessage(
                                timepoint,
                                "进程管理器",
                                string.Join("\n", std_true)),
                                ConsoleColor.Green);
                        }
                        if (std_task.Count > 0)
                        {
                            TraceExtensions.Print(TraceExtensions.FormatMessage(
                                timepoint,
                                "异步线程",
                                string.Join("\n", std_task)),
                                ConsoleColor.Blue);
                        }
                        if (std_exp.Count > 0)
                        {
                            TraceExtensions.Print(TraceExtensions.FormatMessage(
                                timepoint,
                                "EfficiencyMode模块",
                                string.Join("\n", std_exp)),
                                ConsoleColor.Yellow);
                        }
                    }
                    timer.WaitOne(TimeSpan.FromSeconds(1), false);
                } while (ProcessManagerloop.IsAlive);
            })
            { IsBackground = true };
            ProcessManagerloop.Start();
        }

        /// <summary>
        /// 中止游戏进程监视组件
        /// </summary>
        public void EndProcessManager()
        {
            EndEventHook();
            if (ProcessManagerloop != null && ProcessManagerloop.IsAlive)
            {
                ProcessManagerloop.Abort();
            }
        }

        /// <summary>
        /// 保存Hook信息的结构体
        /// </summary>
        public struct HookData
        {
            /// <summary>
            /// 当前Hook项目的状态
            /// </summary>
            public bool state;
            /// <summary>
            /// 当前Hook项目要过滤的目标句柄
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// 当前Hook需要挂钩的处理程序
            /// </summary>
            public Action<IntPtr>[] _void;

            /// <summary>
            /// 初始化未生效Hook实例
            /// </summary>
            /// <param name="_">已放弃</param>
            public HookData(object _)
            {
                this.state = false;
                this.hwnd = IntPtr.Zero;
                this._void = null;
            }

            /// <summary>
            /// 初始化特定Hook实例
            /// </summary>
            /// <param name="state">Hook生效状态</param>
            /// <param name="hwnd">Hook要过滤的目标句柄</param>
            /// <param name="_void">Hook需要挂钩的处理程序</param>
            public HookData(bool state, IntPtr hwnd, Action<IntPtr>[] _void)
            {
                this._void = _void;
                this.hwnd = hwnd;
                this.state = state;
            }
        }

        /// <summary>
        /// 实现原子级读取操作
        /// </summary>
        /// <param name="index">项目索引</param>
        /// <returns>返回对应的Hook信息实例</returns>
        public HookData Read(int index)
        {
            _lock.EnterReadLock();
            try
            {
                return hds[index];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 实现原子级写入操作
        /// </summary>
        /// <param name="index">项目索引</param>
        /// <param name="value">对应的Hook信息实例</param>
        public void Write(int index, HookData value)
        {
            _lock.EnterWriteLock();
            try
            {
                hds[index] = value;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 用于保存主应用程序域实例的内部字段
        /// </summary>
        private readonly App AppBase = null;

        /// <summary>
        /// 用于保存米哈游启动器进程实例的内部字段
        /// </summary>
        private readonly Process HYP = null;

        /// <summary>
        /// 用于保存Hook信息的内部字段
        /// </summary>
        private readonly HookData[] hds = null;

        /// <summary>
        /// 用于实现读写竞态锁的组件
        /// <para>能够实现多写一读的一种方法</para>
        /// </summary>
        public readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Hook实例1
        /// </summary>
        private IntPtr KeyHook;

        /// <summary>
        /// Hook实例2
        /// </summary>
        private IntPtr WindowLocationHook;

        /// <summary>
        /// Hook实例3
        /// </summary>
        private IntPtr WindowTitleHook;

        /// <summary>
        /// 用于为Hook实例1实现内存固定的全局字段
        /// </summary>
        private HOOKPROC HOOKPROC;

        /// <summary>
        /// Hook实例1的处理程序
        /// </summary>
        /// <param name="nCode">参数1</param>
        /// <param name="wParam">参数2</param>
        /// <param name="lParam">参数3</param>
        /// <returns>返回值</returns>
        /// <exception cref="Win32Exception">发生win32错误</exception>
        private int Win32CallBack(int nCode, int wParam, IntPtr lParam)
        {
            try
            {
                KeyBoardHookStruct keyBoardHookStruct = new KeyBoardHookStruct();
                try
                {
                    keyBoardHookStruct = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
                }
                catch (Exception) { }
                finally
                {
                    if (keyBoardHookStruct == null)
                    {
                        Thread t = new Thread(() =>
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error(),
                                $"LocalLowHook捕获发生异常。");
                        })
                        { IsBackground = true };
                        t.Start();
                    }
                }
                if (wParam == WM_KEYUP)
                {
                    if (keyBoardHookStruct.vkCode == VK_F12)
                    {
                        IntPtr hwnd = GetForegroundWindow();
                        bool tracecheck = false;
                        if (AppBase.TraceReady && HYP != null)
                        {
                            IntPtr hyph = HYP.MainWindowHandle;
                            if (hyph != IntPtr.Zero && hwnd == hyph)
                            {
                                TraceExtensions.Set(AppBase);
                                tracecheck = true;
                            }
                        }
                        if (!tracecheck)
                        {
                            for (int i = 0; i < hds.Length; i++)
                            {
                                HookData hd = Read(i);
                                if (hd.state)
                                {
                                    if (hwnd == hd.hwnd)
                                    {
                                        hd._void[0](hwnd);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Thread t = new Thread(() =>
                {
                    throw exp;
                })
                { IsBackground = true };
                t.Start();
            }
            return CallNextHookEx((int)KeyHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// 用于为Hook实例2实现内存固定的全局字段
        /// </summary>
        private WinEventDelegate EventDelegate_L;

        /// <summary>
        /// Hook实例2的处理程序
        /// </summary>
        /// <param name="hWinEventHook">参数1</param>
        /// <param name="eventType">参数2</param>
        /// <param name="hwnd">参数3</param>
        /// <param name="idObject">参数4</param>
        /// <param name="idChild">参数5</param>
        /// <param name="dwEventThread">参数6</param>
        /// <param name="dwmsEventTime">参数7</param>
        private void WinEventHook_L(
            IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                if (idObject == OBJID_WINDOW && idChild == CHILDID_SELF && hwnd != IntPtr.Zero)
                {
                    for (int i = 0; i < hds.Length; i++)
                    {
                        HookData hd = Read(i);
                        if (hd.state)
                        {
                            if (hwnd == hd.hwnd)
                            {
                                hd._void[1](hwnd);
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }


        /// <summary>
        /// 用于为Hook实例3实现内存固定的全局字段
        /// </summary>
        private WinEventDelegate EventDelegate_T;

        /// <summary>
        /// Hook实例3的处理程序
        /// </summary>
        /// <param name="hWinEventHook">参数1</param>
        /// <param name="eventType">参数2</param>
        /// <param name="hwnd">参数3</param>
        /// <param name="idObject">参数4</param>
        /// <param name="idChild">参数5</param>
        /// <param name="dwEventThread">参数6</param>
        /// <param name="dwmsEventTime">参数7</param>
        private void WinEventHook_T(
            IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                if (idObject == OBJID_WINDOW && idChild == CHILDID_SELF && hwnd != IntPtr.Zero)
                {
                    for (int i = 0; i < hds.Length; i++)
                    {
                        HookData hd = Read(i);
                        if (hd.state)
                        {
                            if (hwnd == hd.hwnd)
                            {
                                hd._void[2](hwnd);
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 开始Hook
        /// </summary>
        /// <exception cref="Win32Exception">发生win32错误</exception>
        private void BeginEventHook()
        {
            this.HOOKPROC = Win32CallBack;
            KeyHook = SetWindowsHookEx(WH_KEYBOARD_LL, this.HOOKPROC, IntPtr.Zero, 0);
            if (KeyHook == IntPtr.Zero)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(),
                        $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                });
            }
            EventDelegate_L = WinEventHook_L;
            WindowLocationHook = SetWinEventHook(
                EVENT_OBJECT_LOCATIONCHANGE,
                EVENT_OBJECT_LOCATIONCHANGE,
                IntPtr.Zero,
                EventDelegate_L,
                0U,
                0U,
                WINEVENT_OUTOFCONTEXT);
            if (WindowLocationHook == IntPtr.Zero)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(),
                        $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                });
            }
            EventDelegate_T = WinEventHook_T;
            WindowTitleHook = SetWinEventHook(
                EVENT_OBJECT_NAMECHANGE,
                EVENT_OBJECT_NAMECHANGE,
                IntPtr.Zero,
                EventDelegate_T,
                0U,
                0U,
                WINEVENT_OUTOFCONTEXT);
            if (WindowTitleHook == IntPtr.Zero)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(),
                        $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                });
            }
        }

        /// <summary>
        /// 卸载Hook
        /// </summary>
        /// <exception cref="Win32Exception">发生win32错误</exception>
        private void EndEventHook()
        {
            if (!UnhookWindowsHookEx((int)KeyHook))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(),
                        $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                });
            };
            if (!UnhookWinEvent(WindowLocationHook))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(),
                        $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                });
            };
            if (!UnhookWinEvent(WindowTitleHook))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(),
                        $"挂接全局HOOK挂钩过程中出现Win32异常({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                });
            };
            GC.KeepAlive(HOOKPROC);
            GC.KeepAlive(EventDelegate_L);
            GC.KeepAlive(EventDelegate_T);
        }
    }
}
