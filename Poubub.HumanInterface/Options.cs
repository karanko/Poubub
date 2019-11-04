using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.HumanInterface
{
    public static class Options
    {
    

        private static Dictionary<string, object> _options = new Dictionary<string, object>();

        public static string Read(string KeyName,string ifnullvalue = null)
        {
            if(_options.Count == 0 )
            {
                Read();
            }

            try
            {
                if (_options.ContainsKey(KeyName))
                {
                    return ifnullvalue;
                }
                return (string)_options[KeyName];
            }
            catch {
                return ifnullvalue;
            }
        }

        public static int Read(string KeyName, int ifnullvalue )
        {
            if (_options.Count == 0)
            {
                Read();
            }
            if (_options.ContainsKey(KeyName))
            {
                return ifnullvalue;
            }
            return (int)_options[KeyName];
        }
        public static int? Read(string KeyName)
        {
            if (_options.Count == 0)
            {
                Read();
            }
            if (_options.ContainsKey(KeyName))
            {
                return null;
            }
            return (int)_options[KeyName];
        }

        public static  bool Write(string KeyName, object Value)
        {

            if (!_options.ContainsKey(KeyName))
            {
                _options.Add(KeyName, Value);
            }
            else
            {
                 _options[KeyName] = Value;
            }
            return Commit();
        }

        private static bool Commit()
        {
            try
            {
                System.IO.File.WriteAllText("options.json", Newtonsoft.Json.JsonConvert.SerializeObject(_options));
                return true;
            }
            catch(Exception ex)  {

                System.Diagnostics.Debug.WriteLine(ex);
            }
            return false;
        }
        private static bool Read()
        {
            try
            {
                _options = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(System.IO.File.ReadAllText("options.json"));
                return true;
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(ex);
            }
            return false;
        }

        public static bool DeleteKey(string KeyName)
        {
            if (_options.ContainsKey(KeyName))
            {
                _options.Remove(KeyName);
            }
            return true;
        }


     
    }

}
