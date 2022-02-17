namespace NavCodeViewer.Forms
{
    partial class NavFCTB
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemApercuDef = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemGoToDef = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemFindAllRefs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemModePlan = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemReduireAuxDefs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemToutDevelopper = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSearchMessages = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSelectBlock = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSignets = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemActivate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDeleteAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSignetPrec = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSignetSuivant = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCopy,
            this.toolStripSeparator1,
            this.toolStripMenuItemApercuDef,
            this.toolStripMenuItemGoToDef,
            this.toolStripMenuItemFindAllRefs,
            this.toolStripSeparator2,
            this.toolStripMenuItemModePlan,
            this.toolStripSeparator3,
            this.toolStripMenuItemSearch,
            this.toolStripMenuItemSearchMessages,
            this.toolStripSeparator4,
            this.toolStripMenuItemSelectAll,
            this.toolStripMenuItemSelectBlock,
            this.toolStripSeparator5,
            this.toolStripMenuItemSignets});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(240, 254);
            // 
            // toolStripMenuItemCopy
            // 
            this.toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
            this.toolStripMenuItemCopy.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemCopy.Text = "&Copy";
            this.toolStripMenuItemCopy.Click += new System.EventHandler(this.toolStripMenuItemCopy_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(236, 6);
            // 
            // toolStripMenuItemApercuDef
            // 
            this.toolStripMenuItemApercuDef.Name = "toolStripMenuItemApercuDef";
            this.toolStripMenuItemApercuDef.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemApercuDef.Text = "ApercuDef";
            this.toolStripMenuItemApercuDef.Click += new System.EventHandler(this.toolStripMenuItemApercuDef_Click);
            // 
            // toolStripMenuItemGoToDef
            // 
            this.toolStripMenuItemGoToDef.Name = "toolStripMenuItemGoToDef";
            this.toolStripMenuItemGoToDef.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemGoToDef.Text = "Atteindre la définition";
            // 
            // toolStripMenuItemFindAllRefs
            // 
            this.toolStripMenuItemFindAllRefs.Name = "toolStripMenuItemFindAllRefs";
            this.toolStripMenuItemFindAllRefs.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemFindAllRefs.Text = "Recherche toutes les références";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(236, 6);
            // 
            // toolStripMenuItemModePlan
            // 
            this.toolStripMenuItemModePlan.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemReduireAuxDefs,
            this.toolStripMenuItemToutDevelopper});
            this.toolStripMenuItemModePlan.Name = "toolStripMenuItemModePlan";
            this.toolStripMenuItemModePlan.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemModePlan.Text = "ModePlan";
            // 
            // toolStripMenuItemReduireAuxDefs
            // 
            this.toolStripMenuItemReduireAuxDefs.Name = "toolStripMenuItemReduireAuxDefs";
            this.toolStripMenuItemReduireAuxDefs.Size = new System.Drawing.Size(158, 22);
            this.toolStripMenuItemReduireAuxDefs.Text = "ReduireAuxDefs";
            // 
            // toolStripMenuItemToutDevelopper
            // 
            this.toolStripMenuItemToutDevelopper.Name = "toolStripMenuItemToutDevelopper";
            this.toolStripMenuItemToutDevelopper.Size = new System.Drawing.Size(158, 22);
            this.toolStripMenuItemToutDevelopper.Text = "ToutDevelopper";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(236, 6);
            // 
            // toolStripMenuItemSearch
            // 
            this.toolStripMenuItemSearch.Name = "toolStripMenuItemSearch";
            this.toolStripMenuItemSearch.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemSearch.Text = "Search";
            // 
            // toolStripMenuItemSearchMessages
            // 
            this.toolStripMenuItemSearchMessages.Name = "toolStripMenuItemSearchMessages";
            this.toolStripMenuItemSearchMessages.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemSearchMessages.Text = "Search text messages";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(236, 6);
            // 
            // toolStripMenuItemSelectAll
            // 
            this.toolStripMenuItemSelectAll.Name = "toolStripMenuItemSelectAll";
            this.toolStripMenuItemSelectAll.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemSelectAll.Text = "SelectAll";
            // 
            // toolStripMenuItemSelectBlock
            // 
            this.toolStripMenuItemSelectBlock.Name = "toolStripMenuItemSelectBlock";
            this.toolStripMenuItemSelectBlock.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemSelectBlock.Text = "Select Block";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(236, 6);
            // 
            // toolStripMenuItemSignets
            // 
            this.toolStripMenuItemSignets.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemActivate,
            this.toolStripMenuItemDeleteAll,
            this.toolStripMenuItemSignetPrec,
            this.toolStripMenuItemSignetSuivant});
            this.toolStripMenuItemSignets.Name = "toolStripMenuItemSignets";
            this.toolStripMenuItemSignets.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItemSignets.Text = "Signets";
            // 
            // toolStripMenuItemActivate
            // 
            this.toolStripMenuItemActivate.Name = "toolStripMenuItemActivate";
            this.toolStripMenuItemActivate.Size = new System.Drawing.Size(225, 22);
            this.toolStripMenuItemActivate.Text = "Activer /Désactiver un signet";
            // 
            // toolStripMenuItemDeleteAll
            // 
            this.toolStripMenuItemDeleteAll.Name = "toolStripMenuItemDeleteAll";
            this.toolStripMenuItemDeleteAll.Size = new System.Drawing.Size(225, 22);
            this.toolStripMenuItemDeleteAll.Text = "Effacer tous les signets";
            // 
            // toolStripMenuItemSignetPrec
            // 
            this.toolStripMenuItemSignetPrec.Name = "toolStripMenuItemSignetPrec";
            this.toolStripMenuItemSignetPrec.Size = new System.Drawing.Size(225, 22);
            this.toolStripMenuItemSignetPrec.Text = "Signet précédent";
            // 
            // toolStripMenuItemSignetSuivant
            // 
            this.toolStripMenuItemSignetSuivant.Name = "toolStripMenuItemSignetSuivant";
            this.toolStripMenuItemSignetSuivant.Size = new System.Drawing.Size(225, 22);
            this.toolStripMenuItemSignetSuivant.Text = "Signet suivant";
            // 
            // NavFCTB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Name = "NavFCTB";
            this.ToolTipNeeded += new System.EventHandler<FastColoredTextBoxNS.ToolTipNeededEventArgs>(this.NavFCTB_ToolTipNeeded);
            this.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.NavFCTB_TextChangedDelayed);
            this.SelectionChangedDelayed += new System.EventHandler(this.NavFCTB_SelectionChangedDelayed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NavFCTB_MouseDown);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemApercuDef;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemGoToDef;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFindAllRefs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemModePlan;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemReduireAuxDefs;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemToutDevelopper;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSearch;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSearchMessages;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSelectBlock;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSignets;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemActivate;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDeleteAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSignetPrec;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSignetSuivant;
    }
}
