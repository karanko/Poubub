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
using System.Diagnostics;

namespace Poubub.App
{
    public partial class PatternControl : UserControl
    {
        public PatternControl()
        {
            InitializeComponent();
            this.Name = "Sequence";
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CurrentState.thisSession.Sequence.Patterns.First().Steps = ((TextBox)sender).Text;
            GetResult();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            CurrentState.thisSession.Sequence.Patterns.First().Offsets = ((TextBox)sender).Text;
            GetResult();
        }
        private void GetResult()
        {
            try
            {
                textBox3.Text = CurrentState.thisSession.Sequence.ExpandSteps(CurrentState.thisSession.Sequence.Patterns.First());
            }
            catch (Exception ex)
            {
                textBox3.Text = ex.Message;
                Debug.WriteLine(ex);
            }
        }
        private void PatternControl_Load(object sender, EventArgs e)
        {
            if (CurrentState.thisSession.Sequence.Patterns.Count == 0)
            {
                CurrentState.thisSession.Sequence.Patterns.Add(new StepPattern());
            }

            textBox1.Text = CurrentState.thisSession.Sequence.Patterns.First().Steps;
            textBox2.Text = CurrentState.thisSession.Sequence.Patterns.First().Offsets;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
