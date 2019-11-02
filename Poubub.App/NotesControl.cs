using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poubub.App
{
    public partial class NotesControl : UserControl
    {
        public NotesControl()
        {
            InitializeComponent();
            textBox1.Text = CurrentState.thisSession.Notes;
            this.Name = "Notes";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CurrentState.thisSession.Notes = ((TextBox)sender).Text;
        }
    }
}
