using MHYLAUNCHER_GO.Functions;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MHYLAUNCHER_GO.Functions.Win32;

namespace MHYLAUNCHER_GO.Plugins
{
    /// <summary>
    /// 实现显示提示信息的公开类
    /// </summary>
    public partial class Notify : Form
    {
        /// <summary>
        /// 初始化提示信息窗口
        /// </summary>
        /// <param name="Width">窗口宽度</param>
        public Notify(int Width)
        {
            InitializeComponent();
            this.widthcache = Width;
            this.Load += Notify_Load;
            this.Shown += Notify_Shown;
            this.label.Paint += Label_Paint;
            this.FormClosing += Notify_FormClosing;
        }

        /// <summary>
        /// 用于存储窗口设计宽度的内部字段
        /// </summary>
        private readonly int widthcache = 0;

        /// <summary>
        /// 重写CreateParams属性用于配置窗口风格
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(int)WS_CAPTION;
                cp.ExStyle |= (int)WS_EX_NOACTIVATE;
                cp.ExStyle |= (int)WS_EX_TRANSPARENT;
                return cp;
            }
        }

        /// <summary>
        /// 自动计算窗口高度
        /// </summary>
        /// <param name="pad">要预留的额外高度空间</param>
        /// <returns>返回计算结果</returns>
        private int GetHeight(int pad)
        {
            double ratio = 25D / 235D;
            if (widthcache == 235)
            {
                this.copyright.Height = 25;
            }
            else
            {
                this.copyright.Height = (int)Math.Round(widthcache * ratio, MidpointRounding.AwayFromZero); ;
            }
            string std = this.label.Tag as string;
            int labelheight = TextRenderer.MeasureText(std, this.label.Font).Height;
            return labelheight + this.copyright.Height + pad;
        }

        /// <summary>
        /// 窗口加载时配置窗口属性
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Notify_Load(object sender, EventArgs e)
        {
            this.SetDWM(true, true, this.label.BackColor.TO_COLORREF(), this.ForeColor.TO_COLORREF(), false);
            this.Width = widthcache;
            this.Height = GetHeight(50);
            this.Top -= this.Height;
            this.label.Height = this.copyright.Location.Y;
            if (this.Handle == IntPtr.Zero) return;
            IntPtr menuhwnd = GetSystemMenu(this.Handle, false);
            if (menuhwnd != IntPtr.Zero)
            {
                RemoveMenu(menuhwnd, SC_SIZE, MF_BYCOMMAND);
                RemoveMenu(menuhwnd, SC_MOVE, MF_BYCOMMAND);
                RemoveMenu(menuhwnd, SC_CLOSE, MF_BYCOMMAND);
            }
        }

        /// <summary>
        /// Task组件退出标志#1
        /// </summary>
        private readonly CancellationTokenSource token_1 = new CancellationTokenSource();

        /// <summary>
        /// Task组件退出标志#2
        /// </summary>
        private readonly CancellationTokenSource token_2 = new CancellationTokenSource();

        /// <summary>
        /// 标准透明度常量
        /// </summary>
        private const double OP = 0.85;

        /// <summary>
        /// 窗口显示完成时应用透明度动画
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Notify_Shown(object sender, EventArgs e)
        {
            AutoResetEvent timer = new AutoResetEvent(false);
            Task.Run(() =>
            {
                do
                {
                    bool fin = false;
                    double step = OP / 5D;
                    if (token_1.IsCancellationRequested) break;
                    this.Invoke(new Action(() =>
                    {
                        if (this.Opacity == OP)
                        {
                            fin = true;
                        }
                        else if (this.Opacity + step >= OP)
                        {
                            this.Opacity = OP;
                            fin = true;
                        }
                        else
                        {
                            this.Opacity += step;
                            fin = false;
                        }
                    }));
                    if (fin)
                    {
                        token_1.Cancel();
                    }
                    else
                    {
                        timer.WaitOne(TimeSpan.FromMilliseconds(1000 / 60D), false);
                    }
                } while (!token_1.IsCancellationRequested);
            }).ContinueWith(t =>
            {
                if (t.Status != TaskStatus.RanToCompletion)
                {
                    _ = t.Exception;
                }
            });
        }

        /// <summary>
        /// 对提示信息进行绘制
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Label_Paint(object sender, PaintEventArgs e)
        {
            string std = this.label.Tag as string;
            int textheight = TextRenderer.MeasureText(std, this.label.Font).Height;
            Rectangle paintrect = new Rectangle(
                e.ClipRectangle.X + this.label.Padding.Left,
                e.ClipRectangle.Y,
                e.ClipRectangle.Width - this.label.Padding.Horizontal,
                e.ClipRectangle.Height);
            Point liner_1 = new Point(4, 0);
            Point liner_2 = new Point(e.ClipRectangle.Width - 4, 0);
            using (Brush brush_1 = new SolidBrush(label.ForeColor),
                brush_2 = new LinearGradientBrush(liner_1, liner_2, this.copyright.BackColor, this.label.BackColor))
            {
                e.Graphics.FillRectangle(
                    brush_1,
                    paintrect.X,
                    paintrect.Y + (paintrect.Height - textheight) / 2,
                    4,
                    textheight);
                e.Graphics.FillRectangle(
                    brush_2,
                    paintrect.X + 4,
                    paintrect.Y + (paintrect.Height - textheight) / 2,
                    paintrect.Width - 4,
                    textheight);
            }
            TextRenderer.DrawText(
                e.Graphics,
                std,
                this.Font,
                paintrect,
                this.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        /// <summary>
        /// 确保窗口关闭操作单例的内部字段
        /// </summary>
        private bool is_closing = false;

        /// <summary>
        /// 窗口关闭时应用透明度动画
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        private void Notify_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Opacity == 0)
            {
                token_2.Cancel();
                e.Cancel = false;
            }
            else
            {
                if (is_closing)
                {
                    e.Cancel = true;
                    return;
                }
                is_closing = true;
                token_1.Cancel();
                AutoResetEvent timer = new AutoResetEvent(false);
                Task.Run(() =>
                {
                    do
                    {
                        bool fin = false;
                        double step = OP / 5D;
                        if (token_2.IsCancellationRequested) break;
                        this.Invoke(new Action(() =>
                        {
                            if (this.Opacity == 0)
                            {
                                fin = true;
                            }
                            else if (this.Opacity - step <= 0)
                            {
                                this.Opacity = 0;
                                fin = true;
                            }
                            else
                            {
                                this.Opacity -= step;
                                fin = false;
                            }
                        }));
                        if (fin)
                        {
                            this.Invoke(new Action(() =>
                            {
                                this.Close();
                            }));
                        }
                        else
                        {
                            timer.WaitOne(TimeSpan.FromMilliseconds(1000 / 60D), false);
                        }
                    } while (!token_2.IsCancellationRequested);
                }).ContinueWith(t =>
                {
                    if (t.Status != TaskStatus.RanToCompletion)
                    {
                        _ = t.Exception;
                    }
                });
                e.Cancel = true;
            }
        }
    }
}
