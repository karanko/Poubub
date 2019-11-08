using Commons.Music.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.HumanInterface
{
    public class APCIntance
    {
        public enum LED
        {
            Off = 0, Green = 1, GreenBlink = 2, Red = 3, RedBlink = 4, Yellow = 5, YellowBlink = 6
        }
        public enum PageType
        {
            Seq8a = 0, Seq64 = 1, Chord =2, Seq8b = 3, Clip =4
        }
        public PageType CurrentPageType = PageType.Seq8a;
        private List<PageType> pagetypehistory = new List<PageType>();
        public APCIntance(IMidiOutput output, IMidiInput input)
        {
            outdevice = output;
            indevice = input;
            indevice.MessageReceived += Message_Received;
            for (int row = 0; row < 64; row++)
            {
                Seq64.Add(row, false);
                Clips.Add(row, false);
            }
            for (int row = 0; row < 8; row++)
            {
                Seq8a.Add(row, new Dictionary<int, bool>());
                Seq8b.Add(row, new Dictionary<int, bool>());
                Chords.Add(row, new List<int>());
                for (int col = 0; col < 8; col++)
                {
                    Seq8a[row].Add(col, false);
                    Seq8b[row].Add(col, false);
                }
            }
            DrawPage(true);
        }

        public int[] Faders = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        public int Master = 0;

        private bool shiftdown = false;
        //private bool selectSoftKeys = false;
        //public bool selectMute = false;
        //public bool selectSolo = false;

        //public bool selectRecarm = false;
        //public bool selectStopclip = false;
        //public bool selectB1 = false;
        //public bool selectB2 = false;
        public int UpDown = 0;
        public int LeftRight = 0;
        public int Seq64Length = 16;
        public Dictionary<int,  bool> Seq64 = new Dictionary<int, bool>();
        public Dictionary<int,  bool> Clips = new Dictionary<int, bool>();
        public Dictionary<int, Dictionary<int, bool>> Seq8a = new Dictionary<int, Dictionary<int, bool>>();
        public Dictionary<int, Dictionary<int, bool>> Seq8b = new Dictionary<int, Dictionary<int, bool>>();
        public Dictionary<int, List<int>> Chords = new Dictionary<int, List<int>>();
        private void DrawPage(bool clear = false)
        {

            if (clear)
            {
                for (int note = 0; note <= 63; note++)
                {
                    SetLED(note, LED.Off, clear);
                }
                LEDcache.Clear();
            }

          
            SetLED(84, LED.Off);
            SetLED(85, LED.Off);
            SetLED(86, LED.Off);
            SetLED(87, LED.Off);
            SetLED(88, LED.Off);
            SetLED(84 + (int)CurrentPageType, LED.Red);

            if (CurrentPageType == PageType.Seq64)
            {
               // SetLED(87, LED.Red);
                foreach (var step in Seq64)
                {
                    if (step.Key + 1 == Seq64Length & shiftdown)
                    {
                        SetLED(step.Key, LED.Yellow);
                    }
                    else if (step.Value)
                    {
                        SetLED(step.Key, LED.Red);
                    }
                    else
                    {
                        SetLED(step.Key, LED.Off);
                    }
                }
            }
            else if (CurrentPageType == PageType.Clip)
            {
              //  SetLED(85 + (int)CurrentPageType, LED.Red);
                foreach (var step in Clips)
                {
                    if (step.Value)
                    {
                        SetLED(step.Key, LED.Yellow);
                    }
                    else
                    {
                        SetLED(step.Key, LED.Off);
                    }
                }
            }
            else if (CurrentPageType == PageType.Chord)
            {
               // SetLED(86, LED.Red);
                foreach (var x in Chords.Keys)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (Chords[x].Contains(i))
                        {
                            SetLED((x * 8) + i, LED.Green);
                        }
                        else
                        {
                            SetLED((x * 8) + i, LED.Off);
                        }
                    }
                }
            }
            else if (CurrentPageType == PageType.Seq8a)
            {
              //  SetLED(88, LED.Red);
                foreach (var seq in Seq8a)
                {
                    foreach (var step in seq.Value)
                    {
                        //if (seq.Key +step.Key + 1 == Seq64Length & shiftdown)
                        //{
                        //    SetLED(step.Key, LED.Yellow);
                        //}
                        /* else */

                        if (step.Value)
                        {
                            SetLED(seq.Key * 8 + step.Key, (LED)((seq.Key % 3) + (seq.Key % 3)) + 1);
                        }
                        else
                        {
                            SetLED(seq.Key * 8 + step.Key, LED.Off);
                        }
                    }
                }
            }
            else if (CurrentPageType == PageType.Seq8b)
            {
                //  SetLED(88, LED.Red);
                foreach (var seq in Seq8b)
                {
                    foreach (var step in seq.Value)
                    {
                        //if (seq.Key +step.Key + 1 == Seq64Length & shiftdown)
                        //{
                        //    SetLED(step.Key, LED.Yellow);
                        //}
                        /* else */

                        if (step.Value)
                        {
                            SetLED(seq.Key * 8 + step.Key, (LED)((seq.Key % 3) + (seq.Key % 3)) + 1);
                        }
                        else
                        {
                            SetLED(seq.Key * 8 + step.Key, LED.Off);
                        }
                    }
                }
            }
        }
        private void Message_Received(object sender, MidiReceivedEventArgs eargs)
        {
            MidiEvent e = MidiEvent.Convert(eargs.Data, 0, eargs.Length).First();

            if (e.EventType == MidiEvent.CC)
            {
                int CC = (int)e.Msb;
                int value = (int)e.Lsb;
                if ((CC >= 48) && (CC <= 55))
                {
                    Faders[CC - 48] = value;
                }
                return;
            }



            int NoteNumber = (int)e.Msb;

            if (NoteNumber >= 84 && NoteNumber <= 88 && ((int)CurrentPageType) != (NoteNumber - 84))
            {
                pagetypehistory.Add(CurrentPageType);
                CurrentPageType = (PageType)(NoteNumber - 84);
            }

            else if (shiftdown && (NoteNumber == 66 || NoteNumber == 67) && e.EventType == MidiEvent.NoteOn)
            {
                pagetypehistory.Add(CurrentPageType);
                if (NoteNumber == 67)
                {
                    CurrentPageType = Enum.GetValues(typeof(PageType)).Cast<PageType>().Where(ze => (int)ze > (int)CurrentPageType).OrderBy(ze => ze).FirstOrDefault();
                }
                else if (NoteNumber == 67)
                {
                    CurrentPageType = Enum.GetValues(typeof(PageType)).Cast<PageType>().Where(ze => (int)ze < (int)CurrentPageType).OrderByDescending(ze => ze).FirstOrDefault();
                }

            }
            else if (NoteNumber == 98 & (e.EventType == MidiEvent.NoteOn || e.EventType == MidiEvent.NoteOff))
            {
                shiftdown = !shiftdown;
            }
            else if (CurrentPageType == PageType.Seq64 && e.EventType == MidiEvent.NoteOn)
            {
                if (NoteNumber <= 63)
                {
                    if (shiftdown)
                    {
                        Seq64Length = NoteNumber + 1;
                    }
                    else
                    {
                        Seq64[NoteNumber] = !Seq64[NoteNumber];
                    }
                }
                if (NoteNumber == 89)
                {
                    foreach (var x in Seq64.Keys.ToList())
                    {
                        Seq64[x] = false;
                    }
                }
            }
            else if (CurrentPageType == PageType.Clip && e.EventType == MidiEvent.NoteOn)
            {
                if (NoteNumber <= 63)
                {
                    Clips[NoteNumber] = !Clips[NoteNumber];
                }
                if (NoteNumber == 89)
                {
                    foreach (var x in Clips.Keys.ToList())
                    {
                        Clips[x] = false;
                    }
                }
            }
            else if (CurrentPageType == PageType.Seq8a && e.EventType == MidiEvent.NoteOn)
            {
                if (NoteNumber <= 63)
                {
                    Seq8a[(NoteNumber - NoteNumber % 8) / 8][NoteNumber % 8] = !Seq8a[(NoteNumber - NoteNumber % 8) / 8][NoteNumber % 8];
                }
                if (NoteNumber == 89)
                {
                    foreach (var x in Seq8a.Keys.ToList())
                    {
                        foreach (var y in Seq8a[x].Keys.ToList())
                        {
                            Seq8a[x][y] = false;
                        }
                    }
                }
            }
            else if (CurrentPageType == PageType.Seq8b && e.EventType == MidiEvent.NoteOn)
            {
                if (NoteNumber <= 63)
                {
                    Seq8b[(NoteNumber - NoteNumber % 8) / 8][NoteNumber % 8] = !Seq8b[(NoteNumber - NoteNumber % 8) / 8][NoteNumber % 8];
                }
                if (NoteNumber == 89)
                {
                    foreach (var x in Seq8b.Keys.ToList())
                    {
                        foreach (var y in Seq8b[x].Keys.ToList())
                        {
                            Seq8b[x][y] = false;
                        }
                    }
                }
            }
            else if (CurrentPageType == PageType.Chord && e.EventType == MidiEvent.NoteOn)
            {
                if (NoteNumber <= 63)
                {
                    if (Chords[(NoteNumber - NoteNumber % 8) / 8].Contains(NoteNumber % 8))
                    {
                        Chords[(NoteNumber - NoteNumber % 8) / 8].Remove(NoteNumber % 8);
                    }
                    else
                    {
                        Chords[(NoteNumber - NoteNumber % 8) / 8].Add(NoteNumber % 8);
                    }
                }
                if (NoteNumber == 89)
                {
                    foreach (var x in Chords.Keys.ToList())
                    {
                        Chords[x].Clear();
                    }
                }
            }
            DrawPage();
        }
        private Dictionary<int, LED> LEDcache = new Dictionary<int, LED>();
        private void SetLED(int note, LED led, bool force = false)
        {
            if (led == LED.Off)
            {
                NoteOn(note, 0);
                NoteOff(note);
            }
            else
            {
                NoteOn(note, (int)led);
            }
        }
        private void NoteOn(int note, int vel, int channel =1 )
        {
            outdevice.Send(new byte[] { Convert.ToByte(MidiEvent.NoteOn + (channel - 1)), Convert.ToByte(note), Convert.ToByte(vel) }, 0, 3, 0);
        }
        private void NoteOff(int note, int vel = 0, int channel = 1)
        {
            outdevice.Send(new byte[] { Convert.ToByte(MidiEvent.NoteOff + (channel - 1)), Convert.ToByte(note), Convert.ToByte(vel) }, 0, 3, 0);
        }
        private IMidiOutput outdevice;
        private IMidiInput indevice;
    }
}
