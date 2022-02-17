using FastColoredTextBoxNS;

namespace NavCodeViewer.Forms
{
    partial class SourceViewer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceViewer));
            this.fctb = new NavFCTB();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cmbTriggers = new System.Windows.Forms.ToolStripComboBox();
            this.cmbElements = new System.Windows.Forms.ToolStripComboBox();
            this.cmbFunctions = new System.Windows.Forms.ToolStripComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.fctb)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fctb
            // 
            this.fctb.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fctb.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\n^\\s*(case|default)\\s*[^:]*(" +
    "?<range>:)\\s*(?<range>[^;]+);";
            this.fctb.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.fctb.BackBrush = null;
            this.fctb.CharHeight = 14;
            this.fctb.CharWidth = 8;
            this.fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fctb.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fctb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fctb.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.fctb.IsReplaceMode = false;
            this.fctb.Location = new System.Drawing.Point(0, 27);
            this.fctb.Name = "fctb";
            this.fctb.Paddings = new System.Windows.Forms.Padding(0);
            this.fctb.SelectedLineBorderColor = System.Drawing.Color.Empty;
            this.fctb.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fctb.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fctb.ServiceColors")));
            this.fctb.Size = new System.Drawing.Size(800, 423);
            this.fctb.TabIndex = 0;
            this.fctb.Zoom = 100;
            this.fctb.SizeChanged += new System.EventHandler(this.fctb_SizeChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbTriggers,
            this.cmbElements,
            this.cmbFunctions});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 27);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // cmbTriggers
            // 
            this.cmbTriggers.Name = "cmbTriggers";
            this.cmbTriggers.Size = new System.Drawing.Size(121, 23);
            this.cmbTriggers.SelectedIndexChanged += new System.EventHandler(this.cmbTriggers_SelectedIndexChanged);
            // 
            // cmbElements
            // 
            this.cmbElements.Name = "cmbElements";
            this.cmbElements.Size = new System.Drawing.Size(121, 23);
            this.cmbElements.SelectedIndexChanged += new System.EventHandler(this.cmbElements_SelectedIndexChanged);
            // 
            // cmbFunctions
            // 
            this.cmbFunctions.Name = "cmbFunctions";
            this.cmbFunctions.Size = new System.Drawing.Size(121, 23);
            this.cmbFunctions.SelectedIndexChanged += new System.EventHandler(this.cmbFunctions_SelectedIndexChanged);
            // 
            // SourceViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.fctb);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SourceViewer";
            this.Text = "SourceViewer";
            this.Load += new System.EventHandler(this.SourceViewer_Load);
            this.ResizeEnd += new System.EventHandler(this.SourceViewer_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.fctb)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NavFCTB fctb;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripComboBox cmbTriggers;
        private System.Windows.Forms.ToolStripComboBox cmbElements;
        private System.Windows.Forms.ToolStripComboBox cmbFunctions;
    }
}