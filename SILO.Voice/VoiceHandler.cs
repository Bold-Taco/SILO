using SILO.Sonos.SonosUPnP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SILO.Voice
{
    public class VoiceHandler
    {
        SonosHandler sonos;
        SpeechRecognitionEngine recognizer;

        public VoiceHandler(SonosHandler sonos)
        {
            this.sonos = sonos;

            initializeGrammar(sonos.getZones());
        }

        private void initializeGrammar(IEnumerable<string> locationList)
        {
            // Create a new SpeechRecognitionEngine instance (will be specific to this process, not shared with the system)
            recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            Grammar g = createGrammar(locationList);

            // load the grammar into the speech recognizer
            recognizer.LoadGrammar(g);

            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_SpeechRecognized);
            recognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sr_SpeechRecognitionRejected);
            recognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(recognizer_SpeechDetected);

            // configure input to the speech recognizer
            recognizer.SetInputToDefaultAudioDevice(); // could change this if necessary

            // start asynchronous, continuous speech recognition
            recognizer.RecognizeAsync(RecognizeMode.Multiple); // can stop listening at any time
        }

        void recognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            Console.WriteLine("something");
        }

        void sr_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.85) // only trust things where the confidence is extremely high
            {
                Console.WriteLine(e.Result.Text + "(" + e.Result.Confidence + ")");
                SpeechSynthesizer reader = new SpeechSynthesizer();

                var s = e.Result.Semantics;

                if (s.ContainsKey("Function"))
                {
                    Console.Write("Function" + s["Function"].Value + ", Command: " + s["Command"].Value + ", Location: " + s["Location"].Value);
                    switch (s["Function"].Value.ToString())
                    {
                        case "Sonos":
                            sonos.parseVoiceCommand(s["Command"].Value.ToString(), s["Location"].Value.ToString());
                            break;
                        case "Hue":
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    // just recognized "silo", blink a light to show that it's listening
                }
            }
        }

        void sr_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine(e.Result.Text + "(" + e.Result.Confidence + ")");
        }

        private Grammar createGrammar(IEnumerable<string> locationList)
        {
            // load in the grammar XML file
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlReader.Create(new MemoryStream(Properties.Resources.grammar)));

            // Add the namespace info
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("gr", "http://www.w3.org/2001/06/grammar");

            // navigate to the location node in the XML file
            XmlNode locationNode = doc.DocumentElement.SelectNodes("/gr:grammar/gr:rule[@id='id_Location']", nsmgr)[0].FirstChild;

            // populate with locations that we expect the user to say
            foreach (string location in locationList)
            { 
                // need to specify the namespace here or else the new entries won't be recognized
                XmlElement elem = doc.CreateElement("item", @"http://www.w3.org/2001/06/grammar");
                elem.InnerText = location;

                locationNode.AppendChild(elem);
            }

            // for testing - write the modified grammar out to the screen
            doc.Save(Console.Out);

            // load the modified grammar into the final variable
            Stream grammarStream = new MemoryStream();
            doc.Save(grammarStream);

            // make sure we are at the beginning of the stream
            grammarStream.Flush();
            grammarStream.Position = 0;

            // return the final grammar
            return new Grammar(grammarStream);
        }
    }
}
