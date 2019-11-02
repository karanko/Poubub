using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.Core
{
    public class Session
    {
        public string Name = Utils.RandName()+".pousess";
        // public ClockInfo Clock = new ClockInfo();
        public ClockSteps Sequence = new ClockSteps(); 
        public List<CVGFunction> Modules = new List<CVGFunction>();
        public CVG InitalData = new CVG();
        public CVG Process(CVG workingdata = null)
        {
            if (workingdata == null)
            {
                workingdata = InitalData;
            }
            //todo: make this better!?!
            this.Modules.ForEach(x => workingdata = workingdata.Process(x));
            ProcessedResults.Add(workingdata);
            return workingdata;
        }
        public List<CVG> ProcessedResults = new List<CVG>();
        public string Notes = String.Empty;
    }
}
