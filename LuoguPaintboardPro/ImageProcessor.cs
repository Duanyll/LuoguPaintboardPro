using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace LuoguPaintboardPro
{
    static class ImageProcessor
    {
        static readonly string[] BoardColorStrings = { "rgb(0, 0, 0)", "rgb(255, 255, 255)", "rgb(170, 170, 170)", "rgb(85, 85, 85)", "rgb(254, 211, 199)", "rgb(255, 196, 206)", "rgb(250, 172, 142)", "rgb(255, 139, 131)", "rgb(244, 67, 54)", "rgb(233, 30, 99)", "rgb(226, 102, 158)", "rgb(156, 39, 176)", "rgb(103, 58, 183)", "rgb(63, 81, 181)", "rgb(0, 70, 112)", "rgb(5, 113, 151)", "rgb(33, 150, 243)", "rgb(0, 188, 212)", "rgb(59, 229, 219)", "rgb(151, 253, 220)", "rgb(22, 115, 0)", "rgb(55, 169, 60)", "rgb(137, 230, 66)", "rgb(215, 255, 7)", "rgb(255, 246, 209)", "rgb(248, 203, 140)", "rgb(255, 235, 59)", "rgb(255, 193, 7)", "rgb(255, 152, 0)", "rgb(255, 87, 34)", "rgb(184, 63, 39)", "rgb(121, 85, 72)" };
        static Color[] BoardColors = null;

        static void prepareColors()
        {
            if (BoardColors == null)
            {
                BoardColors = new Color[BoardColorStrings.Length];
                for (int i = 0; i < BoardColorStrings.Length; i++)
                {
                    var s = BoardColorStrings[i][4..^1].Split(", ");
                    BoardColors[i] = Color.FromArgb(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]));
                }
            }
        }

        static int getClosestColorIndex(Color color)
        {
            int result = 0;
            int resultDis = 0x3f3f3f3f;
            for (int i = 0; i < BoardColors.Length; i++)
            {
                var cur = BoardColors[i];
                int dis = (cur.R - color.R) * (cur.R - color.R) + (cur.G - color.G) * (cur.G - color.G) + (cur.B - color.B) * (cur.B - color.B);
                if (dis < resultDis)
                {
                    result = i;
                    resultDis = dis;
                }
            }
            return result;
        }

        static char indexToChar(int index)
        {
            return index < 10 ? (char)('0' + index) : (char)('a' + index - 10);
        }

        public static void ProcessImage(string input, string output)
        {
            prepareColors();
            Console.WriteLine($"输入文件: {input}");
            if (string.IsNullOrEmpty(output)) output = "data.txt";

            var image = new Bitmap(input);
            var result = new char[image.Height, image.Width];
            var preview = new Bitmap(image.Width, image.Height);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var color = image.GetPixel(x, y);
                    result[y, x] = indexToChar(getClosestColorIndex(color));
                    preview.SetPixel(x, y, BoardColors[getClosestColorIndex(color)]);
                }
            }

            var outFile = new StreamWriter(output);
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    outFile.Write(result[i, j]);
                }
                outFile.Write(Environment.NewLine);
            }
            outFile.Flush();
            outFile.Close();

            preview.Save("preview.png");

            Console.WriteLine($"图片处理完成, 输出文件 {output}");
        }
    }
}
