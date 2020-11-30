using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graf_zh
{
    public partial class Form1 : Form
    {
        Graphics g;
        Random rnd = new Random();
        List<Rectangle> rects;
        Rectangle rect;
        float dx, dy;

        int gotcha = -1;

        public Form1()
        {
            InitializeComponent();
            rects = new List<Rectangle>();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;

            for (int i = 0; i < rects.Count - 1; i++)
            {
                PointF difRightUp = new Point((int)rects[i].topLeft.X + rects[i].width, (int)rects[i].topLeft.Y);
                PointF difLeftUp = new PointF((int)rects[i + 1].topLeft.X - rects[i + 1].width, (int)rects[i + 1].topLeft.Y);

                PointF difRightDown = new Point((int)rects[i].topLeft.X - rects[i].width, (int)rects[i].topLeft.Y + rects[i].height);
                PointF difLeftDown = new PointF((int)rects[i + 1].topLeft.X, (int)rects[i + 1].topLeft.Y + rects[i].height);

                g.FillRectangle(new SolidBrush(rects[i].color), rects[i].topLeft.X, rects[i].topLeft.Y, rects[i].width, rects[i].height);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                DrawHermiteArc(Color.Blue, Color.Red, difRightUp, rects[i + 1].topLeft,
                    Mult(Mult(Subs(rects[i].topLeft, difRightUp), -1), 2), Mult(Mult(Subs(difLeftUp, rects[i + 1].topLeft), -1), 2));

                g.FillRectangle(new SolidBrush(rects[i].color), rects[i].topLeft.X, rects[i].topLeft.Y, rects[i].width, rects[i].height);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                DrawHermiteArc(Color.Blue, Color.Red, difRightDown, difLeftDown,
                    Mult(Mult(Subs(rects[i].topLeft, difRightDown), -1), 2), Mult(Mult(Subs(difLeftDown, difLeftDown), -1), 2));
            }




        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            rect = rects.FirstOrDefault(i =>
            i.topLeft.X < e.Location.X && i.topLeft.Y < e.Location.Y && i.topLeft.X + i.width > e.Location.X && i.topLeft.Y + i.height > e.Location.Y);
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        foreach (Rectangle rectangle in rects)
                        {
                            if (rectangle.topLeft.X <= e.X && e.X <= rectangle.topLeft.X + rectangle.width &&
                            rectangle.topLeft.Y <= e.Y && e.Y <= rectangle.topLeft.Y + rectangle.height)
                            {
                                gotcha = 1;
                                dx = e.X - rectangle.topLeft.X;
                                dy = e.Y - rectangle.topLeft.Y;
                            }
                        }
                        if (rect != null)
                        {
                            if (rect.topLeft.X + rect.width - 20 <= e.Location.X && e.Location.X <= rect.topLeft.X + rect.width + 20 &&
                            rect.topLeft.Y + rect.height - 20 <= e.Location.Y && e.Location.Y <= rect.topLeft.Y + rect.height + 20)
                            {
                                gotcha = 2;
                                dx = e.X - rect.topLeft.X;
                                dy = e.Y - rect.topLeft.Y;
                            }
                        }
                        break;
                    }
                case MouseButtons.Left:
                    {
                        Rectangle tmp = new Rectangle(new PointF(e.X, e.Y), Color.FromArgb(rnd.Next(255), rnd.Next(255), rnd.Next(255)));
                        rects.Add(tmp);
                        break;
                    }
                default: break;
            }

            canvas.Refresh();
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            gotcha = -1;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {


            if (rect != null)
            {
                if (gotcha == 1)
                {
                    rect.topLeft = new PointF(e.Location.X - dx, e.Location.Y - dy);
                }
                else if (gotcha == 2)
                {
                    rect.width = (int)(e.X - rect.topLeft.X);
                    rect.height = (int)(e.Y - rect.topLeft.Y);
                }
            }
            canvas.Refresh();
        }

        private double H0(double t) { return 2 * t * t * t - 3 * t * t + 1; }
        private double H1(double t) { return -2 * t * t * t + 3 * t * t; }
        private double H2(double t) { return t * t * t - 2 * t * t + t; }
        private double H3(double t) { return t * t * t - t * t; }

        private void DrawHermiteArc(Color c1, Color c2, PointF p0, PointF p1, PointF t0, PointF t1)
        {
            Brush aGradientBrush = new LinearGradientBrush(p0, p1, Color.Blue, Color.Red);
            Pen aGradientPen = new Pen(aGradientBrush);

            double t = 0;
            double h = 1.0 / 500.0;
            PointF d0, d1;
            d0 = new PointF((float)(H0(t) * p0.X + H1(t) * p1.X + H2(t) * t0.X + H3(t) * t1.X),
                            (float)(H0(t) * p0.Y + H1(t) * p1.Y + H2(t) * t0.Y + H3(t) * t1.Y));

            while (t < 1)
            {
                t += h;
                d1 = new PointF((float)(H0(t) * p0.X + H1(t) * p1.X + H2(t) * t0.X + H3(t) * t1.X),
                                (float)(H0(t) * p0.Y + H1(t) * p1.Y + H2(t) * t0.Y + H3(t) * t1.Y));
                g.DrawLine(aGradientPen, d0, d1);
                d0 = d1;
            }
        }

        private PointF Add(PointF a, PointF b) { return new PointF(b.X + a.X, b.Y + a.Y); }
        private PointF Subs(PointF a, PointF b) { return new PointF(b.X - a.X, b.Y - a.Y); }
        private PointF Mult(PointF a, float l) { return new PointF(a.X * l, a.Y * l); }

    }
}
