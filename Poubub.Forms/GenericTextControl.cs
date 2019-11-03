using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poubub.Forms
{
    public partial class GenericTextControl : UserControl
    {
        public GenericTextControl()
        {
            InitializeComponent();
        }
        public GenericTextControl(string name, string inputobject , Action<string> action, int? height = null)
        {
            
            InitializeComponent();
            this.Name = name;
            this.ChangeAction = action;
            textBox1.Text = inputobject;
            if(height != null)
            {
                this.Height = (int)height;
            }
        }
        public GenericTextControl(string name, Action<TextBox> remoteaction, Action<string> action, int? height = null)
        {
            
            InitializeComponent();
            this.Name = name;
            this.ChangeAction = action;
            this.RemoteAction = remoteaction;
            if(action == null)
            {
                textBox1.Enabled = false;
            }

            if (height != null)
            {
                this.Height = (int)height;
            }
        }
        private Action<string> ChangeAction;
        private Action<TextBox> RemoteAction;
        public string UpdateData()
        {
            if (RemoteAction != null )
            {
                RemoteAction(textBox1);
                return textBox1.Text;
            }
            return null;
        }
        private void Text_Changed(object sender, EventArgs e)
        {
            if (ChangeAction != null)
            {
                ChangeAction(((TextBox)sender).Text);
            }
        }
    }
}
