using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace SILO.Voice
{
    public class VoiceHandler
    {
        public VoiceHandler()
        {
            // Create a new SpeechRecognitionEngine instance (will be specific to this process, not shared with the system)
            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US")))
            {
                // load in the grammar XML file
                Stream grammarStream = new MemoryStream(Properties.Resources.grammar);
                Grammar g = new Grammar(grammarStream);

                // load the grammar into the speech recognizer
                recognizer.LoadGrammar(g);

                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sr_SpeechRecognized);
                recognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sr_SpeechRecognitionRejected);

                // configure input to the speech recognizer
                recognizer.SetInputToDefaultAudioDevice(); // could change this if necessary

                // start asynchronous, continuous speech recognition
                recognizer.RecognizeAsync(RecognizeMode.Multiple); // can stop listening at any time

                while (true) { }
            }
        }

        static void sr_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.Result.Text + "(" + e.Result.Confidence + ")");

            if (e.Result.Confidence > 0.95) // only trust things where the confidence is extremely high
            {
                SpeechSynthesizer reader = new SpeechSynthesizer();
                reader.SpeakAsync("I heard you say: " + e.Result.Text);
                Console.WriteLine("finished speaking");
            }


            var s = e.Result.Semantics;
        }

        static void sr_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine(e.Result.Text + "(" + e.Result.Confidence + ")");
        }
    }
}
