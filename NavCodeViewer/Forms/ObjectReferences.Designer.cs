namespace NavCodeViewer.Forms
{
    partial class ObjectReferences
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
            this.tvObjectRefs = new System.Windows.Forms.TreeView();
            this.TVimageList = new System.Windows.Forms.ImageList(this.components);
            this.NodeDoubleClicTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tvObjectRefs
            // 
            this.tvObjectRefs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvObjectRefs.ImageIndex = 0;
            this.tvObjectRefs.ImageList = this.TVimageList;
            this.tvObjectRefs.Location = new System.Drawing.Point(0, 0);
            this.tvObjectRefs.Name = "tvObjectRefs";
            this.tvObjectRefs.SelectedImageIndex = 0;
            this.tvObjectRefs.Size = new System.Drawing.Size(468, 401);
            this.tvObjectRefs.TabIndex = 0;
            this.tvObjectRefs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvObjectStructure_AfterSelect);
            this.tvObjectRefs.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvObjectRefs_NodeMouseClick);
            this.tvObjectRefs.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvObjectRefs_NodeMouseDoubleClick);
            this.tvObjectRefs.Click += new System.EventHandler(this.tvObjectRefs_Click);
            this.tvObjectRefs.DoubleClick += new System.EventHandler(this.tvObjectRefs_DoubleClick);
            this.tvObjectRefs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvObjectRefs_MouseDown);
            // 
            // TVimageList
            // 
            this.TVimageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.TVimageList.ImageSize = new System.Drawing.Size(16, 16);
            this.TVimageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timer1
            // 
            this.NodeDoubleClicTimer.Interval = 500;
            this.NodeDoubleClicTimer.Tick += new System.EventHandler(this.NodeDoubleClicTimer_Tick);
            // 
            // ObjectReferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 401);
            this.Controls.Add(this.tvObjectRefs);
            this.Name = "ObjectReferences";
            this.Text = "ObjectStructure";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvObjectRefs;
        private System.Windows.Forms.ImageList TVimageList;
        private System.Windows.Forms.Timer NodeDoubleClicTimer;
    }
}