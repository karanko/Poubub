using Newtonsoft.Json;
using Poubub.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poubub.App
{
    public static class CurrentState
    {

        public static Session thisSession;

        public static void Save(string filename = null)
        {
            //TODO: default sve folder
            if (filename == null)
            {
                filename = thisSession.Name;
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
            thisSession.Name = Path.GetFileName(filename);
            File.WriteAllText(filename, JsonConvert.SerializeObject(thisSession,Formatting.Indented));
        }

        public static void Load(string filename = null)
        {
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
                thisSession = newsess;
            }
        }
    }
}
