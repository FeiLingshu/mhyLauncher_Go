using MHYLAUNCHER_GO.Functions;
using MHYLAUNCHER_GO.MainFrame.AnimationHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MHYLAUNCHER_GO.MainFrame
{
    /// <summary>
    /// Hello.xaml 的交互逻辑
    /// </summary>
    public partial class Hello : Window
    {
        /// <summary>
        /// 用于版本标识的内部字段
        /// </summary>
        private readonly string version = "https://github.com/FeiLingshu/mhyLauncher_Go/releases/tag/V2-Opt-2";

        /// <summary>
        /// 用于检查版本更新的http地址
        /// </summary>
        private readonly string updatelink = "https://gitee.com/FeiLingshu/mhyLauncher_Go_mirror/raw/master/version";

        /// <summary>
        /// 用于检查版本更新的备用http地址
        /// </summary>
        private readonly string backuplink = "https://raw.githubusercontent.com/FeiLingshu/mhyLauncher_Go/refs/heads/resources/version";

        /// <summary>
        /// 用于指示运行情况的内部字段
        /// </summary>
        private (bool, bool) running_state = (false, false);

        /// <summary>
        /// Hello.xaml 的默认启动逻辑
        /// </summary>
        /// <param name="app">主应用程序域</param>
        /// <param name="resource">应用程序资源目录</param>
        /// <param name="_void">待调用的外部方法体</param>
        public Hello(App app, string resource, Action _void)
        {
            InitializeComponent();
            this.app = app;
            this.Loaded += (sender, e) =>
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                hwnd.SetDWM(
                    false,
                    true,
                    System.Drawing.Color.FromArgb(0xFF, 0x1E, 0x20, 0x25).TO_COLORREF(),
                    System.Drawing.Color.FromArgb(0xFF, 0xE5, 0xE5, 0xE5).TO_COLORREF(),
                    true);
                RUN.IsEnabled = false;
                TITLETEXT.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0xE5, 0xE5));
                TITLETEXT.Text = "正在获取更新 ...";
                if (string.IsNullOrEmpty(resource) || RenderCapability.Tier >> 16 == 0)
                {
                    BGVIDEO.Close();
                    BGVIDEO.Source = null;
                    BGVIDEO.Visibility = Visibility.Collapsed;
                    BGIMAGE.Visibility = Visibility.Visible;
                }
                else
                {
                    BGVIDEO.MediaFailed += (ss, ee) =>
                    {
                        BGVIDEO.Stop();
                        BGVIDEO.Close();
                        BGVIDEO.Source = null;
                        BGVIDEO.Visibility = Visibility.Collapsed;
                        BGIMAGE.Visibility = Visibility.Visible;
                    };
                    BGIMAGE.Visibility = Visibility.Collapsed;
                    BGVIDEO.Source = new Uri($"{resource}\\video.mp4", UriKind.Absolute);
                    BGVIDEO.MediaEnded += (ss, ee) =>
                    {
                        BGVIDEO.Position = TimeSpan.FromSeconds(0);
                        BGVIDEO.Play();
                    };
                    BGVIDEO.Position = TimeSpan.FromSeconds(0);
                    BGVIDEO.Visibility = Visibility.Visible;
                    BGVIDEO.Play();
                }
            };
            this.ContentRendered += (sender, e) =>
            {
                net = new Thread(() =>
                {
                    AppContext.SetSwitch("System.Net.DisableIPv6", false);
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                    httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                        NoStore = true,
                        MustRevalidate = true
                    };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
                    httpClient.DefaultRequestHeaders.Add("Strict-Transport-Security", "max-age=31536000");
                    httpClient.DefaultRequestHeaders.Add("X-Frame-Options", "DENY");
                    int times = 1;
                    HttpResult[] results = new HttpResult[2] {
                        new HttpResult(
                            "https://gitee.com",
                            $"{updatelink}",
                            false,
                            -1,
                            string.Empty),
                        new HttpResult(
                            "https://gh-proxy.com",
                            $"https://gh-proxy.org/{backuplink}",
                            false,
                            -1,
                            string.Empty)
                    };
                    HttpResult? result = null;
                    do
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            TITLETEXT.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0xE5, 0xE5));
                            TITLETEXT.Text = $"正在获取更新 ...   ({times}/3)";
                        });
                        try
                        {
                            results = HttpData(
                                results[0].URL,
                                results[0].Host,
                                results[1].URL,
                                results[1].Host).GetAwaiter().GetResult();
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                cts = null;
                            });
                        }
                        catch (TaskCanceledException) { /* Do Nothing ...*/ }
                        catch (OperationCanceledException) { /* Do Nothing ...*/ }
                        catch (Exception exp)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (!is_cancel) throw exp;
                            });
                        }
                        if (is_cancel) return;
                        foreach (HttpResult r in results)
                        {
                            if (r.state)
                            {
                                result = r;
                                break;
                            }
                        }
                        if (result.HasValue)
                        {
                            break;
                        }
                        else
                        {
                            if (times < 3)
                            {
                                times++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    } while (true);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (result.HasValue)
                        {
                            hostavailable = true;
                            HostName = result.Value.Host;
                            switch (result.Value.data)
                            {
                                case "1B1FBA27-60A8-4F8B-B107-73EA7DA0CE56":
                                    if (version == "1B1FBA27-60A8-4F8B-B107-73EA7DA0CE56")
                                    {
                                        TITLETEXT.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x2D, 0xA8, 0xFF));
                                        TITLETEXT.Text = $"内部测试版本 ...";
                                    }
                                    else
                                    {
                                        TITLETEXT.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xDB, 0x29));
                                        TITLETEXT.Text = $"开发者版本 ...";
                                    }
                                    break;
                                case "57DB5563-758E-45D8-A290-3A48BDB2A263":
                                    throw new NotSupportedException($"识别到开发者发布的终止指令，程序已停用。");
                                default:
                                    if (version == result.Value.data)
                                    {
                                        TITLETEXT.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x4B, 0xD3, 0x7B));
                                        TITLETEXT.Text = $"已是最新版本 ...";
                                    }
                                    else
                                    {
                                        TITLETEXT.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xDB, 0x29));
                                        TITLETEXT.Text = string.Empty;
                                        TITLETEXT.Inlines.Add(new Run("存在版本更新 ...   ("));
                                        SolidColorBrush brush1 = new SolidColorBrush(Color.FromArgb(0xFF, 0x2D, 0xA8, 0xFF));
                                        SolidColorBrush brush2 = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xDB, 0x29));
                                        Style fixedColorStyle = new Style(typeof(Hyperlink));
                                        fixedColorStyle.Setters.Add(new Setter(Hyperlink.ForegroundProperty, brush1));
                                        Hyperlink hyperlink = new Hyperlink(new Run("点击查看"))
                                        {
                                            Cursor = Cursors.Arrow,
                                            Foreground = brush1,
                                            Style = fixedColorStyle
                                        };
                                        if (Uri.TryCreate(result.Value.data, UriKind.Absolute, out Uri uriResult))
                                        {
                                            hyperlink.NavigateUri = uriResult;
                                        }
                                        else
                                        {
                                            hyperlink.NavigateUri = new Uri("https://github.com/FeiLingshu/mhyLauncher_Go/releases");
                                        }
                                        hyperlink.MouseEnter += (ss, ee) =>
                                        {
                                            ((Hyperlink)ss).Foreground = brush2;
                                        };
                                        hyperlink.MouseLeave += (ss, ee) =>
                                        {
                                            ((Hyperlink)ss).Foreground = brush1;
                                        };
                                        hyperlink.RequestNavigate += (ss, ee) =>
                                        {
                                            using (Process.Start(ee.Uri.AbsoluteUri)) { }
                                        };
                                        TITLETEXT.Inlines.Add(hyperlink);
                                        TITLETEXT.Inlines.Add(new Run(")"));
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            hostavailable = false;
                            HostName = string.Join("\n", new string[4] {
                                results[0].Host,
                                results[0].code.ToString(),
                                results[1].Host,
                                results[1].code.ToString()
                            });
                            TITLETEXT.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFE, 0x69, 0x52));
                            TITLETEXT.Text = $"获取更新失败 ...   (悬停查看)";
                        }
                        running_state.Item1 = true;
                        if (running_state.Item1 && running_state.Item2)
                        {
                            RUN.IsEnabled = true;
                        }
                    });
                })
                { IsBackground = true };
                net.Start();
                Task.Run(_void);
            };
            this.MouseLeftButtonDown += (sender, e) =>
            {
                this.DragMove();
            };
            void block(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
            }
            CLOSE.MouseLeftButtonDown += block;
            RUN.MouseLeftButtonDown += block;
            TITLE.MouseLeftButtonDown += block;
            CLOSE.MouseLeftButtonUp += CLOSE_MouseLeftButtonUp;
            RUN.MouseLeftButtonUp += RUN_MouseLeftButtonUp;
            TITLE.MouseEnter += TITLE_MouseEnter;
            TITLE.MouseLeave += TITLE_MouseLeave;
        }

        /// <summary>
        /// 用于存储主应用程序域实例的内部字段
        /// </summary>
        private readonly App app;

        /// <summary>
        /// 用于存储网络请求线程实例的内部字段
        /// </summary>
        private Thread net = null;

        /// <summary>
        /// 关闭按钮响应
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件数据</param>
        private void CLOSE_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            is_cancel = true;
            cts?.Cancel();
            if (net != null && net.IsAlive)
            {
                net.Abort();
            }
            BGVIDEO.Stop();
            BGVIDEO.Close();
            BGVIDEO.Source = null;
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// 运行按钮响应
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件数据</param>
        private void RUN_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BGVIDEO.Stop();
            BGVIDEO.Close();
            BGVIDEO.Source = null;
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// 显示标题提示信息
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件数据</param>
        private void TITLE_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                To = 0.5,
                Duration = TimeSpan.FromSeconds(0.1),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            TIPSTD.BeginAnimation(TextBlock.TextProperty, null, HandoffBehavior.SnapshotAndReplace);
            tipavailable = true;
            HostName = _hostname;
            TIP.BeginAnimation(UIElement.OpacityProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// 隐藏标题提示信息
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件数据</param>
        private void TITLE_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.1),
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };
            //WeakEventManager<AnimationClock, EventArgs>.AddHandler(
            //    (AnimationClock)animation.CreateClock(),
            //    nameof(animation.Completed),
            //    TITLE_Storyboard_Completed
            //);
            animation.CompletedEx(
                TIP,
                UIElement.OpacityProperty,
                TITLE_Storyboard_Completed);
            tipavailable = false;
            TIP.BeginAnimation(UIElement.OpacityProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        /// <summary>
        /// 隐藏标题提示信息后续操作
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件数据</param>
        private void TITLE_Storyboard_Completed(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!TITLE.IsMouseOver) TIPSTD.Text = string.Empty;
                AnimationManager.CleanUp(TIP, UIElement.OpacityProperty);
            });
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="log">日志信息</param>
        public void Log(string log)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LOG.Inlines.Add(new LineBreak());
                LOG.Inlines.Add(new Run($"[{DateTime.Now:HH:mm:ss} LOG] → {log}")
                {
                    Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x8D, 0x8E, 0x90)),
                });
            });
        }

        /// <summary>
        /// 预先播放减速动画（进度）
        /// </summary>
        /// <param name="t">目标进度(小数)</param>
        /// <param name="time">耗时操作的预计时间</param>
        public void PreStep(double t, TimeSpan time)
        {
            if (!StepSignal.CheckLimit)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (t < 0) t = 0;
                if (t > 1) t = 1;
                DoubleAnimation animation = new DoubleAnimation()
                {
                    To = t,
                    Duration = time,
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
                };
                if (t == 1)
                {
                    //WeakEventManager<AnimationClock, EventArgs>.AddHandler(
                    //    (AnimationClock)animation.CreateClock(),
                    //    nameof(animation.Completed),
                    //    AllStep
                    //);
                    animation.CompletedEx(
                        STEP,
                        ScaleTransform.ScaleXProperty,
                        AllStep);
                }
                else
                {
                    //WeakEventManager<AnimationClock, EventArgs>.AddHandler(
                    //    (AnimationClock)animation.CreateClock(),
                    //    nameof(animation.Completed),
                    //    AfterStep
                    //);
                    animation.CompletedEx(
                        STEP,
                        ScaleTransform.ScaleXProperty,
                        AfterStep);
                }
                STEP.BeginAnimation(ScaleTransform.ScaleXProperty, animation, HandoffBehavior.SnapshotAndReplace);
            });
        }

        /// <summary>
        /// 用于等待进度动画结束的公开指示器
        /// </summary>
        public readonly AutoResetEventEx StepSignal = new AutoResetEventEx(false, 1);

        /// <summary>
        /// 进度动画结束指示器（一般）
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件数据</param>
        private void AfterStep(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AnimationManager.CleanUp(STEP, ScaleTransform.ScaleXProperty);
                StepSignal.Set();
            });
        }

        /// <summary>
        /// 进度动画结束指示器（终结）
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件数据</param>
        private void AllStep(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AnimationManager.CleanUp(STEP, ScaleTransform.ScaleXProperty);
                StepSignal.Set();
                Set();
            });
        }

        /// <summary>
        /// 发送完成信号
        /// </summary>
        private void Set()
        {
            running_state.Item2 = true;
            if (running_state.Item1 && running_state.Item2)
            {
                RUN.IsEnabled = true;
            }
        }

        /// <summary>
        /// 用于进行Http请求的内部字段(可复用)
        /// </summary>
        private readonly HttpClient httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(10),
            MaxResponseContentBufferSize = 1024 * 1024
        };

        /// <summary>
        /// 用于保存http请求结果的结构体
        /// </summary>
        private struct HttpResult
        {
            /// <summary>
            /// 主机名称（域名）
            /// </summary>
            public string Host;
            /// <summary>
            /// 目标网络地址
            /// </summary>
            public string URL;
            /// <summary>
            /// 访问状态
            /// </summary>
            public bool state;
            /// <summary>
            /// 状态代码
            /// </summary>
            public int code;
            /// <summary>
            /// 返回的网络信息
            /// </summary>
            public string data;
            /// <summary>
            /// 初始化HttpResult结构体实例
            /// </summary>
            /// <param name="Host">主机名称（域名）</param>
            /// <param name="URL">目标网络地址</param>
            /// <param name="state">访问状态</param>
            /// <param name="code">状态代码</param>
            /// <param name="data">返回的网络信息</param>
            public HttpResult(string Host, string URL, bool state, int code, string data)
            {
                this.Host = Host;
                this.URL = URL;
                this.state = state;
                this.code = code;
                this.data = data;
            }
        }

        /// <summary>
        /// 指示提示框组件是否正在被http请求结果使用
        /// </summary>
        private bool tipavailable = false;

        /// <summary>
        /// 指示当前数据源是否已返回结果
        /// </summary>
        private bool hostavailable = false;

        /// <summary>
        /// 用于存储提示信息的内部字段
        /// </summary>
        private string _hostname = "正在拉取更新信息";

        /// <summary>
        /// 用于即时响应http请求结果的属性字段
        /// </summary>
        private string HostName
        {
            get
            {
                return _hostname;
            }
            set
            {
                _hostname = value;
                if (tipavailable)
                {
                    TIPSTD.Text = string.Empty;
                    if (hostavailable)
                    {
                        TIPSTD.Inlines.Add(new Run("数据源："));
                        TIPSTD.Inlines.Add(new Run(value) { Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x2D, 0xA8, 0xFF)) });
                    }
                    else
                    {
                        _hostname = value;
                        string[] strings = value.Split('\n');
                        if (strings.Length == 1)
                        {
                            TIPSTD.Text = value;
                        }
                        else
                        {
                            List<int> _ = new List<int>();
                            for (int i = 0; i < strings.Length; i += 2)
                            {
                                _.Add(strings[i].Length);
                            }
                            int padding = _.Max();
                            for (int i = 0; i < strings.Length; i += 2)
                            {
                                if (i != 0)
                                {
                                    TIPSTD.Inlines.Add(new LineBreak());
                                }
                                TIPSTD.Inlines.Add(new Run($"{strings[i]}   ".PadRight(padding + 3)) {
                                    Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x2D, 0xA8, 0xFF))
                                });
                                TIPSTD.Inlines.Add(new Run($"{strings[i + 1],3}"));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 全局异步取消标志
        /// </summary>
        private CancellationTokenSource cts = null;

        /// <summary>
        /// 指示是否处于全局取消状态
        /// <para>
        /// - bool类型的值的操作本身是原子的，但处理器可能重排指令造成代码逻辑上本身先写后读的过程变为先读后写<br/>
        /// - 添加volatile关键字用于禁止指令排序以维持代码顺序一致
        /// </para>
        /// </summary>
        private volatile bool is_cancel = false;

        /// <summary>
        /// 获取Http数据（异步）
        /// </summary>
        /// <param name="url">目标网络链接</param>
        /// <param name="host">目标网络链接主机地址</param>
        /// <param name="backupurl">备用网络链接</param>
        /// <param name="backuphost">备用网络链接主机地址</param>
        /// <returns>返回http响应数据</returns>
        private async Task<HttpResult[]> HttpData(string url, string host, string backupurl, string backuphost)
        {
            using (cts = new CancellationTokenSource())
            {
                HttpResult[] R = new HttpResult[2] {
                    new HttpResult(host, url, false, -1, string.Empty),
                    new HttpResult(backuphost, backupurl, false, -1, string.Empty)
                };
                Task<HttpResponseMessage> task = httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                Task<HttpResponseMessage> backuptask = httpClient.GetAsync(backupurl, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                int index;
                int backupindex;
                Task<HttpResponseMessage> completedTask = await Task.WhenAny(task, backuptask);
                Task<HttpResponseMessage> backupTask;
                if (completedTask == task)
                {
                    index = 0;
                    backupindex = 1;
                    backupTask = backuptask;
                }
                else
                {
                    index = 1;
                    backupindex = 0;
                    backupTask = task;
                }
                if (completedTask.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        using (HttpResponseMessage response = await completedTask)
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                R[index].state = true;
                                R[index].code = (int)response.StatusCode;
                                R[index].data = await response.Content.ReadAsStringAsync();
                                R[index].data = R[index].data.Trim();
                                try
                                {
                                    cts.Cancel();
                                    await Task.WhenAll(task, backuptask);
                                }
                                catch (Exception) { }
                                finally
                                {
                                    _ = task.Exception;
                                    _ = backuptask.Exception;
                                }
                                // Debug.Print(R[index].Host);
                                return R;
                            }
                            else
                            {
                                R[index].state = false;
                                R[index].code = (int)response.StatusCode;
                                R[index].data = string.Empty;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        R[index].state = false;
                        R[index].code = -1;
                        R[index].data = string.Empty;
                    }
                }
                else
                {
                    R[index].state = false;
                    R[index].code = -1;
                    R[index].data = string.Empty;
                    _ = completedTask.Exception;
                }
                _ = await Task.WhenAny(backupTask);
                if (backupTask.Status == TaskStatus.RanToCompletion)
                {
                    try
                    {
                        using (HttpResponseMessage backupResponse = await backupTask)
                        {
                            if (backupResponse.IsSuccessStatusCode)
                            {
                                R[backupindex].state = true;
                                R[backupindex].code = (int)backupResponse.StatusCode;
                                R[backupindex].data = await backupResponse.Content.ReadAsStringAsync();
                                R[backupindex].data = R[backupindex].data.Trim();
                            }
                            else
                            {
                                R[backupindex].state = false;
                                R[backupindex].code = (int)backupResponse.StatusCode;
                                R[backupindex].data = string.Empty;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        R[backupindex].state = false;
                        R[backupindex].code = -1;
                        R[backupindex].data = string.Empty;
                    }
                }
                else
                {
                    R[backupindex].state = false;
                    R[backupindex].code = -1;
                    R[backupindex].data = string.Empty;
                    _ = backupTask.Exception;
                }
                // Debug.Print(R[backupindex].Host);
                return R;
            }
        }
    }
}
