namespace NavCodeViewer.Forms
{
    partial class FindErrorMessage
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
            this.cmdFindAll = new System.Windows.Forms.Button();
            this.LblExample = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cmdFindAll
            // 
            this.cmdFindAll.Location = new System.Drawing.Point(328, 70);
            this.cmdFindAll.Name = "cmdFindAll";
            this.cmdFindAll.Size = new System.Drawing.Size(210, 23);
            this.cmdFindAll.TabIndex = 0;
            this.cmdFindAll.Text = "button1";
            this.cmdFindAll.UseVisualStyleBackColor = true;
            this.cmdFindAll.Click += new System.EventHandler(this.cmdFindAll_Click);
            // 
            // LblExample
            // 
            this.LblExample.AutoSize = true;
            this.LblExample.Location = new System.Drawing.Point(10, 43);
            this.LblExample.Name = "LblExample";
            this.LblExample.Size = new System.Drawing.Size(35, 13);
            this.LblExample.TabIndex = 1;
            this.LblExample.Text = "label1";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(8, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(530, 20);
            this.txtSearch.TabIndex = 2;
            // 
            // FindErrorMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 100);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.LblExample);
            this.Controls.Add(this.cmdFindAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FindErrorMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FindErrorMessage";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FindErrorMessage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdFindAll;
        private System.Windows.Forms.Label LblExample;
        private System.Windows.Forms.TextBox txtSearch;
    }
}