//using NAudio.Midi;
using Commons.Music.Midi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

namespace Poubub.HumanInterface
{
    public static class APC
    {
        public static void InitMidi()
        {

            //TODO:PrintStuffHere
            Midi.MidiInDevice().MessageReceived += APC.MessageReceived;
            Midi.MidiOutDevice();
        }

            private static Random _rnd = new Random(DateTime.Now.Millisecond);
        public static void init()
        {
                InitMidi();
         

            //master page


            for (int i = 0; i < 8; i++)
            {

                 _pages.Add(new HumanInterface.Page());

                for (int x = 0; x < 4; x++)
                {
                     _pages[i].Pattern.WriteStep(_rnd.Next(1, _pages[i].Pattern.Length), true);
                }

            }
           
            _pages.Add(new Page() { led = LED.Red, Note = 1 });
            _pages.LastOrDefault().Pattern.Length = 8;
            for (int i = 0; i < _pages.LastOrDefault().Pattern.Length; i++)
            {
                _pages.LastOrDefault().Pattern.WriteStep(i, true);
            }
           // System.Threading.Thread.Sleep(300);
            _currentpage = 0;
            DrawCurrentPage();

        }
        private static List<Page> _pages = new List<Page>();
        private static int _currentpage;
        private static bool _shiftisdown = false;

