using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poubub.HumanInterface
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            Console.WriteLine("------- GetMIDIInDevices -------");
            foreach (var x in Midi.GetMIDIInDevices())
            {
                Console.WriteLine(x);
            }

            Console.WriteLine("------- GetMIDIOutDevices -------");
            foreach (var x in Midi.GetMIDIOutDevices())
            {
                Console.WriteLine(x);
            }

          //  Midi.MidiOutDeviceName = "APC MINI" ;
            Console.WriteLine("------- Manufacturer -------");
            Console.WriteLine(Midi.MidiInDevice().Details.Manufacturer);
            Console.WriteLine("------- Name -------");
            Console.WriteLine(Midi.MidiInDevice().Details.Name);
            Console.WriteLine("------- Version -------");
            Console.WriteLine(Midi.MidiInDevice().Details.Version);
          APC.init();

            Console.ReadLine();
            Midi.MidiInDevice().CloseAsync();
        }
    }
}
