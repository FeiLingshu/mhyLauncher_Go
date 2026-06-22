using online.HTTP;
using online.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace online
{
    public class Program
    {
        public static string SelfPath = Environment.CurrentDirectory;

        public readonly static bool FLAG = false;

        private static readonly string GUID = "AEC64209-E614-4D7D-A0A8-711368AAAEE9";

        private static EventWaitHandle ProgramStarted;

        [STAThread]
        public static void Main(string[] args)
        {
            if (!FLAG)
            {
                if (!args.Contains("EV") && !args.Contains("Environment"))
                {
                    string[] _args = new string[args.Length + 1];
                    _args[0] = "EV";
                    args.CopyTo(_args, 1);
                    args = _args;
                }
            }
            using (Process self = Process.GetCurrentProcess())
            {
                SelfPath = Path.GetDirectoryName(self.MainModule.FileName);
            }
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (sender, e) =>
            {
                HandleException(e.Exception);
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                HandleException(e.ExceptionObject as Exception);
            };
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                HandleException(e.Exception);
            };
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, GUID, out bool createNew);
            if (!createNew)
            {
                ProgramStarted.Set();
                throw new AggregateException($"Process [{GUID}] is already running ...");
            }
            ThreadPool.RegisterWaitForSingleObject(
                ProgramStarted,
                (state, timeout) => { },
                null,
                -1,
                false);
            HTML.Fake();
            bool global = args.Contains("Global");
            if (args.Contains("EV") || args.Contains("Environment"))
            {
                void Download(Download self, bool source, bool file_1, bool file_2, bool file_3, bool file_self)
                {
                    List<Task> tasks = new List<Task>(3);
                    string[][] urls;
                    if (source)
                    {
                        urls = new string[4][] { HTML.DLL, HTML.MP4, HTML.OTF, new string[1] { HTML.EXE } };
                    }
                    else
                    {
                        urls = new string[4][] { HTML._DLL, HTML._MP4, HTML._OTF, new string[1] { HTML._EXE } };
                    }
                    if (file_1)
                    {
                        string path = $"{SelfPath}\\EfficiencyMode.dll";
                        FileStream stream;
                        if (File.Exists(path))
                        {
                            stream = File.Open(path, FileMode.Truncate, FileAccess.Write);
                        }
                        else
                        {
                            stream = File.Open(path, FileMode.CreateNew, FileAccess.Write);
                        }
                        tasks.Add(HTML.FetchHtmlAsync(urls[0], stream).ContinueWith(result =>
                        {
                            if (result.IsCompleted && result.Result == true)
                            {
                                try
                                {
                                    using (stream)
                                    {
                                        stream.Flush();
                                    }
                                    self.SetStatus(self.radioButton_dll, true);
                                }
                                catch (Exception) { self.SetStatus(self.radioButton_dll, false); }
                            }
                            else
                            {
                                self.SetStatus(self.radioButton_dll, false);
                            }
                        }));
                    }
                    if (file_2)
                    {
                        string path = $"{SelfPath}\\video.mp4";
                        FileStream stream;
                        if (File.Exists(path))
                        {
                            stream = File.Open(path, FileMode.Truncate, FileAccess.Write);
                        }
                        else
                        {
                            stream = File.Open(path, FileMode.CreateNew, FileAccess.Write);
                        }
                        tasks.Add(HTML.FetchHtmlAsync(urls[1], stream).ContinueWith(result =>
                        {
                            if (result.IsCompleted && result.Result == true)
                            {
                                try
                                {
                                    using (stream)
                                    {
                                        stream.Flush();
                                    }
                                    self.SetStatus(self.radioButton_mp4, true);
                                }
                                catch (Exception) { self.SetStatus(self.radioButton_mp4, false); }
                            }
                            else
                            {
                                self.SetStatus(self.radioButton_mp4, false);
                            }
                        }));
                    }
                    if (file_3)
                    {
                        string path = $"{SelfPath}\\GiteeGit.otf";
                        FileStream stream;
                        if (File.Exists(path))
                        {
                            stream = File.Open(path, FileMode.Truncate, FileAccess.Write);
                        }
                        else
                        {
                            stream = File.Open(path, FileMode.CreateNew, FileAccess.Write);
                        }
                        tasks.Add(HTML.FetchHtmlAsync(urls[2], stream).ContinueWith(result =>
                        {
                            if (result.IsCompleted && result.Result == true)
                            {
                                try
                                {
                                    using (stream)
                                    {
                                        stream.Flush();
                                    }
                                    self.SetStatus(self.radioButton_otf, true);
                                }
                                catch (Exception) { self.SetStatus(self.radioButton_otf, false); }
                            }
                            else
                            {
                                self.SetStatus(self.radioButton_otf, false);
                            }
                        }));
                    }
                    if (file_self)
                    {
                        string path = $"{SelfPath}\\MHYLAUNCHER_GO.exe";
                        FileStream stream;
                        if (File.Exists(path))
                        {
                            stream = File.Open(path, FileMode.Truncate, FileAccess.Write);
                        }
                        else
                        {
                            stream = File.Open(path, FileMode.CreateNew, FileAccess.Write);
                        }
                        tasks.Add(HTML.FetchHtmlAsync(urls[3], stream).ContinueWith(result =>
                        {
                            if (result.IsCompleted && result.Result == true)
                            {
                                try
                                {
                                    using (stream)
                                    {
                                        stream.Flush();
                                    }
                                    self.SetStatus(self.checkBox_self, true);
                                }
                                catch (Exception) { self.SetStatus(self.checkBox_self, false); }
                            }
                            else
                            {
                                self.SetStatus(self.checkBox_self, false);
                            }
                        }));
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                OpenWindow(new Download(global, Download));
                Environment.Exit(0);
            }
            StringBuilder sbuilder = new StringBuilder(short.MaxValue);
            string url;
            if (global)
            {
                bool l1 = false;
                HTML.FetchHtmlAsync(HTML._UPDATE, sbuilder).ContinueWith(result =>
                {
                    if (result.IsCompleted && result.Result == true)
                    {
                        l1 = true;
                    }
                }).Wait();
                if (l1)
                {
                    url = sbuilder.ToString();
                }
                else
                {
                    url = HTML.EXE;
                }
            }
            else
            {
                url = HTML.EXE;
            }
            HTML.FetchHtmlAsync(url).ContinueWith(result =>
            {
                if (result.IsCompleted && result.Result != null && result.Result.Length > 0)
                {
                    BASE.DATA = result.Result;
                }
            }).Wait();
            if (BASE.DATA == null || BASE.DATA.Length == 0)
            {
                throw new ArgumentNullException("读取远程文件失败(文件为空)。");
            }
            Assembly assembly = Assembly.Load(BASE.DATA);
            MethodInfo entrypoint = assembly.EntryPoint;
            if (entrypoint == null)
            {
                throw new ArgumentNullException("无法找到远程可执行文件的入口点。");
            }
            else
            {
                StartRun(entrypoint, args);
            }
            Environment.Exit(0);
        }

        private static void OpenWindow(Form form)
        {
            Application.Run(form);
        }

        private static void StartRun(MethodInfo entrypoint, string[] args)
        {
            //由于程序仅加载至内存，相关内部资源无法在存储设备中检索到，程序执行会出现错误，因此项目终结
            entrypoint.Invoke(null, args);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void HandleException(Exception e)
        {
            StringBuilder exp = new StringBuilder(short.MaxValue);
            exp.Append(
                $"[应用程序内部异常] [{DateTime.Now:yyyy/MM/dd HH:mm:ss}]"
                + $"\n\n根命名空间:{e.Source}"
                + $"\n方法体:{e.TargetSite}");
            if (e is AggregateException ae && ae.InnerException != null)
            {
                exp.Append(
                    $"\nInnerException:{e.InnerException.GetType().Name}"
                    + $"\n    根命名空间:{e.InnerException.Source}"
                    + $"\n    方法体:{e.InnerException.TargetSite}"
                    + $"\n    详细信息:\n        {e.InnerException.Message}"
                    + $"{(Regex.IsMatch(e.InnerException.Message, @"\n\z") ? string.Empty : "\n")}"
                    + $"    位置:");
                if (string.IsNullOrEmpty(e.InnerException.StackTrace)
                    || !e.InnerException.StackTrace.Contains("\n"))
                {
                    exp.Append($"\n        {e.InnerException.StackTrace.Trim()}");
                }
                else
                {
                    foreach (string st in e.InnerException.StackTrace.Split('\n'))
                    {
                        exp.Append($"\n        {st.Trim()}");
                    }
                }
                exp.Append("\n\nMHYLAUNCHER_GO V2 (online) - Exceptions Processed By FeiLingshu");
            }
            else
            {
                exp.Append(
                    $"\n详细信息:{e.GetType().Name}\n    {e.Message}"
                    + $"{(Regex.IsMatch(e.Message, @"\n\z") ? string.Empty : "\n")}"
                    + $"位置:");
                if (string.IsNullOrEmpty(e.StackTrace)
                    || !e.StackTrace.Contains("\n"))
                {
                    exp.Append($"\n    {e.StackTrace.Trim()}");
                }
                else
                {
                    foreach (string st in e.StackTrace.Split('\n'))
                    {
                        exp.Append($"\n    {st.Trim()}");
                    }
                }
                exp.Append("\n\nMHYLAUNCHER_GO V2 (online) - Exceptions Processed By FeiLingshu");
            }
            MessageBox.Show(
                exp.ToString(),
                $"MHYLAUNCHER_GO V2 (online)",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
            exp.Clear();
            Environment.Exit(int.MinValue);
        }
    }
}
