using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Poubub.Core;
using Poubub.Forms;

namespace Poubub.App
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.BackColor = Forms.Theme.GetDarkColor();
            this.ForeColor = Forms.Theme.GetLightColor();
            outputViewControl1.StyleEverything();
         
            CurrentState.Load();
            BuildSession();
            outputViewControl1.AddThing("CVG1", CurrentState.thisSession, new Func<object, string>(x => Newtonsoft.Json.JsonConvert.SerializeObject(((Session)x).ProcessedResults.LastOrDefault(),Newtonsoft.Json.Formatting.Indented)));
        }

    
     
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    firstPanel.Controls.Clear();
        //    firstPanel.Controls.Add((Button)sender);
        //    for (int i = 0; i < 10; i++)
        //    {
        //        var c = Forms.Utils.GetRainbowColor();
        //        firstPanel.Controls.Add(new TextBox() { Height = 10, Width = 150, BackColor = c, Text = c.ToString() });
        //    }
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            BuildSession();
        }
        public void BuildSession()
        {
            //Clean up
            firstPanel.Clear();
            secondPanel.Clear();

            //import session

            CurrentState.thisSession.Modules.ForEach(m => firstPanel.AddControl(new FunctionModuleControl(m)));
            secondPanel.AddControl(new PatternControl());
            secondPanel.AddControl(new GenericTextControl("Global Functions", Utils.JSBeautify( CurrentState.Settings.Functions), new Action<string>(x => CurrentState.Settings.Functions = x)));
            secondPanel.AddControl(new GenericTextControl("Session Notes", CurrentState.thisSession.Notes, new Action<string>(x => CurrentState.thisSession.Notes = x)));
            secondPanel.AddControl(new GenericTextControl("Global Notes", CurrentState.Settings.Notes, new Action<string>(x => CurrentState.Settings.Notes = x)));
            this.Text = System.IO.Path.GetFileNameWithoutExtension(CurrentState.thisSession.Name);

        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
             CurrentState.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Poubub Session|*.pousess|JSON|*.json";
            dialog.Title = "Save Session";
            dialog.ShowDialog();
            if (!String.IsNullOrEmpty(dialog.FileName))
            {
                CurrentState.Save(dialog.FileName);
                BuildSession();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Poubub Session|*.pousess";
            dialog.Title = "Open Session";
            dialog.ShowDialog();
            if (!String.IsNullOrEmpty(dialog.FileName))
            {
                CurrentState.Load(dialog.FileName);
                BuildSession();
            }
        }

        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Really Quit?", "Confirmation", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void processModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentState.thisSession.ProcessedResults.Add(CurrentState.thisSession.Process(CurrentState.thisSession.InitalData));
            if(CurrentState.thisSession.ProcessedResults.Count > 10)
            {
                CurrentState.thisSession.ProcessedResults.RemoveAt(0);
            }
            outputViewControl1.Update("CVG1");
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentState.Load();
            BuildSession();
        }
    }
}
