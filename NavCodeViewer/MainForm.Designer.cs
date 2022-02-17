

namespace NavCodeViewer
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripSplitButtonZoom = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripComboBoxZoom = new System.Windows.Forms.ToolStripComboBox();
            this.MainProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.TSStatusLabelProcessing = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsBtnCopy = new System.Windows.Forms.ToolStripButton();
            this.tsBtnCollapseSelectedBlock = new System.Windows.Forms.ToolStripButton();
            this.tsBtnCollapAllObjectBlocks = new System.Windows.Forms.ToolStripButton();
            this.tsBtnExpandAllObjectBlocks = new System.Windows.Forms.ToolStripButton();
            this.tsBtnCollapseAllFunctions = new System.Windows.Forms.ToolStripButton();
            this.bookmarkPlusButton = new System.Windows.Forms.ToolStripButton();
            this.bookmarkMinusButton = new System.Windows.Forms.ToolStripButton();
            this.gotoBookmark = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsBtnRemoveAllBookmarks = new System.Windows.Forms.ToolStripButton();
            this.tsBtnNavigationPrec = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsBtnNavigationSuiv = new System.Windows.Forms.ToolStripButton();
            this.tsBtnNaviguerPrec = new System.Windows.Forms.ToolStripButton();
            this.tsBtnSearch = new System.Windows.Forms.ToolStripButton();
            this.tsBtnFindMessagesText = new System.Windows.Forms.ToolStripButton();
            this.tsBtnSelectAll = new System.Windows.Forms.ToolStripButton();
            this.TreeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.statusStripMain.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButtonZoom,
            this.MainProgressBar,
            this.TSStatusLabelProcessing});
            this.statusStripMain.Location = new System.Drawing.Point(0, 428);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(800, 22);
            this.statusStripMain.TabIndex = 1;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // toolStripSplitButtonZoom
            // 
            this.toolStripSplitButtonZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripSplitButtonZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBoxZoom});
            this.toolStripSplitButtonZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButtonZoom.Name = "toolStripSplitButtonZoom";
            this.toolStripSplitButtonZoom.Size = new System.Drawing.Size(133, 20);
            this.toolStripSplitButtonZoom.Text = "toolStripSplitButton1";
            // 
            // toolStripComboBoxZoom
            // 
            this.toolStripComboBoxZoom.Items.AddRange(new object[] {
            "20%",
            "50%",
            "70%",
            "100%",
            "110%",
            "150%",
            "200%"});
            this.toolStripComboBoxZoom.Name = "toolStripComboBoxZoom";
            this.toolStripComboBoxZoom.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBoxZoom.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxZoom_SelectedIndexChanged);
            this.toolStripComboBoxZoom.TextChanged += new System.EventHandler(this.toolStripComboBoxZoom_TextChanged);
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.Size = new System.Drawing.Size(500, 16);
            // 
            // TSStatusLabelProcessing
            // 
            this.TSStatusLabelProcessing.Name = "TSStatusLabelProcessing";
            this.TSStatusLabelProcessing.Size = new System.Drawing.Size(118, 17);
            this.TSStatusLabelProcessing.Text = "toolStripStatusLabel1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtnCopy,
            this.tsBtnCollapseSelectedBlock,
            this.tsBtnCollapAllObjectBlocks,
            this.tsBtnExpandAllObjectBlocks,
            this.tsBtnCollapseAllFunctions,
            this.bookmarkPlusButton,
            this.bookmarkMinusButton,
            this.gotoBookmark,
            this.tsBtnRemoveAllBookmarks,
            this.tsBtnNavigationPrec,
            this.tsBtnNavigationSuiv,
            this.tsBtnNaviguerPrec,
            this.tsBtnSearch,
            this.tsBtnFindMessagesText,
            this.tsBtnSelectAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsBtnCopy
            // 
            this.tsBtnCopy.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnCopy.Image")));
            this.tsBtnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnCopy.Name = "tsBtnCopy";
            this.tsBtnCopy.Size = new System.Drawing.Size(55, 22);
            this.tsBtnCopy.Text = "&Copy";
            this.tsBtnCopy.Click += new System.EventHandler(this.tsBtnCopy_Click);
            // 
            // tsBtnCollapseSelectedBlock
            // 
            this.tsBtnCollapseSelectedBlock.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnCollapseSelectedBlock.Image")));
            this.tsBtnCollapseSelectedBlock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnCollapseSelectedBlock.Name = "tsBtnCollapseSelectedBlock";
            this.tsBtnCollapseSelectedBlock.Size = new System.Drawing.Size(145, 22);
            this.tsBtnCollapseSelectedBlock.Text = "CollapseSelectedBlock";
            this.tsBtnCollapseSelectedBlock.Click += new System.EventHandler(this.tsBtnCollapseSelectedBlock_Click);
            // 
            // tsBtnCollapAllObjectBlocks
            // 
            this.tsBtnCollapAllObjectBlocks.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnCollapAllObjectBlocks.Image")));
            this.tsBtnCollapAllObjectBlocks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnCollapAllObjectBlocks.Name = "tsBtnCollapAllObjectBlocks";
            this.tsBtnCollapAllObjectBlocks.Size = new System.Drawing.Size(144, 22);
            this.tsBtnCollapAllObjectBlocks.Text = "CollapAllObjectBlocks";
            this.tsBtnCollapAllObjectBlocks.Click += new System.EventHandler(this.tsBtnCollapseAllObjectBlocks_Click);
            // 
            // tsBtnExpandAllObjectBlocks
            // 
            this.tsBtnExpandAllObjectBlocks.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnExpandAllObjectBlocks.Image")));
            this.tsBtnExpandAllObjectBlocks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnExpandAllObjectBlocks.Name = "tsBtnExpandAllObjectBlocks";
            this.tsBtnExpandAllObjectBlocks.Size = new System.Drawing.Size(149, 22);
            this.tsBtnExpandAllObjectBlocks.Text = "ExpandAllObjectBlocks";
            this.tsBtnExpandAllObjectBlocks.Click += new System.EventHandler(this.tsBtnExpandAllObjectBlocks_Click);
            // 
            // tsBtnCollapseAllFunctions
            // 
            this.tsBtnCollapseAllFunctions.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnCollapseAllFunctions.Image")));
            this.tsBtnCollapseAllFunctions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnCollapseAllFunctions.Name = "tsBtnCollapseAllFunctions";
            this.tsBtnCollapseAllFunctions.Size = new System.Drawing.Size(138, 22);
            this.tsBtnCollapseAllFunctions.Text = "CollapseAllFunctions";
            this.tsBtnCollapseAllFunctions.Click += new System.EventHandler(this.tsBtnCollapseAllFunctions_Click);
            // 
            // bookmarkPlusButton
            // 
            this.bookmarkPlusButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bookmarkPlusButton.Name = "bookmarkPlusButton";
            this.bookmarkPlusButton.Size = new System.Drawing.Size(132, 22);
            this.bookmarkPlusButton.Text = "Add bookmark (Ctrl-B)";
            this.bookmarkPlusButton.Click += new System.EventHandler(this.bookmarkPlusButton_Click);
            // 
            // bookmarkMinusButton
            // 
            this.bookmarkMinusButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bookmarkMinusButton.Name = "bookmarkMinusButton";
            this.bookmarkMinusButton.Size = new System.Drawing.Size(182, 19);
            this.bookmarkMinusButton.Text = "Remove bookmark (Ctrl-Shift-B)";
            this.bookmarkMinusButton.Click += new System.EventHandler(this.bookmarkMinusButton_Click);
            // 
            // gotoBookmark
            // 
            this.gotoBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.gotoBookmark.Image = ((System.Drawing.Image)(resources.GetObject("gotoBookmark.Image")));
            this.gotoBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.gotoBookmark.Name = "gotoBookmark";
            this.gotoBookmark.Size = new System.Drawing.Size(55, 19);
            this.gotoBookmark.Text = "Goto...";
            this.gotoBookmark.DropDownOpening += new System.EventHandler(this.gotoBookmark_DropDownOpening);
            // 
            // tsBtnRemoveAllBookmarks
            // 
            this.tsBtnRemoveAllBookmarks.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnRemoveAllBookmarks.Image")));
            this.tsBtnRemoveAllBookmarks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnRemoveAllBookmarks.Name = "tsBtnRemoveAllBookmarks";
            this.tsBtnRemoveAllBookmarks.Size = new System.Drawing.Size(147, 20);
            this.tsBtnRemoveAllBookmarks.Text = "Remove all bookmarks";
            this.tsBtnRemoveAllBookmarks.Click += new System.EventHandler(this.tsBtnRemoveAllBookmarks_Click);
            // 
            // tsBtnNavigationPrec
            // 
            this.tsBtnNavigationPrec.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnNavigationPrec.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnNavigationPrec.Image")));
            this.tsBtnNavigationPrec.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnNavigationPrec.Name = "tsBtnNavigationPrec";
            this.tsBtnNavigationPrec.Size = new System.Drawing.Size(76, 19);
            this.tsBtnNavigationPrec.Text = "GotoNav...";
            this.tsBtnNavigationPrec.DropDownOpening += new System.EventHandler(this.tsBtnNavigationPrec_DropDownOpening);
            // 
            // tsBtnNavigationSuiv
            // 
            this.tsBtnNavigationSuiv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsBtnNavigationSuiv.Enabled = false;
            this.tsBtnNavigationSuiv.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnNavigationSuiv.Image")));
            this.tsBtnNavigationSuiv.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnNavigationSuiv.Name = "tsBtnNavigationSuiv";
            this.tsBtnNavigationSuiv.Size = new System.Drawing.Size(98, 19);
            this.tsBtnNavigationSuiv.Text = "NaviguerSuivant";
            this.tsBtnNavigationSuiv.Click += new System.EventHandler(this.tsBtnNavigationSuiv_Click);
            // 
            // tsBtnNaviguerPrec
            // 
            this.tsBtnNaviguerPrec.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnNaviguerPrec.Image")));
            this.tsBtnNaviguerPrec.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnNaviguerPrec.Name = "tsBtnNaviguerPrec";
            this.tsBtnNaviguerPrec.Size = new System.Drawing.Size(98, 20);
            this.tsBtnNaviguerPrec.Text = "NaviguerPrec";
            this.tsBtnNaviguerPrec.Click += new System.EventHandler(this.tsBtnNaviguerPrec_Click);
            // 
            // tsBtnSearch
            // 
            this.tsBtnSearch.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnSearch.Image")));
            this.tsBtnSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnSearch.Name = "tsBtnSearch";
            this.tsBtnSearch.Size = new System.Drawing.Size(50, 20);
            this.tsBtnSearch.Text = "Find";
            this.tsBtnSearch.Click += new System.EventHandler(this.tsBtnSearch_Click);
            // 
            // tsBtnFindMessagesText
            // 
            this.tsBtnFindMessagesText.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnFindMessagesText.Image")));
            this.tsBtnFindMessagesText.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnFindMessagesText.Name = "tsBtnFindMessagesText";
            this.tsBtnFindMessagesText.Size = new System.Drawing.Size(73, 20);
            this.tsBtnFindMessagesText.Text = "FindMsg";
            this.tsBtnFindMessagesText.Click += new System.EventHandler(this.tsBtnFindMessagesText_Click);
            // 
            // tsBtnSelectAll
            // 
            this.tsBtnSelectAll.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnSelectAll.Image")));
            this.tsBtnSelectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnSelectAll.Name = "tsBtnSelectAll";
            this.tsBtnSelectAll.Size = new System.Drawing.Size(72, 20);
            this.tsBtnSelectAll.Text = "SelectAll";
            this.tsBtnSelectAll.Click += new System.EventHandler(this.tsBtnSelectAll_Click);
            // 
            // TreeViewImageList
            // 
            this.TreeViewImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.TreeViewImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.TreeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStripMain);
            this.IsMdiContainer = true;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsBtnCopy;
        private System.Windows.Forms.ToolStripButton tsBtnCollapseSelectedBlock;
        private System.Windows.Forms.ImageList TreeViewImageList;
        private System.Windows.Forms.ToolStripProgressBar MainProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel TSStatusLabelProcessing;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButtonZoom;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxZoom;
        private System.Windows.Forms.ToolStripButton tsBtnCollapAllObjectBlocks;
        private System.Windows.Forms.ToolStripButton tsBtnExpandAllObjectBlocks;
        private System.Windows.Forms.ToolStripButton tsBtnCollapseAllFunctions;
        private System.Windows.Forms.ToolStripButton bookmarkPlusButton;
        private System.Windows.Forms.ToolStripButton bookmarkMinusButton;
        private System.Windows.Forms.ToolStripDropDownButton gotoBookmark;
        private System.Windows.Forms.ToolStripButton tsBtnRemoveAllBookmarks;
        private System.Windows.Forms.ToolStripDropDownButton tsBtnNavigationPrec;
        private System.Windows.Forms.ToolStripButton tsBtnNavigationSuiv;
        private System.Windows.Forms.ToolStripButton tsBtnNaviguerPrec;
        private System.Windows.Forms.ToolStripButton tsBtnSearch;
        private System.Windows.Forms.ToolStripButton tsBtnFindMessagesText;
        private System.Windows.Forms.ToolStripButton tsBtnSelectAll;
    }
}