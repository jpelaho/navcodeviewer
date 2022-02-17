using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            //        var text= @"
            //        IF SalesHeader.""Prices Including VAT"" THEN
            //  ItemJnlLine.""Discount Amount"" :=
            //    -((""Line Discount Amount"" + ""Inv. Discount Amount"") / (1 + ""VAT %"" / 100) *
            //      (QtyToBeInvoiced / ""Qty. to Invoice"") - RemDiscAmt)
            //ELSE
            //  ItemJnlLine.""Discount Amount"" :=
            //    -((""Line Discount Amount"" + ""Inv. Discount Amount"") * (QtyToBeInvoiced / ""Qty. to Invoice"") - RemDiscAmt)";

            //var text = @"InvPostingBuffer[i+k][j][k].DELETEALL;";
            var text = @"IF STRPOS(ToName,';') > 0 THEN BEGIN
                CcName:= COPYSTR(ToName, STRPOS(ToName, ';') + 1)";
            var text2 = @"Sum(""Employee Absence"".""Quantity(Base)"" WHERE (Employee No.=FIELD(No.),
                                                                                                               Cause of Absence Code = FIELD(Cause of Absence Filter),
                                                                                                               From Date = FIELD(Date Filter))); ";

            //text2 = RemoveComments(text2);
            //var markers = GetLineMarkers_RefTableRelation(text2);
            var markers = GetLineMarkers_line(text,0,true);
            foreach (var s in markers)
            {
                Console.WriteLine("{0} {1}",s.Order, s.Name.Trim());
                i++;
            }
            //var result = GetLineMarkers(text2);
            //Console.WriteLine(result);
            Console.ReadLine();
        }
        public static List<Marker> GetLineMarkers(string expression)
        {
            var markers = new List<Marker> { };
            var stack = new Stack<string>();

            //remove [ ]
            var pattern =
                @"""|'|,|\[|\]|\/\/|\s|:=|\.|\(|\)|\:\:|\.\.|\+|-|\*|\/|\bdiv\b|\bmod\b|=|>|>=|<|<=|<>|\bin\b|\band\b|\bor\b|\bnot\b|\bxor\b|\@|;|\r\n|\r|\n";

            var start = 0;int iLine = 0;
            var end = start;
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(expression);
            bool canSplit = true;
            while (m.Success)
            {
                if (m.Value == "\n"|| m.Value == "\r")
                {
                    if (m.Index + 1 < expression.Length)
                    {
                        if (expression[m.Index + 1] != '\r'&& expression[m.Index + 1] != '\n')
                        {
                            iLine++;
                        }
                    }
                    
                }
                if (m.Value == "[")
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(m.Value);
                        canSplit = false;
                    }
                    else
                    {
                        //if (stack.Count == 1)
                        //{
                        //    var stackPeek = stack.Peek();
                        //    if (stackPeek == m.Value)
                        //    {
                        //        if (m.Index + 1 < expression.Length)
                        //        {
                        //            if (expression[m.Index + 1] != '\"')
                        //            {
                        //                //End of double string
                        //                stack.Pop();
                        //                canSplit = true;
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
                if (m.Value == "]")
                {
                    if (stack.Count == 0)
                    {
                        //stack.Push(m.Value);
                        //canSplit = false;
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == "[")
                            {
                                //if (m.Index + 1 < expression.Length)
                                //{
                                //    if (expression[m.Index + 1] != '\"')
                                //    {
                                //End of double string
                                stack.Pop();
                                canSplit = true;
                                //}
                                //}
                            }
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

                if (canSplit && m.Value!="\""&&m.Value!="\'" && m.Value != "]")
                {
                    end = m.Index;
                    var expr = expression.Substring(start, (end - start));
                    InsertExpr(markers, expr, iLine);
                    if (m.Value == "." || m.Value=="::" || m.Value == ":=" || m.Value == "(")
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
                var expr = expression.Substring(end+1);
                InsertExpr(markers, expr, iLine);
            }

            return markers;
        }
        private static void InsertExpr(List<Marker> markers, string expr,int iLine)
        {
            if (expr.Trim()=="") return;
            Marker r = new Marker();
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
        public static List<Marker> GetLineMarkers_line(string expression, int iLine, bool CanProcessingArray)
        {
            var markers = new List<Marker> { };
            var stack = new Stack<string>();

            //var pattern =
            //    @"""|'|,|\/\/|\s|:=|\.|\[|\]|\(|\)|\:\:|\.\.|\+|-|\*|\/|\bdiv\b|\bmod\b|=|>|>=|<|<=|<>|\bin\b|\band\b|\bor\b|\bnot\b|\bxor\b|\@|;|\r\n|\r|\n";
            var pattern =
                @"""|'|,|\s|:=|\.|\[|\]|\(|\)|\:\:|\.\.|\+|-|\*|\/|\bdiv\b|\bmod\b|=|>|>=|<|<=|<>|\bin\b|\band\b|\bor\b|\bnot\b|\bxor\b|\@|;|\:";

            //if(expression.Contains(@"""Wizard Step""::""3""..""Wizard Step""::""4"":"))
            //{
            //    var p = 0;
            //}
            var start = 0;
            var end = 0;
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(expression);
            bool canSplit = true; int lastMatchLength = 0;
            while (m.Success)
            {
                if (m.Value == "[" && CanProcessingArray)
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(m.Value);
                        canSplit = false;
                    }
                }
                if (m.Value == "]" && CanProcessingArray)
                {
                    if (stack.Count == 0)
                    {
                        //stack.Push(m.Value);
                        //canSplit = false;
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == "[")
                            {
                                stack.Pop();
                                canSplit = true;
                            }
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
                        //if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == "[")
                            {
                                stack.Push(m.Value);
                                canSplit = false;
                            }
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
                                else
                                {
                                    stack.Pop();
                                    canSplit = true;
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
                        //if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == "[")//dans indices tableau ex = IF COPYSTR(Formula,p,1) IN ['[',']','.'] THEN BEGIN
                            {
                                stack.Push(m.Value);
                                canSplit = false;
                            }
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
                                else
                                {
                                    stack.Pop();
                                    canSplit = true;
                                }
                            }
                        }
                    }
                }



                if (CanProcessingArray)
                {
                    if (canSplit && m.Value != "\"" && m.Value != "\'" && m.Value != "]")
                    {
                        start = collectExpr(expression, iLine, markers, start, out end, m, out lastMatchLength);
                    }
                }
                else
                {
                    if (canSplit && m.Value != "\"" && m.Value != "\'")
                    {
                        start = collectExpr(expression, iLine, markers, start, out end, m, out lastMatchLength);
                    }
                }


                m = m.NextMatch();
            }

            if (canSplit)
            {
                var expr = expression.Substring(end + lastMatchLength);
                InsertMarker(markers, expr, iLine);
            }

            return markers;
        }
        private static int collectExpr(string expression, int iLine, List<Marker> markers,
    int start, out int end, Match m, out int lastMatchLength)
        {
            end = m.Index;
            var expr = expression.Substring(start, (end - start));
            InsertMarker(markers, expr, iLine);

            var isPlage = false;
            isPlage = DetectPlage(expression, m, isPlage);
            //insert operator
            string strVal = m.Value;
            if (IsPertinentOperator(strVal) && !isPlage)
            {
                var expr2 = expression.Substring(end, m.Length);
                InsertMarker(markers, expr2, iLine);
            }
            start = end + m.Length;
            lastMatchLength = m.Length;
            return start;
        }
        public static void InsertMarker(List<Marker> markers, string expr, int iLine)
        {
            if (expr.Trim() == "") return;
            Marker r = new Marker();
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
        private static bool DetectPlage(string expression, Match m, bool isPlage)
        {
            if (m.Index + 1 < expression.Length)
            {
                if (m.Value == "." && expression[m.Index + 1] == '.')
                {
                    isPlage = true;
                }
            }
            if (m.Index - 1 >= 0)
            {
                if (m.Value == "." && expression[m.Index - 1] == '.')
                {
                    isPlage = true;
                }
            }

            return isPlage;
        }

        private static bool IsPertinentOperator(string strVal)
        {
            return strVal == "." || strVal == "::" || strVal == ":=" || strVal == "(" || strVal == ",";
        }
        public static string RemoveComments(string source)
        {
            var stack = new Stack<string>();
            string returnedString = "";
            bool isLineComment = false, isBlocComment = false;

            for (int i = 0; i < source.Length; i++)
            {
                var c = source[i];

                if ((isBlocComment || isLineComment) && (@"/{}".Contains(c.ToString()) == false))
                {
                    if (c == '\n')
                    {
                        isLineComment = false;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (@"""'/{}".Contains(c.ToString()) == false)
                {
                    returnedString += c.ToString();
                    continue;
                }
                
                if (c == '/')
                {
                    returnedString += c.ToString();

                    if (i - 1 >= 0)
                    {
                        if (source[i - 1] == '/')
                        {
                            if (stack.Count == 0)
                            {
                                isLineComment = true;
                                returnedString = ReplaceLastOccurrence(returnedString, "/", "");
                                returnedString = ReplaceLastOccurrence(returnedString, "/", "");
                            }
                        }
                    }
                }
                if (c == '\"')
                {
                    returnedString += c.ToString();

                    if (stack.Count == 0)
                    {
                        stack.Push(c.ToString());
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == c.ToString())
                            {
                                if (i + 1 < source.Length)
                                {
                                    if (source[i + 1] != c)
                                    {
                                        //End of double string
                                        stack.Pop();
                                    }
                                }
                            }
                        }
                    }
                }
                if (c == '\'')
                {
                    returnedString += c.ToString();

                    if (stack.Count == 0)
                    {
                        stack.Push(c.ToString());
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == c.ToString())
                            {
                                if (i + 1 < source.Length)
                                {
                                    if (source[i + 1] != c)
                                    {
                                        //End of double string
                                        stack.Pop();
                                    }
                                }
                            }
                        }
                    }
                }
                if (c == '{')
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(c.ToString());
                        isBlocComment = true;
                    }
                    else
                    {
                        returnedString += c.ToString();

                        var stackPeek = stack.Peek();
                        if (stackPeek == "{")
                        {
                            stack.Push(c.ToString());
                        }
                    }
                }
                if (c == '}')
                {
                    if (stack.Count == 0)
                    {
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == "{")
                            {
                                //Closed comment bloc
                                stack.Pop();
                                isBlocComment = false;
                            }
                        }
                        else
                        {
                            returnedString += c.ToString();

                            var stackPeek = stack.Peek();
                            if (stackPeek == "{")
                            {
                                stack.Pop();
                            }
                        }
                    }
                }
            }

            return returnedString;
        }
        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
        public static string[] SplitLines(string oneFieldSource)
        {
            return oneFieldSource.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }
        public static List<Marker> GetLineMarkers_RefTableRelation(string expression)
        {
            var markers = new List<Marker> { };
            var lines = SplitLines(expression);
            bool canSplit = true;
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim or RemoveQuotes
                var line = lines[i].Trim();
                if (line == "") continue;
                var m = GetMarkers_line_RefTableRelation(line, i,ref canSplit);
                markers.AddRange(m);
            }
            return markers;
        }
        public static List<Marker> GetMarkers_line_RefTableRelation(string expression, int iLine,ref bool canSplit)
        {
            var markers = new List<Marker> { };
            var stack = new Stack<string>();

            var pattern =
    @"""|\(|\)|;|\.";
            //var pattern =
            //    @"""|\(|\)|;|\.";

            var start = 0;
            var end = 0;
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(expression);
             int lastMatchLength = 0;
            while (m.Success)
            {
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

                if (m.Value == "(")
                {
                    //if (stack.Count == 0)
                    {
                        stack.Push(m.Value);
                        canSplit = false;
                    }
                    //else
                    //{
                    //    if (stack.Count == 1)
                    //    {
                    //        var stackPeek = stack.Peek();
                    //        if (stackPeek == m.Value)
                    //        {
                    //            if (m.Index + 1 < expression.Length)
                    //            {
                    //                if (expression[m.Index + 1] != '\"')
                    //                {
                    //                    //End of double string
                    //                    stack.Pop();
                    //                    canSplit = true;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
                if (m.Value == ")")
                {
                    if (stack.Count == 0)
                    {
                        //stack.Push(m.Value);
                        //canSplit = false;
                    }
                    else
                    {
                        if (stack.Count == 1)
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == "(")
                            {
                                //if (m.Index + 1 < expression.Length)
                                {
                                    //if (expression[m.Index + 1] != '\"')
                                    {
                                        //End of double string
                                        stack.Pop();
                                        canSplit = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var stackPeek = stack.Peek();
                            if (stackPeek == "(")
                            {
                                stack.Pop();
                            }
                        }
                    }
                }
                if (canSplit && m.Value!="(" && m.Value != ")" && m.Value!="\"")
                {
                    end = m.Index;
                    var expr = expression.Substring(start, (end - start));
                    InsertExpr(markers, expr, iLine);
                    //insert operator
                    string strVal = m.Value;
                    //if (IsPertinentOperator(strVal))
                    {
                        var expr2 = expression.Substring(end, m.Length);
                        InsertExpr(markers, expr2, iLine);
                    }
                    start = end + m.Length;
                    lastMatchLength = m.Length;
                    //canSplit = false;
                }

                m = m.NextMatch();
            }

            //if (canSplit)
            {
                var expr = expression.Substring(end + lastMatchLength);
                InsertExpr(markers, expr, iLine);
            }

            return markers;
        }
    }
    public class Marker
    {
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
