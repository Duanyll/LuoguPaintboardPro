using System;
using System.Collections.Generic;
using System.Text;

namespace LuoguPaintboardPro
{
    struct PointToDraw
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Color { get; set; }
        public int R { get; set; }

        static Random rand = new Random();

        public PointToDraw(int x, int y, int color)
        {
            X = x;
            Y = y;
            Color = color;
            R = rand.Next();
        }
    }

    class PointToDrawComparer : IComparer<PointToDraw>
    {
        int IComparer<PointToDraw>.Compare(PointToDraw x, PointToDraw y)
        {
            // NOTE: 在此修改按顺序画还是随机画
            return x.R.CompareTo(y.R);
            // return x.Y.CompareTo(y.Y) == 0 ? x.X.CompareTo(y.X) : x.Y.CompareTo(y.Y);
        }
    }
}
