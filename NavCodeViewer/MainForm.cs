//using NavCodeViewer.Forms;
using FastColoredTextBoxNS;
using NavCodeViewer.Business;
using NavCodeViewer.Forms;
using NavCodeViewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NavCodeViewer
{
    public partial class MainForm : Form
    {
        private const int MaxLinesForNewNaviLine = 40;
        WeifenLuo.WinFormsUI.Docking.DockPanel CodeSourceDockPanel;
        //WeifenLuo.WinFormsUI.Docking.DockPanel dockPanelOthers;
        public ObjectViewer ObjectViewerDockContent;
        ObjectReferences ObjectRefsDockContent;
        ObjectReferences SearchDockContent;
        SearchOptions FindOptionsDockContent;
        ObjectStructure ObjectStructureDockContent;
        List<NavigationLineItem> NaviLines = new List<NavigationLineItem>();
        int _ActualNaviOrder = 0;
        bool DoNotRecordNavigation = false;
        public Project ActiveProject;
        //SourceViewer dockSource2;
        private int _zoomCode = 100;
        public List<string> FindWorlds = new List<string>();

        public int ZoomVal { get => _zoomCode; set
            {
                _zoomCode = value;
                RefreshZoomText();
            }
        }
        public MainForm()
        {
            try
            {
                //this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
                //this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Left;
                //this.Controls.Add(this.dockPanel1);

                this.CodeSourceDockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
                this.CodeSourceDockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                this.Controls.Add(this.CodeSourceDockPanel);

                //this.dockPanelOthers = new WeifenLuo.WinFormsUI.Docking.DockPanel();
                //this.dockPanelOthers.Dock = System.Windows.Forms.DockStyle.Left;
                //this.Controls.Add(this.dockPanelOthers);


                InitializeComponent();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void ShowObjectSource(NavObject obj, bool quickview, int idLine = -1)
        {
            //if (quickview)
            //{
            //    if (IsOpenOnNormalView(obj))
            //    {
            //        CloseQuickView();
            //        OpenObjectSource(obj, idLine);
            //    }
            //    else
            //    {
            //        OpenObjectSourceQuickView(obj, idLine);
            //    }
            //}
            //else
            {
                
                OpenObjectSource(obj, quickview, idLine);
            }
        }
        public bool IsOpenOnNormalView(NavObject rec)
        {
            foreach (var view in GetListViewers)
            {
                if (view.NavObject.ObjectName == rec.ObjectName && !view.IsQuickView)
                {
                    return true;
                }
            }
            return false;
        }
        public SourceViewer CurrentSourceViewer
        {
            get
            {
                if (CodeSourceDockPanel.ActiveDocument != null)
                {
                    if (CodeSourceDockPanel.ActiveDocument is SourceViewer)
                    {
                        return (SourceViewer)CodeSourceDockPanel.ActiveDocument;
                    }
                }
                return null;
            }

            set
            {
                //tsFiles.SelectedItem = (value.Parent as FATabStripItem);
                //value.Focus();
                if (value != null)
                {
                    foreach (var view in GetListViewers)
                    {
                        if (view.ViewID == value.ViewID)
                        {
                            view.Focus();
                        }
                    }
                }
            }
        }
        //public void OpenObjectSourceQuickView(NavObject rec, int GotoLineNum = -1)
        //{
        //    if (rec == null) return;
        //    DockPane lastPane = null;
        //    IDockContent lastDockContent = null;
        //    bool found = false;
        //    foreach (var view in GetListViewers)
        //    {
        //        lastPane = view.Pane;
        //        lastDockContent = view;
        //        if (view.IsQuickView)
        //        {
        //            view.Select();
        //            view.ShowSource(rec);
        //            if (GotoLineNum >= 0)
        //            {
        //                view.GoToLine(GotoLineNum);
        //            }
        //            found = true;
        //            break;
        //        }
        //    }
        //    if (!found)
        //    {
        //        var viewer = new SourceViewer();
        //        viewer.IsQuickView = true;
        //        viewer.Mainform = this;
        //        //viewer.NavObject=nav
        //        viewer.ShowSource(rec);
        //        if (lastPane != null && lastDockContent != null)
        //        {
        //            viewer.Show(lastPane, lastDockContent);
        //        }
        //        else
        //        {
        //            viewer.Show(CodeSourceDockPanel);
        //        }
        //        if (GotoLineNum >= 0)
        //        {
        //            viewer.GoToLine(GotoLineNum);
        //        }
        //    }

        //    ObjectStructureDockContent.SetNavObject(rec, ObjectRefsDockContent);
        //}
        public void OpenObjectSource(NavObject rec, bool quickview ,int GotoLineNum = -1)
        {

            if (rec == null) return;
            DockPane lastPane = null;
            IDockContent lastDockContent = null;
            bool found = false;
            foreach (var view in GetListViewers)
            {
                lastPane = view.Pane;
                lastDockContent = view;
                if (view.ViewID == rec.ObjectName)
                {
                    if (view.IsQuickView)
                    {
                        view.IsQuickView = quickview;
                    }
                    view.Select();
                    if (GotoLineNum >= 0)
                    {
                        view.GoToLine(GotoLineNum);
                    }
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                

                var viewer = new SourceViewer();
                viewer.Mainform = this;
                viewer.ShowSource(rec);
                if (lastPane != null && lastDockContent != null)
                {
                    viewer.Show(lastPane, lastDockContent);
                }
                else
                {
                    viewer.Show(CodeSourceDockPanel);
                }

                CloseQuickView();//Close Prec quickview

                viewer.IsQuickView = quickview;//AFTER CloseQuickView

                if (GotoLineNum >= 0)
                {
                    viewer.GoToLine(GotoLineNum);
                }
            }

            ObjectStructureDockContent.SetNavObject(rec, ObjectRefsDockContent);

        }
        public List<SourceViewer> GetListViewers
        {
            get
            {
                var rep = new List<SourceViewer>();
                foreach (var dc in CodeSourceDockPanel.Contents)
                {
                    if (dc is SourceViewer)
                    {
                        rep.Add((SourceViewer)dc);
                    }
                }
                return rep;
            }
        }

        public int ActualNaviOrder { get => _ActualNaviOrder; set
            {
                _ActualNaviOrder = value;
                tsBtnNavigationSuiv.Enabled = NaviLines.Exists(c => c.Order > value);
            }
        }

        public SourceViewer GetSourceViewer(string viewId)
        {
            foreach (var dc in CodeSourceDockPanel.Contents)
            {
                if (dc is SourceViewer)
                {
                    if (((SourceViewer)dc).ViewID == viewId)
                    {
                        return (SourceViewer)dc;
                    }
                }
            }
            return null;
        }
        public SourceViewer GetSourceViewerByName(string formName)
        {
            foreach (var dc in CodeSourceDockPanel.Contents)
            {
                if (dc is SourceViewer)
                {
                    if (((SourceViewer)dc).Name == formName)
                    {
                        return (SourceViewer)dc;
                    }
                }
            }
            return null;
        }
        public void CloseQuickView()
        {
            foreach (var view in GetListViewers)
            {
                if (view.IsQuickView)
                {
                    view.Close();
                    break;
                }
            }
        }
        /// <summary>
        /// Adds text into a System.Windows.Forms.ProgressBar
        /// </summary>
        /// <param name="Target">The target progress bar to add text into</param>
        /// <param name="Text">The text to add into the progress bar. 
        /// Leave null or empty to automatically add the percent.</param>
        /// <param name="Location">Where the text is to be placed</param>
        /// <param name="TextColor">The color the text should be drawn in</param>
        /// <param name="TextFont">The font the text should be drawn in</param>
        private void SetProgressBarText
            (
            System.Windows.Forms.ToolStripProgressBar Target, //The target progress bar
            string Text, //The text to show in the progress bar
            ProgressBarTextLocation Location, //Where the text is to be placed
            System.Drawing.Color TextColor, //The color the text is to be drawn in
            System.Drawing.Font TextFont //The font we use to draw the text
            )
        {

            //Make sure we didn't get a null progress bar
            if (Target == null) { throw new ArgumentException("Null Target"); }

            //Now we can get to the real code

            int percent = (int)(((double)(Target.Value - Target.Minimum) /
    (double)(Target.Maximum - Target.Minimum)) * 100);

            //Check to see if we are to add in the percent
            if (string.IsNullOrEmpty(Text))
            {
                //We are to add in the percent meaning we got a null or empty Text
                //We give text a string value representing the percent

                Text = percent.ToString() + "%";
            }
            else
            {
                Text = Text + " " + percent.ToString() + "%";
            }

            //Now we can add in the text

            //gr will be the graphics object we use to draw on Target
            using (Graphics gr = Target.ProgressBar.CreateGraphics())
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.DrawString(Text,
                    TextFont, //The font we will draw it it (TextFont)
                    new SolidBrush(TextColor), //The brush we will use to draw it

                    //Where we will draw it
                    new PointF(
                        // X location (Center or Left)
                        Location == ProgressBarTextLocation.Left ?
                        5 : //Left side
                        Target.Width / 2 - (gr.MeasureString(Text, //Centered
                        TextFont).Width / 2.0F),
                    // Y Location (This is the same regardless of Location)
                    Target.Height / 2 - (gr.MeasureString(Text,
                        TextFont).Height / 2.0F)));
            }
        }

        public enum ProgressBarTextLocation
        {
            Left,
            Centered
        }

        private void pbPrecentage(ToolStripProgressBar pb)
        {
            int percent = (int)(((double)(pb.Value - pb.Minimum) /
            (double)(pb.Maximum - pb.Minimum)) * 100);

            using (Graphics gr = pb.ProgressBar.CreateGraphics())
            {
                //Switch to Antialiased drawing for better (smoother) graphic results
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.DrawString(percent.ToString() + "%",
                    SystemFonts.DefaultFont,
                    Brushes.Black,
                    new PointF(pb.Width / 2 - (gr.MeasureString(percent.ToString() + "%",
                        SystemFonts.DefaultFont).Width / 2.0F),
                    pb.Height / 2 - (gr.MeasureString(percent.ToString() + "%",
                        SystemFonts.DefaultFont).Height / 2.0F)));
            }
        }
        public void ShowDockContent()
        {
            ObjectViewerDockContent = new ObjectViewer();
            ObjectViewerDockContent.Show(this.CodeSourceDockPanel, DockState.DockLeft);

            ObjectStructureDockContent = new ObjectStructure();
            ObjectStructureDockContent.Mainform = this;
            ObjectStructureDockContent.Show(this.CodeSourceDockPanel, DockState.DockLeftAutoHide);

            ObjectRefsDockContent = new ObjectReferences(this);
            ObjectRefsDockContent.Show(this.CodeSourceDockPanel, DockState.DockBottomAutoHide);

            SearchDockContent = new ObjectReferences(this);
            SearchDockContent.Show(this.CodeSourceDockPanel, DockState.DockBottomAutoHide);

            FindOptionsDockContent = new SearchOptions(this);
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            RefreshZoomText();

            ShowDockContent();

            MainProgressBar.Maximum = 100;
            MainProgressBar.Step = 1;
            //SetProgressBarText(MainProgressBar, "Processing", ProgressBarTextLocation.Centered, Color.Red,new Font( FontFamily.GenericSansSerif,8.5f));
            //MainProgressBar.ProgressBar.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            //MainProgressBar.ProgressBar.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            var progress = new Progress<ProgressBarInfo>(v =>
            {
                // This lambda is executed in context of UI thread,
                // so it can safely update form controls
                MainProgressBar.Value = v.Value;
                int percent = (int)(((double)(MainProgressBar.Value - MainProgressBar.Minimum) /
                     (double)(MainProgressBar.Maximum - MainProgressBar.Minimum)) * 100);

                TSStatusLabelProcessing.Text = v.Text + " " + percent.ToString() + "%";
                //SetProgressBarText(MainProgressBar, "", ProgressBarTextLocation.Centered, Color.Blue, new Font(FontFamily.GenericSansSerif, 9f));
            });

            ActiveProject = new Project();
            //await Task.Run(() => LoadFile(pro,progress)).ConfigureAwait(false);
            //LoadFile(pro);
            //ActiveProject = pro;
            await LoadFileAsync(ActiveProject, progress);

            //Task tp =  CollectAllReferencesPagesAsync(pro, progress);
            //Task tt = CollectAllReferencesTablesAsync(pro, progress);
            //Task tc = CollectAllReferencesCodeunitsAsync(pro, progress);
            //Task tx = CollectAllReferencesXmlportsAsync(pro, progress);
            //Task tm = CollectAllReferencesMenusuitesAsync(pro, progress);
            //Task tr = CollectAllReferencesReportsAsync(pro, progress);
            //Task tq = CollectAllReferencesQuerysAsync(pro, progress);
            //await tp;
            //await tt;
            //await tc;
            //await tx;
            //await tm;
            //await tr;
            //await tq;

            await CollectAllReferencesAsync(ActiveProject, progress);

            ClearProgressInfos();
            //MessageBox.Show("End of process");


        }

        private void RefreshZoomText()
        {
            toolStripSplitButtonZoom.Text = _zoomCode.ToString() + "%";
            //if (dockSource2 != null)
            //{
            //    dockSource2.GetFastTB().Zoom = ZoomVal;
            //}
            foreach (var dc in GetListViewers)
            {
                dc.SetZoom(ZoomVal);
            }
        }

        private void LoadFile(Project pro, IProgress<ProgressBarInfo> progress)
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            pro.LoadFileAndCollectReferences(@"D:\TestsFiles\TestNavEditor06.TXT",
                progress);

#if DEBUG
            Console.WriteLine("LoadFile: " + sw.ElapsedMilliseconds);
#endif
            ObjectViewerDockContent.Mainform = this;
            ObjectViewerDockContent.SetSourceDockPanel(CodeSourceDockPanel, ObjectStructureDockContent, ObjectRefsDockContent);
            ObjectViewerDockContent.SetProject(pro);
            ObjectViewerDockContent.LoadMenu();
            //ClearProgressInfos();
        }
        private void CollectAllReferencesPages(Project pro, IProgress<ProgressBarInfo> progress)
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            pro.CollectAllReferencesPages(progress);

#if DEBUG
            Console.WriteLine("CollectAllReferences Pages: " + sw.ElapsedMilliseconds);
#endif
        }
        private void CollectAllReferencesTables(Project pro, IProgress<ProgressBarInfo> progress)
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            pro.CollectReferencesTables(progress);

#if DEBUG
            Console.WriteLine("CollectAllReferences Tables: " + sw.ElapsedMilliseconds);
#endif
        }
        private void CollectAllReferencesCodeunits(Project pro, IProgress<ProgressBarInfo> progress)
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            pro.CollectReferencesCodeunits(progress);

