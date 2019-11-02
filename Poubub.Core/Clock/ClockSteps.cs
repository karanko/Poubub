using System;
using System.Collections.Generic;
using System.Text;

namespace Poubub.Core
{
    public class ClockSteps
    {
        public List< StepPattern> Patterns = new List<StepPattern>();
        public void Clone(int pattern)
        {
            Patterns.Add(Patterns[pattern]);
        }
        private Jurassic.ScriptEngine _jsengine = new Jurassic.ScriptEngine();
        public string ExpandSteps(StepPattern pattern)
        {
            return _jsengine.Evaluate<string>("JSON.stringify([].concat.apply([],(" + pattern.Offsets + ").map(function (x){ return ("+ pattern.Steps + ").map(function(y){ return y + x}) })));");
        }
    }
}
