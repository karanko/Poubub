using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
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

            CurrentState.Load();
            ProcessedResultsControl = new GenericTextControl("[Procesed Data]", new Action<TextBox>(x => x.Text = Utils.JSBeautify(JsonConvert.SerializeObject(CurrentState.Session.ProcessedResults.LastOrDefault(), Formatting.Indented))), null, 200);
        }
        private GenericTextControl ProcessedResultsControl;
        private void Form1_Load(object sender, EventArgs e)
        {
            BuildSession();
        }
        public void BuildSession()
        {
            //Clean up
            firstPanel.Clear();
            secondPanel.Clear();
            thirdPanel.Clear();
            forthPanel.Clear();

            //import session
            firstPanel.AddControl(new GenericTextControl("[Inital Data]", Utils.JSBeautify(JsonConvert.SerializeObject(CurrentState.Session.InitalData, Formatting.Indented)), new Action<string>(x => { try { CurrentState.Session.InitalData = JsonConvert.DeserializeObject<CVG>(x); } catch { } }), 200));

            //PanelControl modulepanel = new PanelControl();
            //modulepanel.Height = 500;
            //firstPanel.AddControl(modulepanel);
            //CurrentState.thisSession.Modules.ForEach(m => modulepanel.AddControl(new FunctionModuleControl(m)));
            CurrentState.Session.Modules.ForEach(m => firstPanel.AddControl(new FunctionModuleControl(m)).Parent.Parent.Parent.SizeChanged += UpdateMetadata);

            forthPanel.AddControl(ProcessedResultsControl);

            secondPanel.AddControl(new PatternControl());
            secondPanel.AddControl(new GenericTextControl("Global Functions", Utils.JSBeautify(CurrentState.Settings.Functions), new Action<string>(x => CurrentState.Settings.Functions = x),400));

            forthPanel.AddControl(new GenericTextControl("Session Notes", CurrentState.Session.Notes, new Action<string>(x => CurrentState.Session.Notes = x)));
            forthPanel.AddControl(new GenericTextControl("Global Notes", CurrentState.Settings.Notes, new Action<string>(x => CurrentState.Settings.Notes = x)));
            this.Text = System.IO.Path.GetFileNameWithoutExtension(CurrentState.Session.Name);

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
            BuildSession();
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
            CurrentState.Session.ProcessedResults.Add(CurrentState.Session.Process(CurrentState.Session.InitalData));
            if (CurrentState.Session.ProcessedResults.Count > 10)
            {
                CurrentState.Session.ProcessedResults.RemoveAt(0);
            }
            ProcessedResultsControl.UpdateData();
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentState.Load();
            BuildSession();
        }
        private void UpdateMetadata(object sender, EventArgs e)
        {
            ///HACK:This is dirty as 
            if (sender.GetType() == typeof(WrapperControl))
            {
                foreach(Control c in ((Control)sender).FindChildControlsOfType(typeof(FunctionModuleControl),10))
                {
                    if (!((WrapperControl)sender).Hidden)
                    {
                        if (!CurrentState.Metadata.ContainsKey("FunctionModuleControl_LastUnHidden"))
                        {
                            CurrentState.Metadata.Add("FunctionModuleControl_LastUnHidden", c);
                        }
                        else
                        {
                            CurrentState.Metadata["FunctionModuleControl_LastUnHidden"] = c;
                        }
                    }
                }
            }
        }

        private void tableLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {

        }

        private void insertModuleAboveStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!CurrentState.Metadata.ContainsKey("FunctionModuleControl_LastUnHidden"))
            //{
            //    CurrentState.Metadata.Add("FunctionModuleControl_LastUnHidden", c);
            //}
            if (CurrentState.Metadata.ContainsKey("FunctionModuleControl_LastUnHidden"))
            {
                FunctionModuleControl currentselcted = (FunctionModuleControl)CurrentState.Metadata["FunctionModuleControl_LastUnHidden"];
                int index = CurrentState.Session.Modules.IndexOf(currentselcted.Function);
                CurrentState.Session.Modules.Insert(index, new CVGFunction("function (data) {return data;}"));
                BuildSession();
            }
        }

        private void insertBelowStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!CurrentState.Metadata.ContainsKey("FunctionModuleControl_LastUnHidden"))
            //{
            //    CurrentState.Metadata.Add("FunctionModuleControl_LastUnHidden", c);
            //}
            if (CurrentState.Metadata.ContainsKey("FunctionModuleControl_LastUnHidden"))
            {
                FunctionModuleControl currentselcted = (FunctionModuleControl)CurrentState.Metadata["FunctionModuleControl_LastUnHidden"];
                int index = CurrentState.Session.Modules.IndexOf(currentselcted.Function);
                CurrentState.Session.Modules.Insert(1+ index, new CVGFunction("function (data) {return data;}"));          
                BuildSession();
            }
            
        }

        private void deleteStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentState.Metadata.ContainsKey("FunctionModuleControl_LastUnHidden"))
            {
                FunctionModuleControl currentselcted = (FunctionModuleControl)CurrentState.Metadata["FunctionModuleControl_LastUnHidden"];
                int index = CurrentState.Session.Modules.IndexOf(currentselcted.Function);
                CurrentState.Session.Modules.Remove( currentselcted.Function);
                BuildSession();
            }
        }
    }
}
