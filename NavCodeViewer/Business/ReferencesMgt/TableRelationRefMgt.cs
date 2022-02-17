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
    public class TableRelationRefMgt
    {
        NavObject _navObject;
        NavObjectCollectRefs _collectRefs;
        public TableRelationRefMgt(NavObject _navObject1, NavObjectCollectRefs _collectRefs1)
        {
            _navObject = _navObject1;
            _collectRefs = _collectRefs1;
        }
        //public void CollectRef_TableRelation(CodeRange range)
        //{
        //    var rangeText = range.GetRangeText(_navObject.tb);
        //    var elem = "TableRelation=";
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
        //                InsertRefTableRelation(expr, r, range);
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
        public List<Marker> GetLineMarkers_RefTableRelation(string expression)
        {
            if (expression.StartsWith("[") && expression.EndsWith("]"))
            {
                expression = expression.RemoveAtBorders("[", "]");
            }
            var markers = new List<Marker> { };
            var lines = expression.SplitLines();
            bool canSplit = true;
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    //DO NOT Trim or RemoveQuotes
            //    var line = lines[i].Trim();
            //    if (line == "") continue;
            //    var m = GetMarkers_line_RefTableRelation(line, i, ref canSplit);
            //    markers.AddRange(m);
            //}
            markers = GetMarkers_line_RefTableRelation(expression, 0, ref canSplit);
            return markers;
        }
        public List<Marker> GetMarkers_line_RefTableRelationwww(string expression, int iLine, ref bool canSplit)
        {
            var markers = new List<Marker> { };
            var stack = new Stack<string>();

            var pattern = @"""|\(|\)|;|\.\.|\||WHERE|IF|ELSE|FILTER|CONST";
            //var pattern = @"""|\(|\)|;|\s|\.";
            //var pattern = @"""|\(|\)|;|\.";

            if (!expression.EndsWith(";"))
            {
                expression = expression + ";";
            }

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

                //if (m.Value == "(")
                //{
                //    //if (stack.Count == 0)
                //    {
                //        stack.Push(m.Value);
                //        canSplit = false;
                //    }
                //    //else
                //    //{
                //    //    if (stack.Count == 1)
                //    //    {
                //    //        var stackPeek = stack.Peek();
                //    //        if (stackPeek == m.Value)
                //    //        {
                //    //            if (m.Index + 1 < expression.Length)
                //    //            {
                //    //                if (expression[m.Index + 1] != '\"')
                //    //                {
                //    //                    //End of double string
                //    //                    stack.Pop();
                //    //                    canSplit = true;
                //    //                }
                //    //            }
                //    //        }
                //    //    }
                //    //}
                //}
                //if (m.Value == ")")
                //{
                //    if (stack.Count == 0)
                //    {
                //        //stack.Push(m.Value);
                //        //canSplit = false;
                //    }
                //    else
                //    {
                //        if (stack.Count == 1)
                //        {
                //            var stackPeek = stack.Peek();
                //            if (stackPeek == "(")
                //            {
                //                //if (m.Index + 1 < expression.Length)
                //                {
                //                    //if (expression[m.Index + 1] != '\"')
                //                    {
                //                        //End of double string
                //                        stack.Pop();
                //                        canSplit = true;
                //                    }
                //                }
                //            }
                //        }
                //        else
                //        {
                //            var stackPeek = stack.Peek();
                //            if (stackPeek == "(")
                //            {
                //                stack.Pop();
                //            }
                //        }
                //    }
                //}
                if (canSplit && m.Value != "\"")
                {
                    end = m.Index;
                    var expr = expression.Substring(start, (end - start)).Trim();
                    InsertMarker_TableRelation_Main(iLine, markers, expr);
                    //insert operator
                    string strVal = m.Value;
                    //if (IsPertinentOperator(strVal))
                    if (strVal != "(" && strVal != ")" && strVal != "|" && strVal != ";")
                    {
                        var expr2 = expression.Substring(end, m.Length);
                        _collectRefs.InsertMarker(markers, expr2, iLine,end);
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
                InsertMarker_TableRelation_Main(iLine, markers, expr);
                //_collectRefs.InsertMarker(markers, expr, iLine);
            }

            return markers;
        }
        private void InsertMarker_TableRelation_Main(int iLine, List<Marker> markers, string expr)
        {
            if (expr == "") return;
            int SepEgale = expr.IndexOfAvoidQuotes('=');
            var strGauche = ""; var strDroite = "";
            if (SepEgale >= 0)
            {
                strGauche = expr.Substring(0,SepEgale).RemoveIfStartWith("(");
                InsertMarker_TableRelation(iLine, markers, strGauche);

                strDroite = expr.Substring(SepEgale + 1);
                InsertMarker_TableRelation(iLine, markers, strDroite);
            }
            else
            {
                InsertMarker_TableRelation(iLine, markers, expr); 
            }
        }
        private void InsertMarker_TableRelation(int iLine, List<Marker> markers, string expr)
        {
            if (expr.Trim() == "") return;
            int SepPoint = expr.IndexOfAvoidQuotes('.');
            var strTab = ""; var strfield = "";
            if (SepPoint >= 0)
            {
                //if (SepPoint + 1 < expr.Length)
                //{
                //    if (expr[SepPoint + 1] == '=')
                //    {
                //        strTab = expr.Substring(0, SepPoint + 1);
                //    }
                //    else
                //    {
                //        strTab = expr.Substring(0, SepPoint);
                //    }
                //}
                //else
                {
                    strTab = expr.Substring(0, SepPoint);
                }
                strTab = strTab.RemoveIfStartWith("(");
                _collectRefs.InsertMarker(markers, strTab, iLine, 0);
                strfield = expr.Substring(SepPoint + 1);
                _collectRefs.InsertMarker(markers, strfield, iLine, 0);
            }
            else
            {
                _collectRefs.InsertMarker(markers, expr, iLine, 0);
            }
        }

        public List<Marker> GetMarkers_line_RefTableRelation(string expression, int iLine, ref bool canSplit)
        {
            var markers = new List<Marker> { };
            var stack = new Stack<string>();

            var pattern = @"""|\(|\)|;|\s";
            //var pattern = @"""|\(|\)|;|\s|\.";
            //var pattern = @"""|\(|\)|;|\.";

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
                                else
                                {
                                    stack.Pop();
                                    canSplit = true;
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
                if (canSplit && m.Value != "(" && m.Value != ")" && m.Value != "\"")
                {
                    end = m.Index;
                    var expr = expression.Substring(start, (end - start)).Trim();
                    //int SepPoint = expr.IndexOfAvoidQuotes('.');
                    //int SepEgale = expr.IndexOfAvoidQuotes('=');
                    //var strTab = "";var strfield = "";
                    //if (SepPoint >= 0)
                    //{
                    //    if (expr[SepPoint + 1] == '=')
                    //    {
                    //        strTab = expr.Substring(0, SepPoint+1);
                    //    }
                    //    else
                    //    {
                    //        strTab = expr.Substring(0, SepPoint);
                    //    }
                    //    strTab = strTab.RemoveIfStartWith("(");
                    //    //strTab = expr.Substring(0, Sep);
                    //    _collectRefs.InsertMarker(markers, strTab, iLine);
                    //    strfield = expr.Substring(SepPoint + 1);
                        
                    //    _collectRefs.InsertMarker(markers, strfield, iLine);
                    //}
                    //else
                    {
                        _collectRefs.InsertMarker(markers, expr, iLine, start);
                    }
                    //insert operator
                    string strVal = m.Value;
                    //if (IsPertinentOperator(strVal))
                    {
                        var expr2 = expression.Substring(end, m.Length);
                        _collectRefs.InsertMarker(markers, expr2, iLine, end);
                    }
                    start = end + m.Length;
                    lastMatchLength = m.Length;
                    //canSplit = false;
                }

                m = m.NextMatch();
            }

            //if (canSplit)
            {
                int startIndex = end + lastMatchLength;
                var expr = expression.Substring(startIndex);
                _collectRefs.InsertMarker(markers, expr, iLine, startIndex);
            }

            return markers;
        }
        public void InsertRefTableRelation(string sourceTableRelation, CodeRange r, CodeRange ZoneRange)
        {
            //var rangeText = GetRangeText(r);
            var markers = GetLineMarkers_RefTableRelation(sourceTableRelation);
            var toProcess = new List<Marker> { };
            for (int i = 0; i < markers.Count; i++)
            {
                var m = markers[i];

                if (m.Name.EqualsIgnoreCase("ELSE"))
                {
                    TraiterTranche_TableRelation(toProcess.ToArray(), ZoneRange.FromLine + r.Start.iLine);
                    toProcess = new List<Marker> { };
                    continue;
                }
                if (m.Name != ";" && m.Name != "}")
                {
                    toProcess.Add(m);
                }
            }
            if (toProcess.Count > 0)
            {
                TraiterTranche_TableRelation(toProcess.ToArray(), ZoneRange.FromLine + r.Start.iLine);
            }
        }
        private void TraiterTranche_TableRelation(Marker[] markers, int startLine)
        {
            var toProcess = new List<Marker> { };
            //var refLine = startLine + m.Order; 
            int TableRelationId = 0;
            bool HaveIf = false; bool HaveWhere = false;
            for (int i = 0; i < markers.Length; i++)
            {
                var m = markers[i];

                if (m.Name.EqualsIgnoreCase("WHERE"))
                {
                    HaveWhere = true;
                    TraiterTableRelationSource_RefTableRelation(toProcess.ToArray(), startLine, ref TableRelationId);
                    toProcess = new List<Marker> { };
                    continue;
                }
                if (m.Name.EqualsIgnoreCase("IF"))
                {
                    HaveIf = true;
                    continue;
                }

                toProcess.Add(m);

                if (HaveIf && m.Name.Trim().EndsWith(")"))
                {
                    //IfSource += m.Name;
                    TraiterIfSource_RefTableRelation(toProcess.ToArray(), startLine);
                    HaveIf = false;
                    toProcess = new List<Marker> { };
                    continue;
                }

                if (HaveWhere && m.Name.Trim().EndsWith(")"))
                {
                    TraiterWhereSource_RefTableRelation(toProcess.ToArray(), startLine, TableRelationId);
                    HaveIf = false;
                    toProcess = new List<Marker> { };
                    continue;
                }
            }
            if (toProcess.Count > 0)
            {
                TraiterTableRelationSource_RefTableRelation(toProcess.ToArray(), startLine, ref TableRelationId);
            }
        }
        public void TraiterTableRelationSource_RefTableRelation(Marker[] markers, int startLine, ref int TableRelationID)
        {
            //string expr = "";
            //bool HasField = false;
            for (int i = 0; i < markers.Length; i++)
            {
                var m = markers[i];
                string strExpr = m.Name.Trim();
                if (strExpr == "") continue;

                int posPoint = strExpr.IndexOfAvoidQuotes('.');
                if (posPoint >= 0)
                {
                    var strTable = strExpr.Substring(0, posPoint).RemoveQuotes();
                    var obj = _navObject.NavProject.GetObject(strTable, ObjectType.Table);
                    if (obj != null)
                    {
                        TableRelationID = ((NavObject)obj).ID;
                        _collectRefs.AddReference(strTable, startLine + m.Order,0, RefType.ExternalObject, TableRelationID, ObjectType.Table);
                    }

                    var strField = strExpr.Substring(posPoint+1).RemoveQuotes();
                    _collectRefs.AddReference(strField, startLine + m.Order,0, RefType.ExternalField, TableRelationID, ObjectType.Table);
                }
                else
                {
                    var obj = _navObject.NavProject.GetObject(strExpr.RemoveQuotes(), ObjectType.Table);
                    if (obj != null)
                    {
                        TableRelationID = ((NavObject)obj).ID;
                        _collectRefs.AddReference(strExpr, startLine + m.Order,0, RefType.ExternalObject, TableRelationID, ObjectType.Table);
                    }
                }

                //if (m.Name.EqualsIgnoreCase("."))
                //{
                //    HasField = true;
                //    continue;
                //}
                //if (HasField)
                //{
                //    _collectRefs.AddReference(m.Name, startLine + m.Order, RefType.ExternalField, TableRelationID, ObjectType.Table);
                //}
                //else
                //{
                //    //Table Relation
                //    var obj = _navObject.NavProject.GetObject(m.Name, ObjectType.Table);
                //    if (obj != null)
                //    {
                //        TableRelationID = ((NavObject)obj).ID;
                //        _collectRefs.AddReference(m.Name, startLine + m.Order, RefType.ExternalObject, TableRelationID, ObjectType.Table);
                //    }
                //}
            }
        }
        private void TraiterIfSource_RefTableRelation(Marker[] markers, int startLine)
        {
            //string expr = "";
            for (int i = 0; i < markers.Length; i++)
            {
                var m = markers[i];
                string name = m.Name.Trim();
                if (name.EqualsIgnoreCase("IF")) continue;
                if (name.EndsWith(",") || name.EndsWith(")"))
                {
                    var expr = name.ReplaceFirstOccurrence("(", "");
                    int posEgale = expr.IndexOfAvoidQuotes('=');
                    if (posEgale >= 0)
                    {
                        var field = expr.Substring(0,posEgale);
                        _collectRefs.AddReference(field, startLine + m.Order,0, RefType.InternalField, _navObject.ID, ObjectType.Table);


                        var fieldInt = expr.Substring(posEgale + 1);
                        _collectRefs.ProcessFilterFieldConst(startLine, _navObject.ID, field, fieldInt);
                    }


                    //    var tab = expr.Split('=');
                    //if (tab.IsNotNullOrEmpty())
                    //{
                    //    //TODO Validate here, la table depent de l'objet ou est est
                    //    //Internal field of Table or Rec Table
                    //    var field = tab[0];
                    //    _collectRefs.AddReference(field, startLine + m.Order, RefType.InternalField, _navObject.ID, ObjectType.Table);
                    //}
                }
            }
        }
        private void TraiterWhereSource_RefTableRelation(Marker[] markers, int startLine, int tableRelationObjId)
        {
            //string expr = "";
            //Price Includes VAT=FIELD(First Name)
            for (int i = 0; i < markers.Length; i++)
            {
                var m = markers[i];
                if (m.Name.EqualsIgnoreCase("WHERE")) continue;
                var expr = m.Name.RemoveAtBorders("(", ")");
                var tab = expr.Split(',');
                foreach(var str in tab)
                {
                    TraiterOccWhere(startLine, tableRelationObjId, m, str.Trim());
                }
                //if (m.Name.EndsWith(",") || m.Name.EndsWith(")"))
                //{
                //    var expr = m.Name.Trim();
                //    //if (expr.StartsWith("("))
                //    //{
                //    //    expr = expr.ReplaceFirstOccurrence("(", "");
                //    //}
                //    expr = expr.RemoveAtBorders("(", ")");
                //    //var tab = expr.Split('=');
                    
                //}
            }
        }

        private void TraiterOccWhere(int startLine, int tableRelationObjId, Marker m, string expr)
        {
            int posEgale = expr.IndexOfAvoidQuotes('=');
            if (posEgale >= 0)
            {
                //TODO Validate here, la table depent de l'objet ou est est
                //Internal field of Table or Rec Table
                var fieldExt = expr.Substring(0, posEgale);
                _collectRefs.AddReference(fieldExt, startLine + m.Order,0, RefType.ExternalField, tableRelationObjId, ObjectType.Table);

                var fieldInt = expr.Substring(posEgale + 1);
                _collectRefs.ProcessFilterFieldConst(startLine, tableRelationObjId, fieldExt, fieldInt);
            }
        }
    }
}
