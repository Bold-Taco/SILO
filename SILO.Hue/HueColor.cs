using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SILO.Hue
{
    class HueColor
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


    }
}