#if DEBUG
            Console.WriteLine("CollectAllReferences Codeunits: " + sw.ElapsedMilliseconds);
#endif
        }
        private void CollectAllReferencesReports(Project pro, IProgress<ProgressBarInfo> progress)
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            pro.CollectReferencesReports(progress);

#if DEBUG
            Console.WriteLine("CollectAllReferences Reports: " + sw.ElapsedMilliseconds);
#endif
        }
        private void CollectAllReferencesXmlports(Project pro, IProgress<ProgressBarInfo> progress)
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            pro.CollectReferencesXmlports(progress);

#if DEBUG
            Console.WriteLine("CollectAllReferences Xmlports: " + sw.ElapsedMilliseconds);
#endif
        }
        private void CollectAllReferencesQuerys(Project pro, IProgress<ProgressBarInfo> progress)
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            pro.CollectReferencesQuerys(progress);

#if DEBUG
            Console.WriteLine("CollectAllReferences Querys: " + sw.ElapsedMilliseconds);
#endif
        }
        private void CollectAllReferencesMenusuites(Project pro, IProgress<ProgressBarInfo> progress)
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            pro.CollectReferencesMenusuites(progress);

#if DEBUG
            Console.WriteLine("CollectAllReferences Menusuites: " + sw.ElapsedMilliseconds);
