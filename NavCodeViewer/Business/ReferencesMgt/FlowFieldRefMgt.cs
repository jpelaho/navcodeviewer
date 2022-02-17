using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public class CalcFormulaRefMgt
    {
        NavObject _navObject;
        NavObjectCollectRefs _collectRefs;
        TableRelationRefMgt tabRelationMgt;
        public CalcFormulaRefMgt(NavObject _navObject1, NavObjectCollectRefs _collectRefs1)
        {
            _navObject = _navObject1;
            _collectRefs = _collectRefs1;
            tabRelationMgt = new TableRelationRefMgt(_navObject, _collectRefs);
        }
        //public void CollectRef_CalcFormula(CodeRange range)
        //{
        //    var rangeText = range.GetRangeText(_navObject.tb);
        //    var elem = "CalcFormula=";
        //    if (rangeText.IndexOf(elem) < 0) return;

        //    var lines = rangeText.SplitLines();
        //    var start = new Place(0, 0);
        //    var end = new Place(0, 0);
        //    bool found = false;
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        //DO NOT Trim Here
        //        var line = lines[i];

        //        if (found)
        //        {
        //            int k = line.IndexOfAny(new char[] { ';', '}' });
        //            if (k >= 0)
        //            {
        //                end = new Place(k, i);
        //                var r = range.CreateSubRange(start, end);
        //                var expr = r.GetRangeText(lines);
        //                InsertRef_CalcFormula(expr, r, range);
        //                found = false;
        //            }
        //        }
        //        int j = line.IndexOf(elem);
        //        if (j < 0)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            start = new Place(j + elem.Length, i);
        //            found = true;
        //        }
        //    }
        //}
        public void InsertRef_CalcFormula(string expr,CodeRange range,CodeRange ZoneRange)
        {
            var rangeText = expr;
            //var elem = "CalcFormula=";
            //if (rangeText.IndexOf(elem) < 0) return;
            if (expr.Contains(@"Count(""Purchase Header"" WHERE (Document Type=CONST(Credit Memo),"))
            {
                int p = 0;
            }

            var lines = rangeText.SplitLines();
            //var start = new Place(0, 0);
            //var end = new Place(0, 0);
            int tableFormulaID = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim Here
                var line = lines[i];
                int startLine = ZoneRange.FromLine+ range.Start.iLine + i;

                if (line.Contains("WHERE"))
                {
                    var tab = line.Split(new string[] { "WHERE" }, StringSplitOptions.None);
                    if (tab.Length > 0)
                    {
                        var enteteFormule = tab[0].Trim();
                        int ind = enteteFormule.IndexOf("(");
                        var str = enteteFormule.Substring(ind + 1);
                        if (str.IsNotNullOrEmpty())
                        {
                            //var markers = GetLineMarkers_CalcFormula(str);
                            var markers = tabRelationMgt.GetLineMarkers_RefTableRelation(str);

                            //TraiterTableExpr_CalcFormula(markers.ToArray(), startLine, ref tableFormulaID);
                            tabRelationMgt.TraiterTableRelationSource_RefTableRelation(markers.ToArray(), 
                                startLine, ref tableFormulaID);
                        }
                    }
                    if (tab.Length > 1)
                    {
                        var firstWhere = tab[1].Trim();
                        TraiterWhereExpr(tableFormulaID, startLine, firstWhere);
                    }
                }
                else
                {
                    //Autre Where
                    var firstWhere = line.Trim();
                    TraiterWhereExpr(tableFormulaID, startLine, firstWhere);
                }
            }
        }
        private void TraiterWhereExpr(int tableFormulaID, int startLine, string firstWhere)
        {
            if (firstWhere.EndsWith(",")) firstWhere = firstWhere.ReplaceLastOccurrence(",", "");
            //if (firstWhere.EndsWith(")")) firstWhere = firstWhere.ReplaceLastOccurrence(")", "");
            if (firstWhere.StartsWith("(")) firstWhere = firstWhere.ReplaceFirstOccurrence("(", "");

            //var tab2 = firstWhere.Split('=');
            int posEgale = firstWhere.IndexOfAvoidQuotes('=');
            if (posEgale >= 0)
            {
                //TODO Validate here, la table depent de l'objet ou est est
                //Internal field of Table or Rec Table
                var fieldExt = firstWhere.Substring(0, posEgale);
                _collectRefs.AddReference(fieldExt, startLine,0, RefType.ExternalField, tableFormulaID, ObjectType.Table);


                var fieldInt = firstWhere.Substring(posEgale + 1);
                _collectRefs.ProcessFilterFieldConst(startLine, tableFormulaID,fieldExt,fieldInt);
                //if (fieldInt.Contains("FIELD"))
                //{
                //    int firstParan = fieldInt.IndexOf("(");
                //    int lastParan = fieldInt.IndexOf(")");
                //    var field = fieldInt.Substring(firstParan + 1, lastParan - firstParan - 1);
                //    if (field.Contains("UPPERLIMIT"))
                //    {
                //        field = field.Replace("UPPERLIMIT", "");
                //        field = field.RemoveIfStartWith("(").RemoveIfEndWith(")");
                //    }
                //    _collectRefs.AddReference(field, startLine, RefType.InternalField, _navObject.ID, ObjectType.Table);
                //}

            }

            //return firstWhere;
        }
        //private List<Marker> GetLineMarkers_CalcFormula(string expression)
        //{
        //    var markers = new List<Marker> { };
        //    var lines = expression.SplitLines();
        //    bool canSplit = true;
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        //DO NOT Trim or RemoveQuotes
        //        var line = lines[i].Trim();
        //        if (line == "") continue;
        //        var m = GetMarkers_line_CalcFormula(line, i, ref canSplit);
        //        markers.AddRange(m);
        //    }
        //    return markers;
        //}
    //    private List<Marker> GetMarkers_line_CalcFormula(string expression, int iLine, ref bool canSplit)
    //    {
    //        var markers = new List<Marker> { };
    //        var stack = new Stack<string>();

    //        var pattern =
    //@"""|\(|\)|;|\s|\.";
    //        //var pattern =
    //        //    @"""|\(|\)|;|\.";

    //        var start = 0;
    //        var end = 0;
    //        Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
    //        Match m = r.Match(expression);
    //        int lastMatchLength = 0;
    //        while (m.Success)
    //        {
    //            if (m.Value == "\"")
    //            {
    //                if (stack.Count == 0)
    //                {
    //                    stack.Push(m.Value);
    //                    canSplit = false;
    //                }
    //                else
    //                {
    //                    if (stack.Count == 1)
    //                    {
    //                        var stackPeek = stack.Peek();
    //                        if (stackPeek == m.Value)
    //                        {
    //                            if (m.Index + 1 < expression.Length)
    //                            {
    //                                if (expression[m.Index + 1] != '\"')
    //                                {
    //                                    //End of double string
    //                                    stack.Pop();
    //                                    canSplit = true;
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            if (m.Value == "(")
    //            {
    //                //if (stack.Count == 0)
    //                {
    //                    stack.Push(m.Value);
    //                    canSplit = false;
    //                }
    //                //else
    //                //{
    //                //    if (stack.Count == 1)
    //                //    {
    //                //        var stackPeek = stack.Peek();
    //                //        if (stackPeek == m.Value)
    //                //        {
    //                //            if (m.Index + 1 < expression.Length)
    //                //            {
    //                //                if (expression[m.Index + 1] != '\"')
    //                //                {
    //                //                    //End of double string
    //                //                    stack.Pop();
    //                //                    canSplit = true;
    //                //                }
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //            }
    //            if (m.Value == ")")
    //            {
    //                if (stack.Count == 0)
    //                {
    //                    //stack.Push(m.Value);
    //                    //canSplit = false;
    //                }
    //                else
    //                {
    //                    if (stack.Count == 1)
    //                    {
    //                        var stackPeek = stack.Peek();
    //                        if (stackPeek == "(")
    //                        {
    //                            //if (m.Index + 1 < expression.Length)
    //                            {
    //                                //if (expression[m.Index + 1] != '\"')
    //                                {
    //                                    //End of double string
    //                                    stack.Pop();
    //                                    canSplit = true;
    //                                }
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        var stackPeek = stack.Peek();
    //                        if (stackPeek == "(")
    //                        {
    //                            stack.Pop();
    //                        }
    //                    }
    //                }
    //            }
    //            if (canSplit && m.Value != "(" && m.Value != ")" && m.Value != "\"")
    //            {
    //                end = m.Index;
    //                var expr = expression.Substring(start, (end - start));
    //                _collectRefs.InsertMarker(markers, expr, iLine);
    //                //insert operator
    //                string strVal = m.Value;
    //                //if (IsPertinentOperator(strVal))
    //                {
    //                    var expr2 = expression.Substring(end, m.Length);
    //                    _collectRefs.InsertMarker(markers, expr2, iLine);
    //                }
    //                start = end + m.Length;
    //                lastMatchLength = m.Length;
    //                //canSplit = false;
    //            }

    //            m = m.NextMatch();
    //        }

    //        //if (canSplit)
    //        {
    //            var expr = expression.Substring(end + lastMatchLength);
    //            _collectRefs.InsertMarker(markers, expr, iLine);
    //        }

    //        return markers;
    //    }
    //    private void TraiterTableExpr_CalcFormula(Marker[] markers, int startLine, ref int TableRelationID)
    //    {
    //        //string expr = "";
    //        bool HasField = false;
    //        for (int i = 0; i < markers.Length; i++)
    //        {
    //            var m = markers[i];
    //            if (m.Name.EqualsIgnoreCase("."))
    //            {
    //                HasField = true;
    //                continue;
    //            }
    //            if (HasField)
    //            {
    //                _collectRefs.AddReference(m.Name, startLine + m.Order, RefType.ExternalField, TableRelationID, ObjectType.Table);
    //            }
    //            else
    //            {
    //                //Table Relation
    //                var obj = _navObject.NavProject.GetObject(m.Name, ObjectType.Table);
    //                if (obj != null)
    //                {
    //                    TableRelationID = ((NavObject)obj).ID;
    //                    _collectRefs.AddReference(m.Name, startLine + m.Order, RefType.ExternalObject, TableRelationID, ObjectType.Table);
    //                }
    //            }
    //        }
    //    }
    }
}
