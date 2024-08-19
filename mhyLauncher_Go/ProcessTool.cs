using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace mhyLauncher_Go
{
    /// <summary>
    /// 游戏进程监视组件的公开类
    /// </summary>
    internal class ProcessTool
    {
        /// <summary>
        /// 包含游戏进程监视组件所需信息的结构实例
        /// </summary>
        private readonly Conditions infos = new Conditions();

        /// <summary>
        /// 包含游戏进程监视组件所需信息的结构
        /// </summary>
        internal struct Conditions
        {
            /// <summary>
            /// 目标名称
            /// </summary>
            internal string name;

            /// <summary>
            /// 目标路径
            /// </summary>
            internal string path;

            /// <summary>
            /// 程序自身命令行参数传递
            /// </summary>
            internal string[] args;

            /// <summary>
            /// 响应函数应触发的方法体
            /// </summary>
            internal Action<Process, string[]> _void;

            /// <summary>
            /// 初始化Conditions结构
            /// </summary>
            /// <param name="name"></param>
            /// <param name="path"></param>
            /// <param name="args"></param>
            /// <param name="_void"></param>
            internal Conditions(string name, string path, string[] args, Action<Process, string[]> _void)
            {
                this.name = name;
                this.path = path;
                this.args = args;
                this._void = _void;
            }

            /// <summary>
            /// 以指定参数执行结构中的方法体
            /// </summary>
            /// <param name="game">运行方法体所需参数</param>
            internal void Run(Process game)
            {
                _void(game, args);
            }
        }

        /// <summary>
        /// 初始化游戏进程监视组件
        /// </summary>
        /// <param name="infos">包含游戏进程监视组件所需信息的结构</param>
        internal ProcessTool(Conditions infos)
        {
            this.infos = infos;
            StartProcessTool();
        }

        /// <summary>
        /// 游戏进程监视组件线程实例
        /// </summary>
        private Thread processtoolloop = null;

        /// <summary>
        /// 多线程操作相关线程实例
        /// </summary>
        private Thread working = null;

        /// <summary>
        /// 指示游戏进程监视组件的前置操作是否已经完成
        /// </summary>
        private bool isready = false;

        /// <summary>
        /// 启动游戏进程监视组件
        /// </summary>
        private void StartProcessTool()
        {
            processtoolloop = new Thread(() =>
            {
                do
                {
                    AutoResetEvent timer = new AutoResetEvent(false);
                    Process game = null;
                    Process[] processlist = null;
                    StringBuilder sb = new StringBuilder(infos.path.Length + 1);
                    uint length = 0;
                    do // 监视游戏进程的启动操作
                    {
                        processlist = Process.GetProcessesByName(infos.name);
                        foreach (Process process in processlist)
                        {
                            // if (process.MainModule.FileName == infos.path) <= 由于游戏进程受到保护，该方法会产生拒绝访问的win32异常
                            length = GetModuleFileNameEx(process.Handle.ToInt32(), IntPtr.Zero, sb, (uint)sb.Capacity);
                            if (length == infos.path.Length && sb.ToString() == infos.path)
                            {
                                game = process;
                                sb.Clear();
                                if (Debugger.IsAttached) // 调试模式下，输出触发信息及相关数据
                                {
                                    Debug.Print("--------\n{0}ProcessTool Triggered\n+ TargateName: {1}\n+ TargatePath: {2}\n+ TargatePID: {3}",
                                        $"[{DateTime.Now:HH:mm:ss}]",
                                        $"{infos.name}.exe", infos.path, game.Id);
                                }
                                break;
                            }
                            continue;
                        }
                        if (game != null) break;
                        timer.WaitOne(TimeSpan.FromSeconds(1));
                        if (Debugger.IsAttached) // 调试模式下，输出超时信息及相关数据
                        {
                            Debug.Print("--------\n{0}ProcessTool Timeout\n+ TargateName: {1}\n+ TargatePath: {2}",
                                $"[{DateTime.Now:HH:mm:ss}]",
                                $"{infos.name}.exe", infos.path);
                        }
                    } while (processtoolloop.IsAlive);
                    working = new Thread(() => // 执行游戏进程处理相关操作
                    {
                        int timeout = 0;
                        do
                        {
                            if (timeout == 20000)
                            {
                                timeout = 0;
                                return;
                            }
                            timer.WaitOne(1);
                            timeout++;
                        } while (!game.HasExited && game.MainWindowHandle == IntPtr.Zero);
                        if (game.HasExited) return;
                        timer.WaitOne(500);
                        isready = true; // 在主程序产生需要释放的资源之前，设置标志量
                        infos.Run(game);
                    })
                    { IsBackground = true };
                    working.Start();
                    game.WaitForExit();
                    if (working != null && working.IsAlive && !isready) // 如在主程序执行前游戏进程已经退出，则阻止相关线程执行
                    {
                        working.Abort();
                    }
                    isready = false;
                    working = null;
                } while (processtoolloop.IsAlive);
            })
            { IsBackground = true };
            processtoolloop.Start();
        }

        /// <summary>
        /// 中止游戏进程监视组件
        /// </summary>
        internal void EndProcessTool()
        {
            if (working != null && working.IsAlive)
            {
                working.Abort();
            }
            if (processtoolloop != null && processtoolloop.IsAlive)
            {
                processtoolloop.Abort();
            }
        }

        /// <summary>
        /// 实现通过进程句柄获取应用程序或模块的路径的WindowsAPI
        /// </summary>
        /// <param name="hProcess">目标进程句柄</param>
        /// <param name="hModule">目标模块句柄</param>
        /// <param name="lpFilename">承载路径信息的StringBuilder实例</param>
        /// <param name="nSize">StringBuilder实例的缓冲区大小</param>
        /// <returns>返回复制到缓冲区的字符串的长度</returns>
        [DllImport("Psapi.dll", EntryPoint = "GetModuleFileNameEx")]
        private static extern uint GetModuleFileNameEx(int hProcess, IntPtr hModule, [Out] StringBuilder lpFilename, uint nSize);
    }
}
