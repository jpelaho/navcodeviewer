using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    
    public class OthersRefMgt
    {
        NavObject _navObject; 
        NavObjectCollectRefs _collectRefs;
        public OthersRefMgt(NavObject _navObject1, NavObjectCollectRefs _collectRefs1)
        {
            _navObject = _navObject1;
            _collectRefs = _collectRefs1;
        }
        public void CollectRef_KEYS(CodeRange range)
        {
            var rangeText = range.GetRangeText(_navObject.Tb);
            var lines = rangeText.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim Here
                var line = lines[i];

                int k = line.IndexOf(";");
                if (k >= 0)
                {
                    var tab = line.Split(';');
                    if (tab.Length > 1)
                    {
                        var keys = tab[1].RemoveAtBorders("{", "}").Trim();
                        AddListFields(range, i, keys);
                    }
                }
                int sif = line.IndexOf("SumIndexFields=");
                if (sif >= 0)
                {
                    var listField = line.Substring(sif + "SumIndexFields=".Length);
                    listField= listField.RemoveAtBorders("{", "}").Trim();
                    if (listField.IsNotNullOrEmpty())
                    {
                        //var keys = tab[1].RemoveAtBorders("{", "}").Trim();
                        AddListFields(range, i, listField);
                    }
                }
            }
        }

        private void AddListFields(CodeRange range, int i, string keys)
        {
            var tab2 = keys.Split(',');
            foreach (var key in tab2)
            {
                int defLine = range.FromLine + i;
                _collectRefs.AddReference(key, defLine,0, RefType.InternalField, _navObject.ID, ObjectType.Table);
            }
        }

        public void CollectRef_FIELDGROUPS(CodeRange range)
        {
            var rangeText = range.GetRangeText(_navObject.Tb);
            var lines = rangeText.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim Here
                var line = lines[i];

                int k = line.IndexOf(";");
                if (k >= 0)
                {
                    var tab = line.Split(';');
                    if (tab.Length > 2)
                    {
                        var keys = tab[2].RemoveAtBorders("{", "}").Trim();
                        var tab2 = keys.Split(',');
                        foreach (var key in tab2)
                        {
                            int defLine = range.FromLine + i;
                            _collectRefs.AddReference(key, defLine,0, RefType.InternalField, _navObject.ID, ObjectType.Table);
                        }
                    }
                }
            }
        }
        //public void CollectRef_AltSearchField(CodeRange range)
        //{
        //    var rangeText = range.GetRangeText(_navObject.tb);
        //    var elem = "AltSearchField=";
        //    if (rangeText.IndexOf(elem) < 0) return;
        //    var lines = rangeText.SplitLines();
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        var line = lines[i];
        //        int j = line.IndexOf(elem);
        //        if (j < 0)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            InsertRefAltSearchField(range, i, line);
        //            //break;
        //        }

        //    }
        //}
        public void InsertRef_InternalField(string source, CodeRange r, CodeRange ZoneRange)
        {
            var field = source;
            if (field.IsNullOrEmpty()) return;
            int defLine = ZoneRange.FromLine + r.Start.iLine + 0;
            _collectRefs.AddReference(field, defLine,0, RefType.InternalField, _navObject.ID, ObjectType.Table);
        }

        //public void CollectRef_AccessByPermission(CodeRange range)
        //{
        //    var rangeText = range.GetRangeText(_navObject.tb);
        //    var elem = "AccessByPermission=";
        //    if (rangeText.IndexOf(elem) < 0) return;
        //    var lines = rangeText.SplitLines();
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        var line = lines[i];
        //        int j = line.IndexOf(elem);
        //        if (j < 0)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            InsertAccessByPermission(range, i, line);
        //        }

        //    }
        //}
        /// <summary>
        /// AccessByPermission=TableData 167=R;
        /// </summary>
        /// <param name="source"></param>
        /// <param name="r"></param>
        /// <param name="ZoneRange"></param>
        public void InsertRefAccessByPermission(string source, CodeRange r, CodeRange ZoneRange)
        {
            var tab = source.Trim().Split('=');
            if (tab.IsNullOrEmpty()) return;
            var field = tab[0];
            if (field.IsNullOrEmpty()) return;
            field = field.Replace("TableData", "");
            int defLine = ZoneRange.FromLine + r.Start.iLine + 0;
            int.TryParse(field, out int objID);
            if (objID > 0)
            {
                _collectRefs.AddReference(field, defLine,0, RefType.ExternalObject, objID, ObjectType.Table);
            }
        }

        //public void CollectRef_DataCaptionFields(CodeRange range)
        //{
        //    var rangeText = range.GetRangeText(_navObject.tb);
        //    var elem = "    DataCaptionFields=";
        //    if (rangeText.IndexOf(elem) < 0) return;
        //    var lines = rangeText.SplitLines();
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        var line = lines[i];
        //        int j = line.IndexOf(elem);
        //        if (j < 0)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            InsertRefDataCaptionFields(range, i, line);
        //            break;
        //        }

        //    }
        //}
        public void InsertRefDataCaptionFields(string source, CodeRange r, CodeRange ZoneRange)
        {
            source = source.RemoveIfEndWith(";");
            var tab = source.Trim().Split(',');
            if (tab.IsNullOrEmpty()) return;
            for (int k = 0; k < tab.Length; k++)
            {
                var field = tab[k];
                if (field.IsNullOrEmpty()) continue;
                int defLine = ZoneRange.FromLine + r.Start.iLine + k;
                _collectRefs.AddReference(field, defLine,0, RefType.InternalField, _navObject.ID, ObjectType.Table);
            }
        }

        //private void InsertRefDataCaptionFields(CodeRange range, int i, string line)
        //{
        //    var tab = line.Trim().Split(' ', '=', ',', ';');
        //    //if (tab.IsNullOrEmpty()) return;
        //    for (int k = 0; k < tab.Length; k++)
        //    {
        //        if (k == 0 || k == tab.Length - 1) continue;
        //        var field = tab[k];
        //        if (field.IsNullOrEmpty()) continue;
        //        int defLine = range.FromLine + i;
        //        _navObject.AddReference(field, defLine, RefType.InternalField, _navObject.ID, ObjectType.Table);
        //    }
        //}

        //public void CollectRef_LookUpDrillDownPages(CodeRange range, string typePage)
        //{
        //    var rangeText = range.GetRangeText(_navObject.tb);
        //    var elem = typePage;
        //    if (rangeText.IndexOf(elem) < 0) return;
        //    var lines = rangeText.SplitLines();
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        var line = lines[i];
        //        int j = line.IndexOf(elem);
        //        if (j < 0)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            InsertRefLookUpDrillDownPages(range, i, line);
        //            break;
        //        }

        //    }
        //}

        public void InsertRefLookUpDrillDownPages(string source, CodeRange r, CodeRange ZoneRange)
        {
            var pageDef = source;
            if (pageDef.IsNullOrEmpty()) return;
            int defLine = ZoneRange.FromLine + r.Start.iLine + 0;
            var strPageId = pageDef.Replace("Page", "");
            int objID = 0;
            int.TryParse(strPageId, out objID);
            _collectRefs.AddReference(pageDef, defLine,0, RefType.ExternalObject, objID, ObjectType.Page);
        }

        //public void CollectRef_SourceTable(CodeRange range)
        //{
        //    var rangeText = range.GetRangeText(_navObject.tb);
        //    var elem = "SourceTable=";
        //    if (rangeText.IndexOf(elem) < 0) return;
        //    var lines = rangeText.SplitLines();
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        var line = lines[i];
        //        int j = line.IndexOf(elem);
        //        if (j < 0)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            InsertRefSourceTable(range, i, line);
        //            break;
        //        }

        //    }
        //}

        public int InsertRefSourceTable(string source, CodeRange r, CodeRange ZoneRange)
        {
            //Table1226;
            var objDef = source;
            if (objDef.IsNullOrEmpty()) return 0;
            int defLine = ZoneRange.FromLine + r.Start.iLine + 0;
            var strPageId = objDef.Replace("Table", "");
            int objID = 0;
            int.TryParse(strPageId, out objID);
            _collectRefs.AddReference(objDef, defLine,0, RefType.ExternalObject, objID, ObjectType.Table);
            return objID;
        }
        public int InsertRefPagePartID(string source, CodeRange r, CodeRange ZoneRange)
        {
            var objDef = source;
            if (objDef.IsNullOrEmpty()) return 0;
            int defLine = ZoneRange.FromLine + r.Start.iLine + 0;
            var strPageId = objDef.Replace("Page", "");
            int PageID = 0;
            int.TryParse(strPageId, out PageID);
            //if (CreateRef)
            {
                _collectRefs.AddReference(objDef, defLine,0, RefType.ExternalObject, PageID, ObjectType.Page);
            }
            return PageID;
        }
        public int GetPageID_PagePartID(string source)
        {
            //var objDef = source.Trim();
            //var tab = objDef.Split('=');
            //if (tab.IsNullOrEmpty()) return 0;
            //if (tab.Length<2) return 0;

            var strPageId = source.Replace("Page", "").Replace(";","") ;
            int PageID = 0;
            int.TryParse(strPageId, out PageID);
            return PageID;
        }

        //public void CollectRef_Page_DataCaptionExpr(CodeRange range)
        //{
        //    var rangeText = range.GetRangeText(_navObject.tb);
        //    var elem = "SourceTable=";
        //    if (rangeText.IndexOf(elem) < 0) return;
        //    var lines = rangeText.SplitLines();
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        var line = lines[i];
        //        int j = line.IndexOf(elem);
        //        if (j < 0)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            var tab = line.Trim().Split(' ', '=', ',', ';');
        //            if (tab.IsNullOrEmpty()) return;
        //            for (int k = 0; k < tab.Length; k++)
        //            {
        //                if (k == 0 || k == tab.Length - 1) continue;
        //                var objDef = tab[k];
        //                if (objDef.IsNullOrEmpty()) continue;
        //                int defLine = range.FromLine + i;
        //                var strPageId = objDef.Replace("Table", "");
        //                int objID = 0;
        //                int.TryParse(strPageId, out objID);
        //                _navObject.AddReference(objDef, defLine, RefType.ExternalObject, objID, ObjectType.Table);
        //            }
        //            break;
        //        }

        //    }
        //}

        public void InsertRefPermissions(string source, CodeRange r, CodeRange ZoneRange)
        {
            var lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                var tab = line.Split('=', ' ');
                if (tab == null) continue;
                if (tab.Length < 2) continue;
                string strType = tab[0];
                string strID = tab[1];
                int defLine = ZoneRange.FromLine + r.Start.iLine + i;
                _collectRefs.AddReference("", defLine,0, RefType.ExternalObject, Convert.ToInt32(strID), ObjectType.Table);
            }
        }
        public void InsertRef_GlobalVariable(string expr, CodeRange r, CodeRange ZoneRange)
        {
            var objDef = expr;
            if (objDef.IsNullOrEmpty()) return;
            int defLine = ZoneRange.FromLine + r.Start.iLine + 0;
            var p = _navObject.GlobalVariables.Where(c => c.Name == expr).FirstOrDefault();
            if (p != null)
            {
                _collectRefs.AddReference(expr, defLine,0, RefType.GlobalVariable);
            }

            //var strPageId = objDef.Replace("Table", "");
            //int objID = 0;
            //int.TryParse(strPageId, out objID);
            //_navObject.AddReference(objDef, defLine, RefType.ExternalObject, objID, ObjectType.Table);
        }
        public void InsertRefCardPageID(string expr, CodeRange r, CodeRange ZoneRange)
        {
            var objDef = expr;
            if (objDef.IsNullOrEmpty()) return;
            int defLine = ZoneRange.FromLine + r.Start.iLine + 0;
            var objID = _navObject.NavProject.GetObjectID(expr, ObjectType.Page);
            _collectRefs.AddReference(expr, defLine,0, RefType.ExternalObject, objID, ObjectType.Page);
        }
        public int InsertRefRunObject(string expr, CodeRange r, CodeRange ZoneRange)
        {
            var objDef = expr.RemoveIfEndWith(";");
            if (objDef.IsNullOrEmpty()) return 0;
            int defLine = ZoneRange.FromLine + r.Start.iLine + 0;
            var obj = _navObject.NavProject.GetRunObject(objDef);
            int objID = 0;ObjectType runObjectType= ObjectType.Table;
            if (obj != null)
            {
                objID = obj.ID;
                runObjectType = obj.Type;
            }
            if (objID > 0)
            {
                _collectRefs.AddReference(expr, defLine,0, RefType.ExternalObject, objID, obj.Type);
                if (runObjectType == ObjectType.Page)
                {
                    return ((NavObject)obj).SourceTableID;
                }
            }
            return 0;
        }
        public void InsertRefSourceTableView(string source, CodeRange r, CodeRange ZoneRange,
            int TableID, bool isXMLPort = false)
        {
            source = source.RemoveIfEndWith(";");
            var tab = source.SplitLines();
            //if (tab.IsNullOrEmpty()) return;
            bool bWhere = false;
            for (int k = 0; k < tab.Length; k++)
            {
                //if (k == 0 || k == tab.Length - 1) continue;
                var line = tab[k].Trim();
                if (line.IsNullOrEmpty()) continue;
                int defLine = ZoneRange.FromLine + r.Start.iLine + k;

                if (line.StartsWith("SORTING"))
                {
                    var fieldInt = line.Replace("SORTING", "");
                    int firstParan = fieldInt.IndexOf("(");
                    int lastParan = fieldInt.IndexOf(")");
                    var field = fieldInt.Substring(firstParan + 1, lastParan - firstParan - 1);
                    var tab2 = field.Split(',');
                    foreach (var f in tab2)
                    {
                        ProcessFieldTableView(TableID, defLine, f);
                    }
                }
                if (line.StartsWith("WHERE"))
                {
                    bWhere = true;
                }
                if (bWhere)
                {
                    var expr = line.Replace("WHERE", "").Trim();
                    expr = expr.RemoveIfEndWith(")");
                    expr = expr.RemoveIfStartWith("(");


                    //var tab2 = fieldInt.Split('=');
                    int posEgale = expr.IndexOfAvoidQuotes('=');
                    if (posEgale > 0)
                    {
                        //var field = tab2[0];
                        var fieldExt = expr.Substring(0, posEgale);
                        if (isXMLPort)
                        {
                            var fieldId = Convert.ToInt32(fieldExt.Replace("Field", ""));
                            var obj = _navObject.NavProject.GetObject(TableID, ObjectType.Table);
                            if (obj != null)
                            {
                                var fieldList = ((NavObject)obj).FieldList;
                                var f = fieldList.Where(c => c.FieldID == fieldId).FirstOrDefault();
                                if (f != null)
                                {
                                    _collectRefs.AddReference(f.FieldName, defLine,0, RefType.ExternalField, TableID, ObjectType.Table);

                                }
                            }
                        }
                        else
                        {
                            _collectRefs.AddReference(fieldExt, defLine,0, RefType.ExternalField, TableID, ObjectType.Table);
                        }


                        //FILTER CONST Zone
                        var fieldInt = expr.Substring(posEgale + 1);
                        _collectRefs.ProcessFilterFieldConst(defLine, TableID, fieldExt, fieldInt);
                    }
                }
            }
        }

        private void ProcessFieldTableView(int TableID, int defLine, string f)
        {
            if (Regex.IsMatch(f, @"Field\d+"))
            {
                var fieldId = f.Replace("Field", "");
                var obj = _navObject.NavProject.GetObject(TableID, ObjectType.Table);
                if (obj != null)
                {
                    var objFields = ((NavObject)obj).FieldList;
                    if (objFields != null)
                    {
                        var foundfield = objFields.Where(c => c.FieldID == Convert.ToInt32(fieldId)).FirstOrDefault();
                        if (foundfield != null)
                        {
                            _collectRefs.AddReference(foundfield.FieldName, defLine,0, RefType.ExternalField, TableID, ObjectType.Table);
                        }
                    }
                }
            }
            else
            {
                _collectRefs.AddReference(f, defLine,0, RefType.ExternalField, TableID, ObjectType.Table);
            }
        }

        public void InsertRefRunPageLink(string source, CodeRange r, CodeRange ZoneRange, int TableID)
        {
            source = source.FormatEndOfElement();
            var tab1 = source.SplitLines();
            //if (tab.IsNullOrEmpty()) return;
            //bool bWhere = false;
            for (int k = 0; k < tab1.Length; k++)
            {
                //if (k == 0 || k == tab.Length - 1) continue;
                var line = tab1[k].Trim();
                if (line.IsNullOrEmpty()) continue;
                int defLine = ZoneRange.FromLine + r.Start.iLine + k;

                //if (bWhere)
                //var expr = m.Name.Trim();
                if (line.StartsWith("("))
                {
                    line = line.ReplaceFirstOccurrence("(", "");
                }
                //var tab2 = line.Split('=');
                //if (tab2.IsNotNullOrEmpty())
                int posEgale = line.IndexOfAvoidQuotes('=');
                if (posEgale >= 0)
                {
                    //TODO Validate here, la table depent de l'objet ou est est
                    //Internal field of Table or Rec Table
                    //var fieldExt = tab2[0];
                    var fieldExt = line.Substring(0, posEgale);
                    _collectRefs.AddReference(fieldExt, defLine,0, RefType.ExternalField, TableID, ObjectType.Table);


                    var fieldInt = line.Substring(posEgale + 1);
                    _collectRefs.ProcessFilterFieldConst(defLine, TableID, fieldExt, fieldInt);
                    //if (tab2.Count() > 1)
                    //{
                    //    var fieldInt = tab2[1];
                    //    //if (fieldInt.Contains("FIELD"))
                    //    //{
                    //    //    int firstParan = fieldInt.FirstOccurrence("(");
                    //    //    int lastParan = fieldInt.LastOccurrence(")");
                    //    //    var field = fieldInt.Substring(firstParan + 1, lastParan - firstParan - 1);
                    //    //    //_collectRefs.AddReference(field, defLine, RefType.InternalField, _navObject.SourceTableID, ObjectType.Table);
                    //    //}
                        
                    //}
                }

            }
        }
        public void InsertRefDataItemLink_Report(string source, CodeRange r, CodeRange ZoneRange, int TableID_Fils, int TableID_Parent)
        {
            source = source.RemoveIfEndWith(";").RemoveIfEndWith("}");
            var tab1 = source.SplitLines();
            for (int k = 0; k < tab1.Length; k++)
            {
                var line = tab1[k].Trim();
                if (line.IsNullOrEmpty()) continue;
                int defLine = ZoneRange.FromLine + r.Start.iLine + k;

                if (line.StartsWith("("))
                {
                    line = line.ReplaceFirstOccurrence("(", "");
                }
                //var tab2 = line.Split('=');
                //if (tab2.IsNotNullOrEmpty())
                //{
                int posEgale = line.IndexOfAvoidQuotes('=');
                if (posEgale >= 0)
                {
                    //TODO Validate here, la table depent de l'objet ou est est
                    //Internal field of Table or Rec Table
                    //var fieldExt = tab2[0];
                    var fieldExt = line.Substring(0, posEgale);
                    _collectRefs.AddReference(fieldExt, defLine,0, RefType.ExternalField, TableID_Fils, ObjectType.Table);


                    var fieldInt = line.Substring(posEgale + 1);
                    _collectRefs.ProcessFilterFieldConst(defLine, TableID_Fils, fieldExt, fieldInt);

                    //if (tab2.Count() > 1)
                    //{
                    //    var fieldInt = tab2[1];
                    //    if (fieldInt.Contains("FIELD"))
                    //    {
                    //        int firstParan = fieldInt.IndexOf("(");
                    //        int lastParan = fieldInt.IndexOf(")");
                    //        var field = fieldInt.Substring(firstParan + 1, lastParan - firstParan - 1);
                    //        //_collectRefs.AddReference(field, defLine, RefType.InternalField, TableID_Parent, ObjectType.Table);
                    //        _collectRefs.ProcessFilterFieldConst(defLine, TableID_Fils, fieldExt, fieldInt);
                    //    }
                    //}
                }

            }
        }
        public void InsertRefElementLinkFields(string source, CodeRange r, CodeRange ZoneRange, int TableID_Fils,int TableID_Parent)
        {
            source = source.RemoveIfEndWith(";").RemoveIfEndWith("}");
            var tab1 = source.SplitLines();
            for (int k = 0; k < tab1.Length; k++)
            {
                var line = tab1[k].Trim();
                if (line.IsNullOrEmpty()) continue;
                int defLine = ZoneRange.FromLine + r.Start.iLine + k;

                if (line.StartsWith("("))
                {
                    line = line.ReplaceFirstOccurrence("(", "");
                }
                //var tab2 = line.Split('=');
                //if (tab2.IsNotNullOrEmpty())
                //{
                int posEgale = line.IndexOfAvoidQuotes('=');
                if (posEgale >= 0)
                {
                    //TODO Validate here, la table depent de l'objet ou est est
                    //Internal field of Table or Rec Table
                    //var fieldId = tab2[0].Replace("Field","");
                    //var fieldId = line.Substring(0, posEgale).Replace("Field", ""); 
                    //var obj = _navObject.NavProject.GetObject(TableID_Fils, ObjectType.Table);
                    //if (obj != null)
                    //{
                    //    var objFields = ((NavObject)obj).FieldList;
                    //    if (objFields != null)
                    //    {
                    //        var f = objFields.Where(c => c.FieldID == Convert.ToInt32(fieldId)).FirstOrDefault();
                    //        if (f != null)
                    //        {
                    //            _collectRefs.AddReference(f.FieldName, defLine, RefType.ExternalField, TableID_Fils, ObjectType.Table);
                    //        }
                    //    }
                    //}
                    var field1 = line.Substring(0, posEgale);
                    ProcessFieldTableView(TableID_Fils, defLine, field1);

                    //if (tab2.Count() > 1)
                    {
                        //var fieldInt = tab2[1];
                        var fieldInt = line.Substring(posEgale + 1);
                        if (fieldInt.Contains("FIELD"))
                        {
                            int firstParan = fieldInt.IndexOf("(");
                            int lastParan = fieldInt.IndexOf(")");
                            var field = fieldInt.Substring(firstParan + 1, lastParan - firstParan - 1);

                            ProcessFieldTableView(TableID_Parent, defLine, field);

                            //var IdField= field.Replace("Field", "");
                            //var obj1 = _navObject.NavProject.GetObject(TableID_Parent, ObjectType.Table);
                            //if (obj1 != null)
                            //{
                            //    var objFields = ((NavObject)obj1).FieldList;
                            //    var f = objFields.Where(c => c.FieldID == Convert.ToInt32(IdField)).FirstOrDefault();
                            //    if (f != null)
                            //    {
                            //        _collectRefs.AddReference(f.FieldName, defLine, RefType.ExternalField, TableID_Parent, ObjectType.Table);
                            //    }
                            //}
                        }
                    }
                }
            }
        }

        public ObjectType InsertRefRunObjectType(string expr, CodeRange r, CodeRange rangeZone)
        {
            expr = expr.FormatEndOfElement();
            return new Variable().GetTypeFromString(expr);
        }

        public void InsertRefRunObjectID(string expr, CodeRange r, CodeRange rangeZone, ObjectType menuSuite_RunObjectType)
        {
            string strIdObj = expr.FormatEndOfElement();
            int idObj = 0;
            int.TryParse(strIdObj, out idObj);
            int defLine = rangeZone.FromLine + r.Start.iLine + 0;
            if (idObj > 0)
            {
                _collectRefs.AddReference(strIdObj, defLine,0, RefType.ExternalObject, idObj, menuSuite_RunObjectType);
            }
        }
    }
}
