using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Web;

namespace LuoguPaintboardPro
{
    class PaintboardOperator
    {
        PriorityQueue<PointToDraw> pointQueue = new PriorityQueue<PointToDraw>(new PointToDrawComparer());
        Queue<LuoguAccount> accountQueue = new Queue<LuoguAccount>();

        public PaintboardOperator(string[] cookies)
        {
            foreach (var i in cookies)
            {
                if (string.IsNullOrWhiteSpace(i)) continue;
                accountQueue.Enqueue(new LuoguAccount(i));
            }
        }

        int CharToColorIndex(char ch)
        {
            return ch <= '9' ? ch - '0' : ch - 'a' + 10;
        }

        void RefreshPointQueue(char[,] image, int w, int h, int sx, int sy)
        {
            Console.WriteLine($"正在刷新画板");
            try
            {
                var newQueue = new PriorityQueue<PointToDraw>(new PointToDrawComparer());
                var currentBoard = accountQueue.Peek().GetBoard().Result;
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        int x = sx + j;
                        int y = sy + i;
                        if (currentBoard[y][x] != image[i, j])
                        {
                            newQueue.Push(new PointToDraw(x, y, CharToColorIndex(image[i, j])));
                        }
                    }
                }
                pointQueue = newQueue;
            } 
            catch (Exception ex)
            {
                Console.WriteLine("出现了异常!");
                Console.WriteLine(ex.Message);
            }
            
        }

        static readonly TimeSpan CoolDownTime = new TimeSpan(0, 0, 11);
        static readonly TimeSpan NetworkCoolDownTime = new TimeSpan(0, 0, 0, 0, 100);

        public async Task Work(char[,] image, int w, int h, int sx, int sy)
        {
            Console.WriteLine($"正在开始绘制, 预计用时 {CoolDownTime * (w * h / accountQueue.Count)}");
            while (true)
            {
                RefreshPointQueue(image, w, h, sx, sy);
                if (pointQueue.Count == 0)
                {
                    Console.WriteLine("已全部绘制完成, 每隔 5 分钟检测一次破坏情况");
                    Task.Delay(new TimeSpan(0, 5, 0)).Wait();
                    continue;
                }
                while (pointQueue.Count > 0)
                {
                    bool ok = false;
                    var point = pointQueue.Pop();
                    do
                    {
                        var cur = accountQueue.Dequeue();
                        if (DateTime.Now < cur.ReadyTime)
                        {
                            Task.Delay(cur.ReadyTime - DateTime.Now).Wait();
                        }
                        ok = await cur.Paint(point.X, point.Y, point.Color);
                        cur.ReadyTime += CoolDownTime;
                        accountQueue.Enqueue(cur);
                    } while (!ok);
                    Task.Delay(NetworkCoolDownTime).Wait();
                }
            }
        }
    }
}
