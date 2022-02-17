using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NavCodeViewer.Business
{
    public class NavObjectCollectRefs
    {
        
        TableRelationRefMgt tableRelationRefMgt;
        OthersRefMgt othersRefMgt;
        CalcFormulaRefMgt calcFormulaRefMgt;
        CodeRefMgt codeRefMgt;
        
        
        private DataItem actualDataItem; 
        private DataItem actualElementItem;
        private DataItem actualQueryColumnFilter;
        private DataItem actualTablePart;

        private int activeTableIDForRunObject = 0;
        private bool RunPageView_IsCollect;
        private bool RunPageLink_IsCollect;
        private CodeRange activeRangeForRunPageView = null;
        private string activeExprForRunObjectRunPageView = "";
        private CodeRange activeRangeForRunPageLink = null;
        private string activeExprForRunObjectRunPageLink = "";
        private ObjectType menuSuite_RunObjectType = ObjectType.Page;
        protected NavObject navObj;

        public NavObjectCollectRefs(NavObject navObject1)
        {
            navObj = navObject1;
            tableRelationRefMgt = new TableRelationRefMgt(navObj,this);
            othersRefMgt = new OthersRefMgt(navObj,this);
            calcFormulaRefMgt = new CalcFormulaRefMgt(navObj,this);
            codeRefMgt = new CodeRefMgt(navObj,this);
        }
        private Reference CreateRef()
        {
            return new Reference
            {
                RefBy_ObjetID = navObj.ID,
                RefBy_ObjetType = navObj.Type
            };
        }
        public void CollectReferences()
        {
            if (navObj.ObjectTextSource.IsNullOrEmpty())
            {
                throw new Exception("ObjectTextSource is null");
            }
            if (navObj.Type == ObjectType.XmlPort && navObj.ID == 1)
            {
                int p = 0;
            }
            //ObjectTextSource = objSource;

            //Objets définis comme parametres de fonctions
            CollectObjectsRefAsFunctionParameter();

            //Objets définis comme variables globales
            CollectObjectsRefAsGlobalVars();

            //objets définis comme variables locales
            CollectObjectsRefAsLocalVars();

            FindReferencesNotInCode();

            FindReferences_SourceExpr();

            codeRefMgt.FindReferencesInCode();
        }
        private void FindReferencesNotInCode()
        {

            if(navObj.Type== ObjectType.XmlPort && navObj.ID == 50083)
            {
                var p = 0;
            }
            for (int index = 0; index < navObj.OthersPlaces.Count; index++)
            {
                var range = navObj.OthersPlaces[index];

                if (range.RangeType == TypeOfCodeRange.PropertiesDef ||
                    range.RangeType == TypeOfCodeRange.FieldsDef ||
                    range.RangeType == TypeOfCodeRange.ControlsDef ||
                    range.RangeType == TypeOfCodeRange.MenuNodesDef ||
                    range.RangeType == TypeOfCodeRange.RequestPage)
                {
                    CollectRef_NonCodeRange(range);
                }
                if (range.RangeType == TypeOfCodeRange.KeysDef)
                {
                    othersRefMgt.CollectRef_KEYS(range);
                }
                if (range.RangeType == TypeOfCodeRange.FieldGroupDef)
                {
                    othersRefMgt.CollectRef_FIELDGROUPS(range);
                }
                if (range.RangeType == TypeOfCodeRange.DataSetDef)
                {
                    CollectRef_DataItems(range);
                }
                if (range.RangeType == TypeOfCodeRange.ElementsDef)
                {
                    CollectRef_ElementsItems(range);
                }
            }
        }
        private void FindReferences_SourceExpr()
        {
            for (int index = 0; index < navObj.PlacesOfSourceExpr.Count; index++)
            {
                var range = navObj.PlacesOfSourceExpr[index];
                var rangeText = range.GetRangeText(navObj.Tb).FormatEndOfElement();

                codeRefMgt. FindReferenceInSourceExpr(rangeText,range, 0);                
            }
        }
        private void CollectRef_NonCodeRange(CodeRange range)
        {
            var rangeText = range.GetRangeText(navObj.Tb);
            var lines = rangeText.SplitLines();
            var start = new Place(0, 0);
            var end = new Place(0, 0);
            NonCodeReferenceType typeRef = NonCodeReferenceType.None;
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim Here
                var line = lines[i];

                if (navObj.Type == ObjectType.Page)
                {
                    if (line.Contains(";Part "))
                    {
                        var newDataItem = new DataItem();
                        var tab = line.Split(';');
                        actualTablePart = navObj.PageParts.Where(c => c.IDRef == tab[0].FormatDateItemID()).FirstOrDefault();
                    }
                }

                if (navObj.Type == ObjectType.Table && navObj.ID == 36)
                {
                    int p = 0;
                }
                if(line.Contains(@"Location WHERE (Use As In-Transit=CONST(No),"))
                {
                    int p = 0;
                }

                SearchAllReferences(ref start, ref typeRef, i, line);

                if (typeRef != NonCodeReferenceType.None)
                {
                    if (line.Length >= start.iChar)
                    {
                        int k1 = -1, k2 = -1; int k = -1;
                         k1 = line.IndexOfAvoidQuotes(';', start.iChar);
                        if (k1 >= 0)
                        {
                            k = k1;
                        }
                        k2 = line.IndexOfAvoidQuotes('}', start.iChar);
                        if (k2 >= 0)
                        {
                            if (k1 >= 0)
                            {
                                k = Math.Min(k1, k2);
                            }
                            else
                            {
                                k = k2;
                            }
                        }
                        //int k = line.IndexOfAny(new char[] { '}', ';' }, start.iChar);
                        if (k >= 0)
                        {
                            end = new Place(k, i);
                            var r = range.CreateSubRange(start, end);
                            var expr = r.GetRangeText(lines);

                            InsertNonCodeReferences(range, typeRef, r, expr);

                            typeRef = NonCodeReferenceType.None;
                        }
                    }
                }
            }
        }
        private static void SearchAllReferences(ref Place start, ref NonCodeReferenceType typeRef, int i, string line)
        {
            SearchForReference(ref start, ref typeRef, i, line, "TableRelation=", NonCodeReferenceType.TableRelation);
            SearchForReference(ref start, ref typeRef, i, line, "Permissions=", NonCodeReferenceType.Permissions);
            SearchForReference(ref start, ref typeRef, i, line, "AltSearchField=", NonCodeReferenceType.AltSearchField);
            SearchForReference(ref start, ref typeRef, i, line, "DataCaptionFields=", NonCodeReferenceType.DataCaptionFields);
            SearchForReference(ref start, ref typeRef, i, line, "LookupPageID=", NonCodeReferenceType.LookUpPage);
            SearchForReference(ref start, ref typeRef, i, line, "DrillDownPageID=", NonCodeReferenceType.DrillDownPage);
            SearchForReference(ref start, ref typeRef, i, line, "AccessByPermission=", NonCodeReferenceType.AccessByPermission);
            SearchForReference(ref start, ref typeRef, i, line, "SourceTable=", NonCodeReferenceType.SourceTable);
            SearchForReference(ref start, ref typeRef, i, line, "CalcFormula=", NonCodeReferenceType.CalcFormula);

            //Page
            SearchForReference(ref start, ref typeRef, i, line, "DataCaptionExpr=", NonCodeReferenceType.CanHaveGlobalVariable);
            SearchForReference(ref start, ref typeRef, i, line, "AutoFormatExpr=", NonCodeReferenceType.CanHaveGlobalVariable);
            SearchForReference(ref start, ref typeRef, i, line, "Visible=", NonCodeReferenceType.CanHaveGlobalVariable);
            SearchForReference(ref start, ref typeRef, i, line, "Enabled=", NonCodeReferenceType.CanHaveGlobalVariable);
            SearchForReference(ref start, ref typeRef, i, line, "Editable=", NonCodeReferenceType.CanHaveGlobalVariable);
            SearchForReference(ref start, ref typeRef, i, line, "HideValue=", NonCodeReferenceType.CanHaveGlobalVariable);
            SearchForReference(ref start, ref typeRef, i, line, "StyleExpr=", NonCodeReferenceType.CanHaveGlobalVariable);
            SearchForReference(ref start, ref typeRef, i, line, "QuickEntry=", NonCodeReferenceType.CanHaveGlobalVariable);
            SearchForReference(ref start, ref typeRef, i, line, "SourceTableView=", NonCodeReferenceType.SourceTableView);
            SearchForReference(ref start, ref typeRef, i, line, "CardPageID=", NonCodeReferenceType.CardPageID);
            SearchForReference(ref start, ref typeRef, i, line, "RunObject=", NonCodeReferenceType.RunObject);
            SearchForReference(ref start, ref typeRef, i, line, "RunPageView=", NonCodeReferenceType.RunPageView);
            SearchForReference(ref start, ref typeRef, i, line, "RunPageLink=", NonCodeReferenceType.RunPageLink);
            SearchForReference(ref start, ref typeRef, i, line, "PagePartID=", NonCodeReferenceType.PagePartID);
            SearchForReference(ref start, ref typeRef, i, line, "SubPageLink=", NonCodeReferenceType.SubPageLink);
            SearchForReference(ref start, ref typeRef, i, line, "SourceExpr=", NonCodeReferenceType.SourceExpr);
            //SearchForReference(ref start, ref typeRef, i, line, "SourceField=", NonCodeReferenceType.SourceField);

            //report            
            SearchForReference(ref start, ref typeRef, i, line, "DataItemTable=", NonCodeReferenceType.DataItemTable);
            
            //Menusuite
            SearchForReference(ref start, ref typeRef, i, line, "RunObjectType=", NonCodeReferenceType.RunObjectType);
            SearchForReference(ref start, ref typeRef, i, line, "RunObjectID=", NonCodeReferenceType.RunObjectID);
        }
        private void CollectRef_DataItems(CodeRange rangeZone)
        {
            var rangeText = rangeZone.GetRangeText(navObj.Tb);
            var lines = rangeText.SplitLines();
            var start = new Place(0, 0);
            var end = new Place(0, 0);
            NonCodeReferenceType typeRef = NonCodeReferenceType.None;
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim Here
                var line = lines[i];

                if (line.Trim().IsNullOrEmpty()) continue;

                //SearchAllReferences(ref start, ref typeRef, i, line);
                if (line.Contains(";DataItem"))
                {
                    var newDataItem = new DataItem();
                    var tab = line.Split(';');
                    actualDataItem = navObj.DataItems.Where(c => c.IDRef == tab[0].FormatDateItemID()).FirstOrDefault();
                }
                if(navObj.Type== ObjectType.Query)
                {
                    if (line.Contains(";Filter")|| line.Contains(";Column"))
                    {
                        var newDataItem = new DataItem();
                        var tab = line.Split(';');
                        actualQueryColumnFilter = navObj.QueryColumnAndFilters.Where(c => c.IDRef == tab[0].FormatDateItemID()).FirstOrDefault();
                    }
                }
                
                if (line.Contains("DataItemTable="))
                {
                    var tab = line.FormatEndOfElement().Split('=');
                    string strTab = ""; int idTable = 0;
                    if (tab.Length > 1)
                    {
                        strTab = tab[1];
                        int.TryParse(strTab.Replace("Table",""), out idTable);
                        AddReference(idTable.ToString(), rangeZone.FromLine + i,0, RefType.ExternalObject, idTable, ObjectType.Table);
                    }
                }
                SearchForReference(ref start, ref typeRef, i, line, "DataItemTableView=", NonCodeReferenceType.DataItemTableView);
                SearchForReference(ref start, ref typeRef, i, line, "DataItemLinkReference=", NonCodeReferenceType.DataItemLinkReference);
                SearchForReference(ref start, ref typeRef, i, line, "DataItemLink=", NonCodeReferenceType.DataItemLink);
                SearchForReference(ref start, ref typeRef, i, line, "ReqFilterFields=", NonCodeReferenceType.ReqFilterFields);
                SearchForReference(ref start, ref typeRef, i, line, "CalcFields=", NonCodeReferenceType.CalcFields);
                SearchForReference(ref start, ref typeRef, i, line, "SourceExpr=", NonCodeReferenceType.SourceExpr);
                
                SearchForReference(ref start, ref typeRef, i, line, "ColumnFilter=", NonCodeReferenceType.ColumnFilter);
                SearchForReference(ref start, ref typeRef, i, line, "DataItemTableFilter=", NonCodeReferenceType.DataItemTableFilter);
                SearchForReference(ref start, ref typeRef, i, line, "DataSource=", NonCodeReferenceType.DataSource);


                if (typeRef != NonCodeReferenceType.None)
                {
                    int k = line.IndexOfAny(new char[] { '}', ';' }, start.iChar);
                    if (k >= 0)
                    {
                        end = new Place(k, i);
                        var r = rangeZone.CreateSubRange(start, end);
                        var expr = r.GetRangeText(lines);

                        if (typeRef == NonCodeReferenceType.DataItemTableView)
                        {
                            var dernItem = actualDataItem;
                            if (dernItem != null)
                            {
                                if (dernItem.DataItemTable == 0)
                                {
                                    throw new Exception("Error on SourceTable");
                                }
                                othersRefMgt.InsertRefSourceTableView(expr, r, rangeZone, dernItem.DataItemTable);
                            }
                        }
                        if (typeRef == NonCodeReferenceType.DataItemLinkReference)
                        {

                        }
                        if (typeRef == NonCodeReferenceType.CalcFields||
                            typeRef == NonCodeReferenceType.ReqFilterFields)
                        {
                            var dernItem = actualDataItem;
                            if (dernItem != null)
                            {
                                if (expr != "")
                                {
                                    expr = expr.FormatEndOfElement();
                                    var tab = expr.Split(',');
                                    foreach(var f in tab)
                                    {
                                        int refLine = r.Start.iLine + rangeZone.FromLine;
                                        AddReference(f, refLine,0, RefType.ExternalField,
                                           dernItem.DataItemTable, ObjectType.Table);
                                    }
                                    //dernItem.DataItemLinkReference = expr;
                                }
                            }
                        }
                        if (typeRef == NonCodeReferenceType.DataItemLink)
                        {
                            if (navObj.Type == ObjectType.Report)
                            {
                                ProcessDataItemLink_Report(rangeZone, r, expr);
                            }
                            if (navObj.Type == ObjectType.Query)
                            {
                                ProcessDataItemLink_Query(rangeZone, r, expr);
                            }
                        }
                        if (typeRef == NonCodeReferenceType.DataSource)
                        {
                            //
                            ProcessDataSource_Query(rangeZone, r, expr);
                        }
                        if (typeRef == NonCodeReferenceType.ColumnFilter)
                        {
                            ProcessColumnFilter_Query(rangeZone, r, expr);
                        }
                        if (typeRef == NonCodeReferenceType.DataItemTableFilter)
                        {
                            ProcessDataItemTableFilter_Query(rangeZone, r, expr);
                        }
                        if (typeRef == NonCodeReferenceType.SourceExpr)
                        {
                            //Geres dans la liste
                        }
                        if (typeRef == NonCodeReferenceType.SourceField)
                        {
                            ProcessSourceField_Xmlport(rangeZone, r, expr);
                        }

                        typeRef = NonCodeReferenceType.None;
                    }
                }
            }
        }

        private void ProcessDataItemLink_Report(CodeRange rangeZone, CodeRange r, string expr)
        {
            var itemFils = actualDataItem;
            if (itemFils != null)
            {
                var IdItemTableParent = 0;
                if (itemFils.DataItemTable == 0)
                {
                    throw new Exception("Error on SourceTable");
                }
                if (itemFils.DataItemLinkReference.IsNotNullOrEmpty())
                {
                    var parent = navObj.DataItems.Where(c => c.Name == itemFils.DataItemLinkReference).LastOrDefault();
                    if (parent != null)
                    {
                        IdItemTableParent = parent.DataItemTable;
                    }
                    else
                    {
                        var itemParent = navObj.DataItems.Where(c => c.Niveau == itemFils.Niveau - 1).LastOrDefault();
                        if (itemParent != null)
                        {
                            IdItemTableParent = itemParent.DataItemTable;
                        }
                    }
                }
                else
                {
                    var itemParent = navObj.DataItems.Where(c => c.Niveau < itemFils.Niveau && c.DataItemTable!= 2000000026).LastOrDefault();
                    if (itemParent != null)
                    {
                        IdItemTableParent = itemParent.DataItemTable;
                    }
                }
                othersRefMgt.InsertRefDataItemLink_Report(expr, r, rangeZone, itemFils.DataItemTable, IdItemTableParent);
            }
        }
        private void ProcessDataItemLink_Query(CodeRange rangeZone, CodeRange r, string expr)
        {
            var itemFils = actualDataItem;
            if (itemFils != null)
            {
                //var IdItemTableParent = 0;
                if (itemFils.DataItemTable == 0)
                {
                    throw new Exception("Error on SourceTable");
                }
                var lines = expr.SplitLines(); for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var iLine = rangeZone.FromLine + r.FromLine + i;
                    if (line.Trim() == "") return;
                    line = line.FormatEndOfElement();
                    int PosEgale = line.IndexOfAvoidQuotes('=');
                    if (PosEgale >= 0)
                    {
                        string valGauche = line.Substring(0, PosEgale).Trim();
                        AddReference(valGauche, iLine,0, RefType.ExternalField, itemFils.DataItemTable, ObjectType.Table);

                        string valDroite = line.Substring(PosEgale + 1).Trim();
                        int PosPoint = valDroite.IndexOfAvoidQuotes('.');
                        if (PosEgale >= 0)
                        {
                            string valTable = valDroite.Substring(0, PosPoint).Trim().RemoveQuotes();
                            var variableTable = navObj.GlobalVariables.Where(c => c.Name == valTable).FirstOrDefault();
                            if (variableTable != null)
                            {
                                AddReference(valTable, iLine,0, RefType.ExternalObject, variableTable.ID, ObjectType.Table);

                                string valField = valDroite.Substring(PosPoint + 1).Trim().RemoveQuotes(); 
                                valField = valField.RemoveIfEndWith(",");
                                AddReference(valField, iLine,0, RefType.ExternalField, variableTable.ID, ObjectType.Table);
                            }
                        }
                    }
                }
            }
        }
        private void ProcessDataSource_Query(CodeRange rangeZone, CodeRange r, string expr)
        {
            var itemFils = actualQueryColumnFilter;
            if (itemFils != null)
            {
                //var IdItemTableParent = 0;
                if (itemFils.DataItemTable == 0)
                {
                    throw new Exception("Error on SourceTable");
                }
                //var lines = expr.SplitLines(); for (int i = 0; i < lines.Length; i++)
                {
                    //string valGauche = line.Substring(0, PosEgale).Trim();
                    AddReference(expr.Trim(), rangeZone.FromLine+r.FromLine,0, RefType.ExternalField, itemFils.DataItemTable, ObjectType.Table);
                }
            }
        }
        private void ProcessSourceField_Xmlport(CodeRange rangeZone, CodeRange r, string expr)
        {
            int PosPoint = expr.IndexOf("::");
            if (PosPoint >= 0)
            {
                string valTable = expr.Substring(0, PosPoint).Trim().RemoveQuotes();
                var vartab = navObj.NavProject.GetObject(valTable, ObjectType.Table);
                if (vartab != null)
                {
                    AddReference(valTable, rangeZone.FromLine + r.FromLine,0, RefType.ExternalObject, vartab.ID, ObjectType.Table);
                    
                    string valField = expr.Substring(PosPoint + 2).Trim().RemoveQuotes();
                    AddReference(valField, rangeZone.FromLine + r.FromLine,0, RefType.ExternalField, vartab.ID, ObjectType.Table);
                }
            } 
        }
        private void ProcessColumnFilter_Query(CodeRange rangeZone, CodeRange r, string expr)
        {
            var itemFils = actualQueryColumnFilter;
            if (itemFils != null)
            {
                if (actualQueryColumnFilter.Name.IsNullOrEmpty())
                {
                    return;
                    //throw new Exception("Error on SourceTable");
                }
                var lines = expr.SplitLines(); for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (line.Trim() == "") return;
                    line = line.FormatEndOfElement();
                    int PosEgale = line.IndexOfAvoidQuotes('=');
                    if (PosEgale >= 0)
                    {
                        //string valGauche = line.Substring(0, PosEgale).Trim();
                        //valGauche = valGauche.Replace(". ", "_").Replace(".", "_").Replace(" ", "_");
                        int iLine = rangeZone.FromLine + r.FromLine + i;
                        AddReference(actualQueryColumnFilter.Name, iLine,0, RefType.ExternalField, actualQueryColumnFilter.DataItemTable, ObjectType.Table);

                        //string valDroite = line.Substring(PosEgale + 1).Trim();
                        //int PosPoint = valDroite.IndexOfAvoidQuotes('.');
                        //if (PosEgale >= 0)
                        //{
                        //    string valTable = valDroite.Substring(0, PosPoint).Trim().RemoveQuotes();
                        //    var variableTable = navObj.GlobalVariables.Where(c => c.Name == valTable).FirstOrDefault();
                        //    if (variableTable != null)
                        //    {
                        //        AddReference(valTable, rangeZone.FromLine + i, RefType.ExternalObject, variableTable.ID, ObjectType.Table);

                        //        string valField = valDroite.Substring(PosPoint + 1).Trim().RemoveQuotes();
                        //        valField = valField.RemoveIfEndWith(",");
                        //        AddReference(valField, rangeZone.FromLine + i, RefType.ExternalField, variableTable.ID, ObjectType.Table);
                        //    }
                        //}
                    }
                }
            }
        }
        private void ProcessDataItemTableFilter_Query(CodeRange rangeZone, CodeRange r, string expr)
        {
            var itemFils = actualDataItem;
            if (itemFils != null)
            {
                if (itemFils.DataItemTable == 0)
                {
                    throw new Exception("Error on SourceTable");
                }
                var lines = expr.SplitLines(); for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (line.Trim() == "") return;
                    line = line.FormatEndOfElement();
                    int PosEgale = line.IndexOfAvoidQuotes('=');
                    if (PosEgale >= 0)
                    {
                        string valGauche = line.Substring(0, PosEgale).Trim();
                        //valGauche = valGauche.Replace(". ", "_").Replace(".", "_").Replace(" ", "_");
                        AddReference(valGauche, rangeZone.FromLine+r.FromLine + i,0, RefType.ExternalField, itemFils.DataItemTable, ObjectType.Table);

                        //string valDroite = line.Substring(PosEgale + 1).Trim();
                        //int PosPoint = valDroite.IndexOfAvoidQuotes('.');
                        //if (PosEgale >= 0)
                        //{
                        //    string valTable = valDroite.Substring(0, PosPoint).Trim().RemoveQuotes();
                        //    var variableTable = navObj.GlobalVariables.Where(c => c.Name == valTable).FirstOrDefault();
                        //    if (variableTable != null)
                        //    {
                        //        AddReference(valTable, rangeZone.FromLine + i, RefType.ExternalObject, variableTable.ID, ObjectType.Table);

                        //        string valField = valDroite.Substring(PosPoint + 1).Trim().RemoveQuotes();
                        //        valField = valField.RemoveIfEndWith(",");
                        //        AddReference(valField, rangeZone.FromLine + i, RefType.ExternalField, variableTable.ID, ObjectType.Table);
                        //    }
                        //}
                    }
                }
            }
        }

        private void CollectRef_ElementsItems(CodeRange rangeZone)
        {
            var rangeText = rangeZone.GetRangeText(navObj.Tb);
            var lines = rangeText.SplitLines();
            var start = new Place(0, 0);
            var end = new Place(0, 0);
            NonCodeReferenceType typeRef = NonCodeReferenceType.None;
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim Here
                var line = lines[i];

                //if (line.Trim().IsNullOrEmpty()) continue;

                //SearchAllReferences(ref start, ref typeRef, i, line);
                if (line.Contains(";Element"))
                {
                    var tab = line.Split(';');
                    actualElementItem = navObj.ElementsItems.Where(c => c.IDRef == tab[0]).FirstOrDefault();

                    //var newDataItem = new DataItem();
                    //var tab = line.Split(';');
                    //string strNiveau = ""; int idNiveau = 0;
                    //if (tab.Length > 1)
                    //{
                    //    strNiveau = tab[1];
                    //    int.TryParse(strNiveau, out idNiveau);
                    //    newDataItem.Niveau = idNiveau;
                    //}
                    //if (tab.Length > 2)
                    //{
                    //    var itemName = tab[2].Trim();
                    //    if (itemName.IsNotNullOrEmpty())
                    //    {
                    //        newDataItem.Name = itemName;
                    //    }
                    //}
                    //ElementsItems.Add(newDataItem);
                }
                if (line.Contains("SourceTable="))
                {
                    var dernItem = actualElementItem;
                    var tab = line.FormatEndOfElement().Split('=');
                    string strTab = ""; int idTable = 0;
                    if (tab.Length > 1)
                    {
                        strTab = tab[1];
                        int.TryParse(strTab.Replace("Table", ""), out idTable);
                        AddReference(idTable.ToString(), rangeZone.FromLine + i,0, RefType.ExternalObject, idTable, ObjectType.Table);
                    }
                    //if (dernItem != null)
                    //{
                    //    if (dernItem.Name.IsNullOrEmpty())
                    //    {
                    //        var obj = NavProject.GetObject(idTable, ObjectType.Table);
                    //        if (obj != null)
                    //        {
                    //            dernItem.Name = ((CommonObjectMgt)obj).Name;
                    //        }
                    //    }

                    //    //Deja Ajoute dans collection des variables
                    //    dernItem.DataItemTable = idTable;
                    //}
                }
                SearchForReference(ref start, ref typeRef, i, line, "SourceTableView=", NonCodeReferenceType.SourceTableView);
                SearchForReference(ref start, ref typeRef, i, line, "LinkTable=", NonCodeReferenceType.LinkTable);
                SearchForReference(ref start, ref typeRef, i, line, "LinkFields=", NonCodeReferenceType.LinkFields);
                SearchForReference(ref start, ref typeRef, i, line, "ReqFilterFields=", NonCodeReferenceType.ReqFilterFields);
                SearchForReference(ref start, ref typeRef, i, line, "CalcFields=", NonCodeReferenceType.CalcFields);
                SearchForReference(ref start, ref typeRef, i, line, "SourceField=", NonCodeReferenceType.SourceField);
                SearchForReference(ref start, ref typeRef, i, line, "SourceTable=", NonCodeReferenceType.SourceTable);





                if (typeRef != NonCodeReferenceType.None)
                {
                    int k = line.IndexOfAny(new char[] { '}', ';' }, start.iChar);
                    if (k >= 0)
                    {
                        end = new Place(k, i);
                        var r = rangeZone.CreateSubRange(start, end);
                        var expr = r.GetRangeText(lines);

                        if (typeRef == NonCodeReferenceType.SourceTableView)
                        {
                            var dernItem = actualElementItem;

                            if (dernItem != null)
                            {
                                if (dernItem.DataItemTable == 0)
                                {
                                    throw new Exception("Error on SourceTable");
                                }
                                othersRefMgt.InsertRefSourceTableView(expr, r, rangeZone, 
                                    dernItem.DataItemTable,true);
                            }
                        }
                        if (typeRef == NonCodeReferenceType.LinkTable)
                        {
                            //var dernItem = DataItems.LastOrDefault();
                            //if (dernItem != null)
                            //{
                            if (expr != "")
                            {
                                //if (typeRef != NonCodeReferenceType.SourceExpr)
                                {
                                    expr = expr.FormatEndOfElement();
                                }
                                //dernItem.DataItemLinkReference = expr;
                            }
                            //}

                            var v = navObj.GlobalVariables.Where(c=>c.Name== expr).FirstOrDefault();
                            if (v != null)
                            {
                                int refLine = r.Start.iLine + rangeZone.FromLine;
                                AddReference(v.Name, refLine,0, RefType.ExternalField, v.ID);
                            }
                        }
                        if (typeRef == NonCodeReferenceType.CalcFields ||
                            typeRef == NonCodeReferenceType.ReqFilterFields)
                        {
                            var dernItem = actualElementItem;
                            if (dernItem != null)
                            {
                                if (expr != "")
                                {
                                    expr = expr.FormatEndOfElement();
                                    var tab = expr.Split(',');
                                    foreach (var f in tab)
                                    {
                                        int refLine = r.Start.iLine + rangeZone.FromLine;
                                        var fName = GetFieldName(dernItem.DataItemTable, f);
                                        AddReference(fName, refLine,0, RefType.ExternalField,
                                           dernItem.DataItemTable, ObjectType.Table);
                                    }
                                }
                            }
                        }
                        if (typeRef == NonCodeReferenceType.LinkFields)
                        {
                            //var dernItem = actualElementItem;
                            var itemFils = actualElementItem;
                            if (itemFils != null)
                            {
                                var IdItemTableParent = 0;
                                if (itemFils.DataItemTable == 0)
                                {
                                    throw new Exception("Error on SourceTable");
                                }
                                if (itemFils.DataItemLinkReference.IsNotNullOrEmpty())
                                {
                                    var parent = navObj.ElementsItems.Where(c => c.Name == itemFils.DataItemLinkReference).LastOrDefault();
                                    if (parent != null)
                                    {
                                        IdItemTableParent = parent.DataItemTable;
                                    }
                                    else
                                    {
                                        var itemParent = navObj.ElementsItems.Where(c => c.Niveau == itemFils.Niveau - 1).LastOrDefault();
                                        if (itemParent != null)
                                        {
                                            IdItemTableParent = itemParent.DataItemTable;
                                        }
                                    }
                                }
                                else
                                {
                                    var itemParent = navObj.ElementsItems.Where(c => c.Niveau == itemFils.Niveau - 1).LastOrDefault();
                                    if (itemParent != null)
                                    {
                                        IdItemTableParent = itemParent.DataItemTable;
                                    }
                                }
                                othersRefMgt.InsertRefElementLinkFields(expr, r, rangeZone, itemFils.DataItemTable, IdItemTableParent);
                            }
                        }

                        if(typeRef== NonCodeReferenceType.SourceField)
                        {
                            ProcessSourceField_Xmlport(rangeZone, r, expr);
                        }
                        if (typeRef == NonCodeReferenceType.SourceTable)
                        {
                            InsertNonCodeReferences(rangeZone, typeRef, r, expr);
                        }

                        typeRef = NonCodeReferenceType.None;
                    }
                }
            }
        }
        private string GetFieldName(int tableID,string fieldNoStr)
        {
            var fieldId = Convert.ToInt32(fieldNoStr.FormatEndOfElement().Replace("Field", ""));
            var obj = navObj.NavProject.GetObject(tableID, ObjectType.Table);
            if (obj != null)
            {
                var fieldList = ((NavObject)obj).FieldList;
                var f = fieldList.Where(c => c.FieldID == fieldId).FirstOrDefault();
                if (f != null)
                {
                    return f.FieldName;
                }
            }
            return "";
        }
        private void InsertNonCodeReferences(CodeRange rangeZone, NonCodeReferenceType typeRef,
            CodeRange r, string expr)
        {
            int iLine = r.Start.iLine + rangeZone.FromLine;
            switch (typeRef)
            {
                case NonCodeReferenceType.TableRelation:
                    {
                        tableRelationRefMgt.InsertRefTableRelation(expr, r, rangeZone);
                        break;
                    }

                case NonCodeReferenceType.None:
                    break;
                case NonCodeReferenceType.Permissions:
                    {
                        othersRefMgt.InsertRefPermissions(expr, r, rangeZone);
                        break;
                    }

                case NonCodeReferenceType.DataCaptionFields:
                    {
                        othersRefMgt.InsertRefDataCaptionFields(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.LookUpPage:
                    {
                        othersRefMgt.InsertRefLookUpDrillDownPages(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.DrillDownPage:
                    {
                        othersRefMgt.InsertRefLookUpDrillDownPages(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.SourceTable:
                    {
                        navObj.SourceTableID = othersRefMgt.InsertRefSourceTable(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.AltSearchField:
                    {
                        othersRefMgt.InsertRef_InternalField(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.AccessByPermission:
                    {
                        othersRefMgt.InsertRefAccessByPermission(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.CalcFormula:
                    {
                        calcFormulaRefMgt.InsertRef_CalcFormula(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.KEYS:
                    break;
                case NonCodeReferenceType.FIELDGROUPS:
                    break;
                case NonCodeReferenceType.CanHaveGlobalVariable:
                    {
                        othersRefMgt.InsertRef_GlobalVariable(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.SourceTableView:
                    {
                        if (navObj.SourceTableID == 0)
                        {
                            throw new Exception("Error on SourceTable");
                        }
                        othersRefMgt.InsertRefSourceTableView(expr, r, rangeZone, navObj.SourceTableID);
                        break;
                    }
                case NonCodeReferenceType.CardPageID:
                    {
                        othersRefMgt.InsertRefCardPageID(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.RunObject:
                    {
                        activeTableIDForRunObject = othersRefMgt.InsertRefRunObject(expr, r, rangeZone);
                        if (RunPageLink_IsCollect && activeTableIDForRunObject>0)
                        {
                            othersRefMgt.InsertRefRunPageLink(activeExprForRunObjectRunPageLink, 
                                activeRangeForRunPageLink, rangeZone, activeTableIDForRunObject);
                        }
                        if (RunPageView_IsCollect && activeTableIDForRunObject > 0)
                        {
                            othersRefMgt.InsertRefSourceTableView(activeExprForRunObjectRunPageView,
                                activeRangeForRunPageView, rangeZone, activeTableIDForRunObject);
                        }
                        break;
                    }
                case NonCodeReferenceType.RunPageView:
                    {
                        if (activeTableIDForRunObject <= 0)
                        {
                            //throw new Exception("Error on RunObject_SourceTable : Line "+ iLine.ToString());
                            activeExprForRunObjectRunPageView = expr;
                            activeRangeForRunPageView = r;
                            RunPageView_IsCollect = true;
                        }
                        else
                        {
                            othersRefMgt.InsertRefSourceTableView(expr, r, rangeZone, activeTableIDForRunObject);
                        }
                        break;
                    }
                case NonCodeReferenceType.RunPageLink:
                    {
                        if (activeTableIDForRunObject <= 0)
                        {
                            //throw new Exception("Error on RunObject_SourceTable : Line " + iLine.ToString());
                            activeExprForRunObjectRunPageLink = expr;
                            activeRangeForRunPageLink = r;
                            RunPageLink_IsCollect = true;
                        }
                        else
                        {
                            othersRefMgt.InsertRefRunPageLink(expr, r, rangeZone, activeTableIDForRunObject);
                        }
                        break;
                    }
                case NonCodeReferenceType.SourceExpr:
                    {
                        break;
                    }
                case NonCodeReferenceType.SubPageLink:
                    {
                        var PagePartID_SourceTable = GetPagePartSourceTableID(actualTablePart);
                        if (PagePartID_SourceTable == 0)
                        {
                            //throw new Exception("Error on PagePartID_SourceTable : Line " + iLine.ToString());
                        }
                        othersRefMgt.InsertRefRunPageLink(expr, r, rangeZone, PagePartID_SourceTable);
                        break;
                    }
                case NonCodeReferenceType.PagePartID:
                    {
                        var PageID = othersRefMgt.InsertRefPagePartID(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.DataItemTable:
                    {
                        othersRefMgt.InsertRefSourceTable(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.LinkFields:
                    {
                        //var linkTableID = GetElementSourceTableID(r, rangeZone);
                        //othersRefMgt.InsertRefSourceTable(expr, r, rangeZone);
                        break;
                    }

                case NonCodeReferenceType.RunObjectType:
                    {
                        menuSuite_RunObjectType = othersRefMgt.InsertRefRunObjectType(expr, r, rangeZone);
                        break;
                    }
                case NonCodeReferenceType.RunObjectID:
                    {
                        othersRefMgt.InsertRefRunObjectID(expr, r, rangeZone, menuSuite_RunObjectType);
                        break;
                    }
                default: break;
            }



        }
        /// <summary>
        /// Table source pour le formulaire PagePartID
        /// </summary>
        /// <param name="rangeZone"></param>
        /// <param name="r"></param>
        /// <param name="PagePartID_SourceTable"></param>
        /// <returns></returns>
        private int GetPagePartSourceTableID(CodeRange rangeZone, CodeRange r)
        {
            int PagePartID_SourceTable = 0;
            int startLine = rangeZone.FromLine + r.Start.iLine;
            for (int i = startLine; i < navObj.Tb.Length; i++)
            {
                var line = navObj.Tb[i];
                if (line.Contains("}"))
                {
                    if (PagePartID_SourceTable == 0)
                    {
                        //throw new Exception("Error on PagePartID_SourceTable Line " + startLine.ToString());
                    }
                }
                if (line.Contains("PagePartID="))
                {
                    var r2 = new CodeRange(i, line);
                    string source = r2.GetRangeText(navObj.Tb);
                    var PageID = othersRefMgt.GetPageID_PagePartID(source);
                    if (PageID > 0)
                    {
                        var objPartPage = navObj.NavProject.GetObject(PageID, ObjectType.Page);
                        if (objPartPage != null)
                        {
                            PagePartID_SourceTable = objPartPage.SourceTableID;
                            break;
                        }
                    }
                    break;
                }
            }

            return PagePartID_SourceTable;
        }
        private int GetPagePartSourceTableID(DataItem part)
        {
            if (part != null)
            {
                var PageID = othersRefMgt.GetPageID_PagePartID(part.PagePartID);
                if (PageID > 0)
                {
                    var objPartPage = navObj.NavProject.GetObject(PageID, ObjectType.Page);
                    if (objPartPage != null)
                    {
                        return objPartPage.SourceTableID;
                    }
                }
            }
            return 0;
        }
        private int GetElementSourceTableID(CodeRange rangeZone, CodeRange r)
        {
            int PagePartID_SourceTable = 0;
            int startLine = rangeZone.FromLine + r.Start.iLine;
            for (int i = startLine; i < navObj.Tb.Length; i++)
            {
                var line = navObj.Tb[i];
                if (line.Contains("}"))
                {
                    if (PagePartID_SourceTable == 0)
                    {
                        //throw new Exception("Error on PagePartID_SourceTable Line " + startLine.ToString());
                    }
                }
                if (line.Contains("LinkTable="))
                {
                    var r2 = new CodeRange(i, line);
                    string source = r2.GetRangeText(navObj.Tb).FormatEndOfElement();
                    var var = navObj.GlobalVariables.Where(c => c.Name == source).FirstOrDefault();
                    if (var != null)
                    {
                        return var.ID;
                    }
                    //return ExtractIDTable_SourceTabExpr(source);
                    //break;
                }
            }

            return PagePartID_SourceTable;
        }
        private int ExtractIDTable_SourceTabExpr(string SourceTable)
        {
            //Table1226;
            var src = SourceTable.FormatEndOfElement().Replace("Table","");
            int idTab = 0;
            int.TryParse(src, out idTab);
            return idTab;
        }
        private static void SearchForReference(ref Place start, ref NonCodeReferenceType typeRef,
            int i, string line, string elem, NonCodeReferenceType referenceType)
        {
            //var elem = "Permissions=";
            int j = line.IndexOf(elem);
            if (line.IndexOf(elem) >= 0)
            {
                start = new Place(j + elem.Length, i);
                typeRef = referenceType;
            }
        }

         
        public void AddReference(string name, int refLine,int iColumn, RefType refType, 
            int objID = 0, ObjectType objType = ObjectType.Table, 
            CodeRange scope = null,string optName="")
        {
            if (objID == 2000000026) return;//Integer Table for Report DataItem
            

            Reference intRef = CreateRef();
            intRef.Name = name;
            intRef.ReferenceLine = refLine+1;
            intRef.ReferenceType = refType;
            intRef.Ref_ObjectID = objID;
            intRef.Ref_ObjectType = objType;
            intRef.Scope = scope;
            intRef.OptionName = optName;
            intRef.ReferenceColumn = iColumn;


            navObj.NavProject.References.Add(intRef);
        }
        private void CollectObjectsRefAsLocalVars()
        {
            var rangesInLocalVar = navObj.PlacesOfCode.Where(c => c.IsFunctionDefinition == false).ToList();
            foreach (var p in rangesInLocalVar)
            {
                var objs = p.LocalVariables.Where(c => c.Type != VariableType.Other).ToList();
                foreach (var o in objs)
                {
                    var r = CreateRef();
                    r.ReferenceLine = o.DefinitionLine + 1;
                    r.ReferenceType = RefType.ExternalObject;
                    r.Ref_ObjectType = o.GetObjectType;
                    r.Ref_ObjectID = o.ID;
                    r.Name = o.Name.RemoveQuotes();
                    //r.ReferenceScope = RefScope.Externe;

                    InsertRef(r);
                }
            }
        }

        private void InsertRef(Reference r)
        {
            //if (r.ReferenceLine <= 1)
            //{
            //    int p = 0;
            //}
            navObj.NavProject.References.Add(r);
        }

        private void CollectObjectsRefAsGlobalVars()
        {
            var objs = navObj.GlobalVariables.Where(c => c.Type != VariableType.Other && !c.IsElementVariable).ToList();
            foreach (var o in objs)
            {
                var r = CreateRef();
                r.ReferenceLine = o.DefinitionLine + 1;
                r.ReferenceType = RefType.ExternalObject;
                r.Ref_ObjectType = o.GetObjectType;
                r.Ref_ObjectID = o.ID;
                r.Name = o.Name.RemoveQuotes();
                //r.ReferenceScope = RefScope.Externe;

                InsertRef(r);
            }
        }
        private void CollectObjectsRefAsFunctionParameter()
        {
            var rangesWithParameters = navObj.PlacesOfCode.Where(c => c.IsFunctionDefinition).ToList();
            foreach (var p in rangesWithParameters)
            {
                var objs = p.LocalVariables.Where(c => c.Type != VariableType.Other).ToList();
                foreach (var o in objs)
                {
                    var r = CreateRef();
                    r.ReferenceLine = o.DefinitionLine + 1;
                    r.ReferenceType = RefType.ExternalObject;
                    r.Ref_ObjectType = o.GetObjectType;
                    r.Ref_ObjectID = o.ID;
                    r.Name = o.Name.RemoveQuotes();
                    //r.ReferenceScope = RefScope.Externe;

                    InsertRef(r);
                }
            }
        }
        public void InsertMarker(List<Marker> markers, string expr, int iLine,int iCol)
        {
            if (expr.Trim() == "") return;
            Marker r = new Marker();
            r.Name = expr;
            r.Order = iLine;
            r.ICol = iCol;
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
        public void ProcessFilterFieldConst(int startLine, int tableRelationObjId, string fieldExternal, string fieldInternal)
        {
            if (fieldInternal.Contains("FIELD"))
            {
                int firstParan = fieldInternal.IndexOf("(");
                int lastParan = fieldInternal.IndexOf(")");
                var field = fieldInternal.Substring(firstParan + 1, lastParan - firstParan - 1);
                if (field.Contains("UPPERLIMIT"))
                {
                    field = field.Replace("UPPERLIMIT", "");
                    field = field.RemoveIfStartWith("(").RemoveIfEndWith(")");
                }
                AddReference(field, startLine,0, RefType.InternalField, navObj.SourceTableID, ObjectType.Table);
            }
            if (fieldInternal.Contains("CONST"))
            {
                int firstParan = fieldInternal.IndexOf("(");
                int lastParan = fieldInternal.IndexOf(")");
                var exprConst = fieldInternal.Substring(firstParan + 1, lastParan - firstParan - 1).RemoveQuotes();
                int intVal = 0;
                if(int.TryParse(exprConst,out intVal))
                {
                    return;
                }
                var extObj = navObj.NavProject.GetObject(tableRelationObjId, ObjectType.Table);
                if (extObj != null)
                {
                    var f = extObj.FieldList.Where(c => c.FieldName == fieldExternal).FirstOrDefault();
                    if (f != null)
                    {
                        if (f.OptionString.IsNotNullOrEmpty())
                        {

                            if (f.OptionString.Contains(exprConst))
                            {
                                AddReference(fieldExternal, startLine,0, RefType.OptionVariable, extObj.ID, ObjectType.Table, null, exprConst);
                            }
                        }
                    }
                }
            }
            if (fieldInternal.Contains("FILTER"))
            {
                int firstParan = fieldInternal.IndexOf("(");
                int lastParan = fieldInternal.LastIndexOf(")");
                var exprFilter = fieldInternal.Substring(firstParan + 1, lastParan - firstParan - 1);
                //fieldInternal = fieldInternal.RemoveIfStartWith("FILTER");
                //var exprFilter = fieldInternal.RemoveAtBorders("(", ")");
                var extObj = navObj.NavProject.GetObject(tableRelationObjId, ObjectType.Table);
                if (extObj != null)
                {
                    var f = extObj.FieldList.Where(c => c.FieldName == fieldExternal).FirstOrDefault();
                    if (f != null)
                    {
                        if (f.OptionString.IsNotNullOrEmpty())
                        {
                            //var tab = exprFilter.Split(new string[] { "..", "|" }, StringSplitOptions.None);
                            var pattern =
    @"""|'|,|:=|\.|\[|\]|\(|\)|\:\:|\.\.|\+|-|\*|\/|\bdiv\b|\bmod\b|=|>|>=|<|<=|<>|\bin\b|\band\b|\bor\b|\bnot\b|\bxor\b|\@|;|\:|\|";
                            var markers = codeRefMgt.GetMarkersWithPattern(exprFilter, 0, true, pattern,0);

                            //codeRefMgt.GetLineMarkers_line(exprFilter, 0, true);
                            foreach (var str in markers)
                            {
                                if (f.OptionString.Contains(str.Name))
                                {
                                    AddReference(fieldExternal, startLine,0, RefType.OptionVariable, extObj.ID, ObjectType.Table, null, str.Name);
                                }
                            }

                        }
                    }
                }
            }
        }

    }
}
