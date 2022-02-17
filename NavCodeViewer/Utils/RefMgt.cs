using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public class RefMgt
    {
        public List<Range> FieldRefsRanges { get; set; } = new List<Range>();
        public List<Range> FunctionRefsRanges { get; set; } = new List<Range>();
        public List<Range> ObjectRefsRanges { get; set; } = new List<Range>();
        public List<Range> CommentRanges { get; set; } = new List<Range>();
        public List<string> GlobalVariables { get; set; } = new List<string>();
        public List<string> PrivateVariables { get; set; } = new List<string>();
        bool isOperator(string txt)
        {
            var operatorPattern = @" \[ | \] | \( | \) |
                    \:\: | \.\. | \+ | - | \* | \/ | \bdiv\b | \bmod\b | = | > | >= | < | <= | 
                    <> | \bin\b | \band\b | \bor\b | \bnot\b | \bxor\b | \@ ";
            return Regex.Match(txt, operatorPattern).Success;
        }
        private void CALSyntaxHighlight2(Range range)
        {
            var stack = new Stack<XmlFoldingTag>();
            var id = 0;
            var fctb = range.tb;int initPos = 0;string collectedText = "";

            //contient un terme 'begin' mais non précédé par '//' 
            var regStartingFoldingPattern = 
                @" "" | { | ' | \/\/ | } | \s | := | \. | \[ | \] | \( | \) |
                    \:\: | \.\. | \+ | - | \* | \/ | \bdiv\b | \bmod\b | = | > | >= | < | <= | 
                    <> | \bin\b | \band\b | \bor\b | \bnot\b | \bxor\b | \@ | ; | \bwith\b";

            Place start = range.Start;
            int ActiveOneLineComment = -1;
            bool isWithSection = false;
            var ListOfMarkers = range
                .GetRanges(regStartingFoldingPattern, RegexOptions.IgnoreCase)
                .ToList();
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

                        initPos = r.Start.iChar + 1;
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
                                    //doubleQuoteStringRange.ClearStyle(StyleIndex.All);
                                    //doubleQuoteStringRange.SetStyle(StringDoubleQuoteStyle);
                                    stack.Pop();

                                    var place1 = new Place(initPos, r.End.iLine);
                                    var place2 = new Place(r.End.iChar-1, r.End.iLine);
                                    collectedText= new Range(fctb, place1, place2).Text;
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

                        initPos = r.Start.iChar + 1;
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
                                    //oneQuoteStringRange.ClearStyle(StyleIndex.All);
                                    //oneQuoteStringRange.SetStyle(StringStyle);
                                    stack.Pop();

                                    var place1 = new Place(initPos, r.End.iLine);
                                    var place2 = new Place(r.End.iChar - 1, r.End.iLine);
                                    collectedText = new Range(fctb, place1, place2).Text;
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
                                var commentBlock = new Range(fctb, stackPeek.startPlace, r.End);
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
        }
        public List<Refer> GetMarkers(string expression)
        {
            var markers = new List<Refer> { };
            var stack = new Stack<string>();

            var pattern =
                @"""|'|,|\/\/|\s|:=|\.|\[|\]|\(|\)|\:\:|\.\.|\+|-|\*|\/|\bdiv\b|\bmod\b|=|>|>=|<|<=|<>|\bin\b|\band\b|\bor\b|\bnot\b|\bxor\b|\@|;|\r\n|\r|\n";

            var start = 0; int iLine = 0;
            var end = start;
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(expression);
            bool canSplit = true;
            while (m.Success)
            {
                if (m.Value == "\n" || m.Value == "\r")
                {
                    if (m.Index + 1 < expression.Length)
                    {
                        if (expression[m.Index + 1] != '\r' && expression[m.Index + 1] != '\n')
                        {
                            iLine++;
                        }
                    }

                }
                if (m.Value == "\"")
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(m.Value);
                        canSplit = false;
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == m.Value)
                            {
                                if (m.Index + 1 < expression.Length)
                                {
                                    if (expression[m.Index + 1] != '\"')
                                    {
                                        //End of double string
                                        stack.Pop();
                                        canSplit = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (m.Value == "\'")
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(m.Value);
                        canSplit = false;
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == m.Value)
                            {
                                if (m.Index + 1 < expression.Length)
                                {
                                    if (expression[m.Index + 1] != '\'')
                                    {
                                        //End of double string
                                        stack.Pop();
                                        canSplit = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (canSplit && m.Value != "\"" && m.Value != "\'")
                {
                    end = m.Index;
                    var expr = expression.Substring(start, (end - start));
                    InsertExpr(markers, expr, iLine);
                    //if (m.Value == "." || m.Value=="::" || m.Value == ":=" || m.Value == "(")
                    {
                        var expr2 = expression.Substring(end, m.Length);
                        InsertExpr(markers, expr2, iLine);
                    }
                    start = end + m.Length;
                }

                m = m.NextMatch();
            }

            if (canSplit)
            {
                var expr = expression.Substring(end);
                InsertExpr(markers, expr, iLine);
            }

            return markers;
        }
        private void InsertExpr(List<Refer> markers, string expr, int iLine)
        {
            if (expr.Trim() == "") return;
            Refer r = new Refer();
            r.Name = expr;
            r.Order = iLine;
            if (expr.StartsWith("'")) return;
            if (markers.Count > 0)
            {
                if (expr.Trim() == "")
                {
                    if (markers[markers.Count - 1].Name.Trim() != "")
                    {
                        markers.Add(r);
                    }
                }
                else
                {
                    markers.Add(r);
                }
            }
            else
            {
                markers.Add(r);
            }
        }
        
    }
    public class Refer
    {
        public string Name { get; set; }
        public int Order { get; set; }
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
}
