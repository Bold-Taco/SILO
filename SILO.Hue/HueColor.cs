using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SILO.Hue
{
    public class HueColor
    {
        public Color Color { get; set; }
        
        public string HexColor
        {
            get 
            {
                return Color.R.ToString("X2") + Color.G.ToString("X2") + Color.B.ToString("X2");
            }
        }

        public HueColor(Color color)
        {
            this.Color = color;
        }

        public static HueColor RandomHueColor()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            return new HueColor(Color.FromArgb(r.Next(255), r.Next(255), r.Next(255)));
        }


    }
}
