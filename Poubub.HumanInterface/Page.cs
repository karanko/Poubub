using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.HumanInterface
{
    public class Page
    {
        public string Id;//added for raven
        public Pattern Pattern = new Pattern() { Length = 16 };
        public int Note;
        public int OctShift = 0;
        public int Vel = 90;
        public APC.LED led = APC.LED.Green;
        public bool Solo = false;
        public bool Mute = false;
        public bool Softkeys = false;

        public bool Blank1 = false;
        public bool Blank2 = false;

        public bool Volume = false;
        public bool Pan = false;
        public bool Send = false;
        public bool Device = false;


    }
    public class Pattern
    {
        private ConcurrentDictionary<int, bool> _steps = new ConcurrentDictionary<int, bool>();
        public int Length = 8;
        public int MaxSteps
        {
            get
            {
                int result = Length;
                foreach (int steps in this._steps.Keys)
                {
                    if (steps > result)
                    {
                        result = steps;
                    }
                }
                return result;

            }

        }
        public bool RecallStep(int step)
        {
            if (_steps.Keys.Contains(step))
            {
                return _steps[step];
            }
            else
            {
                return false;
            }
        }
        public void WriteStep(int step, bool value)
        {
            if (_steps.Keys.Contains(step))
            {
                _steps[step] = value;
            }
            else
            {
                _steps.TryAdd(step, value);
            }
        }
        public bool[] Steps
        {
            get
            {
                List<bool> x = new List<bool>();

                //  for (int i = 0; i <= Length; i++)
                for (int i = 0; i <= 64; i++)
                {
                    x.Add(RecallStep(i));
                }

                return x.ToArray();
            }
            set
            {
                int i = 0;

                foreach (var x in value)
                {
                    if (RecallStep(i) != x)
                    {
                        WriteStep(i, x);
                    }
                    i++;
                }
            }

        }

        public void Clear()
        {
            _steps.Clear();
        }
    }
}
