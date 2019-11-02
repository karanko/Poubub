using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.Core
{
    public class CVG
    {
        public CVG()
        {

        }
        public CVG(Dictionary<string, List<bool>> gate, Dictionary<string, List<short>> cv, Dictionary<string, List<short>> parameters)
        {
            this.Gate = gate;
            this.CV = cv;
            this.Parameters = parameters;
        }
        //public CVG Clone()
        //{
        //    return new CVG(Gate, CV, Parameters);
        //}
        public Dictionary<string, List<bool>> Gate = new Dictionary<string, List<bool>>();
        public Dictionary<string, List<short>> CV = new Dictionary<string, List<short>>();
        public Dictionary<string, List<short>> Parameters = new Dictionary<string, List<short>>();
    }

}
