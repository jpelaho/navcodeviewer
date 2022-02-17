using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml;

namespace NavCodeViewer.Forms
{
    public class NAVSyntaxHighlighter
    {
        //styles
        protected static readonly Platform platformType = PlatformType.GetOperationSystemPlatform();
        public readonly Style BlueBoldStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        public readonly Style BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        public readonly Style ProcStyle = new TextStyle(GetBrush("#800000"), null, FontStyle.Bold);
        public readonly Style BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        public readonly Style GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        public readonly Style GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        public readonly Style MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        public readonly Style MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        public readonly Style RedStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        public readonly Style BlackStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        public readonly Style AquaStyle = new TextStyle(GetBrush("#e68e35"), null, FontStyle.Regular);

        protected Regex CALKeywordRegex, CALSystemFunctionsRegex, CALProcedureNameRegex;
        protected Regex CSharpNumberRegex;

        static Brush GetBrush(string textColor)
        {
            return new SolidBrush(System.Drawing.ColorTranslator.FromHtml(textColor)) as Brush;
        }
        public void FoldingSyntax(Range range)
        {
            range.ClearFoldingMarkers();
            CALFolding(range);
        }
        protected void InitCALRegex()
        {

            CALProcedureNameRegex = new Regex(@"\s*\bPROCEDURE\b\s+\w+\b");

            //remove : to
            CALKeywordRegex =
                new Regex(
                    @"(\bOnRun\b|\bVAR\b|\bRecord\b|\bLOCAL\b|\bWITH\b|\bDO\b|\bBEGIN\b|\bIF\b|\bTHEN\b|\bEXIT\b|
                        |\bEND\b|\bCASE\b|\brepeat\b|\buntil\b|\bAssertError\b|\bDownTo\b|\bElse\b|\bEvent\b|
                        |\bFalse\b|\bFor\b|\bLocal\b|\btrue\b|\bwhile\b|\bAND\b|\bOR\b|\bnot\b)",
                     RegexOptions.IgnoreCase);

