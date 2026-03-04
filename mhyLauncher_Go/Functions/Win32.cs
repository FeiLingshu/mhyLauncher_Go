using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MHYLAUNCHER_GO.Functions.Win32;
using Application = System.Windows.Application;

namespace MHYLAUNCHER_GO.Functions
{
    /// <summary>
    /// 用于支持进行Win32函数调用的公开类
    /// </summary>
    public class Win32
    {
        #region Win32互操作声明

        /// <summary>
        /// 获取窗口矩形
        /// </summary>
        /// <param name="hWnd">目标窗口句柄</param>
        /// <param name="lpRect">out - 目标窗口的窗口矩形</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// 获取客户矩形
        /// </summary>
        /// <param name="hWnd">目标窗口句柄</param>
        /// <param name="lpRect">out - 目标窗口的客户矩形</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// 将客户区坐标转换为屏幕坐标
        /// </summary>
        /// <param name="hWnd">目标窗口句柄</param>
        /// <param name="lpPoint">要转换的客户区坐标(标准值为0,0)</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        /// <summary>
        /// 阻止生成 WM_SYNCPAINT 消息
        /// </summary>
        public const int SWP_DEFERERASE = 0x2000;
        /// <summary>
        /// 丢弃工作区的整个内容
        /// </summary>
        public const int SWP_NOCOPYBITS = 0x0100;
        /// <summary>
        /// 指示不改变窗口Z序
        /// </summary>
        public const int SWP_NOZORDER = 0x0004;
        /// <summary>
        /// 指示不改变窗口位置
        /// </summary>
        public const int SWP_NOMOVE = 0x0002;
        /// <summary>
        /// 指示不改变窗口大小
        /// </summary>
        public const int SWP_NOSIZE = 0x0001;
        /// <summary>
        /// 指示不激活窗口
        /// </summary>
        public const int SWP_NOACTIVATE = 0x0010;
        /// <summary>
        /// 指示不进行重绘
        /// </summary>
        public const int SWP_NOREDRAW = 0x0008;
        /// <summary>
        /// 阻止窗口接收WM_WINDOWPOSCHANGING消息
        /// </summary>
        public const int SWP_NOSENDCHANGING = 0x0400;
        /// <summary>
        /// 指示进行异步操作
        /// </summary>
        public const int SWP_ASYNCWINDOWPOS = 0x4000;
        /// <summary>
        /// 指示重新计算窗口框架
        /// </summary>
        public const int SWP_FRAMECHANGED = 0x0020;
        /// <summary>
        /// 指示窗口需要设置为显示状态
        /// </summary>
        public const uint SWP_SHOWWINDOW = 0x0040;

        /// <summary>
        /// 调整窗口空间信息
        /// </summary>
        /// <param name="hwnd">目标窗口坐标</param>
        /// <param name="hWndInsertAfter">指示窗口Z序如何变化</param>
        /// <param name="x">窗口左上角横坐标</param>
        /// <param name="y">窗口左上角纵坐标</param>
        /// <param name="cx">窗口宽度</param>
        /// <param name="cy">窗口高度</param>
        /// <param name="wFlags">窗口空间信息修改规则</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        /// <summary>
        /// 声明win32结构RECT
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// 距左边界距离
            /// </summary>
            public int left;
            /// <summary>
            /// 距上边界距离
            /// </summary>
            public int top;
            /// <summary>
            /// 距右边界距离
            /// </summary>
            public int right;
            /// <summary>
            /// 距下边界距离
            /// </summary>
            public int bottom;

            /// <summary>
            /// 实现将win32结构RECT转换为C#结构Rectangle
            /// </summary>
            /// <returns></returns>
            public Rectangle ToRectangle()
            {
                return new Rectangle(left, top, right - left, bottom - top);
            }
        }

        /// <summary>
        /// 需要设置窗口的标题文本
        /// </summary>
        public const int WM_SETTEXT = 0x000C;
        /// <summary>
        /// 需要设置窗口的大小
        /// </summary>
        public const int WM_SIZE = 0x0005;
        /// <summary>
        /// 需要窗口客户区进行重绘
        /// </summary>
        public const int WM_PAINT = 0x000F;

        /// <summary>
        /// 向指定窗口发送指定的Win32消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="Msg">消息常量</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        /// <returns>返回消息的处理结果</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// 向指定窗口发送指定的Win32消息(仅用于发送字符串消息)
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="Msg">消息常量</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数(仅传递字符串对象)</param>
        /// <returns>返回消息的处理结果</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        /// <summary>
        /// 获取窗口标题字符串长度
        /// </summary>
        /// <param name="hwnd">目标窗口句柄</param>
        /// <returns>返回窗口标题字符串的长度</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hwnd);

        /// <summary>
        /// 获取窗口标题字符串
        /// </summary>
        /// <param name="hwnd">目标窗口句柄</param>
        /// <param name="lpString">存储字符串的对象</param>
        /// <param name="nMaxCount">获取字符的最大长度</param>
        /// <returns>返回获取到的字符串长度</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// 获取窗口菜单句柄
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="bRevert">是否恢复到保存的窗口菜单副本</param>
        /// <returns>窗口菜单句柄</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// 表示窗口移动菜单项
        /// </summary>
        public const int SC_MOVE = 0xF010;
        /// <summary>
        /// 表示窗口调整大小菜单项
        /// </summary>
        public const int SC_SIZE = 0xF000;
        /// <summary>
        /// 表示窗口关闭菜单项
        /// </summary>
        public const int SC_CLOSE = 0xF060;
        /// <summary>
        /// 表示控制台窗口菜单项#1
        /// </summary>
        public const int SC_DOSA = 0xFFF8;
        /// <summary>
        /// 表示控制台窗口菜单项#2
        /// </summary>
        public const int SC_DOSB = 0xFFF7;
        /// <summary>
        /// 表示通过命令常量查找菜单项
        /// </summary>
        public const int MF_BYCOMMAND = 0;

        /// <summary>
        /// 移除窗口菜单的菜单项
        /// </summary>
        /// <param name="hMenu">窗口菜单句柄</param>
        /// <param name="nPos">窗口菜单项的索引/常量</param>
        /// <param name="flags">指示查找菜单项的方式</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RemoveMenu(IntPtr hMenu, int nPos, int flags);

