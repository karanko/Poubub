using Newtonsoft.Json;
using Poubub.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.Core
{
    public static class CurrentState
    {

        public static GlobalSettings Settings = new GlobalSettings();
        
        public static Session Session;

        public static void Save(string filename = null)
        {
            string settingser = Newtonsoft.Json.JsonConvert.SerializeObject(Settings, Formatting.Indented);
            if (File.ReadAllText("globalSettings.json") != settingser)
            {
                File.WriteAllText("globalSettings.json", settingser);
            }
            //TODO: default sve folder
            if (filename == null)
            {
                filename = Session.Name;
            }
            if (File.Exists(filename))
            {
                //TODO: tidy up make recursive
                if (File.Exists(filename + ".old3"))
                {
                    File.Delete(filename + ".old3");
                }
                if (File.Exists(filename + ".old2"))
                {
                    File.Move(filename + ".old2", filename + ".old3");
                }
                if (File.Exists(filename + ".old"))
                {
                    File.Move(filename + ".old", filename + ".old2");
                }

                File.Move(filename, filename + ".old");
            }
            Session.Name = Path.GetFileName(filename);
            File.WriteAllText(filename, JsonConvert.SerializeObject(Session,Formatting.Indented));
        }

        public static void Load(string filename = null)
        {
            Metadata.Clear();
            if (File.Exists("globalSettings.json"))
            {
                Settings = JsonConvert.DeserializeObject<GlobalSettings>(File.ReadAllText("globalSettings.json"));
            }
            Session newsess;
            if (filename == null)
            {
                filename = "defaultsession";
                if (File.Exists(filename))
                {
                    newsess = JsonConvert.DeserializeObject<Session>(File.ReadAllText(filename));
                }
                else
                {
                    newsess = new Session();
                    newsess.Modules.Add(new Core.CVGFunction(" function (data) { x = 1 + 1 ; return data } "));
                }
                newsess.Name = Utils.RandName() + ".pousess";
                Session = newsess;
            }
        }

        public static Dictionary<string, object> Metadata = new Dictionary<string, object>();
    }
}
