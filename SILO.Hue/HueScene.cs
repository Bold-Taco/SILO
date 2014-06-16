using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace SILO.Hue
{
    public class HueScene
    {
        public string Name { get; set; }
        public List<HueColor> Colors { get; set; }
        private ResourceManager ResMan;

        public HueScene(string SceneName)
        {
            //dynamically load resource file
            ResMan = new ResourceManager("SILO.Hue.Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());

            var colors = ResMan.GetString(SceneName);
            Name = SceneName;
            //parse colors from comma list
            Colors = new List<HueColor>();
            foreach (var color in colors.Split(','))
            {
                //convert to color
                Color newColor = ColorTranslator.FromHtml("#" + color);
                Colors.Add(new HueColor(newColor));
            }
        }

        public static List<string> GetAllScenes()
        {
            ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            List<string> scenes = new List<string>();
            foreach (DictionaryEntry resource in resourceSet)
            {
                scenes.Add(resource.Key.ToString());
            }
            return scenes;
        }
    }
}
