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
    public partial class PanelControl : UserControl
    {
        public PanelControl()
        {
            InitializeComponent();
            this.ModulesPanel.ControlAdded += Control_Added;
        }

        public Control AddControl(Control control)
        {
            var wc = new Forms.WrapperControl(control);
            this.ModulesPanel.Controls.Add(wc);
            return control;
        }
        public void Clear()
        {

            this.ModulesPanel.Controls.Clear();
        
        }

        private void Control_Added(object sender, ControlEventArgs e)
        {
            e.Control.Width = this.Width;
           // e.Control.StyleEverything();
        }
    }
}
