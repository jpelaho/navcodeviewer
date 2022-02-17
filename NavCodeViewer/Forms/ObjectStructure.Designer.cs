namespace NavCodeViewer.Forms
{
    partial class ObjectStructure
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
            this.tvObjectStructure = new System.Windows.Forms.TreeView();
            this.TVimageList = new System.Windows.Forms.ImageList(this.components);
            this.NodeDoubleClicTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tvObjectStructure
            // 
            this.tvObjectStructure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvObjectStructure.ImageIndex = 0;
            this.tvObjectStructure.ImageList = this.TVimageList;
            this.tvObjectStructure.Location = new System.Drawing.Point(0, 0);
            this.tvObjectStructure.Name = "tvObjectStructure";
            this.tvObjectStructure.SelectedImageIndex = 0;
            this.tvObjectStructure.Size = new System.Drawing.Size(468, 401);
            this.tvObjectStructure.TabIndex = 0;
            this.tvObjectStructure.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvObjectStructure_AfterSelect);
            this.tvObjectStructure.Click += new System.EventHandler(this.tvObjectStructure_Click);
            this.tvObjectStructure.DoubleClick += new System.EventHandler(this.tvObjectStructure_DoubleClick);
            this.tvObjectStructure.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvObjectStructure_MouseDown);
            // 
            // TVimageList
            // 
            this.TVimageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.TVimageList.ImageSize = new System.Drawing.Size(16, 16);
            this.TVimageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // NodeDoubleClicTimer
            // 
            this.NodeDoubleClicTimer.Interval = 500;
            this.NodeDoubleClicTimer.Tick += new System.EventHandler(this.NodeDoubleClicTimer_Tick);
            // 
            // ObjectStructure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 401);
            this.Controls.Add(this.tvObjectStructure);
            this.Name = "ObjectStructure";
            this.Text = "ObjectStructure";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvObjectStructure;
        private System.Windows.Forms.ImageList TVimageList;
        private System.Windows.Forms.Timer NodeDoubleClicTimer;
    }
}