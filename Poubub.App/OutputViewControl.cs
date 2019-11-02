using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Poubub.Forms;

namespace Poubub.App
{
    public partial class OutputViewControl : UserControl
    {
        public OutputViewControl()
        {
            InitializeComponent();
            this.StyleEverything();
            tabControl1.TabPages.Clear();
        }

        public void AddThing(string Name, object source, Func<object,string> function)
        {

            if (Data.ContainsKey(Name))
            {
                Data[Name] = new KeyValuePair<object, Func<object,string>>(source, function);
            }
            else
            {
                Data.Add(Name,new KeyValuePair<object, Func<object,string>>(source, function));
                if(!tabControl1.TabPages.ContainsKey(Name))
                {
                    tabControl1.TabPages.Add(Name, Name);
                    tabControl1.TabPages[tabControl1.TabPages.IndexOfKey(Name)].Controls.Add(new TextBox() {  Dock = DockStyle.Fill, Multiline = true});
                    tabControl1.TabPages[tabControl1.TabPages.IndexOfKey(Name)].StyleEverything();
                }
            }

        }
        public void Update(string Name)
        {
            ((TextBox)tabControl1.TabPages[tabControl1.TabPages.IndexOfKey(Name)].Controls[0]).Text = Data[Name].Value(Data[Name].Key);
        }


       private Dictionary<string, KeyValuePair<object, Func<object,string>>> Data = new Dictionary<string, KeyValuePair<object, Func<object,string>>>();



    }
}
