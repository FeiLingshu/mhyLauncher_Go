using System;
using System.Runtime.InteropServices;

namespace mhyLauncher_Go.Func
{
    /// <summary>
    /// C++函数MessageBeep的基础类
    /// </summary>
    internal static class MsgBeep
    {
        /// <summary>
        /// C++结构uType
        /// </summary>
        [Flags] // 启用位运算，保证枚举值的唯一性
        internal enum UType : uint
        {
            MB_OK = 0x00000000U,
            MB_ICONHAND = 0x00000010U,
            [Obsolete("该枚举值无法产生任何效果，原因未知", true)]
            MB_ICONQUESTION = 0x00000020U,
            MB_ICONEXCLAMATION = 0x00000030U,
            MB_ICONASTERISK = 0x00000040U,
            [Obsolete("该枚举值将产生与MB_OK枚举值相同的作用，无需使用", true)]
            MS_UNNAMED = 0xFFFFFFFFU,
        }

        /// <summary>
        /// 播放指定的Windows系统声音
        /// </summary>
        /// <param name="uType">Windows声音类型</param>
        /// <returns>指示操作是否成功</returns>
        [DllImport("user32.dll")]
        private static extern bool MessageBeep(uint uType);

        /// <summary>
        /// 调用系统声音播放函数播放指定声音
        /// </summary>
        /// <param name="uType">指定要播放的声音类型</param>
        internal static void Beep(UType uType)
        {
            switch (uType)
            {
                case UType.MB_OK:
                    MessageBeep(0x00000000U);
                    break;
                case UType.MB_ICONHAND:
                    MessageBeep(0x00000010U);
                    break;
                case UType.MB_ICONEXCLAMATION:
                    MessageBeep(0x00000030U);
                    break;
                case UType.MB_ICONASTERISK:
                    MessageBeep(0x00000040U);
                    break;
            }
        }
    }
}
