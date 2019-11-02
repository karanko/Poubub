using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Poubub.Core;
using Poubub.Forms;

namespace Poubub.App
{
    public partial class FunctionModuleControl : UserControl
    {
        public FunctionModuleControl()
        {
            InitializeComponent();
        }
        public CVGFunction Function;
        private  JSBeautifyLib.JSBeautifyOptions  jsbo  = new JSBeautifyLib.JSBeautifyOptions();
        public FunctionModuleControl(CVGFunction function)
        {
            Function = function;
            InitializeComponent();
            textBox.Text = (new JSBeautifyLib.JSBeautify(Function.Script, jsbo)).GetResult();  ;
            this.Name = Function.Name;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            Function.Script = ((TextBox)sender).Text;
        }
    }
}
