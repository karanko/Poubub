using System;
using System.Collections.Generic;
using System.Text;

namespace Poubub.Core
{
   public class StepPattern
    {
        public bool Enabled = true;
        public string Steps = "[1,2,3,4,5,6,7,8]";
        public string Offsets = "[0,(4*4)]";
        public string ExpandedSteps = "";
    }
}
