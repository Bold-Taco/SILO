using SILO.Hue;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILO
{
    class Program
    {
        static void Main(string[] args)
        {
            HueController controller = new HueController();
            controller.TurnOnLights();
            controller.SetColor(Color.White);
            Console.ReadKey();
            controller.PartyMode(10).Wait();
            Console.ReadKey();
            controller.TurnOffLights();
            Console.ReadKey();
        }
    }
}
