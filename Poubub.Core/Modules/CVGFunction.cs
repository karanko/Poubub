using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.Core
{
    public class CVGFunction
    {

       public  CVGFunction(string script)
        {
            this.Name = Utils.RandName(1,2);
            this.Script = script;
        }

        public string Name
        {
            get {
                return _jsengine.GetGlobalValue<string>("FunctionName");
            }
            set
            {
              //  Debug.WriteLine(value);
                _jsengine.SetGlobalValue("FunctionName", value);
            }
        } 
        public string Script
        {
            get {
                //TODO: jsbeutify? - Pro in the ui
                return _jsengine.Evaluate<string>("Process");
            }
            set {
                try
                {
                   // Debug.WriteLine(value);
                    //TODO: do we need to var here - how to force this to be global nicely?
                    _jsengine.Execute("var Process = " + value);
                    _jsengine.RecursionDepthLimit = 10000;
                    _jsengine.EnableExposedClrTypes = true;
                    _jsengine.SetGlobalFunction("log", new Action<object>(obj => Debug.WriteLine(obj)));
                    // _jsengine.SetGlobalFunction("Serialize", new Action<object>(obj => Newtonsoft.Json.JsonConvert.SerializeObject(obj)));
                    // _jsengine.SetGlobalFunction("Deserialize", new Action<string>(obj => Newtonsoft.Json.JsonConvert.DeserializeObject(obj)));
                }
                catch (Exception ex){
                    Debug.WriteLine(ex.Message);
                }

                
            }
        }
        private Jurassic.ScriptEngine _jsengine = new Jurassic.ScriptEngine();

        public void Reset()
        {
            string name = this.Name;
            string script = this.Script;
            var oldengine = _jsengine;
            _jsengine = new Jurassic.ScriptEngine();
            this.Name = name;
            this.Script = script;

        }
       
        public CVG Process(CVG inputdata, object otherdata = null)
        {
            try
            {
                //HACK: this feels wrong
                if (!String.IsNullOrEmpty(CurrentState.Settings.Functions))
                {
                    _jsengine.Execute(CurrentState.Settings.Functions);
                }
                // Debug.WriteLine("Process();");
                //INPUT HACKS
                //  _jsengine.SetGlobalValue("gate_input", inputdata.Gate);
                // _jsengine.SetGlobalValue("cv_input", inputdata.CV);
                //  _jsengine.SetGlobalValue("parameters_input", inputdata.Parameters);
                // _jsengine.Execute("var cvg_input = {}; cvg_input['Gate'] = gate_input;cvg_input['CV'] = cv_input;cvg_input['Parameters'] = parameters_input;");
                //HACK: i hate this
                if (otherdata != null)
                {
                    _jsengine.SetGlobalValue("otherdata_raw", Newtonsoft.Json.JsonConvert.SerializeObject(otherdata));
                    _jsengine.Execute("var otherdata = JSON.parse(otherdata_raw)");
                }

                _jsengine.SetGlobalValue("cvg_input_raw", Newtonsoft.Json.JsonConvert.SerializeObject(inputdata));
                _jsengine.Execute("var cvg_input = JSON.parse(cvg_input_raw)");


                //TODO: this seems log winded
                _jsengine.Execute("var cvg_result = Process(cvg_input) ");

                //OUTPUT HACKS
                _jsengine.Execute("; if (cvg_result === undefined){ cvg_result = cvg_input}; var cvg_result_raw = JSON.stringify(cvg_result);   ");
                //   _jsengine.Execute(" var gate_result = cvg_result['Gate'] ;var cv_result = cvg_result['CV'] ;var parameters_result = cvg_result['Parameters'] ;");
                //var gate_result = (Dictionary<string, List<bool>>)_jsengine.GetGlobalValue("gate_input");
                //var cv_result = (Dictionary<string, List<short>>)_jsengine.GetGlobalValue("cv_result");
                //var parameters_result = (Dictionary<string, List<short>>)_jsengine.GetGlobalValue("parameters_result");
                //HACK: i hate this
                return Newtonsoft.Json.JsonConvert.DeserializeObject<CVG>(_jsengine.GetGlobalValue<string>("cvg_result_raw"));
                //return new CVG(gate_result, cv_result, parameters_result);
            }catch(Exception ex)
            {

                Debug.WriteLine(ex);
            }
            return inputdata;
        }

    }
}
