//using NAudio.Midi;
using Commons.Music.Midi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poubub.HumanInterface
{
    public static class Midi
    {
        private static IMidiAccess access = MidiAccessManager.Default;
        public static int MidiInChannelNumber
        {
            get { return Options.Read("MidiInChannel", 1); }
            set { Options.Write("MidiInChannel", value); }
        }
        public static int MidiOutChannelNumber
        {
            get { return Options.Read("MidiOutChannel", 1); }
            set { Options.Write("MidiOutChannel", value); }
        }
        public static string MidiInDeviceName
        {
            get {
                return Options.Read("MidiInDevice", GetMIDIInDevices().Last());
            }
            set {
                Options.Write("MidiInDevice", value);
            }
        }
        public static string MidiOutDeviceName
        {
            get { return Options.Read("MidiOutDevice", GetMIDIOutDevices().Last()); }
            set { Options.Write("MidiOutDevice", value); }
        }

        //public static int MidiSyncOutDeviceNumber
        //{
        //    get
        //    {
        //        return Options.Read("MidiSyncOutDevice", MidiOutDeviceNumber); 
        //    }
        //    set { Options.Write("MidiSyncOutDevice", value); }
        //}

        //public static bool SyncOutClock(bool run )
        //{
        //  if(aTimer == null)
        //  {
        //        Debug.WriteLine("SyncOutClock: init");
        //      Thread.CurrentThread.Priority = ThreadPriority.Highest;

        //      aTimer = new System.Timers.Timer();
        //      aTimer.Elapsed += new System.Timers.ElapsedEventHandler(midisyncElapsedEventHandler);
        //   //   aTimer.SynchronizingObject

        //  }
        //     aTimer.Enabled = run;
        //  try
        //  {
        //      if (aTimer.Enabled)
        //      {
        //          MidiOutDevice(MidiSyncOutDeviceNumber).Send(Convert.ToInt16(MidiCommandCode.StartSequence));
        //      }
        //      else
        //      {

        //          MidiOutDevice(MidiSyncOutDeviceNumber).Send(Convert.ToInt16(MidiCommandCode.StopSequence));

        //      }
        //  }
        //  catch (Exception ex)
        //  {
        //        Debug.WriteLine(ex.Message);
        //  }

        ////     Debug.WriteLine("SyncOutClock Enabled: " + aTimer.Enabled);

        //        return aTimer.Enabled;
        //    //    = 500;
        //}
        //public static void SyncOutBPM(double BPM)
        //{
        //        int beats = 4;
        //        int bars = 4;
        //        aTimer.Interval = ((60*1000) / BPM) / (double)tickdivider / (beats * bars);

        //      Debug.WriteLine("Interval:" + aTimer.Interval);

        //    //    = 500;
        //    //   aTimer.Elapsed += new ElapsedEventHandler(tick);
        //}

      //  private static System.Timers.Timer aTimer;
          //private static void midisyncElapsedEventHandler(object source, System.Timers.ElapsedEventArgs e)
          //{
          //    try
          //    {
          //        MidiOutDevice(MidiSyncOutDeviceNumber).Send(Convert.ToInt16(MidiCommandCode.TimingClock));
          //     //   MidiOutDevice(MidiSyncOutDeviceNumber).Send(Convert.ToInt16(MidiCommandCode.TimingClock));
          //      //    Debug.WriteLine("tickout");
          //    }
          //    catch (Exception ex) { 
          //          Debug.WriteLine(ex.Message);
          //     }
         
          //}
      //  public static MidiIn midiIn;
       // private static bool monitoring;
       // //private static int midiInDevice;
       //  private static int rawticks = 0;
       ////  private static int tickdivider = 6;
       //  private static int tickdivider
       //  {
       //      get
       //      {
       //          return Options.Read("tickdivider", 6);
       //      }
       //      set { Options.Write("tickdivider", value); }
       //  }
       //  private static bool clockrunning = false;

        private static Dictionary<int, Dictionary<int, int>> controlchangecache = new Dictionary<int, Dictionary<int, int>>();
        public static int? GetCCValue(int channel, int CC)
        {
            
            if( Midi.controlchangecache.Keys.Contains(CC))
            {
               return Midi.controlchangecache[channel][CC];
            }
            return null;
        }
        public static void SetCCValue(int channel, int CC, int value)
        {
            if(value > 127 | value < 0 )
            {
                throw new Exception("CC value outside of range:"+CC+":"+value);
            }
            if(!Midi.controlchangecache.ContainsKey(channel))
            {
                Midi.controlchangecache.Add(channel, new Dictionary<int, int>());
            }
            if (!Midi.controlchangecache[channel].ContainsKey(CC))
            {
                Midi.controlchangecache[channel].Add(CC,value);
            }
            Midi.controlchangecache[channel][CC] = value ;   
        }

        public static string[] GetMIDIInDevices()
        {
           return  access.Inputs.Select(x => x.Name).ToArray();

        }
        public static string[] GetMIDIOutDevices()
        {
           return access.Outputs.Select(x => x.Name).ToArray();
        }

        private static ConcurrentDictionary<string, IMidiOutput> D_midiOut = new ConcurrentDictionary<string, IMidiOutput>();
        private static ConcurrentDictionary<string, IMidiInput> D_midiIn = new ConcurrentDictionary<string, IMidiInput>();


        public static IMidiInput MidiInDevice(string name= null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Midi.MidiInDeviceName;
            }
            
            try
            {
                if (!D_midiIn.ContainsKey(name))
                {
                    D_midiIn.TryAdd(name, access.OpenInputAsync(access.Inputs.Where(x => x.Name == name).First().Id).Result);
                    D_midiIn[name].MessageReceived += Poubub.HumanInterface.Midi.Event_Received;
                }

                return D_midiIn[name];
            }
            catch (Exception ex)
            {

                  Debug.WriteLine(ex);
                ClearMidiDevices();
            }
            return null;
        }
        public static IMidiOutput MidiOutDevice(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Midi.MidiOutDeviceName;
            }
            try
            {
                if (!D_midiOut.ContainsKey(name))
                {
                    D_midiOut.TryAdd(name, access.OpenOutputAsync(access.Outputs.Where(x => x.Name == name).First().Id).Result);
                }

                return D_midiOut[name];
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
                ClearMidiDevices();
            }
            return null;
        }
        //public static void ResetMidiDevices()
        //{
        //    foreach (var x in D_midiOut)
        //    {
        //        x.Value.CloseAsync();
        //    }
        //    foreach (var x in D_midiIn)
        //    {
        //        x.Value.Reset();
        //    }

        //}
        public static void ClearMidiDevices()
        {
            foreach (var x in D_midiOut)
            {
                try
                {
                    x.Value.CloseAsync();
                    x.Value.Dispose();
                }
                catch { }
            }
            D_midiOut.Clear();

            foreach (var x in D_midiIn)
            { try
                {
                x.Value.CloseAsync();
                x.Value.Dispose();
                }
            catch { }
            }
            D_midiIn.Clear();
        }
        private static void NoteOut(int note, int vel, int channel, int length, string device = null)
        {
            if(String.IsNullOrEmpty(device))
            {
                device = MidiOutDeviceName;
            }

            //MidiOutDevice(device).Send(MidiMessage.StartNote(note, vel, channel).RawData);
            //System.Threading.Thread.Sleep(length);
            //MidiOutDevice(device).Send(MidiMessage.StopNote(note, 0, channel).RawData);

        }
        private static void CCOut(int controller, int value, int channel, string device = null)
        {
            if (String.IsNullOrEmpty(device))
            {
                device = MidiOutDeviceName;
            }
            //MidiOutDevice(device).Send(MidiMessage.ChangeControl(controller, value, channel).RawData);

        }
        //private static void PatchChange(int value, int channel, int device = 0)
        //{

        //    MidiOutDevice(device).Send(MidiMessage.ChangePatch(value, channel).RawData);

        //}
        //public static void CCOut(string allparams)
        //{
        //    string[] words = allparams.Split('|');
        //    new Thread(() =>
        //    {
        //        Thread.CurrentThread.IsBackground = true;
        //        try
        //        {
        //            CCOut(Convert.ToInt16(words[0]), Convert.ToInt16(words[1]), Convert.ToInt16(words[2]),Midi.MidiOutDeviceNumber);
        //        }
        //        catch (Exception ex)
        //        {

        //              Debug.WriteLine("CCOut:", ex);
        //        }
        //    }).Start();
        //}
        //public static void NoteOut(string allparams)
        //{
        //    string[] words = allparams.Split('|');
        //    new Thread(() =>
        //    {
        //        Thread.CurrentThread.IsBackground = true;
        //        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        //        try
        //        {
        //            int n, v, c, l;
        //            if (int.TryParse(words[0], out n) && int.TryParse(words[1], out v) && int.TryParse(words[2], out c) && int.TryParse(words[3], out l))
        //            {
        //                NoteOut(n,v,c,l, Midi.MidiOutDeviceNumber);
        //            }
        //            else
        //            {
        //                 Debug.WriteLine("NoteOut:","Failed to input:"+ allparams);
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //              Debug.WriteLine("NoteOut:", ex);
        //        }
        //    }).Start();
        //}
        //public static void PatchChange(string allparams)
        //{
        //    string[] words = allparams.Split('|');
        //    new Thread(() =>
        //    {
        //        Thread.CurrentThread.IsBackground = true;
        //        try
        //        {
        //            PatchChange(Convert.ToInt16(words[0]), Convert.ToInt16(words[1]), Midi.MidiOutDeviceNumber);
        //        }
        //        catch (Exception ex)
        //        {

        //             Debug.WriteLine("PatchChange:", ex);
        //        }
        //    }).Start();
        //}

        private static void Event_Received(object sender, MidiReceivedEventArgs eargs)
        {
            MidiEvent e = MidiEvent.Convert(eargs.Data, 0, eargs.Length).First();
            Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(e, Newtonsoft.Json.Formatting.Indented));
            Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(MidiEvent.Convert(eargs.Data, 0, eargs.Length).First(), Newtonsoft.Json.Formatting.Indented));
            try
            {
                //// Exit if the MidiEvent is null or is the AutoSensing command code  
                if ( e.EventType == MidiEvent.ActiveSense)
                {
                    return;
                }
                else if (e.EventType == MidiEvent.CC)
                {
                    //TODO: value is wrong
                        Midi.SetCCValue(e.Channel, (int)e.Msb, (int)e.Lsb);
               
                }
                //else if ( e.EventType == MidiEvent.TimingClock)
                //{
                //    rawticks++;
                //    if (rawticks % tickdivider == 0)
                //    {
                //    //    if (clockrunning )
                //        {
                //            Combustion.Tick();
                //        }
                //    }
                //}
                //else if ( e.EventType == MidiEvent.StartSequence)
                //{
                //    rawticks = 0;
                //    clockrunning = true;
                //    Combustion.Start();
                //}
                //else if ( e.EventType == MidiEvent.StopSequence)
                //{
                //    clockrunning = false;

                //    Combustion.Stop();
                //}
                else
                {
                   Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(e,Newtonsoft.Json.Formatting.Indented));
                   Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(MidiEvent.Convert(eargs.Data,0, eargs.Length).First(), Newtonsoft.Json.Formatting.Indented));


                }
            }
            catch (Exception ex)
            {
                  Debug.WriteLine(ex.Message);
            }
        }
    }
}
