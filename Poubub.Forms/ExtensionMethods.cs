using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poubub.Forms
{
    public static class ExtensionMethods
    {
        public static List<Control> FindChildControlsOfType(this Control sender, Type type, int depth = 10)
        {
            List<Control> results = new List<Control>();

            if (sender.GetType() == type)
            {
                results.Add(sender);
            }

            if (depth > 0)
            {
                foreach (Control subcontrol in sender.Controls)
                {
                    results.AddRange(subcontrol.FindChildControlsOfType(type, depth - 1));
                }
            }
            return results;
        }

        public static void StyleEverything(this Control control)
        {
            control.Margin = Padding.Empty;
            control.Padding = Padding.Empty;
           
            if(control.GetType() == typeof(TextBox))
            {
                
                control.BackColor = Color.Black;
                control.ForeColor = Theme.GetLightColor();
                ((TextBox)control).BorderStyle = BorderStyle.FixedSingle;
            }
           
            if (control.Controls.Count > 0)
            {
                foreach (Control subControl in control.Controls)
                {
                    subControl.StyleEverything();
                }
            }
        }
    }
}