        /// <summary>
        /// 获取当前正在前台显示的窗口句柄
        /// </summary>
        /// <returns>返回当前正在前台显示的窗口句柄</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 表示窗口样式
        /// </summary>
        public const int GWL_STYLE = -16;
        /// <summary>
        /// 表示窗口具有最大化窗口
        /// </summary>
        public const long WS_MAXIMIZEBOX = 0x00010000L;
        /// <summary>
        /// 表示窗口具有标题栏
        /// </summary>
        public const long WS_CAPTION = 0x00C00000L;
        /// <summary>
        /// 表示在父窗口内进行绘制时不包括子窗口区域
        /// </summary>
        public const long WS_CLIPCHILDREN = 0x02000000L;
        /// <summary>
        /// 表示允许子窗口间的工作区绘制相互重叠
        /// </summary>
        public const long WS_CLIPSIBLINGS = 0x04000000L;
        /// <summary>
        /// 表示窗口扩展样式
        /// </summary>
        public const int GWL_EXSTYLE = -20;
        /// <summary>
        /// 表示窗口为分层窗口
        /// </summary>
        public const long WS_EX_LAYERED = 0x00080000L;
        /// <summary>
        /// 表示窗口支持鼠标穿透
        /// </summary>
        public const long WS_EX_TRANSPARENT = 0x00000020L;
        /// <summary>
        /// 表示窗口不会获取焦点
        /// </summary>
        public const long WS_EX_NOACTIVATE = 0x08000000L;
        /// <summary>
        /// 表示使用双缓冲从下至上绘制窗口的所有后代
        /// </summary>
        public const long WS_EX_COMPOSITED = 0x02000000L;
        /// <summary>
        /// 表示窗口将显示在任务栏上
        /// </summary>
        public const long WS_EX_APPWINDOW = 0x00040000L;

        /// <summary>
        /// 获取窗口属性
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="nIndex">属性类型索引</param>
        /// <returns>返回当前窗口属性</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern long GetWindowLongPtr(IntPtr hWnd, long nIndex);

        /// <summary>
        /// 表示窗口所有者属性
        /// </summary>
        public const int GWL_HWNDPARENT = -8;

        /// <summary>
        /// 设置窗口属性
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="nIndex">属性类型索引</param>
        /// <param name="dwNewLong">新的窗口属性</param>
        /// <returns>返回之前的窗口属性</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern long SetWindowLongPtr(IntPtr hWnd, int nIndex, long dwNewLong);

        /// <summary>
        /// 实现通过进程句柄获取应用程序或模块的路径的WindowsAPI
        /// </summary>
        /// <param name="hProcess">目标进程句柄</param>
        /// <param name="hModule">目标模块句柄</param>
        /// <param name="lpFilename">承载路径信息的StringBuilder实例</param>
        /// <param name="nSize">StringBuilder实例的缓冲区大小</param>
        /// <returns>返回复制到缓冲区的字符串的长度</returns>
        [DllImport("Psapi.dll", EntryPoint = "GetModuleFileNameEx", SetLastError = true)]
        public static extern uint GetModuleFileNameEx(int hProcess, IntPtr hModule, [Out] StringBuilder lpFilename, uint nSize);

        /// <summary>
        /// 用于将输入线程附加到其他线程中
        /// </summary>
        /// <param name="idAttach">当前输入线程</param>
        /// <param name="idAttachTo">要附加到的目标线程</param>
        /// <param name="fAttach">指示操作是附加还是解除</param>
        /// <returns>指示操作是否成功</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        /// <summary>
        /// 用于将指定窗口设置为前台窗口的WindowsAPI
        /// </summary>
        /// <param name="hWnd">要设置为前台窗口的句柄</param>
        /// <returns>指示操作是否成功</returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 用于将窗口的父级设置为指定窗口的WindowsAPI
        /// </summary>
        /// <param name="hWndChild">子窗口句柄</param>
        /// <param name="hWndNewParent">父窗口句柄</param>
        /// <returns>子窗口的原父窗口的句柄，若函数出现错误或子窗口原为顶级窗口则返回IntPtr.Zero</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// 遍历某窗口的子窗口
        /// </summary>
        /// <param name="hWndParent">要遍历的目标窗口</param>
        /// <param name="lpEnumFunc">委托回调</param>
        /// <param name="lParam">回调参数</param>
        /// <returns>返回执行是否成功</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// 遍历子窗口的委托回调
        /// </summary>
        /// <param name="hWnd">子窗口句柄</param>
        /// <param name="lParam">回调参数</param>
        /// <returns></returns>
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// 获取窗口类名
        /// </summary>
        /// <param name="hWnd">目标窗口的句柄</param>
        /// <param name="lpClassName">用于承载类名字符串的StringBuilder实例</param>
        /// <param name="nMaxCount">可承载的最大字符串长度</param>
        /// <returns>返回执行是否成功</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// 获取窗口的最小化状态
        /// </summary>
        /// <param name="hWnd">目标窗口句柄</param>
        /// <returns>返回目标是否处于最小化状态</returns>
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        /// <summary>
        /// 判断窗口是否存在
        /// </summary>
        /// <param name="hWnd">目标窗口句柄</param>
        /// <returns>返回目标窗口是否存在</returns>
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// 获取当前线程ID
        /// </summary>
        /// <returns>返回当前线程ID</returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        /// <summary>
        /// 表示窗口横向边框宽度
        /// </summary>
        public const int SM_CXBORDER = 5;
        /// <summary>
        /// 表示窗口纵向边框宽度
        /// </summary>
        public const int SM_CYBORDER = 6;
        /// <summary>
        /// 表示窗口横向基础框架宽度
        /// </summary>
        public const int SM_CXEDGE = 45;
        /// <summary>
        /// 表示窗口纵向基础框架宽度
        /// </summary>
        public const int SM_CYEDGE = 46;
        /// <summary>
        /// 表示窗口横向大小调整框架宽度
        /// </summary>
        public const int SM_CXFRAME = 32;
        /// <summary>
        /// 表示窗口纵向大小调整框架宽度
        /// </summary>
        public const int SM_CYFRAME = 33;

        /// <summary>
        /// 获取系统默认矩阵数据
        /// </summary>
        /// <param name="nIndex">目标矩阵的枚举值</param>
        /// <returns>返回目标矩阵的对应数值</returns>
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        /// <summary>
        /// 获取窗口是否可见
        /// </summary>
        /// <param name="hWnd">目标窗口句柄</param>
        /// <returns>返回窗口是否可见</returns>
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// 获取窗口所处的进程和线程ID
        /// </summary>
        /// <param name="hWnd">目标窗口句柄</param>
        /// <param name="lpdwProcessId">窗口进程ID</param>
        /// <returns>返回窗口线程ID</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// 向指定窗口发送指定的Win32消息
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="Msg">消息常量</param>
        /// <param name="wParam">消息参数</param>
        /// <param name="lParam">消息参数</param>
        /// <returns>返回消息的处理结果</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 指示窗口隐藏
        /// </summary>
        public const int SW_HIDE = 0;
        /// <summary>
        /// 指示窗口显示但不获取焦点
        /// </summary>
        public const int SW_SHOWNOACTIVATE = 4;
        /// <summary>
        /// 指示窗口显示
        /// </summary>
        public const int SW_SHOW = 4;
        /// <summary>
        /// 指示窗口最小化
        /// </summary>
        public const int SW_MINIMIZE = 6;

        /// <summary>
        /// 调整窗口显示状态
        /// </summary>
        /// <param name="hWnd">目标窗口句柄</param>
        /// <param name="nCmdShow">系统命令</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #endregion

        #region 全局HOOK挂钩互操作声明

