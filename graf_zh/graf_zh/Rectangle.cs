using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace graf_zh
{
    class Rectangle
    {
        public PointF topLeft;
        public Color color;
        public int width;
        public int height;

        public Rectangle(PointF topLeft, Color color, int width = 50, int height = 50)
        {
            this.topLeft = topLeft;
            this.color = color;
            this.width = width;
            this.height = height;
        }
    }
}
