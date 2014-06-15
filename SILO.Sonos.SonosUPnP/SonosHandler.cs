using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SILO.Sonos.SonosUPnP
{
    public class SonosHandler
    {
        private SonosDiscovery d;

        public SonosHandler()
        {
            d = new SonosDiscovery();
            d.StartScan();
            Thread.Sleep(5000);
        }

        public void parseVoiceCommand(String command, params string[] locations)
        {
            switch ((CommandList)Enum.Parse(typeof(CommandList), command, true))
            {
                case CommandList.Play:
                    PlayZone(locations);
                    break;
                case CommandList.Pause:
                    PauseZone(locations);
                    break;
                default:
                    break;
            }
        }

        #region "get Sonos metadata"
        public IEnumerable<string> getZones()
        {
            var result = new SortedSet<string>();
            var zoneList = (d.Zones.Select(zone => zone.Name));

            foreach (var zone in zoneList)
                foreach (var zoneSplit in zone.Split('+'))
                    result.Add(zoneSplit.Trim());

            return result;
        }
        #endregion

        #region "pause actions"
        public void PauseZone(params string[] zoneSubset)
        {
            var selectedZones = d.Zones.Where(z => zoneSubset.Any(zs => z.Name.Contains(zs)));

            if (selectedZones.Count() > 0)
            {
                foreach (SonosZone z in selectedZones)
                {
                    z.Coordinator.Pause();
                }
            }
            else
            {
                // all zones
                foreach (SonosZone z in d.Zones)
                    z.Coordinator.Pause();
            }
        }
        #endregion

        #region "resume actions"
        public void PlayZone(params string[] zoneSubset)
        {
            var selectedZones = d.Zones.Where(z => zoneSubset.Any(zs => z.Name.Contains(zs)));

            if (selectedZones.Count() > 0)
            {
                foreach (SonosZone z in selectedZones)
                {
                    z.Coordinator.Play();
                }
            }
            else
            {
                // all zones
                foreach (SonosZone z in d.Zones)
                    z.Coordinator.Play();
            }
        }
        #endregion
    }
}