        /// <summary>
        /// 声明win32结构KeyBoardHookStruct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class KeyBoardHookStruct
        {
            /// <summary>
            /// 触发挂钩过程的键盘按键的键值
            /// </summary>
            public int vkCode;
            /// <summary>
            /// 挂钩过程的扫描代码(unchecked)
            /// </summary>
            public int scanCode;
            /// <summary>
            /// 挂钩过程的位标志(unchecked)
            /// </summary>
            public int flags;
            /// <summary>
            /// 挂钩过程触发的时间(unchecked)
            /// </summary>
            public int time;
            /// <summary>
            /// 挂钩过程包含的状态详细信息(unchecked)
            /// </summary>
            public int dwExtraInfo;
        }

        /// <summary>
        /// 键盘按键抬起事件常量
        /// </summary>
        public const int WM_KEYUP = 0x0101;
        /// <summary>
        /// 表示键盘F12按键
        /// </summary>
        public const int VK_F12 = 0x7B;
        /// <summary>
        /// 表示键盘F11按键
        /// </summary>
        public const int VK_F11 = 0x7A;

        /// <summary>
        /// 用于执行挂钩过程的委托函数
        /// </summary>
        /// <param name="nCode">通知下个挂钩过程如何处理挂钩信息</param>
        /// <param name="wParam">主要挂钩数据</param>
        /// <param name="lParam">挂钩事件参数的相关标志信息的位组合数据</param>
        /// <returns>返回当前挂钩处理结果</returns>
        public delegate int HOOKPROC(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// 表示用于监视低级别键盘输入事件的挂钩过程
        /// </summary>
        public const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// 安装全局HOOK挂钩
        /// <para>【对于低级别全局HOOK挂钩，显式设置dwThreadId参数将会引发Win32Error(1429)_只能全局设置该挂接过程】</para>
        /// </summary>
        /// <param name="idHook">要安装的挂钩过程的类型</param>
        /// <param name="lpfn">指向挂钩过程的指针</param>
        /// <param name="hmod">指向挂钩过程的dll的句柄，若挂接线程由当前进程创建，并且挂钩过程位于与当前进程关联的代码中，则必须为null</param>
        /// <param name="dwThreadId">要挂接的线程的线程标识符</param>
        /// <returns>返回挂接的挂钩句柄</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HOOKPROC lpfn, IntPtr hmod, int dwThreadId);

        /// <summary>
        /// 传递挂钩数据并执行下一个挂钩函数
        /// </summary>
        /// <param name="idHook">此参数被忽略</param>
        /// <param name="nCode">确认如何处理挂钩信息</param>
        /// <param name="wParam">主要挂钩数据</param>
        /// <param name="lParam">挂钩事件参数的相关标志信息的位组合数据</param>
        /// <returns>返回值由链中的下一个挂钩过程返回</returns>
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// 卸载全局HOOK挂钩
        /// </summary>
        /// <param name="idHook">要卸载的挂钩的句柄</param>
        /// <returns>返回操作是否成功执行</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        #endregion

        #region Win32事件挂钩互操作声明

        /// <summary>
        /// 配置Win32事件挂钩
        /// </summary>
        /// <param name="eventMin">最小事件常量</param>
        /// <param name="eventMax">最大事件常量</param>
        /// <param name="hmodWinEventProc">挂钩函数所在DLL的句柄</param>
        /// <param name="lpfnWinEventProc">指向挂钩函数的指针</param>
        /// <param name="idProcess">产生事件的目标进程</param>
        /// <param name="idThread">产生事件的目标线程</param>
        /// <param name="dwFlags">指定要跳过的挂钩函数和事件的位置</param>
        /// <returns>返回Win32事件挂钩实例</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(
            uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess,
            uint idThread, uint dwFlags);

        /// <summary>
        /// 卸载Win32事件挂钩
        /// </summary>
        /// <param name="hWinEventHook">目标Win32事件挂钩实例</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        /// <summary>
        /// 表示对象位置/大小发生变化的事件
        /// </summary>
        public const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
        /// <summary>
        /// 表示对象名称发生变化的事件
        /// </summary>
        public const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        /// <summary>
        /// 表示窗口的移动或调整大小已完成
        /// </summary>
        public const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;
        /// <summary>
        /// 表示窗口即将最小化
        /// </summary>
        public const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;
        /// <summary>
        /// 表示回调函数不会映射到生成事件的进程的地址空间中
        /// </summary>
        public const uint WINEVENT_OUTOFCONTEXT = 0x0000;
        /// <summary>
        /// 表示目标元素类型为窗口
        /// </summary>
        public const uint OBJID_WINDOW = 0x0000;
        /// <summary>
        /// 表示目标元素为自身
        /// </summary>
        public const uint CHILDID_SELF = 0;

        /// <summary>
        /// Win32事件挂钩的委托类型
        /// </summary>
        /// <param name="hWinEventHook">事件挂钩函数的句柄</param>
        /// <param name="eventType">发生的事件</param>
        /// <param name="hwnd">生成事件的窗口的句柄</param>
        /// <param name="idObject">与事件关联的对象</param>
        /// <param name="idChild">事件触发者</param>
        /// <param name="dwEventThread">指定生成事件的时间</param>
        /// <param name="dwmsEventTime">指定生成事件的时间</param>
        public delegate void WinEventDelegate(
            IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
            int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        /// <summary>
        /// 表示窗口样式正在被修改
        /// </summary>
        public const int WM_STYLECHANGING = 0x007C;

        /// <summary>
        /// 声明Win32结构STYLESTRUCT
        /// </summary>
        public struct STYLESTRUCT
        {
            public uint styleOld;
            public uint styleNew;
        }

        #endregion

        #region INI文件互操作声明（已弃用）

        /// <summary>
        /// 实现读取INI配置文件的WindowsAPI
        /// </summary>
        /// <param name="section">要检索的节</param>
        /// <param name="key">要检索的键</param>
        /// <param name="defaultValue">检索结果的默认值</param>
        /// <param name="retVal">用于承载返回结果的变量</param>
        /// <param name="size">用于承载返回结果的变量的最大容量</param>
        /// <param name="filePath">INI文件路径</param>
        /// <returns>返回检索结果的字节大小</returns>
        [DllImport("kernel32.dll")]
        public static extern long GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 实现写入INI配置文件的WindowsAPI
        /// </summary>
        /// <param name="section">要检索的节</param>
        /// <param name="key">要检索的键</param>
        /// <param name="value">要写入指定位置的值</param>
        /// <param name="filePath">INI文件路径</param>
        /// <returns>指示操作是否成功，非零表示成功，零表示失败</returns>
        [DllImport("kernel32.dll")]
        public static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        #endregion

        #region 控制台相关互操作声明

        /// <summary>
        /// 获取控制台窗口句柄
        /// </summary>
        /// <returns>返回当前进程绑定的控制台窗口句柄</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// 为当前进程绑定新的控制台窗口
        /// </summary>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AllocConsole();

        /// <summary>
        /// 指示绑定目标为当前进程
        /// </summary>
        public const uint ATTACH_PARENT_PROCESS = 0xFFFFFFFF;

        /// <summary>
        /// 将现有的控制台窗口绑定到当前进程
        /// </summary>
        /// <param name="dwDesiredAccess">指示绑定目标</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(uint dwDesiredAccess);

        /// <summary>
        /// 释放(解除绑定)当前进程的控制台窗口
        /// </summary>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeConsole();

        /// <summary>
        /// 进程权限枚举
        /// </summary>
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            PROCESS_VM_READ = 0x00000010,
            PROCESS_QUERY_INFORMATION = 0x00000400
        }

