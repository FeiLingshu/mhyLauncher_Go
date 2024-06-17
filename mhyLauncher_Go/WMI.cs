using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Runtime.Serialization;
using System.Threading;

namespace mhyLauncher_Go
{
    /// <summary>
    /// 实现WMI互操作的帮助类
    /// </summary>
    internal class WMI
    {
        /// <summary>
        /// WMI事件响应函数实例
        /// </summary>
        internal ManagementEventWatcher eventWatcher = null;

        /// <summary>
        /// WMI事件命令参数
        /// </summary>
        private readonly string name = null;

        /// <summary>
        /// WMI事件筛选条件
        /// </summary>
        private readonly string path = null;

        /// <summary>
        /// 程序自身命令行参数传递
        /// </summary>
        private readonly string[] args = null;

        /// <summary>
        /// WMI事件响应函数应触发的方法体
        /// </summary>
        private readonly Action<Process, string[]> _void = null;

        /// <summary>
        /// 初始化WMI互操作类
        /// </summary>
        /// <param name="name">WMI事件命令参数</param>
        /// <param name="path">WMI事件筛选条件</param>
        /// <param name="args">程序自身命令行参数传递</param>
        /// <param name="_void">WMI事件响应函数应触发的方法体</param>
        internal WMI(string name, string path, string[] args, Action<Process, string[]> _void)
        {
            this.name = name;
            this.path = path.Replace(@"\", @"\\");
            this.args = args;
            this._void = _void;
            StartWMI();
        }

        /// <summary>
        /// WMI同步查询线程实例
        /// </summary>
        private Thread wmi_thread = null;

        /// <summary>
        /// 多线程操作相关线程实例
        /// </summary>
        private Thread working = null;

        /// <summary>
        /// 指示WMI事件响应函数的前置操作是否已经完成
        /// </summary>
        private bool isready = false;

        /// <summary>
        /// 启动WMI事件
        /// </summary>
        private void StartWMI()
        {
            if (eventWatcher != null) return;
            WqlEventQuery regQuery = new WqlEventQuery("__InstanceCreationEvent",
                TimeSpan.FromSeconds(1),
                "TargetInstance ISA 'Win32_Process'" + $@" AND TargetInstance.Name = '{name}'" + $@" AND TargetInstance.ExecutablePath = '{path}'");
            eventWatcher = new ManagementEventWatcher(regQuery);
            eventWatcher.Options.Timeout = TimeSpan.FromSeconds(5);
            wmi_thread = new Thread(() => // 启动WMI查询循环线程
            {
                do // WMI查询循环
                {
                    ManagementBaseObject e = null;
                    try
                    {
                        e = eventWatcher.WaitForNextEvent();
                    }
                    catch (ManagementException) // 捕获WMI超时信号
                    {
                        if (Debugger.IsAttached) // 调试模式下，输出超时信息及相关数据
                        {
                            Debug.Print("{0}ManagementException happened\n+ Targate: {1}",
                                $"[{DateTime.Now:HH:mm:ss}]",
                                eventWatcher.Query.QueryString);
                        }
                        if (needstop) // 判断本次超时后需要继续循环/退出循环
                        {
                            stopdelay.Set();
                            break;
                        }
                        else continue;
                    }
                    if (!Thread.CurrentThread.IsAlive) break;
                    if (Debugger.IsAttached) // 调试模式下，输出WMI查询到的相关信息
                    {
                        Debug.Print("{0}Process has been created\n+ Name: {1}\n+ Path: {2}",
                            $"[{DateTime.Now:HH:mm:ss}]",
                            ((ManagementBaseObject)e["TargetInstance"])["Name"],
                            ((ManagementBaseObject)e["TargetInstance"])["ExecutablePath"]);
                    }
                    ManagementBaseObject instence = (ManagementBaseObject)e["TargetInstance"];
                    if (!int.TryParse(instence["ProcessId"].ToString(), out int pid) || pid == 0)
                    {
                        continue;
                    }
                    Process game = Process.GetProcessById(pid);
                    working = new Thread(() => // 执行游戏进程处理相关操作
                    {
                        AutoResetEvent timer = new AutoResetEvent(false);
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
                        _void(game, args);
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
                } while (Thread.CurrentThread.IsAlive);
            })
            { IsBackground = true };
            wmi_thread.Start();
        }

        /// <summary>
        /// 指示WMI组件是否需要退出查询循环
        /// </summary>
        private bool needstop = false;

        /// <summary>
        /// 用于通知主线程WMI组件已退出查询循环的同步组件
        /// </summary>
        private readonly AutoResetEvent stopdelay = new AutoResetEvent(false);

        /// <summary>
        /// 关闭WMI事件
        /// </summary>
        internal void EndWMI()
        {
            if (eventWatcher == null) return;
            if (working != null && working.IsAlive)
            {
                working.Abort();
            }
            needstop = true;
            stopdelay.WaitOne(TimeSpan.FromSeconds(5)); // 阻塞直到WMI组件经历一次超时并自动退出查询循环
            AutoResetEvent blocker = new AutoResetEvent(false);
            bool blockover = false;
            for (int i = 0; i < 5; i++) // 预留500ms等待进程自行结束
            {
                if (wmi_thread != null && wmi_thread.IsAlive)
                {
                    blocker.WaitOne(100);
                    continue;
                };
                blockover = true;
            }
            if (!blockover)
            {
                throw new WMIhelper_Exception("WMI组件无法正常卸载，指示代码块中可能存在阻塞，程序源代码中可能存在严重错误。");
            }
            eventWatcher.Stop();
            eventWatcher.Dispose();
            eventWatcher = null;
        }

        /// <summary>
        /// 表示程序运行中出现了[级别·严重]等级的异常
        /// </summary>
        internal class WMIhelper_Exception : Win32Exception
        {
            internal WMIhelper_Exception() : base() { } 
            internal WMIhelper_Exception(int error) : base(error) { }
            internal WMIhelper_Exception(int error, string message) : base(error, message) { }
            internal WMIhelper_Exception(string message) : base(message) { }
            internal WMIhelper_Exception(string message, Exception innerException) : base(message, innerException) { }
            internal WMIhelper_Exception(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
    }
}
