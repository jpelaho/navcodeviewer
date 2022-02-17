namespace NavCodeViewer.Forms
{
    partial class SearchOptions
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
            this.cmbSearchText = new System.Windows.Forms.ComboBox();
            this.cbMatchCasse = new System.Windows.Forms.CheckBox();
            this.cbMatchWord = new System.Windows.Forms.CheckBox();
            this.cbRegex = new System.Windows.Forms.CheckBox();
            this.cmbSearchIn = new System.Windows.Forms.ComboBox();
            this.lblSearchIn = new System.Windows.Forms.Label();
            this.btnSearchPrec = new System.Windows.Forms.Button();
            this.btnSearchSuiv = new System.Windows.Forms.Button();
            this.btnSearchAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbSearchText
            // 
            this.cmbSearchText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSearchText.FormattingEnabled = true;
            this.cmbSearchText.Location = new System.Drawing.Point(12, 12);
            this.cmbSearchText.Name = "cmbSearchText";
            this.cmbSearchText.Size = new System.Drawing.Size(409, 21);
            this.cmbSearchText.TabIndex = 0;
            this.cmbSearchText.DropDown += new System.EventHandler(this.cmbSearchText_DropDown);
            this.cmbSearchText.SelectedIndexChanged += new System.EventHandler(this.cmbSearchText_SelectedIndexChanged);
            this.cmbSearchText.TextChanged += new System.EventHandler(this.cmbSearchText_TextChanged);
            // 
            // cbMatchCasse
            // 
            this.cbMatchCasse.AutoSize = true;
            this.cbMatchCasse.Location = new System.Drawing.Point(12, 39);
            this.cbMatchCasse.Name = "cbMatchCasse";
            this.cbMatchCasse.Size = new System.Drawing.Size(55, 17);
            this.cbMatchCasse.TabIndex = 1;
            this.cbMatchCasse.Text = "Casse";
            this.cbMatchCasse.UseVisualStyleBackColor = true;
            this.cbMatchCasse.CheckedChanged += new System.EventHandler(this.cbMatchCasse_CheckedChanged);
            // 
            // cbMatchWord
            // 
            this.cbMatchWord.AutoSize = true;
            this.cbMatchWord.Location = new System.Drawing.Point(12, 62);
            this.cbMatchWord.Name = "cbMatchWord";
            this.cbMatchWord.Size = new System.Drawing.Size(83, 17);
            this.cbMatchWord.TabIndex = 2;
            this.cbMatchWord.Text = "Whole word";
            this.cbMatchWord.UseVisualStyleBackColor = true;
            this.cbMatchWord.CheckedChanged += new System.EventHandler(this.cbMatchWord_CheckedChanged);
            // 
            // cbRegex
            // 
            this.cbRegex.AutoSize = true;
            this.cbRegex.Location = new System.Drawing.Point(12, 85);
            this.cbRegex.Name = "cbRegex";
            this.cbRegex.Size = new System.Drawing.Size(52, 17);
            this.cbRegex.TabIndex = 3;
            this.cbRegex.Text = "regex";
            this.cbRegex.UseVisualStyleBackColor = true;
            this.cbRegex.CheckedChanged += new System.EventHandler(this.cbRegex_CheckedChanged);
            // 
            // cmbSearchIn
            // 
            this.cmbSearchIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSearchIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchIn.FormattingEnabled = true;
            this.cmbSearchIn.Location = new System.Drawing.Point(12, 121);
            this.cmbSearchIn.Name = "cmbSearchIn";
            this.cmbSearchIn.Size = new System.Drawing.Size(409, 21);
            this.cmbSearchIn.TabIndex = 4;
            this.cmbSearchIn.SelectedIndexChanged += new System.EventHandler(this.cmbSearchIn_SelectedIndexChanged);
            // 
            // lblSearchIn
            // 
            this.lblSearchIn.AutoSize = true;
            this.lblSearchIn.Location = new System.Drawing.Point(12, 105);
            this.lblSearchIn.Name = "lblSearchIn";
            this.lblSearchIn.Size = new System.Drawing.Size(47, 13);
            this.lblSearchIn.TabIndex = 5;
            this.lblSearchIn.Text = "seach in";
            // 
            // btnSearchPrec
            // 
            this.btnSearchPrec.Location = new System.Drawing.Point(12, 184);
            this.btnSearchPrec.Name = "btnSearchPrec";
            this.btnSearchPrec.Size = new System.Drawing.Size(125, 23);
            this.btnSearchPrec.TabIndex = 6;
            this.btnSearchPrec.Text = "Prec";
            this.btnSearchPrec.UseVisualStyleBackColor = true;
            this.btnSearchPrec.Click += new System.EventHandler(this.btnSearchPrec_Click);
            // 
            // btnSearchSuiv
            // 
            this.btnSearchSuiv.Location = new System.Drawing.Point(12, 155);
            this.btnSearchSuiv.Name = "btnSearchSuiv";
            this.btnSearchSuiv.Size = new System.Drawing.Size(125, 23);
            this.btnSearchSuiv.TabIndex = 7;
            this.btnSearchSuiv.Text = "Suiv";
            this.btnSearchSuiv.UseVisualStyleBackColor = true;
            this.btnSearchSuiv.Click += new System.EventHandler(this.btnSearchSuiv_Click);
            // 
            // btnSearchAll
            // 
            this.btnSearchAll.Location = new System.Drawing.Point(143, 184);
            this.btnSearchAll.Name = "btnSearchAll";
            this.btnSearchAll.Size = new System.Drawing.Size(125, 23);
            this.btnSearchAll.TabIndex = 8;
            this.btnSearchAll.Text = "Seach all";
            this.btnSearchAll.UseVisualStyleBackColor = true;
            this.btnSearchAll.Click += new System.EventHandler(this.btnSearchAll_Click);
            // 
            // SearchOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 213);
            this.Controls.Add(this.btnSearchAll);
            this.Controls.Add(this.btnSearchSuiv);
            this.Controls.Add(this.btnSearchPrec);
            this.Controls.Add(this.lblSearchIn);
            this.Controls.Add(this.cmbSearchIn);
            this.Controls.Add(this.cbRegex);
            this.Controls.Add(this.cbMatchWord);
            this.Controls.Add(this.cbMatchCasse);
            this.Controls.Add(this.cmbSearchText);
            this.Name = "SearchOptions";
            this.Text = "SearchOptions";
            this.Activated += new System.EventHandler(this.SearchOptions_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSearchText;
        private System.Windows.Forms.CheckBox cbMatchCasse;
        private System.Windows.Forms.CheckBox cbMatchWord;
        private System.Windows.Forms.CheckBox cbRegex;
        private System.Windows.Forms.ComboBox cmbSearchIn;
        private System.Windows.Forms.Label lblSearchIn;
        private System.Windows.Forms.Button btnSearchPrec;
        private System.Windows.Forms.Button btnSearchSuiv;
        private System.Windows.Forms.Button btnSearchAll;
    }
}