        private static ConcurrentDictionary<int, int> controlchangecache = new ConcurrentDictionary<int, int>();
        private static ConcurrentDictionary<int, int> notecache = new ConcurrentDictionary<int, int>();
        private static void MessageReceived(object sender, MidiReceivedEventArgs eargs)
        {
            MidiEvent e = MidiEvent.Convert(eargs.Data, 0, eargs.Length).First();

            //try
            //{
            //    //// Exit if the MidiEvent is null or is the AutoSensing command code  
            //    if (e.EventType.EventType == MidiEvent.ActiveSense)
            //    {
            //        return;
            //    }
            //    else if (e.EventType.EventType == MidiEvent.CC)
            //    {

            //        Midi.SetCCValue(e.Channel, (int)e.Value, (int)e.Msb);

            //    }
                try
            {
                // Exit if the MidiEvent is null or is the AutoSensing command code  
                if (e.EventType == MidiEvent.ActiveSense)
                {
                    return;
                }
                else if (e.EventType == MidiEvent.CC)
                {
                    APC.SetCCValue( (int)e.Msb,(int)e.Lsb);
                }
                else if (e.EventType == MidiEvent.NoteOn)
                {
                    int NoteNumber = (int)e.Msb;
                    if (_shiftisdown)
                    {
                        if (NoteNumber <= 63)
                        {
                            _pages[_currentpage].Pattern.Length = NoteNumber + 1;
                            Debug.WriteLine(_pages[_currentpage].Pattern.Length);
                        }
                    }
                    else if (NoteNumber == 83)
                    {
                        Page(_currentpage).Solo = !Page(_currentpage).Solo;
                        int i = 0;
                        while (i < _pages.Count)
                        {
                            if (i != _currentpage)
                            {
                                _pages[i].Solo = false;
                            }
                            i++;
                        }
                        DrawPage(_currentpage, false);
                    }
                    else if (NoteNumber == 87)
                    {
                        Page(_currentpage).Blank1 = !Page(_currentpage).Blank1;
                        DrawPage(_currentpage, false);
                        DrawPage(_currentpage, false);
                    }
                    else if (NoteNumber == 88)
                    {
                        Page(_currentpage).Blank2 = !Page(_currentpage).Blank2;
                        DrawPage(_currentpage, false);
                        DrawPage(_currentpage, false);
                    }
                    else if (NoteNumber == 86)
                    {
                        Page(_currentpage).Softkeys = !Page(_currentpage).Softkeys;
                        DrawPage(_currentpage, true);
                        DrawPage(_currentpage, false);
                    }

                    //fader ctrl
                    else if (NoteNumber == 68)
                    {
                        Page(_currentpage).Volume = !Page(_currentpage).Volume;
                        DrawPage(_currentpage, false);
                        DrawPage(_currentpage, false);
                    }
                    else if (NoteNumber == 69)
                    {
                        Page(_currentpage).Pan = !Page(_currentpage).Pan;
                        DrawPage(_currentpage, false);
                        DrawPage(_currentpage, false);
                    }
                    else if (NoteNumber == 70)
                    {
                        Page(_currentpage).Send = !Page(_currentpage).Send;
                        DrawPage(_currentpage, false);
                        DrawPage(_currentpage, false);
                    }
                    else if (NoteNumber == 71)
                    {
                        Page(_currentpage).Device = !Page(_currentpage).Device;
                        DrawPage(_currentpage, false);
                        DrawPage(_currentpage, false);
                    }


                    else if (NoteNumber == 66)
                    {
                        Page(_currentpage).OctShift = -1;
                        DrawPage(_currentpage, false);
                        DrawPage(_currentpage, false);
                    }

                    else if (NoteNumber == 67)
                    {
                        Page(_currentpage).OctShift = +1;
                        DrawPage(_currentpage, false);
                        DrawPage(_currentpage, false);
                    }



                    else if (NoteNumber == 89)
                    {
                        int i = 0;
                        while (i < _pages.Count)
                        {
                            if (i != _currentpage)
                            {
                                _pages[i].Pattern.Clear();
                            }
                            i++;
                        }
                        DrawPage(_currentpage);
                    }
                    else if (NoteNumber == 82)
                    {
                        Page(_currentpage).Pattern.Clear();
                        DrawPage(_currentpage);
                    }
                    else if (NoteNumber == 85)
                    {
                        Page(_currentpage).Mute = !Page(_currentpage).Mute;
                        DrawPage(_currentpage, false);
                    }
                    else if (NoteNumber <= 63)
                    {

                        MainGridHandler(NoteNumber);

                    }
                    else if (NoteNumber == 64)
                    {
                        _currentpage = (_currentpage + 1) % _pages.Count;
                        // System.Threading.Thread.Sleep(100);
                        DrawPage(_currentpage);
                    }
                    else if (NoteNumber == 65)
                    {
                        _currentpage = (_currentpage - 1) % _pages.Count;
                        if (_currentpage < 0)
                        {
                            _currentpage = _pages.Count - 1;
                        }
                        // System.Threading.Thread.Sleep(100);
                        DrawPage(_currentpage);
                    }
                    else if (NoteNumber == 98)
                    {
                        _shiftisdown = true;
                    }
                    else
                    {
                        Debug.WriteLine(NoteNumber);
                        // Debug.WriteLine(e.MidiEvent.CommandCode.ToString());
                    }
                }
                else if (e.EventType == MidiEvent.NoteOff)
                {
                    int NoteNumber = (int)e.Msb;
                    if (NoteNumber == 98)
                    {
                        _shiftisdown = false;
                        DrawCurrentPage(false);
                        // DrawCurrentPage(false);
                    }
                }
                else
                {
                    //Debug.WriteLine(e.MidiEvent.ToString());
                    //Debug.WriteLine(e.MidiEvent.CommandCode.ToString());
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //  Debug.WriteLine(ex.Message);
            }
            //Database.RavenDBSavePage(_pages[_currentpage ]);
        }

        public static void SetCCValue(int CC, int value)
        {
            //if (value > 127 | value < 0)
            //{
            //    throw new Exception("CC value outside of range:" + CC + ":" + value);
            //}

            if (!APC.controlchangecache.Keys.Contains(CC))
            {
                APC.controlchangecache.TryAdd(CC, value);
            }
            APC.controlchangecache[CC] = value;
            if ((CC >= 48) && (CC <= 55))
            {
                Page(CC - 48).Vel = value;
                if (_currentpage != CC - 48 & _shiftisdown)
                {
                    SelectPage(CC - 48);

                }
            }
        }

        public enum LED
        {
            Off = 0, Green = 1, GreenBlink = 2, Red = 3, RedBlink = 4, Yellow = 5, YellowBlink = 6
        }

        private static void SetLED(int note, LED led)
        {
            int channel = 1;

            if (!APC.notecache.Keys.Contains(note))
            {
                APC.notecache.TryAdd(note, Convert.ToInt16(led));
            }
            else
            {
                Midi.MidiOutDevice().Send(new byte[] { Convert.ToByte(MidiEvent.NoteOff + (channel - 1)), Convert.ToByte(note), Convert.ToByte(0) }, 0, 3, 0);

              //  Midi.MidiOutDevice().Send(Poubub.HumanInterface.MidiMessage.StopNote(note, 0, channel).Data, 0, 3, 0);
                    
    
                APC.notecache[note] = Convert.ToInt16(led);
            }

            if (led == 0)
            { 
             //   Midi.MidiOutDevice().Send(Poubub.HumanInterface.MidiMessage.StartNote(note, APC.notecache[note], channel).Data, 0, 3, 0);
                Midi.MidiOutDevice().Send(new byte[] { Convert.ToByte(MidiEvent.NoteOn + (channel - 1)), Convert.ToByte( note), Convert.ToByte( APC.notecache[note]) }, 0, 3, 0);
                int remove;
                APC.notecache.TryRemove(note, out remove);
            }
            else
            {
                //Midi.MidiOutDevice().Send(Poubub.HumanInterface.MidiMessage.StartNote(note, APC.notecache[note], channel).Data, 0, 3, 0);
                Midi.MidiOutDevice().Send(new byte[] { Convert.ToByte(MidiEvent.NoteOn + (channel - 1)), Convert.ToByte(note), Convert.ToByte(APC.notecache[note]) }, 0, 3, 0);
            }
        }


        private static void MainGridHandler(int note)
        {

            if (!Page(_currentpage).Softkeys)
            {
                _pages[_currentpage].Pattern.WriteStep(note, !_pages[_currentpage].Pattern.RecallStep(note));
                DrawPage(_currentpage, false);
            }
            else
            {
                _pages[_currentpage].Note = 36 + (12 * _pages[_currentpage].OctShift) + note;
                DrawPage(_currentpage, true);
            }


            // int col = -1;
            // if (APC.notecache.Keys.Contains(note))
            // {
            //     col = APC.notecache[note];
            // }
            // else
            // {
            //     col = 3;

            // }
            //// Debug.WriteLine(col);
            // if (col == 3)
            // {
            //     col = 5;
            // }
            // else if (col == 5)
            // {
            //     col = 1;
            // }
            // else if (col == 1)
            // {
            //     col = 0;
            // }
            // else if (col <= 0)
            // {
            //     col = 3;
            // }

            // Debug.WriteLine(col);
            //APC.SetLED(note, (LED)col);

        }

        public static void ClearBoard()
        {
            //  


            for (int i = 0; i <= 3; i++)
            {
                for (int note = 0; note <= 63; note++)
                {

                    try
                    {
                        //  Debug.WriteLine(note);
                        Midi.MidiOutDevice().Send(new byte[] { MidiEvent.NoteOff , Convert.ToByte(note),0 }, 0, 3, 0);
                      //  Midi.MidiOutDevice().Send(Poubub.HumanInterface.MidiMessage.StartNote(note, 0, 1).Value, 0, 3, 0);
                      //  Midi.MidiOutDevice().Send(Poubub.HumanInterface.MidiMessage.StopNote(note, 0, 1).DaValueta, 0, 3, 0);

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
              //  Thread.Sleep(75);
            }
            notecache.Clear();

        }


        private static void DrawCurrentPage(bool clear = true)
        {
            DrawPage(_currentpage, clear);
        }

        private static void DrawPage(int number, bool clear = true)
        {
            Page page = Page(number);
            if (clear)
            {
                //   System.Threading.Thread.Sleep(150);
                ClearBoard();
            }
            int i = 0;
            if (!Page(_currentpage).Softkeys)
            {
                while (i <= 64)
                {
                    if (page.Pattern.RecallStep(i))
                    {
                        if (i + 1 == page.Pattern.Length)
                        {
                            if (page.led.Equals(LED.Green))
                            {
                                SetLED(i, LED.GreenBlink);
                            }
                            else if (page.led.Equals(LED.Red))
                            {
                                SetLED(i, LED.RedBlink);
                            }
                            else if (page.led.Equals(LED.Yellow))
                            {
                                SetLED(i, LED.YellowBlink);
                            }
                        }
                        else
                        {
                            SetLED(i, page.led);
                        }
                    }
                    else
                    {
                        if (i + 1 == page.Pattern.Length)
                        {
                            SetLED(i, LED.YellowBlink);
                        }
                        else
                        {
                            SetLED(i, LED.Off);
                        }

                    }
                    i++;
                }
            }
            else
            {
                SetLED((page.Note % 12), LED.Red);


            }
            if (Page(_currentpage).Solo)
            {
                SetLED(83, LED.Red);
            }
            else
            {
                SetLED(83, LED.Off);
            }

            if (Page(_currentpage).Mute)
            {
                SetLED(85, LED.Red);
            }
            else
            {
                SetLED(85, LED.Off);
            }

            if (Page(_currentpage).Blank1)
            {
                SetLED(87, LED.Red);
            }
            else
            {
                SetLED(87, LED.Off);
            }
            if (Page(_currentpage).Blank2)
            {
                SetLED(88, LED.Red);
            }
            else
            {
                SetLED(88, LED.Off);
            }
            if (Page(_currentpage).Softkeys)
            {
                SetLED(86, LED.Red);
            }
            else
            {
                SetLED(86, LED.Off);
            }

            if (Page(_currentpage).Volume)
            {
                SetLED(68, LED.Red);
            }
            else
            {
                SetLED(68, LED.Off);
            }

            if (Page(_currentpage).Pan)
            {
                SetLED(69, LED.Red);
            }
            else
            {
                SetLED(69, LED.Off);
            }
            if (Page(_currentpage).Send)
            {
                SetLED(70, LED.Red);
            }
            else
            {
                SetLED(70, LED.Off);
            }
            if (Page(_currentpage).Device)
            {
                SetLED(71, LED.Red);
            }
            else
            {
                SetLED(71, LED.Off);
            }

            SetLED(66, LED.Off);
            SetLED(67, LED.Off);
            if (Page(_currentpage).OctShift == -1)
            {
                SetLED(66, LED.Red);
            }
            else if (Page(_currentpage).OctShift == 1)
            {
                SetLED(67, LED.Red);
            }



        }
        public static List<Poubub.HumanInterface.Page> ListPages()
        {
            return _pages;
        }
        public static Page Page(int i)
        {
            if(_pages.Count < i)
            {
                _pages.Add(new HumanInterface.Page());
            }
            return _pages[i];
        }
        public static void SelectPage(int i)
        {
            //  Database.RavenDBSavePage(_pages[_currentpage]);

            if (_currentpage != i)
            {
                _currentpage = i;
                //   _pages[_currentpage] = Database.RavenDBLoadPage(_currentpage);
                DrawCurrentPage();
            }

        }


    }
}
