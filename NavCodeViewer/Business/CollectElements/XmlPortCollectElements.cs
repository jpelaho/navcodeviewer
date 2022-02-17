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
    public partial class XmlPortCollectElements : NavObjectCollectElements
    {
        private CodeRange actualCodeRange = null;
        private Function actualFunction=null;
        //private int NosOfLines;
        private int currentLineOnActualZone = 0; 
        string actualCodeTriggerName = "";

        TextBlockType actualZone = TextBlockType.None;
        TextBlockCodeType actualCodeType = TextBlockCodeType.None;
        private int RequestPageSourceTableID;
        int ActualDataItemTableID = 0;
        private Variable actualVariableElement;

        public XmlPortCollectElements(NavObject a) : base(a)
        {

        }
        public string CollectElements(string source)
        {
            string[] lines = source.SplitLines();
            navObj.NosOfLines = lines.Length;
            navObj.ObjectTextSource = source;

            if (lines.Length > 1)
            {
                //CreateNewObject(lines[0]);
                AddDocRange(0, lines[0]);
                AddDocRange(1, lines[1]);
            }
            //for (int k = lines.Length - 1; k >= 0; k--)
            //{
            //    var line = lines[k];
            //    if (line.StartsWith("  END"))
            //    {
            //        AddEndOfFileCodeRange(k);
            //        break;
            //    }
            //}
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
                if (i == 1453)
                {
                    var p = 4;
                }
                var line = lines[i];
                UpdateActualZone(line, ref actualZone,ref isVarZone,ref currentLineOnActualZone);
                CollectOthersZones(line, ref startZone, ref endZone, i);

                InsertGlobalVariable_ElementItem(line);

                RequestPageSourceTableID = FindRequestPageSourceTable(line);


                if (actualZone == TextBlockType.ObjectsProperties)
                {
                    CollectElementsObjectsProperties(ref isCodeLines, ref prevIsLastCodeLine, i, line,currentLineOnActualZone);
                }
                if (actualZone == TextBlockType.Properties)
                {
                    CollectElementsProperties(ref isCodeLines, ref prevIsLastCodeLine,ref isVarZone, i, line,0);
                }
                if (actualZone == TextBlockType.Elements)
                {
                    CollectElementsElements(ref isCodeLines, ref prevIsLastCodeLine,ref isVarZone, i, line);
                }
                if (actualZone == TextBlockType.RequestPage)
                {
                    CollectElementsRequestPage(ref isCodeLines, ref prevIsLastCodeLine, i, line, RequestPageSourceTableID);
                }
                if (actualZone == TextBlockType.RequestPageControls)
                {
                    CollectElementsRequestPageControls(ref isCodeLines, ref prevIsLastCodeLine, ref isVarZone,i, line, RequestPageSourceTableID);
                }
                if (actualZone == TextBlockType.RequestPageProperties)
                {
                    CollectElementsRequestPageProperties(ref isCodeLines, ref prevIsLastCodeLine,ref isVarZone, i, line, RequestPageSourceTableID);
                }
                if (actualZone == TextBlockType.XMLEvents)
                {
                    CollectElementsEvents(ref isCodeLines, ref prevIsLastCodeLine, i, line);
                }               
                if (actualZone == TextBlockType.Code)
                {
                    //TODO : Ajouter les dataItems comme variable
                    CollectElementsCode(ref isCodeLines, ref prevIsLastCodeLine,
                        ref actualFunction,ref actualCodeRange,currentLineOnActualZone, i, line,ref isVarZone,0);
                }
                currentLineOnActualZone++;
            }

            return source;
        }
        private void CollectOthersZones(string line, ref Place startZone, ref Place endZone, int i)
        {
            if (line.StartsWith(@"  PROPERTIES"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.ObjectsPropertiesDef);
            }
            if (line.StartsWith(@"  ELEMENTS"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);
            }
            if (line.StartsWith(@"  EVENTS"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.ElementsDef);
            }
            if (line.StartsWith(@"  REQUESTPAGE"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.EventsDef);
            }
            if (line.StartsWith(@"  CODE"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.RequestPage);

                //Collect code Start Zone
                Place startZone1 = new Place(0, i);
                InsertZone(startZone1, startZone1, TypeOfCodeRange.CodeDef);
            }
        }
        private void CollectElementsProperties(ref bool isCodeLines, 
            ref bool isLastCodeLine,ref bool isVarZone, int i, string line,int RecId)
        {
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone ,i, line,
                ref actualCodeRange,ref actualCodeType,ref actualCodeTriggerName,  TextBlockCodeType.XMLPort_OnInit,
                @"    OnInitXMLport=BEGIN",
                @"                  BEGIN",
                @"                  END;",RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnPre,
                @"    OnPreXMLport=BEGIN",
                @"                 BEGIN",
                @"                 END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnPost,
                @"    OnPostXMLport=BEGIN",
                @"                  BEGIN",
                @"                  END;", RecId);

            if (!isCodeLines)
            {
                if (currentLineOnActualZone == 0)
                {
                    AddDocRange(i, line);
                    AddDocRange(i+1, line);
                }
                AddEndOfZoneCodeRange(i, line, RecId);
            }
        }
        private void FindActualDataItemTable(string line)
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
                    ActualDataItemTableID = recId;
                    //return recId;
                }
            }
        }
        private int FindRequestPageSourceTable(string line)
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
                    return recId;
                }
            }
            return 0;
        }
        private void CollectElementsElements(ref bool isCodeLines, 
            ref bool isLastCodeLine, ref bool isVarZone, int i, string line)
        {
            FindActualDataItemTable(line);
            int RecId = ActualDataItemTableID;

            ProcessDataSource(i, line, RecId);

            

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
            ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnAfterAssignVariable,
            @"                                                  Import::OnAfterAssignVariable=BEGIN",
            @"                                                                                BEGIN",
            @"                                                                                END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnBeforePassVariable,
                @"                                                  Export::OnBeforePassVariable=BEGIN",
                @"                                                                               BEGIN",
                @"                                                                               END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnAfterInitRecord,
                @"                                                  Import::OnAfterInitRecord=BEGIN",
                @"                                                                            BEGIN",
                @"                                                                            END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnBeforeInsertRecord,
                @"                                                  Import::OnBeforeInsertRecord=BEGIN",
                @"                                                                               BEGIN",
                @"                                                                               END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnPreXMLItem,
                @"                                                  Export::OnPreXMLItem=BEGIN",
                @"                                                                       BEGIN",
                @"                                                                       END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnAfterGetRecord,
                @"                                                  Export::OnAfterGetRecord=BEGIN",
                @"                                                                           BEGIN",
                @"                                                                           END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnAfterInsertRecord,
                @"                                                  Import::OnAfterInsertRecord=BEGIN",
                @"                                                                              BEGIN",
                @"                                                                              END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnBeforeModifyRecord,
                @"                                                  Import::OnBeforeModifyRecord=BEGIN",
                @"                                                                               BEGIN",
                @"                                                                               END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.XMLPort_OnAfterModifyRecord,
                @"                                                  Import::OnAfterModifyRecord=BEGIN",
                @"                                                                              BEGIN",
                @"                                                                              END;", RecId);
            if (!isCodeLines)
            {
                if (currentLineOnActualZone == 0)
                {
                    //PlacesOfCode.Add(new CodeRange(i, line));
                    //PlacesOfCode.Add(new CodeRange(i + 1, line));
                    AddDocRange(i, line);
                    AddDocRange(i + 1, line);
                }
                AddEndOfZoneCodeRange(i, line, RecId);

            }
        }
        /// <summary>
        /// Insertion des variables crees sur DataItems comme variables globales
        /// </summary>
        /// <param name="line"></param>
        private void InsertGlobalVariable_ElementItem(string line)
        {
            if (line.Contains("VariableName=PaymentExportData"))
            {
                int p = 0;
            }
            if (line.StartsWith("    { [{") && line.Contains("Element"))
            {
                var newDataItem = new DataItem();
                string strNiveau = ""; int idNiveau = 0;

                actualVariableElement = new Variable();
                var tab = line.Split(';');

                if (tab.Length > 0)
                {
                    newDataItem.IDRef = tab[0];
                }
                if (tab.Length > 1)
                {
                    strNiveau = tab[1];
                    int.TryParse(strNiveau, out idNiveau);
                    newDataItem.Niveau = idNiveau;
                }
                if (tab.Length > 2)
                {
                    actualVariableElement.Name = tab[2].Trim();
                    newDataItem.Name = actualVariableElement.Name;
                }
                if (tab.Length > 4)
                {
                    var type = tab[4].Trim();
                    if (type == "Field")
                    {
                        actualVariableElement = null;
                    }
                }
                navObj.ElementsItems.Add(newDataItem);
            }
            if (line.Contains("SourceTable="))
            {
                //if (actualVariableElement != null)
                //{
                var tab = line.FormatEndOfElement().Split('=');
                if (tab.Length > 1)
                {
                    var idTable = tab[1].Replace("Table", "").Trim();
                    int idObj = 0;
                    int.TryParse(idTable, out idObj);
                    actualVariableElement.ID = idObj;
                    actualVariableElement.Type = VariableType.Record;

                    var obj = navObj.NavProject.GetObject(idObj, ObjectType.Table);
                    if (obj != null)
                    {
                        if (!actualVariableElement.VariableNameIsSet)
                        {
                            actualVariableElement.Name = ((NavObject)obj).Name;
                        }
                    }


                    var dernItem = navObj.ElementsItems.LastOrDefault();
                    if (dernItem != null)
                    {
                        if (dernItem.Name.IsNullOrEmpty())
                        {
                            //var obj1 = navObj.NavProject.GetObject(idTable, ObjectType.Table);
                            //if (obj1 != null)
                            {
                                if (!actualVariableElement.VariableNameIsSet)
                                {
                                    dernItem.Name = actualVariableElement.Name;
                                }
                            }
                        }
                        dernItem.DataItemTable = idObj;
                    }
                }
                //}
            }
            if (line.Contains("VariableName="))
            {
                var dernItem = navObj.ElementsItems.LastOrDefault();
                var tab = line.FormatEndOfElement().Split('=');
                if (actualVariableElement != null)
                {
                    if (tab.Length > 1)
                    {
                        actualVariableElement.Name = tab[1].Trim();
                        actualVariableElement.VariableNameIsSet = true;
                    }
                }
                if (dernItem != null)
                {
                    if (tab.Length > 1)
                    {
                        dernItem.Name = tab[1].Trim();
                    }
                }
            }
            if (line.Contains("LinkTable="))
            {
                var dernItem = navObj.ElementsItems.LastOrDefault();
                if (dernItem != null)
                {
                    var expr = line.Trim().FormatEndOfElement();
                    if (expr != "")
                    {
                        var tab = expr.FormatEndOfElement().Split('=');
                        if (tab.Length > 1)
                        {
                            dernItem.DataItemLinkReference = tab[1].Trim();
                        }
                    }
                }
            }
            if (line.EndsWith("}"))
            {
                if (actualVariableElement != null)
                {
                    actualVariableElement.IsElementVariable = true;
                    navObj.GlobalVariables.Add(actualVariableElement);
                    actualVariableElement = null;
                }
            }
        }

        private void CollectElementsEvents(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line)
        {
            if (line.StartsWith(@"  BEGIN"))
            {
                isCodeLines = true;
                AddDocRange(i, line);
            }
            if (line.StartsWith(@"  END"))
            {
                isLastCodeLine = true;
                AddDocRange(i, line);
            }
            if (!isCodeLines)
            {
                if (currentLineOnActualZone == 0)
                {
                    AddDocRange(i, line);
                }
            }
        }
        
        private void CollectElementsRequestPage(ref bool isCodeLines, 
            ref bool isLastCodeLine, int i, string line, int RecId)
        {
            if (!isCodeLines)
            {
                if (currentLineOnActualZone == 0)
                {
                    AddDocRange(i, line);
                    AddDocRange(i + 1, line);
                }
                AddEndOfZoneCodeRange(i, line,RecId);
            }
        }
        private void CollectElementsRequestPageProperties(ref bool isCodeLines, 
            ref bool isLastCodeLine, ref bool isVarZone, int i, string line,int RecId)
        {
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnInit,
                @"      OnInit=BEGIN",
                @"             BEGIN",
                @"             END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnOpen,
                @"      OnOpenPage=BEGIN",
                @"                 BEGIN",
                @"                 END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnClose,
                @"      OnClosePage=BEGIN",
                @"                  BEGIN",
                @"                  END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnFindRecord,
                @"      OnFindRecord=BEGIN",
                @"                   BEGIN",
                @"                   END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnNextRecord,
                @"      OnNextRecord=BEGIN",
                @"                   BEGIN",
                @"                   END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnAfterGetRecord,
                @"      OnAfterGetRecord=BEGIN",
                @"                       BEGIN",
                @"                       END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnNewRecord,
                @"      OnNewRecord=BEGIN",
                @"                  BEGIN",
                @"                  END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnInsertRecord,
                @"      OnInsertRecord=BEGIN",
                @"                     BEGIN",
                @"                     END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnModifyRecord,
                @"      OnModifyRecord=BEGIN",
                @"                     BEGIN",
                @"                     END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnDeleteRecord,
                @"      OnDeleteRecord=BEGIN",
                @"                     BEGIN",
                @"                    END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnQueryClose,
                @"      OnQueryClosePage=BEGIN",
                @"                       BEGIN",
                @"                       END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnAfterGetCurrRecord,
                @"      OnAfterGetCurrRecord=BEGIN",
                @"                           BEGIN",
                @"                           END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnAction,
                @"                        OnAction=BEGIN",
                @"                                 BEGIN",
                @"                                 END;", RecId);


            if (!isCodeLines)
            {
                if (currentLineOnActualZone == 0)
                {
                    AddDocRange(i, line);
                    AddDocRange(i + 1, line);
                }

                if (line.StartsWith(@"    END"))
                {
                    int x = line.IndexOfLastChar(@"    END");
                    var start = new Place(0, i);
                    var end = new Place(x, i);
                    AddCodeRange(new CodeRange(start, end), RecId);
                }

            }
        }
        private void CollectElementsRequestPageControls(ref bool isCodeLines, 
            ref bool isLastCodeLine, ref bool isVarZone ,int i, string line,int RecId)
        {

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref  isVarZone,i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnValidate,
                @"                  OnValidate=BEGIN",
                @"                             BEGIN",
                @"                             END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone,i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnLookup,
                @"                  OnLookup=BEGIN",
                @"                           BEGIN",
                @"                           END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnDrillDown,
                @"                  OnDrillDown=BEGIN",
                @"                              BEGIN",
                @"                              END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnLookup,
                @"                  OnAssistEdit=BEGIN",
                @"                               BEGIN",
                @"                               END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnDrillDown,
                @"                  OnControlAddIn=BEGIN",
                @"                                 BEGIN",
                @"                                 END;", RecId);



            if (!isCodeLines)
            {
                if (currentLineOnActualZone <= 1)
                {
                    AddDocRange(i, line);
                }
                if (line.StartsWith(@"    END"))
                {
                    int x = line.IndexOfLastChar(@"    END");
                    var start = new Place(0, i);
                    var end = new Place(x, i);
                    AddCodeRange(new CodeRange(start, end), RecId);
                }

                //Inclure le END de RequestPage
                AddEndOfZoneCodeRange(i, line, RecId);
            }
        }

    }
}
