using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;

namespace MHYLAUNCHER_GO.MainFrame.AnimationHelper
{
    /// <summary>
    /// 为WPF动画提供自动化事件绑定管理的静态类
    /// </summary>
    public static class AnimationManager
    {
        /// <summary>
        /// 自定义字典键类型（包含约束条件）
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        private readonly struct AnimationKey : IEquatable<AnimationKey>
        {
            /// <summary>
            /// 目标元素（可承载动画类型）
            /// </summary>
            public IAnimatable Element { get; }
            /// <summary>
            /// 目标属性依赖
            /// </summary>
            public DependencyProperty Property { get; }

            /// <summary>
            /// 初始化结构体字段
            /// </summary>
            /// <param name="element">目标元素（可承载动画类型）</param>
            /// <param name="property">目标属性依赖</param>
            /// <exception cref="ArgumentNullException">参数值为null</exception>
            public AnimationKey(IAnimatable element, DependencyProperty property)
            {
                Element = element ?? throw new ArgumentNullException(nameof(element));
                Property = property ?? throw new ArgumentNullException(nameof(property));
            }

            /// <summary>
            /// 重写内部Equals方法
            /// </summary>
            /// <param name="obj">比较目标</param>
            /// <returns>返回两者是否相等</returns>
            public override bool Equals(object obj) =>
                obj is AnimationKey key && Equals(key);

            /// <summary>
            /// Equals方法实现
            /// </summary>
            /// <param name="other">比较目标</param>
            /// <returns>返回两者是否相等</returns>
            public bool Equals(AnimationKey other) =>
                ReferenceEquals(Element, other.Element) && Property == other.Property;

            /// <summary>
            /// 重写默认哈希函数
            /// </summary>
            /// <returns>返回结构体实例哈希值</returns>
            public override int GetHashCode()
            {
                ulong prehash = (ulong)(Element?.GetHashCode() ?? 0) << 32 | (uint)(Property?.GetHashCode() ?? 0);
                return unchecked((int)(prehash ^ (prehash >> 32)));
            }
        }

        /// <summary>
        /// 用于存储动画实例和事件订阅委托的结构体
        /// </summary>
        private readonly struct AnimationData
        {
            /// <summary>
            /// 动画实例
            /// </summary>
            public AnimationTimeline Animation { get; }
            /// <summary>
            /// 事件订阅委托
            /// </summary>
            public EventHandler Handler { get; }

            /// <summary>
            /// 初始化结构体字段
            /// </summary>
            /// <param name="animation">动画实例<param>
            /// <param name="handler">事件订阅委托</param>
            /// <exception cref="ArgumentNullException">参数值为null</exception>
            public AnimationData(AnimationTimeline animation, EventHandler handler)
            {
                Animation = animation ?? throw new ArgumentNullException(nameof(animation));
                Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }

            /// <summary>
            /// 获取一个空的结构体实例
            /// </summary>
            [Obsolete("由于结构体初始化时执行null检查，使用该只读属性字段会抛出null异常", true)]
            public static AnimationData Empty
            {
                get
                {
                    return new AnimationData(null, null);
                }
            }
        }

        /// <summary>
        /// 用于存储数据的字典
        /// </summary>
        private readonly static Dictionary<AnimationKey, AnimationData> _animations = new Dictionary<AnimationKey, AnimationData>();

        /// <summary>
        /// 尝试查找字典数据
        /// </summary>
        /// <param name="element">目标元素（可承载动画类型）</param>
        /// <param name="property">目标属性依赖</param>
        /// <param name="data">字典值</param>
        /// <returns>返回字典中是否包含指定键</returns>
        private static bool TryGet(
            IAnimatable element,
            DependencyProperty property,
            out AnimationData? data)
        {
            AnimationKey key = new AnimationKey(element, property);
            if (_animations.TryGetValue(key, out AnimationData _data))
            {
                data = _data;
                return true;
            }
            data = null;
            return false;
        }

        /// <summary>
        /// 更新字典数据
        /// </summary>
        /// <param name="element">目标元素（可承载动画类型）</param>
        /// <param name="property">目标属性依赖</param>
        /// <param name="data">字典值</param>
        private static void Update(
            IAnimatable element,
            DependencyProperty property,
            AnimationData data)
        {
            AnimationKey key = new AnimationKey(element, property);
            _animations[key] = data;
        }

        /// <summary>
        /// 尝试移除字典数据
        /// </summary>
        /// <param name="element">目标元素（可承载动画类型）</param>
        /// <param name="property">目标属性依赖</param>
        /// <returns>返回移除是否成功</returns>
        private static bool TryRemove(IAnimatable element, DependencyProperty property)
        {
            AnimationKey key = new AnimationKey(element, property);
            return _animations.Remove(key);
        }

        /// <summary>
        /// 清理缓存字典
        /// <para>
        /// 注意：直接清理字典并不会解除事件订阅！<br/>
        /// 仅在确认所有拥有动画实例的元素销毁后才可执行！
        /// </para>
        /// </summary>
        public static void Clear() => _animations.Clear();

        /// <summary>
        /// 获取当前字典中的缓存数量
        /// </summary>
        public static int Count => _animations.Count;

        /// <summary>
        /// 安全绑定动画的Completed事件（使用强引用）
        /// <para>
        /// 需要以同步方式调用！
        /// </para>
        /// </summary>
        /// <param name="Animation">目标动画实例</param>
        /// <param name="element">目标元素（可承载动画类型）</param>
        /// <param name="property">目标属性依赖</param>
        /// <param name="Handler">事件订阅处理程序</param>
        public static void CompletedEx(
            this AnimationTimeline Animation,
            IAnimatable element,
            DependencyProperty property,
            EventHandler Handler)
        {
            if (TryGet(element, property, out AnimationData? _data) && _data.HasValue)
            {
                _data.Value.Animation.Completed -= _data.Value.Handler;
            }
            Animation.Completed += Handler;
            Update(element, property, new AnimationData(Animation, Handler));
        }

        /// <summary>
        /// 移除指定元素和属性下的动画事件订阅
        /// <para>
        /// 需要以同步方式调用！
        /// </para>
        /// </summary>
        /// <param name="element">目标元素（可承载动画类型）</param>
        /// <param name="property">目标属性依赖</param>
        public static void CleanUp(
            IAnimatable element,
            DependencyProperty property)
        {
            if (TryGet(element, property, out AnimationData? _data) && _data.HasValue)
            {
                _data.Value.Animation.Completed -= _data.Value.Handler;
                TryRemove(element, property);
            }
        }
    }
}
