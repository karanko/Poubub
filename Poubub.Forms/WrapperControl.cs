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
    public partial class WrapperControl : UserControl
    {
        public Control Control= new TextBox() { Name = "ThingName", Text = "Some Data", BorderStyle = BorderStyle.None, Multiline = true } ;
        public WrapperControl()
        {
            InitializeComponent();
            Init();
        }
        private int naturalHeight = 300;
        public WrapperControl(Control control, bool show = false)
        {
            Hidden = !show;
            Control = control;
           
            InitializeComponent();

            Init();
        }
        private void Init()
        {
            this.StyleEverything();
            
            panel1.ControlAdded += Control_Added;
            this.tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            if (Control != null)
            {
                naturalHeight = Control.Height;
                this.Name = Control.Name;
                panel1.Controls.Add(Control);
            }
            HeaderButton.FlatStyle = FlatStyle.Flat;
            HeaderButton.FlatAppearance.BorderSize = 0;
            HeaderButton.BackColor = rainbowColor;
            HeaderButton.ForeColor = darkColor;
            HeaderButton.Text = this.Name;
            menuStrip.BackColor = darkColor;
            menuStrip.ForeColor = lightColor;
            nameToolStripMenuItem.Text = this.Name;
            nameToolStripMenuItem.BackColor = HeaderButton.BackColor;
            nameToolStripMenuItem.ForeColor = HeaderButton.ForeColor;
            nameToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            SetWindowHeights();

        }


        private void Control_Added(object sender, ControlEventArgs e)
        {
            
            e.Control.Dock = DockStyle.Fill;
            e.Control.StyleEverything();
        }

        private void Header_Click(object sender, EventArgs e)
        {
            Hidden = !Hidden;
            //saveLabel.Enabled = !Hidden;
            SetWindowHeights();

            if (!Hidden)
            {
                foreach (var pchild in this.Parent.Controls)
                {

                    if (!pchild.Equals(this) && pchild.GetType() == typeof(WrapperControl))
                    {
                        WrapperControl otherchild = (WrapperControl)pchild;
                        if (!otherchild.Hidden)
                        {
                            otherchild.Hidden = true;
                            otherchild.SetWindowHeights();
                        }
                    }
                }
            }
        }
        private void SetWindowHeights()
        {
            if (!Hidden)
            {
                // HideShowlabel.Text = "Hide";
                this.Height = naturalHeight;
                this.tableLayoutPanel1.RowStyles[0].Height = 10;

                
            }
            else
            {
                //  HideShowlabel.Text = "Show";
                this.Height = 20;
                this.tableLayoutPanel1.RowStyles[0].Height = this.Height;
                this.HeaderButton.Height = this.Height;

            }
        }
        internal Color rainbowColor = Forms.Theme.GetRainbowColor();
        internal Color darkColor = Forms.Theme.GetDarkColor();
        internal Color lightColor = Forms.Theme.GetLightColor();

        public bool Hidden { get; internal set; } = false;
    }
}
