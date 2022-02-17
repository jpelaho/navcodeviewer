namespace NavCodeViewer.Forms
{
    partial class ObjectViewer
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
            this.TViewObjViewer = new System.Windows.Forms.TreeView();
            this.TVimageList = new System.Windows.Forms.ImageList(this.components);
            this.NodeDoubleClicTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // TViewObjViewer
            // 
            this.TViewObjViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TViewObjViewer.ImageIndex = 0;
            this.TViewObjViewer.ImageList = this.TVimageList;
            this.TViewObjViewer.Location = new System.Drawing.Point(0, 0);
            this.TViewObjViewer.Name = "TViewObjViewer";
            this.TViewObjViewer.SelectedImageIndex = 0;
            this.TViewObjViewer.Size = new System.Drawing.Size(800, 450);
            this.TViewObjViewer.TabIndex = 0;
            this.TViewObjViewer.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TViewObjViewer_AfterSelect);
            this.TViewObjViewer.Click += new System.EventHandler(this.TViewObjViewer_Click);
            this.TViewObjViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TViewObjViewer_MouseDown);
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
            // ObjectViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.TViewObjViewer);
            this.Name = "ObjectViewer";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ObjectViewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView TViewObjViewer;
        private System.Windows.Forms.ImageList TVimageList;
        private System.Windows.Forms.Timer NodeDoubleClicTimer;
    }
}