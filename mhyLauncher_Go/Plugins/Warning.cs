using MHYLAUNCHER_GO.Functions;
using System;
using System.Drawing;
using System.Windows.Forms;
using static MHYLAUNCHER_GO.Functions.Win32;

namespace MHYLAUNCHER_GO.Plugins
{
    /// <summary>
    /// 警告窗口基础类
    /// </summary>
    public partial class Warning : Form
    {
        /// <summary>
        /// Warning类的默认交互逻辑
        /// </summary>
        /// <param name="input">所有者窗口实例</param>
        public Warning(Input input)
        {
            InitializeComponent();
            Size offset = Size.Empty;
            this.Load += (sender, e) =>
            {
                this.SetDWM(false, true, this.BackColor.TO_COLORREF(), this.ForeColor.TO_COLORREF(), true);
                if (this.Handle == IntPtr.Zero) return;
                IntPtr menuhwnd = GetSystemMenu(this.Handle, false);
                if (menuhwnd != IntPtr.Zero)
                {
                    RemoveMenu(menuhwnd, SC_SIZE, MF_BYCOMMAND);
                    RemoveMenu(menuhwnd, SC_MOVE, MF_BYCOMMAND);
                    RemoveMenu(menuhwnd, SC_CLOSE, MF_BYCOMMAND);
                }
                offset = input.Size - this.Size;
                offset.Width /= 2;
                offset.Height /= 2;
                this.Location = input.Location + offset;
            };
            void Func_1(object sender_2, EventArgs e_2)
            {
                this.Location = input.Location + offset;
            };
            this.Shown += (sender, e) =>
            {
                input.Move += Func_1;
            };
            this.button_Yes.Click += (sender, e) =>
            {
                input.signal = true;
                input.Move -= Func_1;
                this.Close();
            };
            this.button_No.Click += (sender, e) =>
            {
                input.signal = false;
                input.Move -= Func_1;
                this.Close();
            };
            this.FormClosed += (sender, e) =>
            {
                input.wform = false;
                if (input.signal)
                {
                    input.Close();
                }
            };
        }

        /// <summary>
        /// 重写CreateParams属性用于配置窗口风格
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(int)WS_CAPTION;
                // cp.ExStyle |= (int)WS_EX_NOACTIVATE;
                return cp;
            }
        }
    }
}