            //remove : Type
            CALSystemFunctionsRegex =
                new Regex(
                    @"\b(Abs|Activate|Active|Addlink|Addtext|Applicationpath|ArrayLen|Ascending|Beep|Break|CalcDate|CalcField|CalcSum|CalcSums|ChangeCompany|CheckLicenseFile|
                        |Class|Clear|ClearAll|ClearLastError|ClearMarks|ClientType|Close|ClosingDate|CodeCoverageLog|CommandLine|Commit|CompanyName|CompressArray|Confirm|
                        |Consistent|ContextURL|ConvertStr|Copy|CopyArray|CopyFilter|CopyFilters|CopyLinks|CopyStr|CopyStream|Count|CountApprox|Create|CreateDateTime|CreateGUID|
                        |CreateInstream|CreateOutstream|CreateTempFile|CurrentClientType|CurrentDateTime|CurrentExecutionMode|CurrentKey|CurrentKeyIndex|CurrentTransactionType|
                        |Database|Date2DMY|Date2DWY|DaTi2Variant|Debugger|DecimalPlacesMax|DecimalPlacesMin|DefaultClientType|DelChr|Delete|DeleteAll|DeleteLink|DeleteLinks|
                        |DelStr|DMY2Date|Download|DownloadFromStream|DT2Date|DT2Time|Duplicate|DWY2Date|Environ|EOS|Erase|Error|Evaluate|Exists|Export|ExportObjects|Field|
                        |FieldActive|FieldCaption|FieldCount|FieldError|FieldExist|FieldIndex|FieldName|FieldNo|FilterGroup|Find|FindFirst|FindLast|FindSet|Get|GetFilter|GetFilters|
                        |GetLastErrorText|GetPosition|GetRangeMax|GetRangeMin|GetRecord|GetStamp|GetSubtext|GetTable|GetURL|GetView|GlobalLanguage|GUIAllowed|HasFilter|HasLinks|
                        |HasValue|Hyperlink|ImportObjects|IncStr|Init|Input|Insert|InsStr|IsAction|IsAutomation|IsBinary|IsBoolean|IsChar|IsClear|IsCode|IsCodeunit|IsDate|
                        |IsDateFormula|IsDecimal|IsEmpty|IsFile|IsInstream|IsInteger|IsNullGUID|IsOption|IsOutstream|IsRecord|IsText|IsTime|IsTransactionType|KeyCount|KeyGroupDisable|
                        |KeyGroupEnable|KeyGroupEnabled|KeyIndex|Language|Len|Length|LockTable|LockTimeout|Lowercase|Mark|MarkedOnly|MaxStrLen|Message|Modify|ModifyAll|Next|
                        |NormalDate|Number|ObjectType|Open|OSVersion|PadStr|PAGENO|PAPERSOURCE|Pos|Power|Preview|QueryReplace|Quit|Random|Randomize|Read|ReadConsistency|
                        |ReadPermission|ReadText|RecordLevelLocking|Relation|Rename|Reset|Round|RoundDateTime|Run|RunModal|SaveAsExcel|SaveAsHTML|SaveAsPDF|SaveAsXML|SaveRecord|
                        |Seek|SelectLatestVersion|SelectStr|SerialNumber|SetAutoCalcFields|SetCurrentKey|SetFilter|SetPermissionFilter|SetPosition|SetRange|SetRecFilter|SetRecord|
                        |SetSelectionFilter|SetStamp|SetTable|SetTableView|SetView|Shell|SID|Skip|Sleep|STARTSESSION|STOPSESSION|StrCheckSum|StrLen|StrMenu|StrPos|StrSubstNo|
                        |SynchronizeAllLogins|SynchronizeSingleLogin|TableCaption|TableName|TemporaryPath|TestField|TextEncoding|TextMode|TextPos|Today|TOTALSCAUSEDBY|TransferFields|
                        |Trunc|Update|UpdateControls|UpdateEditable|UpdateFontBold|UpdateForeColor|UpdateIndent|UpdateSelected|Upload|UploadIntoStream|Uppercase|UserID|
                        |Validate|Value|VariableActive|Variant2Date|Variant2Time|WindowsLanguage|WordDate|Write|WriteMode|WritePermission|WriteText|Yield|USERID|MAX|min|
                        |calcfields|calcsums|recordid)\b",
                    RegexOptions.IgnoreCase);

            CSharpNumberRegex = new Regex(@"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
        }

