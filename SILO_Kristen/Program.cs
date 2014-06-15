using SILO.Sonos.SonosUPnP;
using System;
using System.Linq;
using System.Threading;

namespace SILO_Kristen
{
    class Program
    {
        static void Main(string[] args)
        {
            //VoiceHandler vh = new VoiceHandler();

            SonosDiscovery d = new SonosDiscovery();
            d.StartScan();
            Thread.Sleep(5000);

            int zones = d.Zones.Count;
            int players = d.Players.Count;

            SonosPlayer player = d.Players.First(p => p.Name == "Kitchen");
            player.Pause();

            Console.ReadLine();

            player.Play();
        }
    }
}
