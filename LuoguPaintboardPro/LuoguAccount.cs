using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace LuoguPaintboardPro
{
    class LuoguAccount
    {
        public string Uid { get; private set; }
        public string OriginalCookie { get; private set; }

        public int FailureCount { get; set; } = 0;
        public DateTime ReadyTime { get; set; }

        HttpClient client;

        public LuoguAccount(string cookieText)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36 Edg/79.0.309.56");
            client.DefaultRequestHeaders.Add("cookie", cookieText);
            OriginalCookie = cookieText;
            Uid = Regex.Match(cookieText, @"_uid=(\d*)").Captures[0].Value[5..];
            ReadyTime = DateTime.Now;
        }

        public async Task<string[]> GetBoard()
        {
            var response = await client.GetAsync("https://www.luogu.com.cn/paintBoard/board");
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"使用账号 {Uid} 时 Http 状态码异常! 返回状态码: {response.StatusCode}");
            }
            var result = (await response.Content.ReadAsStringAsync()).Split('\n');
            if (result.Length < 2) throw new Exception($"使用账号 {Uid} 时获取画板失败! 返回状态码: {response.StatusCode}");
            return result;
        }

        public async Task<bool> Paint(int x, int y, int color)
        {
            try
            {
                Console.WriteLine($"正在使用账号 {Uid} 绘制 ({x}, {y}), 颜色 {color}");
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("x", x.ToString()),
                    new KeyValuePair<string, string>("y", y.ToString()),
                    new KeyValuePair<string, string>("color", color.ToString())
                });
                var response = await client.PostAsync("https://www.luogu.com.cn/paintBoard/paint", content);
                if (!response.IsSuccessStatusCode)
                {
                    FailureCount++;
                    Console.WriteLine($"使用账号 {Uid} 时 Http 状态码异常! 返回状态码: {response.StatusCode}");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    return false;
                }
                else
                {
                    var res = await response.Content.ReadAsStringAsync();
                    if (res.Contains("200"))
                    {
                        Console.WriteLine($"使用账号 {Uid} 绘制成功");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"使用账号 {Uid} 时洛谷返回了错误!");
                        Console.WriteLine(res);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"使用账号 {Uid} 时出现了异常!");
                Console.WriteLine(ex.Message);
                FailureCount++;
                return false;
            }
        }
    }

    class LuoguAccountComparer : IComparer<LuoguAccount>
    {
        int IComparer<LuoguAccount>.Compare(LuoguAccount x, LuoguAccount y)
        {
            return x.ReadyTime.CompareTo(y.ReadyTime);
        }
    }
}
