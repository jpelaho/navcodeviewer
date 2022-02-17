using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace NavCodeViewer.Business
{
    public partial class ReportCollectElements : NavObjectCollectElements
    {
        private CodeRange actualCodeRange = null;
        private Function actualFunction=null;
        private int currentLineOnActualZone = 0;
        string actualCodeTriggerName = "";

        TextBlockType actualZone = TextBlockType.None;
        TextBlockCodeType actualCodeType = TextBlockCodeType.None;
        int ActualDataItemTable = 0;
        int RequestPageSourceTableID=0;
        private Variable actualVariableElement;

        public ReportCollectElements(NavObject a) : base(a)
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
                if (i == 1453)
                {
                    var p = 4;
                }
                if(navObj.Type== ObjectType.Report && navObj.ID == 88)
                {
                    int p = 5;
                }
                var line = lines[i];
                UpdateActualZone(line, ref actualZone,ref isVarZone, ref currentLineOnActualZone);
                CollectOthersZones(line, ref startZone, ref endZone, i);

                InsertGlobalVariable_DataItem(line,ref actualVariableElement);

                FindRequestPageSourceTable(line);

                if (actualZone == TextBlockType.ObjectsProperties)
                {
                    CollectElementsObjectsProperties(ref isCodeLines, ref prevIsLastCodeLine, i, line,currentLineOnActualZone);
                }
                if (actualZone == TextBlockType.Properties)
                {
                    CollectElementsProperties(ref isCodeLines, ref prevIsLastCodeLine,ref isVarZone, i, line,0);
                }
                if (actualZone == TextBlockType.DataSet)
                {
                    CollectElementsDataSet(ref isCodeLines, ref prevIsLastCodeLine, i,ref isVarZone, line);
                }
                if (actualZone == TextBlockType.RequestPage)
                {
                    CollectElementsRequestPage(ref isCodeLines, ref prevIsLastCodeLine, i, line, RequestPageSourceTableID);
                }
                if (actualZone == TextBlockType.RequestPageControls)
                {
                    CollectElementsRequestPageControls(ref isCodeLines, ref prevIsLastCodeLine,ref isVarZone, i, line, RequestPageSourceTableID);
                }
                if (actualZone == TextBlockType.RequestPageProperties)
                {
                    CollectElementsRequestPageProperties(ref isCodeLines, ref prevIsLastCodeLine,ref isVarZone, i, line, RequestPageSourceTableID);
                }
                if (actualZone == TextBlockType.Labels)
                {
                    CollectElementsLabels(ref isCodeLines, ref prevIsLastCodeLine, i, line);
                }
                
                if (actualZone == TextBlockType.Code)
                {
                    //TODO : Ajouter les dataItems comme variables globale
                    //Et les columns comme variables
                    CollectElementsCode(ref isCodeLines, ref prevIsLastCodeLine,
                        ref actualFunction,ref actualCodeRange,currentLineOnActualZone, i, line,ref isVarZone,0);
                }
                if (actualZone == TextBlockType.RdlData)
                {
                    if (line.StartsWith(@"  BEGIN"))
                    {
                        isCodeLines = true;
                        AddDocRange(i, line);

                        AddDocRange(lines.Length - 1, "          ");


                        //Collect RdlData Start Zone
                        Place startZone1 = new Place(0, i-1);
                        InsertZone(startZone1, startZone1, TypeOfCodeRange.RDLDataDef);

                        break;
                    }
                    
                    //CollectElementsRDLData(ref isCodeLines, ref prevIsLastCodeLine, i, line);
                    //AddCodeRange(new CodeRange {Start=new Place(0,i),End=new Place(50, lines.Length-1) }, 0);
                    
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
            if (line.StartsWith(@"  DATASET"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);
            }
            if (line.StartsWith(@"  REQUESTPAGE"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.DataSetDef);
            }
            if (line.StartsWith(@"  LABELS"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.RequestPage);
            }
            if (line.StartsWith(@"  CODE"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.LabelsDef);

                //Collect code Start Zone
                Place startZone1 = new Place(0, i);
                InsertZone(startZone1, startZone1, TypeOfCodeRange.CodeDef);
            }
        }
        
        private void FindActualDataItemTable(string line)
        {
            if (line.Contains("DataItemTable="))
            {
                var expr = line.Trim().FormatEndOfElement();
                var tab = expr.Split('=');
                if (tab.Length > 1)
                {
                    var strTab = tab[1].Replace("Table", "");
                    int recId = 0;
                    int.TryParse(strTab, out recId);
                    ActualDataItemTable = recId;
                }
            }
        }
        private void FindRequestPageSourceTable(string line)
        {
            if (line.Contains("SourceTable="))
            {
                var expr = line.Trim().FormatEndOfElement(); 
                var tab = expr.Split('=');
                if (tab.Length > 1)
                {
                    var strTab = tab[1].Replace("Table", "");
                    int recId = 0;
                    int.TryParse(strTab, out recId);
                    RequestPageSourceTableID = recId;
                }
            }
        }
        private void CollectElementsProperties(ref bool isCodeLines, ref bool isLastCodeLine,
            ref bool isVarZone,int i, string line,int RecId)
        {
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine,ref isVarZone, i, line,
                ref actualCodeRange,ref actualCodeType,ref actualCodeTriggerName, TextBlockCodeType.Report_OnInitReport,
                @"    OnInitReport=BEGIN",
                @"                 BEGIN",
                @"                 END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName,TextBlockCodeType.Report_OnPreReport,
                @"    OnPreReport=BEGIN",
                @"                BEGIN",
                @"                END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName,TextBlockCodeType.Report_OnPostReport,
                @"    OnPostReport=BEGIN",
                @"                 BEGIN",
                @"                 END;", RecId);

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


        private void CollectElementsDataSet(ref bool isCodeLines,
            ref bool isLastCodeLine, int i, ref bool isVarZone, string line)
        {

            FindActualDataItemTable(line);
            int RecId = ActualDataItemTable;

            ProcessSourceExpr(i, line, RecId);

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Report_OnPreDataItem,
                @"               OnPreDataItem=BEGIN",
                @"                             BEGIN",
                @"                             END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Report_OnAfterGetRecord,
                @"               OnAfterGetRecord=BEGIN",
                @"                                BEGIN",
                @"                                END;", RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Report_OnPostDataItem,
                @"               OnPostDataItem=BEGIN",
                @"                              BEGIN",
                @"                              END;", RecId);


            if (!isCodeLines)
            {
                if (currentLineOnActualZone <= 1)
                {
                    AddDocRange(i, line);
                }
                AddEndOfZoneCodeRange(i, line, RecId);
            }
        }

        

        private void CollectElementsLabels(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line)
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
        private void CollectElementsRDLData(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line)
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
            ref bool isLastCodeLine, int i, string line,int RecId)
        {
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
        private void CollectElementsRequestPageProperties(ref bool isCodeLines, ref bool isLastCodeLine,
             ref bool isVarZone,int i, string line,int RecId)
        {
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnInit,
                @"      OnInit=BEGIN",
                @"             BEGIN",
                @"             END;",RecId);
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
                    AddDocRange(i+1, line);
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
        private void CollectElementsRequestPageControls(ref bool isCodeLines, ref bool isLastCodeLine,
            ref bool isVarZone,int i, string line,int RecId)
        {

            ProcessSourceExpr(i, line, RequestPageSourceTableID);


            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Page_OnValidate,
                @"                  OnValidate=BEGIN",
                @"                             BEGIN",
                @"                             END;",RecId);
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
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
                    //PlacesOfCode.Add(new CodeRange(i, line));
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
