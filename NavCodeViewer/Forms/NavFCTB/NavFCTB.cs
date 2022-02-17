using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using NavCodeViewer.Forms.Controls;
using NavCodeViewer.Business;
using System.Text.RegularExpressions;
using NavCodeViewer.Properties;

namespace NavCodeViewer.Forms
{
    public partial class NavFCTB : FastColoredTextBox
    {
        private NAVSyntaxHighlighter nAVSyntaxHighlighter = new NAVSyntaxHighlighter();
        Point relativeContextMenuClickedPosition = new Point();

        private NavObject navobject;
        private SourceViewer sourceView;
        public NavFCTB() : base()
        {
            try
            {

                InitializeComponent();
                Init();
            }
            catch (Exception ex)
            {
                GlobalUI.ErrorMsg(ex.Message);
            }
        }

        public NavObject Navobject { get => navobject; set => navobject = value; }
        public SourceViewer SourceView { get => sourceView; set => sourceView = value; }

        public void Init()
        {
            if (nAVSyntaxHighlighter != null)
                nAVSyntaxHighlighter.InitStyleSchema();
            MaxLinesForFolding = 10000;
            this.LeftBracket = '(';
            this.ReadOnly = true;
            this.RightBracket = ')';
            this.SelectedLineBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            //this.fctb.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fctb.ServiceColors")));
            //this.fctb.ShowCaretWhenInactive = true;
            this.ShowFoldingLines = true;
            this.Font = new System.Drawing.Font("Consolas", 13F);
            this.BackColor = Color.White;
            this.CurrentLineColor = Color.Gray;
            this.LeftPadding = 15;
        }


        public override void OnSyntaxHighlight(TextChangedEventArgs args)
        {
            try
            {
#if debug
            Stopwatch sw = Stopwatch.StartNew();
#endif

                foreach (var range in Poc)
                {
                    nAVSyntaxHighlighter.CALSyntaxHighlight(range);
                }

                nAVSyntaxHighlighter.FoldingSyntax(Range);

#if debug
            Console.WriteLine("OnSyntaxHighlight: "+ sw.ElapsedMilliseconds);
#endif
            }
            catch (Exception ex)
            {
                GlobalUI.ErrorMsg(ex.Message);
            }
        }

        private void NavFCTB_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            //if (navobject.Type == Domain.ObjectType.Report)
            //{
            //    CollapseRdlRange();
            //}
            //Hints.Clear();
            ////foreach(var f in reco)
            //foreach (var r in GetRanges("PROCEDURE "))
            //{
            //    var r1 = new Range(this);
            //    r1.Start = new Place(6, r.Start.iLine - 1);
            //    r1.End = new Place(13, r.End.iLine);
            //    string functionName = GetFunctionName(this[r.Start.iLine].Text);
            //    ReferenceHint innerControl = new ReferenceHint(sourceView, functionName);
            //    innerControl.BackColor = this.BackColor;
            //    int NbreRefs = navobject.GetFunctionReferences(functionName).ToList().Count;
            //    innerControl.SetLinkText(string.Format("{0} {1}", NbreRefs, Resources.String17));
            //    innerControl.SetFont(this.Font);
            //    Hint hint = new Hint(r1, innerControl, true, false);
            //    hint.BackColor = this.BackColor;
            //    Hints.Add(hint);
            //}
        }

        private void CollapseRdlRange()
        {
            var rdlZone = navobject.OthersPlaces.Where(c => c.RangeType == TypeOfCodeRange.RDLDataDef)
                                .FirstOrDefault();
            if (rdlZone != null)
            {
                CollapseCodeRange(rdlZone);
            }
        }

        protected string GetFunctionName(string line)
        {
            var str = Regex.Matches(line, @"(?<=\b(PROCEDURE)\s+)(?<range>\w+?)\b");
            if (str.Count > 0)
            {
                return str[0].Value;
            }
            return "";
        }
        //public void ShowApercuFunction(string funcName)
        //{
        //    ToolTip.ToolTipTitle = funcName;
        //    //ToolTip.ToolTipIcon = ea.ToolTipIcon;
        //    string text = navobject.GetFunctionText(funcName);

        //    //ToolTip.Show(text, this, new Point(lastMouseCoord.X, lastMouseCoord.Y + CharHeight));
        //    ToolTip.Show(text, this, new Point(0, 0));
        //}

