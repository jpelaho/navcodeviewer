using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public partial class PageCollectElements : NavObjectCollectElements
    {
        private CodeRange actualCodeRange = null;
        private Function actualFunction=null;
        private int currentLineOnActualZone = 0;
        TextBlockType actualZone = TextBlockType.None;
        TextBlockCodeType actualCodeType = TextBlockCodeType.None;
        string actualCodeTriggerName = "";
        bool isActionListZone = false;
        private Variable actualVariableElement;

        public PageCollectElements(NavObject a) : base(a)
        {

        }

        public string CollectElements(string source)
        {
            string[] lines = source.SplitLines();
            navObj.NosOfLines = lines.Length;
            navObj.ObjectTextSource = source;

            if (lines.Length > 1)
            {
                AddDocRange(0, lines[0]);
                AddDocRange(1, lines[1]);
            }

            AddEndOfFileCodeRange(lines);

            var startZone = new Place(0, 2);
            var endZone = new Place(0, 0);


            bool isCodeLines = false; bool prevIsLastCodeLine = false; bool isVarZone = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (prevIsLastCodeLine)
                {
                    isCodeLines = false;
                    prevIsLastCodeLine = false;
                    actualCodeType = TextBlockCodeType.None;
                }
                if (i == 58)
                {
                    var p = 4;
                }
                //lines[i] = lines[i].Remove('\r');
                var line = lines[i];

                //GetSourceTableId
                FindSourceTableID(line);
                InsertGlobalVariable_PartSubPage(line);

                CollectOthersZones(line, ref startZone, ref endZone, i);

                UpdateActualZone(line, ref actualZone, ref isVarZone, ref currentLineOnActualZone);

                if (actualZone == TextBlockType.ObjectsProperties)
                {
                    CollectElementsObjectsProperties(ref isCodeLines, ref prevIsLastCodeLine, i, line, currentLineOnActualZone);
                }
                if (actualZone == TextBlockType.Properties)
                {
                    CollectElementsProperties(ref isCodeLines, ref prevIsLastCodeLine, ref isVarZone, i, line, navObj.SourceTableID);
                }
                if (actualZone == TextBlockType.Controls)
                {
                    CollectElementsControls(ref isCodeLines, ref prevIsLastCodeLine, ref isVarZone, i, line, navObj.SourceTableID);
                }
                if (actualZone == TextBlockType.Code)
                {
                    CollectElementsCode(ref isCodeLines, ref prevIsLastCodeLine, ref actualFunction, ref actualCodeRange,
                        currentLineOnActualZone, i, line, ref isVarZone, navObj.SourceTableID);
                }
                currentLineOnActualZone++;
            }

            return source;
        }

        private void FindSourceTableID(string line)
        {
            if (line.Contains("SourceTable="))
            {
                var expr = line.Trim().Replace(";", "");
                var tab = expr.Split('=');
                if (tab.Length > 1)
                {
                    var strTab = tab[1].Replace("Table", "");
                    int recId = 0;
                    int.TryParse(strTab, out recId);
                    navObj.SourceTableID = recId;
                }
            }
        }

        private void CollectOthersZones(string line,ref Place startZone,ref Place endZone,int i)
        {
            if (line.StartsWith(@"  PROPERTIES"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.ObjectsPropertiesDef); 
            }
            if (line.StartsWith(@"  CONTROLS"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);
            }
            if (line.StartsWith(@"  CODE"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.ControlsDef);

                //Collect code Start Zone
                Place startZone1 = new Place(0, i);
                InsertZone(startZone1, startZone1, TypeOfCodeRange.CodeDef);
            }
        }
        private void InsertGlobalVariable_PartSubPage(string line)
        {
            if (line.Contains(";Part "))
            {
                //actualVariableElement = new Variable();
                var newDataItem = new DataItem();
                string strNiveau = ""; int idNiveau = 0;

                var tab = line.Split(';');
                if (tab.Length > 0)
                {
                    newDataItem.IDRef = tab[0].FormatDateItemID();
                }
                if (tab.Length > 1)
                {
                    strNiveau = tab[1];
                    int.TryParse(strNiveau, out idNiveau);
                    newDataItem.Niveau = idNiveau;
                }
                //if (tab.Length > 3)
                //{
                //    var itemName = tab[3].Trim();
                //    if (itemName.IsNotNullOrEmpty())
                //    {
                //        newDataItem.Name = itemName;
                //        actualVariableElement.Name = itemName;
                //    }
                //}
                navObj.PageParts.Add(newDataItem);

            }
            //if (line.Contains("Name="))
            //{
            //    var tab = line.FormatEndOfElement().Split('=');
            //    if (tab.Length > 1)
            //    {
            //        var strName = tab[1];
            //        if (actualVariableElement != null)
            //        {
            //            actualVariableElement.Name = strName.Trim().FormatEndOfElement(); ;
            //        }
            //    }
            //}
            if (line.Contains("PagePartID="))
            {
                var tab = line.FormatEndOfElement().Split('=');
                if (tab.Length > 1)
                {
                    string pagePartId = "";
                    pagePartId=tab[1].Trim().FormatEndOfElement();
                    //var strObjID = tab[1].Replace("Page", "").Trim().FormatEndOfElement();
                    //int idObj = 0;
                    //int.TryParse(strObjID, out idObj );

                    //if (actualVariableElement != null)
                    //{
                    //    actualVariableElement.ID = idObj;
                    //    actualVariableElement.Type = VariableType.Page;

                    //    //var obj = navObj.NavProject.GetObject(idObj, ObjectType.Table);
                    //    //if (obj != null)
                    //    //{
                    //    //    if (actualVariableElement.Name.IsNullOrEmpty())
                    //    //    {
                    //    //        actualVariableElement.Name = ((NavObject)obj).Name;
                    //    //    }
                    //    //}
                    //}


                    var dernItem = navObj.PageParts.LastOrDefault();
                    if (dernItem != null)
                    {
                        //if (dernItem.Name.IsNullOrEmpty())
                        //{
                        //    var obj = navObj.NavProject.GetObject(idTable, ObjectType.Table);
                        //    if (obj != null)
                        //    {
                        //        dernItem.Name = ((NavObject)obj).Name;
                        //    }
                        //}
                        dernItem.PagePartID = pagePartId;
                    }
                }
            }
            //if (line.EndsWith("}"))
            //{
            //    if (actualVariableElement != null)
            //    {
            //        if (actualVariableElement.Name.IsNotNullOrEmpty())
            //        {
            //            navObj.GlobalVariables.Add(actualVariableElement);
            //            actualVariableElement = null;
            //        }
            //    }
            //}
        }
        private void CollectElementsProperties(ref bool isCodeLines, ref bool isLastCodeLine, ref bool isVarZone,
            int i, string line,int RecId)
        {
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnInit,
                @"    OnInit=BEGIN",
                @"           BEGIN",
                @"           END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnOpen,
                @"    OnOpenPage=BEGIN",
                @"               BEGIN",
                @"               END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnClose,
                @"    OnClosePage=BEGIN",
                @"                BEGIN",
                @"                END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnFindRecord,
                @"    OnFindRecord=BEGIN",
                @"                 BEGIN",
                @"                 END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnNextRecord,
                @"    OnNextRecord=BEGIN",
                @"                 BEGIN",
                @"                 END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnAfterGetRecord,
                @"    OnAfterGetRecord=BEGIN",
                @"                     BEGIN",
                @"                     END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnNewRecord,
                @"    OnNewRecord=BEGIN",
                @"                BEGIN",
                @"                END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnInsertRecord,
                @"    OnInsertRecord=BEGIN",
                @"                   BEGIN",
                @"                   END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnModifyRecord,
                @"    OnModifyRecord=BEGIN",
                @"                   BEGIN",
                @"                   END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnDeleteRecord,
                @"    OnDeleteRecord=BEGIN",
                @"                   BEGIN",
                @"                   END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnQueryClose,
                @"    OnQueryClosePage=BEGIN",
                @"                     BEGIN",
                @"                     END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnAfterGetCurrRecord,
                @"    OnAfterGetCurrRecord=BEGIN",
                @"                         BEGIN",
                @"                         END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnAction,
                @"                      OnAction=BEGIN",
                @"                               BEGIN",
                @"                               END;", RecId);

            if(line.StartsWith("    ActionList=ACTIONS"))
            {
                isActionListZone = true;
            }
            if(isActionListZone)
            {
                if(line.StartsWith("    BEGIN"))
                {
                    //PlacesOfCode.Add(new CodeRange(i, line));
                    AddDocRange(i, line);
                }
                if (line.StartsWith("    END"))
                {
                    isActionListZone = false;
                    //PlacesOfCode.Add(new CodeRange(i, line));
                    AddDocRange(i, line);
                }
            }
            if (!isCodeLines)
            {
                if (currentLineOnActualZone == 0)
                {
                    AddDocRange(i, line);
                    AddDocRange(i+1, line);
                }
                AddEndOfZoneCodeRange(i, line,RecId);
            }
        }

        private void CollectElementsControls(ref bool isCodeLines, 
            ref bool isLastCodeLine, ref bool isVarZone,int i, string line,int RecId)
        {

            
            ProcessSourceExpr(i, line, navObj.SourceTableID);


            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnValidate,
                @"                OnValidate=BEGIN",
                @"                           BEGIN",
                @"                           END;",RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnLookup,
                @"                OnLookup=BEGIN",
                @"                         BEGIN",
                @"                         END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnDrillDown,
                @"                OnDrillDown=BEGIN",
                @"                            BEGIN",
                @"                            END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnAssistEdit,
                @"                OnAssistEdit=BEGIN",
                @"                             BEGIN",
                @"                             END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnControlAddIn,
                @"                OnControlAddIn=BEGIN",
                @"                               BEGIN",
                @"                               END;", RecId);


            if (!isCodeLines)
            {
                if (currentLineOnActualZone <= 1)
                {
                    AddDocRange(i, line);
                }
                AddEndOfZoneCodeRange(i, line,RecId);
            }
        }
        
    }
}