        /// <summary>
        /// 获取进程句柄
        /// </summary>
        /// <param name="processAccess">进程权限</param>
        /// <param name="bInheritHandle">指示子进程是否继承句柄</param>
        /// <param name="processId">进程ID</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            ProcessAccessFlags processAccess,
            bool bInheritHandle,
            int processId);

        /// <summary>
        /// 关闭句柄
        /// </summary>
        /// <param name="hObject">目标句柄</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// 获取指定进程的父进程
        /// <para>该函数为内核函数 &lt;- [NtQueryInformationProcess 在 Windows 的未来版本中可能已更改或不可用。 应用程序应使用本主题中列出的备用函数。]</para>
        /// <a href="https://learn.microsoft.com/zh-cn/windows/win32/api/winternl/nf-winternl-ntqueryinformationprocess">MSDN页面</a>
        /// </summary>
        /// <param name="processHandle">目标进程的句柄</param>
        /// <param name="processInformationClass">要检索的进程信息的类型</param>
        /// <param name="processInformation">缓冲区指针</param>
        /// <param name="processInformationLength">缓冲区大小</param>
        /// <param name="returnLength">函数返回所请求信息的大小(指针)</param>
        /// <returns>返回NTSTATUS成功或错误代码</returns>
        [DllImport("ntdll.dll")]
        public static extern int NtQueryInformationProcess(
            IntPtr processHandle,
            int processInformationClass,
            ref PROCESS_BASIC_INFORMATION processInformation,
            uint processInformationLength,
            out uint returnLength);

        /// <summary>
        /// 声明win32结构PROCESS_BASIC_INFORMATION
        /// </summary>
        public struct PROCESS_BASIC_INFORMATION
        {
            /// <summary>
            /// 进程退出代码(ExitStatus)
            /// <para>为了清晰和安全起见，最好使用 GetExitCodeProcess</para>
            /// </summary>
            public IntPtr Reserved1;
            /// <summary>
            /// 指向PEB结构
            /// </summary>
            public IntPtr PebBaseAddress;
            /// <summary>
            /// _(AffinityMask)
            /// <para>可以强制转换为DWORD，并且包含GetProcessAffinityMask为lpProcessAffinityMask参数返回的相同值</para>
            /// </summary>
            public IntPtr Reserved2_0;
            /// <summary>
            /// 进程优先级(BasePriority)
            /// </summary>
            public IntPtr Reserved2_1;
            /// <summary>
            /// 查询过程的唯一标识符
            /// </summary>
            public IntPtr UniqueProcessId;
            /// <summary>
            /// 父进程的唯一标识符
            /// </summary>
            public IntPtr InheritedFromUniqueProcessId;
        }

        /// <summary>
        /// 指示标准输出流
        /// </summary>
        public const int STD_INPUT_HANDLE = -10;
        /// <summary>
        /// 指示标准输出流
        /// </summary>
        public const int STD_OUTPUT_HANDLE = -11;
        /// <summary>
        /// 指示无效句柄
        /// </summary>
        public static readonly IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);

        /// <summary>
        /// 获取当前进程指定标准设备的句柄
        /// </summary>
        /// <param name="nStdHandle">标准设备的类型</param>
        /// <returns>返回当前进程指定标准设备的句柄</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        /// <summary>
        /// 配置控制台代码页
        /// </summary>
        /// <param name="wCodePageID">代码页ID</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleOutputCP(uint wCodePageID);

        /// <summary>
        /// 声明win32结构CONSOLE_FONT_INFO_EX
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            /// <summary>
            /// 结构大小
            /// </summary>
            public int cbSize;
            /// <summary>
            /// 系统控制台字体表中字体的索引
            /// </summary>
            public uint nFont;
            /// <summary>
            /// 字符宽度和高度信息
            /// </summary>
            public COORD dwFontSize;
            /// <summary>
            /// 字体间距和家族
            /// </summary>
            public int FontFamily;
            /// <summary>
            /// 字体粗细
            /// </summary>
            public int FontWeight;
            /// <summary>
            /// 字体名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }

        /// <summary>
        /// 设置控制台字体
        /// </summary>
        /// <param name="hConsoleOutput">控制台标准输出流句柄</param>
        /// <param name="bMaximumWindow">?是否设置最大窗口大小的字体信息</param>
        /// <param name="lpConsoleCurrentFont">包含字体信息的win32结构lpConsoleCurrentFont</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);

        /// <summary>
        /// 声明win32结构COORD
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            /// <summary>
            /// X维度的值(视调用方不同存在不同含义)
            /// </summary>
            public short X;
            /// <summary>
            /// Y维度的值(视调用方不同存在不同含义)
            /// </summary>
            public short Y;
        }

        /// <summary>
        /// 声明win32结构SMALL_RECT
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            /// <summary>
            /// 左边距
            /// </summary>
            public short Left;
            /// <summary>
            /// 上边距
            /// </summary>
            public short Top;
            /// <summary>
            /// 右边距
            /// </summary>
            public short Right;
            /// <summary>
            /// 下边距
            /// </summary>
            public short Bottom;
        }

        /// <summary>
        /// 设置控制台缓冲区大小
        /// </summary>
        /// <param name="hConsoleOutput">目标控制台的标准输出流句柄</param>
        /// <param name="dwSize">大小信息</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleScreenBufferSize(
            IntPtr hConsoleOutput, COORD dwSize);

        /// <summary>
        /// 设置控制台显示区域大小
        /// </summary>
        /// <param name="hConsoleOutput">目标控制台的标准输出流句柄</param>
        /// <param name="bAbsolute">指示使用控制台参考系(true)还是屏幕参考系(false)</param>
        /// <param name="lpConsoleWindow">大小信息</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleWindowInfo(
            IntPtr hConsoleOutput, bool bAbsolute, ref SMALL_RECT lpConsoleWindow);

        /// <summary>
        /// 指示 Ctrl+C 由系统处理，且不会放入输入缓冲区中
        /// </summary>
        public const uint ENABLE_PROCESSED_INPUT = 0x0001;
        /// <summary>
        /// 指示用户可通过此标志使用鼠标选择和编辑文本
        /// </summary>
        public const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        /// <summary>
        /// 指示允许控制台处理 ASCII 控制序列
        /// </summary>
        public const uint ENABLE_PROCESSED_OUTPUT = 0x0001;
        /// <summary>
        /// 指示启用虚拟终端的ANSI转义序列支持
        /// </summary>
        public const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        /// <summary>
        /// 获取控制台模式
        /// </summary>
        /// <param name="hConsoleHandle">控制台标准设备句柄</param>
        /// <param name="dwMode">当前控制台的模式数据</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint dwMode);

        /// <summary>
        /// 设置控制台模式
        /// </summary>
        /// <param name="hConsoleHandle">控制台标准设备句柄</param>
        /// <param name="dwMode">要设置的控制台的模式数据</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        /// <summary>
        /// 表示Ctrl+C组合键
        /// </summary>
        public const int CTRL_C_EVENT = 0;
        /// <summary>
        /// 表示Ctrl+Break(Pause)组合键
        /// </summary>
        public const int CTRL_BREAK_EVENT = 1;

        /// <summary>
        /// 声明win32委托ConsoleCtrlDelegate
        /// </summary>
        /// <param name="ctrlType">控制台按键触发类型</param>
        /// <returns>返回操作是否由处理程序处理</returns>
        public delegate bool ConsoleCtrlDelegate(int ctrlType);

        /// <summary>
        /// 添加控制台事件处理函数
        /// </summary>
        /// <param name="handler">win32委托ConsoleCtrlDelegate实例</param>
        /// <param name="add">指示操作是否为添加(否则移除对应的处理函数)</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handler, bool add);

        /// <summary>
        /// 声明win32结构CONSOLE_SCREEN_BUFFER_INFOEX
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_SCREEN_BUFFER_INFOEX
        {
            /// <summary>
            /// 结构在内存中的大小
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// 控制台缓冲区大小
            /// </summary>
            public COORD dwSize;
            /// <summary>
            /// 控制台光标在缓冲区中的坐标
            /// </summary>
            public COORD dwCursorPosition;
            /// <summary>
            /// 控制台缓冲区的字符属性
            /// </summary>
            public ushort wAttributes;
            /// <summary>
            /// 控制台的显示范围
            /// </summary>
            public SMALL_RECT srWindow;
            /// <summary>
            /// 控制台窗口的最大大小
            /// </summary>
            public COORD dwMaximumWindowSize;
            /// <summary>
            /// 控制台弹出窗口的填充属性
            /// </summary>
            public ushort wPopupAttributes;
            /// <summary>
            /// 指示是否支持全屏模式
            /// </summary>
            public bool bFullscreenSupported;
            /// <summary>
            /// 控制台16色颜色列表
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] ColorTable;
        }

        /// <summary>
        /// 获取控制台标准设备的屏幕缓冲区信息
        /// </summary>
        /// <param name="hConsoleOutput">控制台标准设备句柄</param>
        /// <param name="lpConsoleScreenBufferInfoEx">win32结构CONSOLE_SCREEN_BUFFER_INFOEX实例</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFOEX lpConsoleScreenBufferInfoEx);

        /// <summary>
        /// 设置控制台标准设备的屏幕缓冲区信息
        /// </summary>
        /// <param name="hConsoleOutput">控制台标准设备句柄</param>
        /// <param name="lpConsoleScreenBufferInfoEx">win32结构CONSOLE_SCREEN_BUFFER_INFOEX实例</param>
        /// <returns>返回操作是否成功</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFOEX lpConsoleScreenBufferInfoEx);

        #endregion

        #region DWM互操作声明

        /// <summary>
        /// 获取DWM是否启用
        /// </summary>
        /// <returns>返回当前设备中DWM的启用情况</returns>
        [DllImport("Dwmapi.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmIsCompositionEnabled();

        /// <summary>
        /// 声明win32枚举DWMWINDOWATTRIBUTE
        /// </summary>
        public enum DWMWINDOWATTRIBUTE : uint
        {
            /// <summary>
            /// 获取当前DWM状态
            /// </summary>
            DWMWA_NCRENDERING_ENABLED = 1,
            /// <summary>
            /// 配置DWM状态
            /// </summary>
            DWMWA_NCRENDERING_POLICY = 2,
            /// <summary>
            /// 配置是否允许渲染工作区
            /// </summary>
            DWMWA_ALLOW_NCPAINT = 3,
            /// <summary>
            /// 配置是否同步系统暗色模式配置(17763~18985)
            /// </summary>
            DWMWA_USE_IMMERSIVE_DARK_MODE_20H1 = 19,
            /// <summary>
            /// 配置是否同步系统暗色模式配置
            /// </summary>
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            /// <summary>
            /// 配置窗口圆角参数
            /// </summary>
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
            /// <summary>
            /// 配置窗口标题栏颜色
            /// </summary>
            DWMWA_CAPTION_COLOR = 35,
            /// <summary>
            /// 配置窗口标题文本颜色
            /// </summary>
            DWMWA_TEXT_COLOR = 36
        }

        /// <summary>
        /// 声明win32枚举DWMNCRENDERINGPOLICY
        /// </summary>
        public enum DWMNCRENDERINGPOLICY : uint
        {
            /// <summary>
            /// 使用系统DWM配置
            /// </summary>
            DWMNCRP_USEWINDOWSTYLE = 0,
            /// <summary>
            /// 禁用DWM
            /// </summary>
            DWMNCRP_DISABLED = 1,
            /// <summary>
            /// 启用DWM
            /// </summary>
            DWMNCRP_ENABLED = 2
        }

        /// <summary>
        /// 声明win32枚举DWM_WINDOW_CORNER_PREFERENCE
        /// </summary>
        public enum DWM_WINDOW_CORNER_PREFERENCE : uint
        {
            /// <summary>
            /// 默认圆角风格
            /// </summary>
            DWMWCP_DEFAULT = 0,
            /// <summary>
            /// 无圆角
            /// </summary>
            DWMWCP_SQUARE = 1,
            /// <summary>
            /// 标准圆角
            /// </summary>
            DWMWCP_ROUND = 2,
            /// <summary>
            /// 较小圆角
            /// </summary>
            DWMWCP_ROUNDSMALL = 3
        }

        /// <summary>
        /// 声明win32结构MARGINS
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            /// <summary>
            /// 左边距
            /// </summary>
            public int Left;
            /// <summary>
            /// 右边距
            /// </summary>
            public int Right;
            /// <summary>
            /// 上边距
            /// </summary>
            public int Top;
            /// <summary>
            /// 下边距
            /// </summary>
            public int Bottom;
        }

        /// <summary>
        /// 获取/设置指定窗口的DWM配置
        /// </summary>
        /// <param name="hwnd">目标窗口句柄</param>
        /// <param name="dwAttribute">要获取/设置的DWM属性枚举</param>
        /// <param name="pvAttribute">承载DWM属性值的特定数据类型实例</param>
        /// <param name="cbAttribute">属性值的内存大小</param>
        /// <returns>返回操作是否成功(S_OK为成功，否则失败)</returns>
        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute", PreserveSig = true)]
        public static extern int DwmGetWindowAttribute(
            IntPtr hwnd,
            DWMWINDOWATTRIBUTE dwAttribute,
            ref bool pvAttribute,
            uint cbAttribute);

        /// <summary>
        /// 获取/设置指定窗口的DWM配置
        /// </summary>
        /// <param name="hwnd">目标窗口句柄</param>
        /// <param name="dwAttribute">要获取/设置的DWM属性枚举</param>
        /// <param name="pvAttribute">承载DWM属性值的特定数据类型实例</param>
        /// <param name="cbAttribute">属性值的内存大小</param>
        /// <returns>返回操作是否成功(S_OK为成功，否则失败)</returns>
        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            DWMWINDOWATTRIBUTE dwAttribute,
            ref bool pvAttribute,
            uint cbAttribute);

        /// <summary>
        /// 获取/设置指定窗口的DWM配置
        /// </summary>
        /// <param name="hwnd">目标窗口句柄</param>
        /// <param name="dwAttribute">要获取/设置的DWM属性枚举</param>
        /// <param name="pvAttribute">承载DWM属性值的特定数据类型实例</param>
        /// <param name="cbAttribute">属性值的内存大小</param>
        /// <returns>返回操作是否成功(S_OK为成功，否则失败)</returns>
        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            DWMWINDOWATTRIBUTE dwAttribute,
            ref uint pvAttribute,
            uint cbAttribute);

        /// <summary>
        /// 获取/设置指定窗口的DWM配置
        /// </summary>
        /// <param name="hwnd">目标窗口句柄</param>
        /// <param name="dwAttribute">要获取/设置的DWM属性枚举</param>
        /// <param name="pvAttribute">承载DWM属性值的特定数据类型实例</param>
        /// <param name="cbAttribute">属性值的内存大小</param>
        /// <returns>返回操作是否成功(S_OK为成功，否则失败)</returns>
        [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            DWMWINDOWATTRIBUTE dwAttribute,
            ref int pvAttribute,
            uint cbAttribute);

        /// <summary>
        /// 设置窗口工作区包含的窗口边框
        /// </summary>
        /// <param name="hwnd">目标窗口句柄</param>
        /// <param name="margins">指示窗口边框的win32结构MARGINS</param>
        /// <returns>返回操作是否成功(S_OK为成功，否则失败)</returns>
        [DllImport("dwmapi.dll", EntryPoint = "DwmExtendFrameIntoClientArea", PreserveSig = true)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        /// <summary>
        /// 指示DWM函数操作成功的值
        /// </summary>
        public const int S_OK = 0;

        #endregion
    }

    /// <summary>
    /// 用于获取操作系统线程ID的扩展类
    /// </summary>
    public static class ThreadExtensions
    {
        /// <summary>
        /// 获取当前线程ID
        /// <para>必须使用CurrentThread实例，否则将返回int.MaxValue</para>
        /// </summary>
        /// <param name="thread">当前线程实例（非必要参数）</param>
        /// <returns>返回当前进程ID</returns>
        public static int ID(this Thread thread)
        {
            if (thread == Thread.CurrentThread)
            {
                return (int)GetCurrentThreadId();
            }
            else
            {
                return int.MaxValue;
            }
        }
    }

    /// <summary>
    /// 用于根据当前跟踪器情况输出Trace信息的扩展类
    /// </summary>
    public static class TraceExtensions
    {
        /// <summary>
        /// 向跟踪器打印Trace信息
        /// </summary>
        /// <param name="message">Trace信息</param>
        public static void Print(string message)
        {
            if (Debugger.IsAttached)
            {
                Debug.Print(message);
            }
            else
            {
                if (Trace.Listeners.OfType<TraceListenerEx>().Any())
                {
                    Trace.WriteLine(message);
                }
            }
        }

        /// <summary>
        /// 格式化Trace信息
        /// <para>
        /// (char)32   -> ' '<br/>
        /// (char)9472 -> '─'<br/>
        /// (char)9516 -> '┬'<br/>
        /// (char)9500 -> '├'<br/>
        /// (char)9492 -> '└'<br/>
        /// (char)9488 -> '┐'<br/>
        /// (char)9474 -> '│'
        /// </para>
        /// <para>
        /// 可以使用'#'字符开头的行实现多行日志的输出<br/>
        /// 具体效果：├ (非'#'开头的行) -> │ ('#'开头的行)<br/>
        /// 注意：该效果仅限中间行，首行和尾行不能使用该方法
        /// </para>
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="title">标题</param>
        /// <param name="_TCL">标题的控制台字符宽度</param>
        /// <param name="message">信息内容</param>
        /// <returns>返回格式化后的Trace字符串</returns>
        public static string FormatMessage(DateTime time, string title, int _TCL, string[] message)
        {
            string basestd = $"{((char)32).ToString().PadRight(10 + 1 + _TCL, (char)9472)}{(char)9488}\n[{time:HH:mm:ss}] {title}";
            if (message.Length == 1)
            {
                basestd += $"\n{((char)32).ToString().PadRight(10, (char)9472)} {message[0]}";
            }
            else
            {
                for (int i = 0; i < message.Length; i++)
                {
                    if (i == 0)
                    {
                        basestd += $"\n{((char)32).ToString().PadRight(9, (char)9472)}{(char)9516} {message[i]}";
                    }
                    else if (i == message.Length - 1)
                    {
                        basestd += $"\n{string.Empty,-9}{(char)9492} {message[i]}";
                    }
                    else
                    {
                        bool flag = message[i].StartsWith("#");
                        string msg = message[i].Trim('#');
                        basestd += $"\n{string.Empty,-9}{(flag ? (char)9474 : (char)9500)} {msg}";
                    }
                }
            }
            return basestd;
        }

        /// <summary>
        /// 启动Trace组件
        /// </summary>
        /// <param name="mainbase">主应用程序域实例</param>
        /// <exception cref="Win32Exception">发生win32错误</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Set(App mainbase)
        {
            if (!Trace.Listeners.OfType<TraceListenerEx>().Any())
            {
                if (!Win32.AttachConsole(Win32.ATTACH_PARENT_PROCESS) && Win32.GetConsoleWindow() == IntPtr.Zero)
                {
                    if (Win32.AllocConsole())
                    {
                        // 启动Trace组件
                        if (mainbase.TraceInitialize == null)
                        {
                            mainbase.TraceInitialize = Task.Run(() =>
                            {
                                BugFix.RefreshConsoleHandle(false, null); // 必须对原始Console类的私有成员进行访问，具体参阅函数说明
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Trace.Listeners.Add(new TraceListenerEx(Win32.GetConsoleWindow()));
                                    mainbase.TraceInitialize = null;
                                });
                            });
                        }
                    }
                    else
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(),
                            $"未识别到正确的控制台窗口句柄({Marshal.GetLastWin32Error().ToString().PadLeft(4, '0')})。");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 修复.NET框架中Console类问题的公开类
    /// </summary>
    public static class BugFix
    {
        /// <summary>
        /// 存储反射实例的内部字段
        /// </summary>
        private static FieldInfo handleField_out = null;
        /// <summary>
        /// 存储反射实例的内部字段
        /// </summary>
        private static FieldInfo handleField_in = null;

        /// <summary>
        /// 通过反射强制刷新内部ConsoleHandle值
        /// <para>
        /// 1.在通过AllocConsole函数绑定控制台后，必须执行Console.SetOut()/Console.SetIn()/Console.SetError()刷新标准设备句柄<br/>
        /// 2.默认Console类中，_consoleOutputHandle/_consoleInputHandle的值一经读取将会永不变化
        /// (由于其使用属性值承载，属性内部只包含get索引器，且get检测到相关值不为空时会直接输出)<br/>
        /// 3.调用FreeConsole释放控制台并通过AllocConsole重新分配新的控制台时，标准设备句柄已经变化，但默认Console类中根本不存在任何能够实现刷新内部缓存的方法<br/>
        /// 4.综上：只要初始控制台被释放，接下来所有的控制台调用Console类内部方法时都会抛出"无效句柄"异常<br/>
        /// 5.由于相关字段为私有字段，只能使用反射的方法实现手动刷新缓存数据
        /// </para>
        /// <para>注意：执行反射会消耗较长时间，代码应在单独线程中执行</para>
        /// </summary>
        /// <param name="reset_to_zero">是否重置相关字段为null</param>
        /// <param name="parent">调用该方法的窗口实例</param>
        public static void RefreshConsoleHandle(bool reset_to_zero, Form parent)
        {
            // 重新分配新的输入/输出/错误流，重定向错误流至输出流(Trace会将日志输出至错误流中)，并配置字符串编码器
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false)) { AutoFlush = true });
            Console.SetIn(new StreamReader(Console.OpenStandardInput(), new UTF8Encoding(false)));
            Console.SetError(new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false)) { AutoFlush = true });
            // 尝试通过反射修改Console类私有字段
            try
            {
                // 获取Console类的Type对象
                Type consoleType = typeof(Console);
                // 获取私有静态字段
                if (handleField_out == null) handleField_out =
                        consoleType.GetField("_consoleOutputHandle", BindingFlags.Static | BindingFlags.NonPublic);
                if (handleField_in == null) handleField_in =
                        consoleType.GetField("_consoleInputHandle", BindingFlags.Static | BindingFlags.NonPublic);
                // 配置私有静态字段
                if (reset_to_zero)
                {
                    handleField_out?.SetValue(null, null);
                    handleField_in?.SetValue(null, null);
                }
                else
                {
                    IntPtr newHandle_out = Win32.GetStdHandle(Win32.STD_OUTPUT_HANDLE);
                    // IntPtr newHandle_in = Win32.GetStdHandle(Win32.STD_INPUT_HANDLE);
                    handleField_out?.SetValue(null, newHandle_out); // 需要移除JIT编译器的类型安全检查
                    handleField_in?.SetValue(null, null); // 由于控制台窗口为动态绑定，且程序并非控制台应用程序，故不使用标准输入流
                }
            }
            catch (Exception ex)
            {
                if (parent == null)
                {
                    Win32.FreeConsole();
                    MessageBox.Show(
                        "通过反射修改Console类内部字段时出现异常。" +
                        $"\n{ex.Message}\n{ex.GetType()}\n{ex.StackTrace}",
                        "反射执行出现异常...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    Win32.FreeConsole();
                    MessageBox.Show(
                        parent,
                        "通过反射修改Console类内部字段时出现异常。" +
                        $"\n{ex.Message}\n{ex.GetType()}\n{ex.StackTrace}",
                        "反射执行出现异常...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 初始化所有反射资源（可用于强制更新）
        /// </summary>
        public static void Initialize()
        {
            try
            {
                Type consoleType = typeof(Console);
                handleField_out =
                        consoleType.GetField("_consoleOutputHandle", BindingFlags.Static | BindingFlags.NonPublic);
                handleField_in =
                        consoleType.GetField("_consoleInputHandle", BindingFlags.Static | BindingFlags.NonPublic);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "初始化反射相关资源时出现异常。" +
                    $"\n{ex.Message}\n{ex.GetType()}\n{ex.StackTrace}",
                    "反射执行出现异常...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// 用于配置窗口DWM参数的公开类
    /// </summary>
    public static class DWMEX
    {
        /// <summary>
        /// 用于获取win32数据类型COLORREF的扩展方法
        /// </summary>
        /// <param name="color">目标颜色值</param>
        /// <returns>返回目标颜色值的COLORREF形式数据</returns>
        public static int TO_COLORREF(this Color color)
        {
            return (color.B << 16) | (color.G << 8) | color.R;
        }

        /// <summary>
        /// 用于将win32数据类型COLORREF转换为Color类型实例的扩展方法
        /// </summary>
        /// <param name="COLORREF">目标颜色的COLORREF值</param>
        /// <returns>返回目标颜色的Color实例</returns>
        public static Color FROM_COLORREF(this int COLORREF)
        {
            byte blue = (byte)(COLORREF & 0xFF);
            byte green = (byte)((COLORREF >> 8) & 0xFF);
            byte red = (byte)((COLORREF >> 16) & 0xFF);
            return Color.FromArgb(0xFF, red, green, blue);
        }

        /// <summary>
        /// 配置窗口DWM参数
        /// </summary>
        /// <param name="window">目标窗口</param>
        /// <param name="C2NC">是否允许工作区绘制</param>
        /// <param name="DARK">是否允许同步暗色模式配置</param>
        /// <param name="B_COLORREF">标题栏颜色</param>
        /// <param name="T_COLORREF">标题文本颜色</param>
        /// <param name="CORNER">是否启用圆角（true：标准圆角，false：较小圆角，null：无圆角）</param>
        /// <returns>返回操作是否成功执行</returns>
        public static bool SetDWM(this IWin32Window window, bool C2NC, bool DARK, int B_COLORREF, int T_COLORREF, bool? CORNER)
        {
            return SetDWM(window.Handle, C2NC, DARK, B_COLORREF, T_COLORREF, CORNER);
        }

        /// <summary>
        /// 配置窗口DWM参数
        /// </summary>
        /// <param name="window">目标窗口句柄</param>
        /// <param name="C2NC">是否允许工作区绘制</param>
        /// <param name="DARK">是否允许同步暗色模式配置</param>
        /// <param name="B_COLORREF">标题栏颜色</param>
        /// <param name="T_COLORREF">标题文本颜色</param>
        /// <param name="CORNER">是否启用圆角（true：标准圆角，false：较小圆角，null：无圆角）</param>
        /// <returns>返回操作是否成功执行</returns>
        public static bool SetDWM(this IntPtr window, bool C2NC, bool DARK, int B_COLORREF, int T_COLORREF, bool? CORNER)
        {
            if (!DwmIsCompositionEnabled()) return false;
            // 系统版本检测：
            // Windows Vista -> 6.0.6000 (RTM)
            //                  6.0.6001 (SP1)
            //                  6.0.6002 (SP2)
            // Windows 7     -> 6.1.7600 (RTM)
            //                  6.1.7601 (SP1)
            // Windows 8     -> 6.2.9200
            // Windows 8.1   -> 6.3.9600
            // Windows 10    -> 10.0.10240
            // Windows 11    -> 10.0.22000
            Version sysver = Environment.OSVersion.Version;
            if (sysver >= new Version(6, 0, 6000))
            {
                bool dwm_result = true;
                bool isDWMenable = false;
                DwmGetWindowAttribute(
                    window,
                    DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_ENABLED,
                    ref isDWMenable,
                    sizeof(int));
                uint flag_0 = (uint)DWMNCRENDERINGPOLICY.DWMNCRP_ENABLED;
                if (isDWMenable || DwmSetWindowAttribute(
                    window,
                    DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY,
                    ref flag_0,
                    sizeof(uint)) == S_OK)
                {
                    bool flag_1 = true;
                    if (C2NC) DwmSetWindowAttribute(
                        window,
                        DWMWINDOWATTRIBUTE.DWMWA_ALLOW_NCPAINT,
                        ref flag_1,
                        sizeof(int));
                    if (sysver >= new Version(10, 0, 17763))
                    {
                        bool flag_2 = true;
                        DWMWINDOWATTRIBUTE ENUM = DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_20H1;
                        if (sysver.Build >= 18985)
                        {
                            ENUM = DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;
                        }
                        if (DARK) DwmSetWindowAttribute(
                            window,
                            ENUM,
                            ref flag_2,
                            sizeof(int));
                        if (sysver.Build >= 19041)
                        {
                            dwm_result &= DwmSetWindowAttribute(
                                window,
                                DWMWINDOWATTRIBUTE.DWMWA_CAPTION_COLOR,
                                ref B_COLORREF,
                                sizeof(uint)) == S_OK;
                            dwm_result &= DwmSetWindowAttribute(
                                window,
                                DWMWINDOWATTRIBUTE.DWMWA_TEXT_COLOR,
                                ref T_COLORREF,
                                sizeof(uint)) == S_OK;
                        }
                    }
                    if (sysver >= new Version(10, 0, 22000))
                    {
                        uint flag_3 = CORNER.HasValue
                            ? (CORNER.Value
                                ? (uint)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND
                                : (uint)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL)
                            : (uint)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_SQUARE;
                        DwmSetWindowAttribute(
                            window,
                            DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                            ref flag_3,
                            sizeof(uint));
                    }
                    MARGINS margins = new MARGINS() { Left = 0, Top = 0, Right = 0, Bottom = 0 };
                    if (C2NC) DwmExtendFrameIntoClientArea(window, ref margins);
                    return dwm_result;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 用于实现自适应限制响应的线程同步组件的公开类
    /// </summary>
    public class AutoResetEventEx
    {
        /// <summary>
        /// 用于存储原始线程同步组件的内部字段
        /// </summary>
        private readonly AutoResetEvent Base = null;

        /// <summary>
        /// 用于存储线程信息的内部字段
        /// </summary>
        private readonly List<Thread> ThreadList = new List<Thread>();

        /// <summary>
        /// 存储限制条件的内部字段
        /// </summary>
        private readonly int TaskLimit = 0;

        /// <summary>
        /// 初始化线程同步组件
        /// <para>
        /// 注意：组件不会主动管理同步组件的原始方法，调用前需先获取CheckLimit属性检查限制状态
        /// </para>
        /// </summary>
        /// <param name="initialState">是否将初始状态设置为终止</param>
        /// <param name="limit">阻塞状态数量限制</param>
        public AutoResetEventEx(bool initialState, int limit)
        {
            Base = new AutoResetEvent(initialState);
            TaskLimit = limit;
        }

        /// <summary>
        /// 阻塞当前线程
        /// </summary>
        /// <returns>当前实例收到信号后将返回true</returns>
        public bool WaitOne()
        {
            _ = ListControl(true);
            bool result = Base.WaitOne();
            _ = ListControl(false);
            return result;
        }
        /// <summary>
        /// 阻塞当前线程
        /// </summary>
        /// <param name="millisecondsTimeout">超时时间</param>
        /// <returns>当前实例收到信号后将返回true</returns>
        public bool WaitOne(int millisecondsTimeout)
        {
            _ = ListControl(true);
            bool result = Base.WaitOne(millisecondsTimeout);
            _ = ListControl(false);
            return result;
        }
        /// <summary>
        /// 阻塞当前线程
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <returns>当前实例收到信号后将返回true</returns>
        public bool WaitOne(TimeSpan timeout)
        {
            _ = ListControl(true);
            bool result = Base.WaitOne(timeout);
            _ = ListControl(false);
            return result;
        }
        /// <summary>
        /// 阻塞当前线程
        /// </summary>
        /// <param name="millisecondsTimeout">超时时间</param>
        /// <param name="exitContext">是否在等待之前退出同步域</param>
        /// <returns>当前实例收到信号后将返回true</returns>
        public bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            _ = ListControl(true);
            bool result = Base.WaitOne(millisecondsTimeout, exitContext);
            _ = ListControl(false);
            return result;
        }
        /// <summary>
        /// 阻塞当前线程
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <param name="exitContext">是否在等待之前退出同步域</param>
        /// <returns>当前实例收到信号后将返回true</returns>
        public bool WaitOne(TimeSpan timeout, bool exitContext)
        {
            _ = ListControl(true);
            bool result = Base.WaitOne(timeout, exitContext);
            _ = ListControl(false);
            return result;
        }

        /// <summary>
        /// 发送信号，解除阻塞状态
        /// </summary>
        /// <returns>返回操作是否成功</returns>
        public bool Set()
        {
            _ = ListControl(null);
            return Base.Set();
        }

        /// <summary>
        /// 对内部缓存进行处理
        /// </summary>
        /// <param name="Add">是否将当前线程添加到缓存（null=不做处理并返回限制状态）</param>
        /// <returns>返回限制状态（如果可能，否则将始终返回true）</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool ListControl(bool? Add)
        {
            if (ThreadList.Count > 0)
            {
                for (int i = ThreadList.Count - 1; i >= 0; i--)
                {
                    if (!ThreadList[i].IsAlive)
                    {
                        ThreadList.RemoveAt(i);
                    }
                }
            }
            if (Add.HasValue)
            {
                if (Add.Value)
                {
                    ThreadList.Add(Thread.CurrentThread);
                }
                else
                {
                    ThreadList.Remove(Thread.CurrentThread);
                }
            }
            else
            {
                return ThreadList.Count < TaskLimit;
            }
            return true;
        }

        /// <summary>
        /// 检查限制状态（符合限制条件为true，否则为false）
        /// </summary>
        public bool CheckLimit
        {
            get
            {
                return ListControl(null);
            }
        }
    }
}