        private void NavFCTB_ToolTipNeeded(object sender, ToolTipNeededEventArgs e)
        {
            //try
            //{
            //    if (!string.IsNullOrEmpty(e.HoveredWord))
            //    {
            //        string title = e.HoveredWord;
            //        string text = "";
            //        if (navobject.IsField(title))
            //        {
            //            text = navobject.GetFieldDefinition(e.HoveredWord);
            //        }
            //        if (navobject.IsFunction(title))
            //        {
            //            text = navobject.GetFunctionText(e.HoveredWord, ref title);
            //        }
            //        //string text = navobject.GetFieldDefinition(e.HoveredWord);
            //        e.ToolTipTitle = title;
            //        e.ToolTipText = text;
            //        //e.ToolTipText = "This is tooltip for '" + e.HoveredWord + "'";
            //    }
            //    //var range = new Range(sender as FastColoredTextBox, e.Place, e.Place);
            //    //string hoveredWord = range.GetFragment("[^\n]").Text;
            //    //e.ToolTipTitle = hoveredWord;
            //    ////e.ToolTipText = "This is tooltip for '" + hoveredWord + "'";
            //    //e.ToolTipText = this.Text;
            //}
            //catch (Exception ex)
            //{
            //    GlobalUI.ErrorMsg(ex.Message);
            //}
        }
        public void CollapseCodeRange(CodeRange pl)
        {
            var iLine = pl.Start.iLine + 1;
            if (this[iLine].FoldingStartMarker.IsNotNullOrEmpty())
            {
                CollapseFoldingBlock(iLine);
            }
        }

        private void NavFCTB_SelectionChangedDelayed(object sender, EventArgs e)
        {
            try
            {
                var tb = sender as FastColoredTextBox;
                if (tb.Selection.IsEmpty && tb.Selection.Start.iLine < tb.LinesCount)
                {
                    sourceView.Mainform.InsertNavigationLine(tb.Selection.Start.iLine, sourceView);
                }
            }
            catch (Exception ex)
            {
                GlobalUI.ErrorMsg(ex.Message);
            }
        }

