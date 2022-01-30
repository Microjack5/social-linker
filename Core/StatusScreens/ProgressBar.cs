using System.Drawing;

namespace SocialLinker.Core.StatusScreens
{
    public class ProgressBar
    {
        Brush c;
        Rectangle r;

        Bitmap btm;
        Graphics g;

        int max;

        int current = 0;

        public ProgressBar(Brush BarColor, Rectangle Area, int MaxValue)
        {
            c = BarColor;
            btm = new Bitmap(Area.Width + 2, Area.Height + 2);
            g = Graphics.FromImage(btm);

            r = Area;
            max = MaxValue;
        }

        public void SetCurrent(int cr)
        {
            current = cr;
        }

        public Point GiveCorner()
        {
            return new Point(r.X, r.Y);
        }

        public Bitmap GiveGraphic()
        {
            float percent = (float)current / (float)max;

            int actual = (int)(percent * r.Width);

            g.Clear(System.Drawing.Color.FromArgb(0, 0, 0, 0));
            g.FillRectangle(c, new Rectangle(0, 0, actual, r.Height));
            g.DrawRectangle(new Pen(System.Drawing.Color.FromArgb(0, 0, 0, 0)), new Rectangle(0, 0, r.Width, r.Height)); //c

            return btm;
        }
    }
}
