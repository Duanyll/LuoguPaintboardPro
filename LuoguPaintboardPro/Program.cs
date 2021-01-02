using System;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;

namespace LuoguPaintboardPro
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption("-h|--help");
            app.VersionOption("-v|--version", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            app.Name = "LuoguPaintboardPro";
            app.Description = "洛谷冬日滑板辅助器增强版.";

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            app.Command("genpic", command =>
            {
                command.Description = "将彩色图片处理为 32 色图.";
                command.HelpOption("-h|--help");

                var inputFileArg = command.Argument("inputFile", "输入的 png 图像.");
                var outputFileArg = command.Argument("[outputFile]", "输出文件.");

                command.OnExecute(() =>
                {
                    ImageProcessor.ProcessImage(inputFileArg.Value, outputFileArg.Value);
                    return 0;
                });
            });

            app.Command("draw", command =>
            {
                command.Description = "在指定坐标绘制图片, 并监视保护.";
                command.HelpOption("-h|--help");

                var sXArg = command.Argument("xpos", "绘制图片位置的 x 坐标");
                var sYArg = command.Argument("ypos", "绘制图片位置的 Y 坐标");
                var imageFileArg = command.Argument("image", "由 genpic 生成的代表图片的 txt");
                var cookieFileArg = command.Argument("cookie", "包含要使用的 cookie 的 txt 文件, 一行一个 cookie.");

                command.OnExecute(() =>
                {
                    int sx = int.Parse(sXArg.Value);
                    int sy = int.Parse(sYArg.Value);
                    var image = File.ReadAllText(imageFileArg.Value ?? "data.txt").Split('\n');
                    var cookie = File.ReadAllText(cookieFileArg.Value ?? "cookie.txt").Split('\n');
                    int w = 0, h = 0;
                    for (int i = 0; i < image.Length; i++)
                    {
                        image[i] = image[i].Trim();
                        if (!string.IsNullOrWhiteSpace(image[i]))
                        {
                            h++;
                            w = Math.Max(w, image[i].Length);
                        }
                    }
                    Console.WriteLine($"读取到图片 {imageFileArg.Value ?? "data.txt"}, 宽 {w}, 高 {h}");
                    var imageArray = new char[h, w];
                    for (int i = 0; i < h; i++)
                    {
                        for (int j = 0; j < w; j++)
                        {
                            imageArray[i, j] = image[i][j];
                        }
                    }
                    if (sy + h > 600 || sx + w > 1000) {
                        Console.WriteLine("坐标超出范围了！");
                        return 1;
                    }
                    var opr = new PaintboardOperator(cookie);
                    var task = opr.Work(imageArray, w, h, sx, sy);
                    Console.CancelKeyPress += (s, args) =>
                    {
                        Console.WriteLine("已终止程序.");
                    };
                    task.Wait();
                    return 0;
                });
            });

            app.Execute(args);
        }
    }
}