#endif
        }
        private void CollectAllReferences(Project pro, IProgress<ProgressBarInfo> progress)
        {
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif
            pro.CollectReferencesTables(progress);
            pro.CollectAllReferencesPages(progress);
            pro.CollectReferencesCodeunits(progress);
            pro.CollectReferencesXmlports(progress);
            pro.CollectReferencesQuerys(progress);
            pro.CollectReferencesReports(progress);
            pro.CollectReferencesMenusuites(progress);

#if DEBUG
            Console.WriteLine("CollectAllReferences Pages: " + sw.ElapsedMilliseconds);
#endif
        }
        private async Task LoadFileAsync(Project pro, IProgress<ProgressBarInfo> progress)
        {
            await Task.Run(() => LoadFile(pro, progress))
                .ConfigureAwait(false);
        }
        private async Task CollectAllReferencesAsync(Project pro, IProgress<ProgressBarInfo> progress)
        {
            await Task.Run(() => CollectAllReferences(pro, progress)).ConfigureAwait(false);
        }
        private async Task CollectAllReferencesPagesAsync(Project pro, IProgress<ProgressBarInfo> progress)
        {
            await Task.Run(() => CollectAllReferencesPages(pro, progress)).ConfigureAwait(false);
        }
        private async Task CollectAllReferencesTablesAsync(Project pro, IProgress<ProgressBarInfo> progress)
        {
            await Task.Run(() => CollectAllReferencesTables(pro, progress)).ConfigureAwait(false);
        }
        private async Task CollectAllReferencesReportsAsync(Project pro, IProgress<ProgressBarInfo> progress)
        {
            await Task.Run(() => CollectAllReferencesReports(pro, progress)).ConfigureAwait(false);
        }
        private async Task CollectAllReferencesCodeunitsAsync(Project pro, IProgress<ProgressBarInfo> progress)
        {
            await Task.Run(() => CollectAllReferencesCodeunits(pro, progress)).ConfigureAwait(false);
        }
        private async Task CollectAllReferencesXmlportsAsync(Project pro, IProgress<ProgressBarInfo> progress)
        {
            await Task.Run(() => CollectAllReferencesXmlports(pro, progress)).ConfigureAwait(false);
        }
        private async Task CollectAllReferencesMenusuitesAsync(Project pro, IProgress<ProgressBarInfo> progress)
        {
            await Task.Run(() => CollectAllReferencesMenusuites(pro, progress)).ConfigureAwait(false);
        }
        private async Task CollectAllReferencesQuerysAsync(Project pro, IProgress<ProgressBarInfo> progress)
        {
            await Task.Run(() => CollectAllReferencesQuerys(pro, progress)).ConfigureAwait(false);
        }
        private void tsBtnCopy_Click(object sender, EventArgs e)
        {
            if (CurrentSourceViewer != null)
            {
                CurrentSourceViewer.Copy();
            }
        }
        private void toolStripComboBoxZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateZoom();
        }
        private void updateZoom()
        {
            var text = toolStripComboBoxZoom.Text;
            text = text.Replace("%", "");
            int zoom = ZoomVal;
            if (int.TryParse(text, out zoom))
            {
                ZoomVal = zoom;
            }
        }
        private void toolStripComboBoxZoom_TextChanged(object sender, EventArgs e)
        {
            updateZoom();
        }
        public void ShowFieldReferences(NavObject navObject, string fieldNameMod)
        {
            var refs = navObject.GetFieldReferences(fieldNameMod);
            string longName = string.Format("{0} {1}", Resources.String5, fieldNameMod);
            ObjectRefsDockContent.SetNavObject(navObject.NavProject, refs, fieldNameMod,false);
        }
        public void ShowFunctionReferences(NavObject rec, string functionName)
        {
            var refs = rec.GetFunctionReferences(functionName);
            ObjectRefsDockContent.SetNavObject(rec.NavProject, refs, functionName,false);
        }
        public void ShowSearchReferences(List<Reference> refs, string searchText)
        {
            //var refs = rec.GetFunctionReferences(functionName);
            SearchDockContent.SetNavObject(ActiveProject, refs, searchText, true);
        }
        private void tsBtnCollapseSelectedBlock_Click(object sender, EventArgs e)
        {
            if (CurrentSourceViewer != null)
            {
                CurrentSourceViewer.CollapseSelectedBlock();
            }
        }

        private void tsBtnCollapseAllObjectBlocks_Click(object sender, EventArgs e)
        {
            if (CurrentSourceViewer != null)
            {
                CurrentSourceViewer.CollapseAllObjectBlocks();
            }
        }

        private void tsBtnExpandAllObjectBlocks_Click(object sender, EventArgs e)
        {
            if (CurrentSourceViewer != null)
            {
                CurrentSourceViewer.ExpandAllObjectBlocks();
            }
        }

        private void tsBtnCollapseAllFunctions_Click(object sender, EventArgs e)
        {
            if (CurrentSourceViewer != null)
            {
                CurrentSourceViewer.CollapseAllObjectFunctions(true);
            }
        }

        private void bookmarkPlusButton_Click(object sender, EventArgs e)
        {
            if (CurrentSourceViewer != null)
            {
                CurrentSourceViewer.AddBookMark();
            }
        }

        private void gotoBookmark_DropDownOpening(object sender, EventArgs e)
        {
            gotoBookmark.DropDownItems.Clear();
            foreach (var tab in GetListViewers)
            {
                var tb = tab.CurrentTB;
                foreach (var bookmark in tb.Bookmarks)
                {
                    string bookmarkText = CollectBookMarkText(tab, bookmark.LineIndex);
                    var item = gotoBookmark.DropDownItems.Add(bookmarkText);
                    item.Tag = new BookMarkItem
                    {
                        Bookmark = bookmark,
                        Name = tab.ViewID
                    };
                    item.Click += (o, a) =>
                    {
                        var b = (BookMarkItem)(o as ToolStripItem).Tag;
                        try
                        {
                            CurrentSourceViewer = GetSourceViewer(b.Name);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        b.Bookmark.DoVisible();
                    };
                }
            }
        }

        private static string CollectBookMarkText(SourceViewer tab, int iLine)
        {
            //var iLine = bookmark.LineIndex;
            string txtLine = tab.CurrentTB[iLine].Text.Trim();
            if (txtLine == "")
            {
                txtLine = string.Format(Resources.String19, iLine + 1);
            }
            return string.Format("{0} : {1}", tab.ViewID, txtLine);
        }

        private void bookmarkMinusButton_Click(object sender, EventArgs e)
        {
            if (CurrentSourceViewer != null)
            {
                CurrentSourceViewer.RemoveBookMark();
            }
        }

        private void tsBtnRemoveAllBookmarks_Click(object sender, EventArgs e)
        {
            if (!GlobalUI.Confirm(Resources.String20))
            {
                return;
            }
            List<int> iLines = new List<int>();
            foreach (var tab in GetListViewers)
            {
                var tb = tab.CurrentTB;
                iLines = new List<int>();
                foreach (var bookmark in tb.Bookmarks)
                {
                    iLines.Add(bookmark.LineIndex);
                }
                foreach (var iLine in iLines)
                {
                    tb.UnbookmarkLine(iLine);
                }
            }
        }
        public void InsertNavigationLine(int currLine,SourceViewer viewer)
        {
            if (DoNotRecordNavigation)
            {
                DoNotRecordNavigation = false;
                return;
            }
            var max = NaviLines.OrderByDescending(c => c.Order).FirstOrDefault();
            if (max != null)
            {
                if (max.ViewID == viewer.ViewID)
                {
                    if (Math.Abs(max.iLine - currLine) > MaxLinesForNewNaviLine)
                    {
                        AddNaviLine(currLine, max.Order + 1, viewer);
                    }
                    else
                    {
                        max.iLine = currLine;
                    }
                }
                else
                {
                    AddNaviLine(currLine, max.Order + 1, viewer);
                }
            }
            else
            {
                AddNaviLine(currLine, 1,viewer);
            }
        }

        private void AddNaviLine(int currLine,int Ord, SourceViewer viewer)
        {
            var newNavi = new NavigationLineItem
            {
                Order = Ord,
                iLine = currLine,
                ViewID = viewer.ViewID,
                Text = CollectBookMarkText(viewer, currLine),
                rec = viewer.NavObject
            };
            ActualNaviOrder = Ord;
            NaviLines.Add(newNavi);
        }

        public void AddFindWord(string word)
        {
            if (!FindWorlds.Contains(word))
            {
                FindWorlds.Add(word);
            }
        }
        private void tsBtnNavigationPrec_DropDownOpening(object sender, EventArgs e)
        {
            tsBtnNavigationPrec.DropDownItems.Clear();
            foreach (var navi in NaviLines.OrderByDescending(c=>c.Order).ToList())
            {
                string bookmarkText = navi.Text;// CollectBookMarkText(navi.view, navi.iLine);
                var item = tsBtnNavigationPrec.DropDownItems.Add(bookmarkText);
                item.Tag = navi;
                if (navi.Order == ActualNaviOrder)
                {
                    item.ForeColor = Color.Blue;
                    item.Font = new System.Drawing.Font(item.Font, FontStyle.Bold | FontStyle.Italic);
                }

                item.Click += (o, a) =>
                {
                    try
                    {
                        var b = (NavigationLineItem)(o as ToolStripItem).Tag;
                    GotoNaviLine(b);
                    }
                    catch (Exception ex)
                    {
                        GlobalUI.ErrorMsg(ex.Message);
                        return;
                    }
                };
            }
        }

        private void GotoNaviLine(NavigationLineItem b)
        {
            ActualNaviOrder = b.Order;
            CurrentSourceViewer = GetSourceViewer(b.ViewID);
            DoNotRecordNavigation = true;
            OpenObjectSource(b.rec,false, b.iLine + 1);
        }

        private void tsBtnNavigationSuiv_Click(object sender, EventArgs e)
        {
            var nextNavi = NaviLines.OrderBy(c=>c.Order).Where(c => c.Order > ActualNaviOrder).FirstOrDefault();
            GotoNaviLine(nextNavi);
        }

        private void tsBtnNaviguerPrec_Click(object sender, EventArgs e)
        {
            var prevNavi = NaviLines.OrderBy(c => c.Order).Where(c => c.Order < ActualNaviOrder).LastOrDefault();
            GotoNaviLine(prevNavi);
        }

        private void tsBtnSearch_Click(object sender, EventArgs e)
        {
            if (FindOptionsDockContent.DockState == DockState.Unknown)
            {
                FindOptionsDockContent = new SearchOptions(this);
            }
            if (FindOptionsDockContent != null)
            {
                FindOptionsDockContent.Show(this.CodeSourceDockPanel, DockState.Float);
                //FindOptionsDockContent.Height = 200;
                //FindOptionsDockContent.Width = 550;
            }
        }
        public async Task SearchInObjectsAsync(string SearchText, string pattern, List<NavObject> objs, RegexOptions opt)
        {
            var progress = new Progress<ProgressBarInfo>(v =>
            {
                MainProgressBar.Value = v.Value;
                int percent = (int)(((double)(MainProgressBar.Value - MainProgressBar.Minimum) /
                     (double)(MainProgressBar.Maximum - MainProgressBar.Minimum)) * 100);

                TSStatusLabelProcessing.Text = v.Text + " " + percent.ToString() + "%";
            });

            var refs = await SearchTextAsync(pattern, objs, opt, progress);

            ShowSearchReferences(refs, SearchText);
        }
        public async Task SearchErrorMessagesInObjectsAsync(string SearchText, string pattern, List<NavObject> objs, RegexOptions opt)
        {
            var progress = new Progress<ProgressBarInfo>(v =>
            {
                MainProgressBar.Value = v.Value;
                int percent = (int)(((double)(MainProgressBar.Value - MainProgressBar.Minimum) /
                     (double)(MainProgressBar.Maximum - MainProgressBar.Minimum)) * 100);

                TSStatusLabelProcessing.Text = v.Text + " " + percent.ToString() + "%";
            });

            var refs = await SearchTextAsync(pattern, objs, opt, progress);

            List<Reference> transformedRefs = null;

            foreach(var r in refs)
            {
                List<Reference> refsExpr;
                var txt = r.LineText;
                var tab = txt.Split('@');
                if (tab != null)
                {
                    if (tab.Length > 0)
                    {
                        var idVar = tab[0].Trim();
                        var navObject = ActiveProject.GetObject(r.RefBy_ObjetID, r.RefBy_ObjetType);
                        refsExpr = await SearchTextAsync(idVar, new List<NavObject>
                        {
                            navObject
                        },  RegexOptions.IgnoreCase, null) ;
                        if (transformedRefs == null)
                        {
                            transformedRefs = new List<Reference>();
                        }
                        transformedRefs.AddRange(refsExpr);
                    }
                    else
                    {
                        refsExpr = null;
                    }
                }
                else
                {
                    refsExpr = null;
                }
            }

            if (transformedRefs == null)
            {
                transformedRefs = refs;
            }

            ShowSearchReferences(transformedRefs, SearchText);

            if (transformedRefs.IsNullOrEmpty())
            {
                GlobalUI.Info(string.Format(Resources.String41, transformedRefs.Count));
            }
            else
            {
                GlobalUI.Info(string.Format(Resources.String40, transformedRefs.Count));
            }
        }

        private List<Reference> FindAllText(string pattern, List<NavObject> objs, RegexOptions opt, IProgress<ProgressBarInfo> progress)
        {
            var refs = ActiveProject.SearchText(pattern, objs, progress, opt);
            return refs;
        }

        public void ClearProgressInfos()
        {
            MainProgressBar.Value = 0;
            TSStatusLabelProcessing.Text = "";
        }

        private async Task<List<Reference>> SearchTextAsync(string pattern, List<NavObject> objs, RegexOptions opt,IProgress<ProgressBarInfo> progress)
        {
            return await Task.Run(() => FindAllText(pattern, objs,opt, progress))
                .ConfigureAwait(false);
        }

        private void tsBtnFindMessagesText_Click(object sender, EventArgs e)
        {
            FindErrorMessage frm = new FindErrorMessage(this);
            frm.Show();
        }

        private void tsBtnSelectAll_Click(object sender, EventArgs e)
        {
            //DoNotRecordNavigation = true;
            CurrentSourceViewer.SelectAll();
        }

        private void tsBtnCommitRefs_Click(object sender, EventArgs e)
        {

        }
    }
    public class BookMarkItem
    {
        public Bookmark Bookmark { get; set; }
        public string Name { get; set; }
    }
    public class NavigationLineItem
    {
        public int Order { get; set; }
        public int iLine { get; set; }
        public string ViewID { get; set; }
        public string Text { get; set; }
        public NavObject rec { get; set; }
    }
}