        private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            sourceView.Copy();
        }

        private void ShowApercuDef()
        {

            //try
            //{
            //    if (!string.IsNullOrEmpty(e.HoveredWord))
            //    {
            //        string title = e.HoveredWord;
            //        string text = navobject.GetFunctionText(e.HoveredWord, ref title);
            //        //string text = navobject.GetFieldDefinition(e.HoveredWord);
            //        e.ToolTipTitle = title;
            //        e.ToolTipText = text;
            //        //e.ToolTipText = "This is tooltip for '" + e.HoveredWord + "'";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    GlobalUI.ErrorMsg(ex.Message);
            //}
        }
        private void toolStripMenuItemApercuDef_Click(object sender, EventArgs e)
        {
            var p = PointToPlace(relativeContextMenuClickedPosition);
            int iCol = 0;
            var expr = GetExpressionInCursor(p,ref iCol);

            //var fonctions = navobject.GetFunctionReferences();

            string title = expr;
            var text = navobject.GetDefinitionOfExpr(expr, iCol, ref title,p.iLine);
            ShowToolTip(title, text);

            //if (navobject.IsLocalFunction(expr))
            //{
            //    string title = expr;
            //    string text = navobject.GetFunctionText(expr, ref title);

            //    ShowToolTip(title, text);
            //}
            //if (navobject.IsLocalField(expr))
            //{
            //    string title = expr;
            //    string text = navobject.GetFieldDefinition(expr);

            //    ShowToolTip(title, text);
            //}
        }

        private void ShowToolTip(string title, string text)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.IsBalloon = false;
            toolTip1.UseAnimation = true;
            toolTip1.UseFading = true;
            toolTip1.ToolTipIcon = ToolTipIcon.Info;
            toolTip1.ToolTipTitle = title; // Title to display.
            toolTip1.Show(text, this, 25000); // Message of the toolTip and to what control to appear.
        }

        private string GetExpressionInCursor(Place p,ref int iStartingCol)
        {
            var wordRange = GetRangeInCursor(p);
            iStartingCol = wordRange.Start.iChar;
            return wordRange.Text.RemoveQuotes();
        }
        private Range GetRangeInQuotes(Range range,Place p,ref bool allLineIsComment,ref bool isComment)
        {
            var stack = new Stack<XmlFoldingTag>();
            var id = 0;
            var fctb = range.tb;

            //contient un terme 'begin' mais non précédé par '//' 
            var regStartingFoldingPattern = @"""|{|'|\/\/|}";

            Place start = range.Start;
            int ActiveOneLineComment = -1;
            var ListOfMarkers = range.GetRanges(regStartingFoldingPattern).ToList();
            for (int j = 0; j < ListOfMarkers.Count; j++)
            {
                var r = ListOfMarkers[j];

                if (r.Start.iLine == ActiveOneLineComment)
                {
                    continue;
                }

                Range nexR = null;

                nexR = new Range(fctb);
                nexR.Start = new Place(r.End.iChar, r.End.iLine);
                nexR.End = new Place(r.End.iChar + 1, r.End.iLine);

                if (r.Text == @"""")
                {
                    if (stack.Count == 0)
                    {
                        var tag = new XmlFoldingTag { Name = r.Text, id = id++, startLine = r.FromLine };
                        tag.startPlace = r.Start;
                        stack.Push(tag);
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek.Name == r.Text)
                            {
                                if (nexR.Text != r.Text)
                                {
                                    //Double quote
                                    if (p.iChar >= stackPeek.startPlace.iChar && p.iChar <= r.End.iChar)
                                    {
                                        return new Range(fctb, stackPeek.startPlace, r.End);
                                    }
                                    stack.Pop();
                                }
                            }
                        }
                    }
                }
                if (r.Text == @"'")
                {
                    if (stack.Count == 0)
                    {
                        var tag = new XmlFoldingTag { Name = r.Text, id = id++, startLine = r.FromLine };
                        tag.startPlace = r.Start;
                        stack.Push(tag);
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek.Name == r.Text)
                            {
                                if (nexR.Text != r.Text)
                                {
                                    //one quote
                                    if (p.iChar >= stackPeek.startPlace.iChar && p.iChar <= r.End.iChar)
                                    {
                                        return new Range(fctb, stackPeek.startPlace, r.End);
                                    }
                                    stack.Pop();
                                }
                            }
                        }
                    }
                }
                if (r.Text == @"//")
                {
                    if (stack.Count == 0)
                    {
                        allLineIsComment = true;
                        isComment = true;
                        //var oneLineComment = new Range(fctb, r.Start, new Range(fctb, r.ToLine).End);
                        //oneLineComment.ClearStyle(StyleIndex.All);
                        //oneLineComment.SetStyle(CommentStyle);

                        ActiveOneLineComment = r.Start.iLine;
                    }
                }
                if (r.Text == @"{")
                {
                    if (stack.Count == 0)
                    {
                        var tag = new XmlFoldingTag { Name = r.Text, id = id++, startLine = r.FromLine };
                        tag.startPlace = r.Start;
                        stack.Push(tag);
                    }
                    else
                    {
                        var stackPeek = stack.Peek();
                        if (stackPeek.Name == "{")
                        {
                            var tag = new XmlFoldingTag { Name = r.Text, id = id++, startLine = r.FromLine };
                            tag.startPlace = r.Start;
                            stack.Push(tag);
                        }
                    }
                }
                if (r.Text == @"}")
                {
                    if (stack.Count == 0)
                    {
                        //var tag = new XmlFoldingTag { Name = r.Text, id = id++, startLine = iLine };
                        //stack.Push(tag);
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek.Name == "{")
                            {
                                if (p.iChar >= stackPeek.startPlace.iChar && p.iChar <= r.End.iChar)
                                {
                                    isComment = true;
                                }
                                //var commentBlock = new Range(fctb, stackPeek.startPlace, r.End);
                                //commentBlock.ClearStyle(StyleIndex.All);
                                //commentBlock.SetStyle(CommentStyle);
                                stack.Pop();
                            }
                        }
                        else
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek.Name == "{")
                            {
                                stack.Pop();
                            }
                        }
                    }
                }
            }

            return null;
        }
        protected override void SelectWord(Place p)
        {
            Selection = GetRangeInCursor(p);
        }

        private Range GetRangeInCursor(Place p)
        {
            int fromX = p.iChar;
            int toX = p.iChar;

            string line = this.Lines[p.iLine];
            var lineRange = new Range(this, p.iLine);
            bool AllIneComment = false, IsComment = false;
            var searchInQuotes = GetRangeInQuotes(lineRange, p, ref AllIneComment, ref IsComment);

            if (searchInQuotes != null)
            {
                var txt = searchInQuotes.Text;
                return searchInQuotes;
            }

            for (int i = p.iChar; i < this.Lines[p.iLine].Count(); i++)
            {
                char c = this.Lines[p.iLine][i];
                if (char.IsLetterOrDigit(c) || c == '_')
                    toX = i + 1;
                else
                    break;
            }

            for (int i = p.iChar - 1; i >= 0; i--)
            {
                char c = this.Lines[p.iLine][i];
                if (char.IsLetterOrDigit(c) || c == '_')
                    fromX = i;
                else
                    break;
            }

            return new Range(this, fromX, p.iLine,toX, p.iLine);
        }

        private void NavFCTB_MouseDown(object sender, MouseEventArgs e)
        {
            var p = PointToPlace(e.Location);
            //var isLineSelect = (e.Location.X < this.LeftIndentLine);

            if (e.Button == MouseButtons.Right)
            {
                relativeContextMenuClickedPosition = e.Location;
                var screenClickedPosition = (sender as Control).PointToScreen(relativeContextMenuClickedPosition);
                contextMenuStrip1.Show(screenClickedPosition);
            }

            //if (!isLineSelect)
            {
                if (e.Clicks == 2)
                {
                    //mouseIsDrag = false;
                    //mouseIsDragDrop = false;
                    //draggedRange = null;

                    SelectWord(p);
                    return;
                }
            }
        }
    }
}
