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
        public GenericTextControl(string name, string inputobject , Action<string> action)
        {
            
            InitializeComponent();
            this.Name = name;
            this.ChangeAction = action;
            textBox1.Text = inputobject;
        }
        public Action<string> ChangeAction;

        private void Text_Changed(object sender, EventArgs e)
        {
            ChangeAction(((TextBox)sender).Text);
        }
    }
}
