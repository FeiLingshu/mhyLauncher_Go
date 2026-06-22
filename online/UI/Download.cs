using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace online.UI
{
    public partial class Download : Form
    {
        public delegate void Function(Download self, bool source, bool file_1, bool file_2, bool file_3, bool file_self);

        public Download(bool global, Function function)
        {
            InitializeComponent();
            if (global)
            {
                this.checkBox_source.Checked = false;
            }
            this.radioButton_dll.Click += RadioButton_Click;
            this.radioButton_mp4.Click += RadioButton_Click;
            this.radioButton_otf.Click += RadioButton_Click;
            this.button_downlaod.Click += (sender, e) =>
            {
                if (radioButton_dll.Checked || radioButton_mp4.Checked || radioButton_otf.Checked)
                {
                    this.button_downlaod.Enabled = false;
                    bool s = checkBox_source.Checked;
                    bool f1 = radioButton_dll.Checked;
                    bool f2 = radioButton_mp4.Checked;
                    bool f3 = radioButton_otf.Checked;
                    bool fs = checkBox_self.Checked;
                    Task.Run(() =>
                    {
                        function(this, s, f1, f2, f3, fs);
                        this.Invoke(new Action(() =>
                        {
                            this.button_downlaod.Enabled = true;
                        }));
                    });
                }
            };
        }

        private void RadioButton_Click(object sender, EventArgs e)
        {
            RadioButton _this = sender as RadioButton;
            if (_this.Checked)
            {
                _this.Checked = false;
            }
            else
            {
                _this.Checked = true;
            }
        }

        public void SetStatus(RadioButton targate, bool? status)
        {
            if (status.HasValue)
            {
                if (status.Value)
                {
                    this.Invoke(new Action(() =>
                    {
                        targate.ForeColor = Color.FromArgb(0xFF, 0x06, 0xB0, 0x25);
                    }));
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        targate.ForeColor = Color.FromArgb(0xFF, 0xE8, 0x11, 0x23);
                    }));
                }
            }
        }

        public void SetStatus(CheckBox targate, bool? status)
        {
            if (status.HasValue)
            {
                if (status.Value)
                {
                    this.Invoke(new Action(() =>
                    {
                        targate.ForeColor = Color.FromArgb(0xFF, 0x06, 0xB0, 0x25);
                    }));
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        targate.ForeColor = Color.FromArgb(0xFF, 0xE8, 0x11, 0x23);
                    }));
                }
            }
        }
    }
}
