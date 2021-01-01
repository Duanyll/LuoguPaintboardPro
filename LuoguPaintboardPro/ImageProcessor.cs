using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;

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

        static int getClosestColorIndex(Vector3 color)
        {
            int result = 0;
            float resultDis = float.PositiveInfinity;
            for (int i = 0; i < BoardColors.Length; i++)
            {
                var cur = BoardColors[i];
                float dis = (cur.R - color.X) * (cur.R - color.X) + (cur.G - color.Y) * (cur.G - color.Y) + (cur.B - color.Z) * (cur.B - color.Z);
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

            var vecImg = new Vector3[image.Height, image.Width];

            Vector3 colorToVector(Color c) => new Vector3(c.R, c.G, c.B);

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    vecImg[y, x] = colorToVector(image.GetPixel(x, y));
                }
            }

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var color = vecImg[y, x];
                    var newColorIndex = getClosestColorIndex(color);
                    result[y, x] = indexToChar(newColorIndex);
                    preview.SetPixel(x, y, BoardColors[newColorIndex]);
                    var newColor = colorToVector(BoardColors[newColorIndex]);
                    var error = color - newColor;

                    if (x < image.Width - 1) vecImg[y, x + 1] += error * 7 / 16;
                    if (y < image.Height - 1) {
                        if (x > 0) vecImg[y + 1, x - 1] += error * 3 / 16;
                        vecImg[y + 1, x] += error * 5 / 16;
                        if (x < image.Width - 1) vecImg[y + 1, x + 1] += error * 1 / 16;
                    }
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
