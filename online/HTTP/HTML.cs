using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace online.HTTP
{
    public static class HTML
    {
        public readonly static string UPDATE = "https://gitee.com/FeiLingshu/mhyLauncher_Go_mirror/raw/master/version";
        public readonly static string _UPDATE = "https://github.com/FeiLingshu/mhyLauncher_Go/raw/refs/heads/resources/version";
        public readonly static string EXE = "https://gitee.com/FeiLingshu/mhyLauncher_Go_mirror/raw/master/source";
        public readonly static string[] DLL = new string[1] { "https://gitee.com/FeiLingshu/mhyLauncher_Go_mirror/raw/master/EfficiencyMode/EfficiencyMode.1" };
        public readonly static string[] MP4 = new string[2] {
            "https://gitee.com/FeiLingshu/mhyLauncher_Go_mirror/raw/master/video/video.1",
            "https://gitee.com/FeiLingshu/mhyLauncher_Go_mirror/raw/master/video/video.2"};
        public readonly static string[] OTF = new string[2] {
            "https://gitee.com/FeiLingshu/mhyLauncher_Go_mirror/raw/master/GiteeGit/GiteeGit.1",
            "https://gitee.com/FeiLingshu/mhyLauncher_Go_mirror/raw/master/GiteeGit/GiteeGit.2"};
        public readonly static string _EXE = "https://github.com/FeiLingshu/mhyLauncher_Go/raw/refs/heads/resources/source";
        public readonly static string[] _DLL = new string[1] { "https://github.com/FeiLingshu/mhyLauncher_Go/raw/refs/heads/resources/EfficiencyMode/EfficiencyMode.1" };
        public readonly static string[] _MP4 = new string[2] {
            "https://github.com/FeiLingshu/mhyLauncher_Go/raw/refs/heads/resources/video/video.1",
            "https://github.com/FeiLingshu/mhyLauncher_Go/raw/refs/heads/resources/video/video.2"};
        public readonly static string[] _OTF = new string[2] {
            "https://github.com/FeiLingshu/mhyLauncher_Go/raw/refs/heads/resources/GiteeGit/GiteeGit.1",
            "https://github.com/FeiLingshu/mhyLauncher_Go/raw/refs/heads/resources/GiteeGit/GiteeGit.2"};

        private static readonly HttpClient _client = new HttpClient();

        public static void Fake()
        {
            _client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/149.0.0.0 Safari/537.36 Edg/149.0.0.0");
            _client.DefaultRequestHeaders.Add("Referer", "https://example.com/");
            _client.DefaultRequestHeaders.Add("Accept", "*/*");
            _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };
            _client.DefaultRequestHeaders.Pragma.ParseAdd("no-cache");
            _client.Timeout = TimeSpan.FromSeconds(30);
        }

        public static async Task<bool> FetchHtmlAsync(string[] url, FileStream fs)
        {
            try
            {
                foreach (var _url in url)
                {
                    HttpResponseMessage response = await _client.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        await stream.CopyToAsync(fs);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.Print(ex.ToString());
                }
                return false;
            }
        }

        public static async Task<byte[]> FetchHtmlAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                return bytes;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.Print(ex.ToString());
                }
                return null;
            }
        }

        public static async Task<bool> FetchHtmlAsync(string url, StringBuilder sbuilder)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string output = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(output))
                {
                    return false;
                }
                else
                {
                    output = output.Trim();
                    output.Replace("tag", "download");
                    output += "/MHYLAUNCHER_GO.exe";
                    if (output.IsValidUrl() && output.SafeCheck())
                    {
                        sbuilder.Append(output);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.Print(ex.ToString());
                }
                return false;
            }
        }

        private static bool IsValidUrl(this string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                return false;
            return uri.Scheme == Uri.UriSchemeHttps;
        }

        private static bool SafeCheck(this string url)
        {
            if (url.StartsWith("https://gitee.com/FeiLingshu/mhyLauncher_Go_mirror")
                || url.StartsWith("https://github.com/FeiLingshu/mhyLauncher_Go"))
            {
                return true;
            }
            return false;
        }
    }
}