        public void InitStyleSchema()
        {
            StringStyle = BrownStyle;
            StringDoubleQuoteStyle = BlackStyle;
            CommentStyle = GreenStyle;
            NumberStyle = MagentaStyle;
            AttributeStyle = GreenStyle;
            ClassNameStyle = ProcStyle;
            KeywordStyle = BlueStyle;
            CommentTagStyle = GrayStyle;
            SystemFunctionStyle = AquaStyle;
            CALProcedureNameStyle = ProcStyle;
        }
        class XmlFoldingTag
        {
            public string Name;
            public int id;
            public int startLine;
            public Place startPlace;
            //****************
            /// <summary>
            /// Pour verifier si la marque a bien ete crée pour le debut du regroupement afin d'en creer aussi pour la fin
            /// Ceci permet de gerer les cas ou on a deux ou plusieurs BEGIN sur la meme ligne
            /// </summary>
            public bool FoldingStartMarkerCreated;
            public string Marker { get { return Name + id; } }
        }
        private void CALFolding(Range range)
        {
            var stack = new Stack<XmlFoldingTag>();
            var id = 0;
            var fctb = range.tb;
            //bool isPermissionFolder = false, isVarFolder = false;
            //extract opening and closing tags (exclude open-close tags: <TAG/>)
            for (int i = 0; i < fctb.LinesCount; i++)
            {
                var line = fctb[i];

                var tagName = "m";
                var iLine = i;
                //if it is opening tag...

                if (iLine == 604)
                {
                    var p = 0;
                }

                //contient un terme 'begin' mais non précédé par '//' 
                var regStartingFoldingPattern = @"(\bcase\b|\bbegin\b|\brepeat\b)(?=(?:[^""]*""[^""]*"")*[^""]*\Z)";
                var rangeLine = new Range(fctb, i);
                foreach (Range rr in rangeLine.GetRanges(regStartingFoldingPattern, RegexOptions.IgnoreCase))
                {
                    if (!rr.CheckHaveStyle(CommentStyle) && !rr.CheckHaveStyle(StringStyle)
                        && !rr.CheckHaveStyle(StringDoubleQuoteStyle) && rr.Icr()
                        && !rr.CheckHaveStyle(CALProcedureNameStyle))
                    {
                        // ...push into stack
                        var tag = new XmlFoldingTag { Name = tagName, id = id++, startLine = iLine };

                        // if this line has no markers - set marker
                        if (string.IsNullOrEmpty(fctb[iLine].FoldingStartMarker))
                        {
                            fctb[iLine].FoldingStartMarker = tag.Marker;
                            tag.FoldingStartMarkerCreated = true;
                        }
                        stack.Push(tag);
                    }
                }

                //Exclure tewte entre " "
                var regEndFoldingPattern = @"(\bend\b|\buntil\b)(?=(?:[^""]*""[^""]*"")*[^""]*\Z)";
                foreach (Range rr in rangeLine.GetRanges(regEndFoldingPattern, RegexOptions.IgnoreCase))
                {
                    if (!rr.CheckHaveStyle(CommentStyle) && !rr.CheckHaveStyle(StringStyle)
                        && !rr.CheckHaveStyle(StringDoubleQuoteStyle) && rr.Icr()
                        && !rr.CheckHaveStyle(CALProcedureNameStyle))
                    {
                        if (stack.Count > 0)
                        {
                            if (iLine == 93)
                            {
                                var p = 0;
                            }
                            XmlFoldingTag tag = new XmlFoldingTag();

                            tag = stack.Pop();


                            //compare line number
                            if (iLine == tag.startLine)
                            {
                                //remove marker, because same line can not be folding
                                if (fctb[iLine].FoldingStartMarker == tag.Marker) //was it our marker?
                                    fctb[iLine].FoldingStartMarker = null;
                            }
                            else
                            {

                                if (tag.FoldingStartMarkerCreated)
                                {
                                    //set end folding marker
                                    if (string.IsNullOrEmpty(fctb[iLine].FoldingEndMarker))
                                        fctb[iLine].FoldingEndMarker = tag.Marker;
                                }

                            }
                        }
                    }
                }
            }
        }
        public bool RangeIsCommentStyle(Range r)
        {
            return r.CheckHaveStyle(CommentStyle);
        }
        public bool RangeIsOneQuoteStyle(Range r)
        {
            return r.CheckHaveStyle(StringStyle);
        }
        public bool RangeIsDoubleQuoteStyle(Range r)
        {
            return r.CheckHaveStyle(StringDoubleQuoteStyle);
        }
        public bool RangeIsProcedureStyle(Range r)
        {
            return r.CheckHaveStyle(CALProcedureNameStyle);
        }
        /// <summary>
        /// Highlights C# code
        /// </summary>
        /// <param name="range"></param>
        public virtual void CALSyntaxHighlight(Range range)
        {
            range.tb.CommentPrefix = "//";
            range.tb.LeftBracket = '(';
            range.tb.RightBracket = ')';
            range.tb.LeftBracket2 = '\x0';
            range.tb.RightBracket2 = '\x0';
            //range.tb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;


            //clear style of changed range
            range.ClearStyle(StringStyle, CommentStyle, NumberStyle, AttributeStyle,
                ClassNameStyle, KeywordStyle, SystemFunctionStyle);
            //
            if (CALKeywordRegex == null)
                InitCALRegex();


            //number highlighting
            range.SetStyle(NumberStyle, CSharpNumberRegex);

            //attribute highlighting
            //range.SetStyle(AttributeStyle, CSharpAttributeRegex);

            //class name highlighting
            range.SetStyle(CALProcedureNameStyle, CALProcedureNameRegex);

            //keyword highlighting
            range.SetStyle(KeywordStyle, CALKeywordRegex);

            //System functions highlighting
            range.SetStyle(SystemFunctionStyle, CALSystemFunctionsRegex);


            CALSyntaxHighlight2(range);
        }
        private void CALSyntaxHighlight2(Range range)
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
                                    var doubleQuoteStringRange = new Range(fctb, stackPeek.startPlace, r.End);
                                    doubleQuoteStringRange.ClearStyle(StyleIndex.All);
                                    doubleQuoteStringRange.SetStyle(StringDoubleQuoteStyle);
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
                                    var oneQuoteStringRange = new Range(fctb, stackPeek.startPlace, r.End);
                                    oneQuoteStringRange.ClearStyle(StyleIndex.All);
                                    oneQuoteStringRange.SetStyle(StringStyle);
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
                        var oneLineComment = new Range(fctb, r.Start, new Range(fctb, r.ToLine).End);
                        oneLineComment.ClearStyle(StyleIndex.All);
                        oneLineComment.SetStyle(CommentStyle);

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
                                var commentBlock = new Range(fctb, stackPeek.startPlace, r.End);
                                commentBlock.ClearStyle(StyleIndex.All);
                                commentBlock.SetStyle(CommentStyle);
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
        }

        #region Styles

        /// <summary>
        /// String style
        /// </summary>
        public Style StringStyle { get; set; }

        /// <summary>
        /// Comment style
        /// </summary>
        public Style CommentStyle { get; set; }

        /// <summary>
        /// Number style
        /// </summary>
        public Style NumberStyle { get; set; }

        /// <summary>
        /// C# attribute style
        /// </summary>
        public Style AttributeStyle { get; set; }

        /// <summary>
        /// Class name style
        /// </summary>
        public Style ClassNameStyle { get; set; }

        /// <summary>
        /// Keyword style
        /// </summary>
        public Style KeywordStyle { get; set; }

        /// <summary>
        /// Style of tags in comments of C#
        /// </summary>
        public Style CommentTagStyle { get; set; }

        /// <summary>
        /// HTML attribute value style
        /// </summary>
        public Style AttributeValueStyle { get; set; }

        /// <summary>
        /// HTML tag brackets style
        /// </summary>
        public Style TagBracketStyle { get; set; }

        /// <summary>
        /// HTML tag name style
        /// </summary>
        public Style TagNameStyle { get; set; }

        /// <summary>
        /// HTML Entity style
        /// </summary>
        public Style HtmlEntityStyle { get; set; }

        /// <summary>
        /// XML attribute style
        /// </summary>
        public Style XmlAttributeStyle { get; set; }

        /// <summary>
        /// XML attribute value style
        /// </summary>
        public Style XmlAttributeValueStyle { get; set; }

        /// <summary>
        /// XML tag brackets style
        /// </summary>
        public Style XmlTagBracketStyle { get; set; }

        /// <summary>
        /// XML tag name style
        /// </summary>
        public Style XmlTagNameStyle { get; set; }

        /// <summary>
        /// XML Entity style
        /// </summary>
        public Style XmlEntityStyle { get; set; }

        /// <summary>
        /// XML CData style
        /// </summary>
        public Style XmlCDataStyle { get; set; }

        /// <summary>
        /// Variable style
        /// </summary>
        public Style VariableStyle { get; set; }

        /// <summary>
        /// Specific PHP keyword style
        /// </summary>
        public Style KeywordStyle2 { get; set; }

        /// <summary>
        /// Specific PHP keyword style
        /// </summary>
        public Style KeywordStyle3 { get; set; }

        /// <summary>
        /// SQL Statements style
        /// </summary>
        public Style StatementsStyle { get; set; }

        /// <summary>
        /// SQL Functions style
        /// </summary>
        public Style FunctionsStyle { get; set; }

        /// <summary>
        /// SQL Types style
        /// </summary>
        public Style TypesStyle { get; set; }
        public Style StringDoubleQuoteStyle { get; set; }
        public Style SystemFunctionStyle { get; set; }
        public Style CALProcedureNameStyle { get; set; }

        #endregion
    }
}
