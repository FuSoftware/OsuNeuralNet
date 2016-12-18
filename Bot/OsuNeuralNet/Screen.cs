using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuNeuralNet
{
    class Screen
    {
        public static List<Color> getColors(int x = 0, int y = 0, int h = 1600, int w = 900)
        {
            List<Color> c = new List<Color>();

            for(int i=0;i<w;i++)
            {
                for (int j = 0; j < h; j++)
                {
                    c.Add(Win32.GetColorAt(new Point(j+x, i+y)));
                }
            }

            return c;
        }
    }
}
