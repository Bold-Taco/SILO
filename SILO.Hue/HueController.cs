using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SILO.Hue
{
    public class HueController
    {
        public List<Group> Groups { get; set; }

        private HueClient client;
        private string BridgeIp { get; set; }
        private List<Light> Lights { get; set; }
        private string ApiKey
        {
            get
            {
                return HueSettings.Default.HueApiKey;
            }
            set
            {
                HueSettings.Default.HueApiKey = value;
                HueSettings.Default.Save();
            }
        }

        public HueController()
        {
            //find the Hue Bridge
            GetBridge().Wait();
            if (BridgeIp == null)
                throw new Exception("No Hue Bridge found on the network");
            
            //initialize the client
            client = new HueClient(BridgeIp, ApiKey);
            client.Initialize(ApiKey);
            if (!client.IsInitialized)
                throw new Exception(String.Format("Unable to initialize bridge at {0}", BridgeIp));
            
            GetGroups().Wait();
            GetLights().Wait();
            BuildGroups();
            //Test();

        }

        public void TurnOffLights(string GroupName = null)
        {
            var command = new LightCommand();
            command.On = false;
            command.TurnOff();
            if(GroupName == null) //no group, apply to all
                client.SendCommandAsync(command);
            else
                client.SendCommandAsync(command, GetGroupByName(GroupName).Lights);
        }

        public void TurnOnLights(string GroupName = null)
        {
            var command = new LightCommand();
            command.On = true;
            command.TurnOn();
            command.Brightness = (byte)255;
            if (GroupName == null)
                client.SendCommandAsync(command);
            else
                client.SendCommandAsync(command, GetGroupByName(GroupName).Lights);
        }

        public void SetColor(Color newColor, string GroupName = null)
        {
            var hColor = new HueColor(newColor);
            var command = new LightCommand();
            command.On = true;
            command.TurnOn().SetColor(hColor.HexColor);
            if (GroupName == null)
                client.SendCommandAsync(command);
            else
                client.SendCommandAsync(command, GetGroupByName(GroupName).Lights);
        }


        /// <summary>
        /// Sets the brightness based on the lights existing brightness  
        /// </summary>
        /// <param name="Percentage"></param>
        /// <param name="GroupName"></param>
        public void ModifyBrightness(double Percentage, string GroupName = null)
        {
            var command = new LightCommand();
            command.On = true;
            if (GroupName != null)
            {
                foreach (var lightId in GetGroupByName(GroupName).Lights)
                {
                    var curLight = client.GetLightAsync(lightId).Result;
                    var newBrightness = (byte)(curLight.State.Brightness * Percentage);
                    command.Brightness = newBrightness;
                    client.SendCommandAsync(command, new List<string>(){(lightId)});
                }
            }
            else
            {
                foreach (var light in Lights)
                {
                    var curLight = client.GetLightAsync(light.Id).Result;
                    var newBrightness = (byte)(curLight.State.Brightness * Percentage);
                    command.Brightness = newBrightness;
                    client.SendCommandAsync(command, new List<string>() { (light.Id) });
                }
            }
        }

        /// <summary>
        /// Sets the brightness to an absolute percentage of the total possible brightness
        /// </summary>
        /// <param name="Percentage"></param>
        /// <param name="GroupName"></param>
        public void SetBrightness(double Percentage, string GroupName = null)
        {
            var command = new LightCommand();
            command.On = true;
            if (GroupName != null)
            {
                foreach (var lightId in GetGroupByName(GroupName).Lights)
                {
                    var newBrightness = (byte)(255 * Percentage);
                    command.Brightness = newBrightness;
                    client.SendCommandAsync(command, new List<string>() { (lightId) });
                }
            }
            else
            {
                foreach (var light in Lights)
                {
                    var newBrightness = (byte)(255 * Percentage);
                    command.Brightness = newBrightness;
                    client.SendCommandAsync(command, new List<string>() { (light.Id) });
                }
            }
        }

        public void Alert(Color NewColor = new Color(), bool BlinkMany = false, string GroupName = null)
        {
            var command = new LightCommand();
            command.On = true;
            command.Alert = BlinkMany ? Q42.HueApi.Alert.Multiple : Q42.HueApi.Alert.Once;
            //if Alert() is called with no params, then set the color to white. Color.White is not a compile time constant
            HueColor hColor = (NewColor == new Color()) ? new HueColor(Color.White) : new HueColor(NewColor);
            command.SetColor(hColor.HexColor);
            if (GroupName == null)
                client.SendCommandAsync(command);
            else
                client.SendCommandAsync(command, GetGroupByName(GroupName).Lights);
        }

        public void StopAlert(string GroupName = null)
        {
            var command = new LightCommand();
            command.On = true;
            command.Alert = Q42.HueApi.Alert.None;
            if (GroupName == null)
                client.SendCommandAsync(command);
            else
                client.SendCommandAsync(command, GetGroupByName(GroupName).Lights);
        }

        public void CycleColors(string GroupName = null)
        {
            var command = new LightCommand();
            command.On = true;
            command.Effect = Effect.ColorLoop;
            if (GroupName == null)
                client.SendCommandAsync(command);
            else
                client.SendCommandAsync(command, GetGroupByName(GroupName).Lights);
        }

        public void StopCycle(string GroupName = null)
        {
            var command = new LightCommand();
            command.On = true;
            command.Effect = Effect.None;
            if (GroupName == null)
                client.SendCommandAsync(command);
            else
                client.SendCommandAsync(command, GetGroupByName(GroupName).Lights);
        }

        public void Strobe(string GroupName = null)
        {
            throw new MissingMethodException();
            //client.SendCommandRawAsync("0A00F1F01F1F1001F1FF100000000001F2F", new List<string>(){"2"});
        }

        #region Init Tasks

        private async Task GetBridge()
        {
            IBridgeLocator locator = new HttpBridgeLocator();
            //get the first (probably only) bridge
            BridgeIp = (await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5))).First();
        }

        private async Task GetGroups()
        {
            Groups = await client.GetGroupsAsync();
        }

        private async Task GetLights()
        {
            Lights = (await client.GetLightsAsync()).ToList();
        }

        private void BuildGroups()
        {
            foreach (var light in Lights)
            {
                int idCount = 0;
                var groupName = GetGroupNameFromLight(light);
                if (!Groups.Any(g => g.Name == groupName))
                {
                    Group g = new Group();
                    g.Name = groupName;
                    g.Id = (idCount++).ToString();
                    g.Lights = new List<string>();
                    Groups.Add(g);
                }
                //add light to current group
                GetGroupByName(groupName).Lights.Add(light.Id);
            }
        }
        
        #endregion

        #region Helpers

        private string GetGroupNameFromLight(Light light)
        {
            return light.Name.Substring(0, light.Name.LastIndexOf(' '));
        }

        private Group GetGroupByName(string GroupName)
        {
            return Groups.Where(g => g.Name.ToLower() == GroupName.ToLower()).Single();
        }

        private Light GetLightById(string Id)
        {
            return Lights.Where(l => l.Id == Id).Single();
        }


        #endregion
    }
}
