using SILO.Sonos.SonosUPnP;
using SILO.Voice;
using System;
using System.Linq;
using System.Threading;

namespace SILO_Kristen
{
    class Program
    {
        static void Main(string[] args)
        {
            #region "initialize all functionality"
            SonosHandler sonos = new SonosHandler();
            #endregion

            VoiceHandler vh = new VoiceHandler(sonos);

            while (true) { }
        }
    }
}
