﻿namespace Poubub.Forms
{
    partial class PanelControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ModulesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // ModulesPanel
            // 
            this.ModulesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModulesPanel.Location = new System.Drawing.Point(0, 0);
            this.ModulesPanel.Name = "ModulesPanel";
            this.ModulesPanel.Size = new System.Drawing.Size(133, 283);
            this.ModulesPanel.TabIndex = 0;
            // 
            // PanelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ModulesPanel);
            this.Name = "PanelControl";
            this.Size = new System.Drawing.Size(133, 283);
            this.SizeChanged += new System.EventHandler(this.PanelControl_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ModulesPanel;
    }
}
