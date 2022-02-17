using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NavCodeViewer.Business
{
    public class NavObjectCollectElements
    {
        private List<Variable> tempLocalVariables = new List<Variable>();
        TableRelationRefMgt tableRelationRefMgt;
        OthersRefMgt othersRefMgt;
        CalcFormulaRefMgt flowFieldRefMgt;
        CodeRefMgt codeRefMgt;
        
        //private DataItem actualDataItem; private DataItem actualElementItem;

        //private int activeTableIDForRunObject = 0;
        //private bool RunPageView_IsCollect;
        //private bool RunPageLink_IsCollect;
        //private CodeRange activeRangeForRunPageView = null;
        //private string activeExprForRunObjectRunPageView = "";
        //private CodeRange activeRangeForRunPageLink = null;
        //private string activeExprForRunObjectRunPageLink = "";
        //private ObjectType menuSuite_RunObjectType = ObjectType.Page;
        protected NavObject navObj;

        public NavObjectCollectElements(NavObject navObject1)
        {
            navObj = navObject1;
            var _collectRefs = new NavObjectCollectRefs(navObj);

            tableRelationRefMgt = new TableRelationRefMgt(navObj, _collectRefs);
            othersRefMgt = new OthersRefMgt(navObj, _collectRefs);
            flowFieldRefMgt = new CalcFormulaRefMgt(navObj, _collectRefs);
           
            codeRefMgt = new CodeRefMgt(navObj, _collectRefs);
        }

        protected bool IsStartOfFunctionLine(string line)
        {
            return line.Contains(" PROCEDURE ") || line.StartsWith("    PROCEDURE ");
        }
        protected bool IsPrivateFunction(string line)
        {
            return !line.StartsWith("    PROCEDURE ");
        }
        protected string GetFunctionName(string line)
        {
            var str = Regex.Matches(line, @"(?<=\b(PROCEDURE)\s+)(?<range>\w+?)\b");
            if (str.Count > 0)
            {
                return str[0].Value;
            }
            return "";
        }
        private string GetFunctionParametersString(string line)
        {
            var str = Regex.Matches(line, @"(?<=.*)\(.+\)");
            if (str.Count > 0)
            {
                return str[0].Value;
            }
            return "";
        }
        protected List<Variable> GetAllParameters(string line, int iLine)
        {
            if (line.Contains("AccountNo"))
            {
                int p = 1;
            }
            List<Variable> rep = new List<Variable>();
            var parameters = GetFunctionParametersString(line).Replace("(", "").Replace(")", "");
            if (parameters.IsNullOrEmpty())
            {
                //return rep;
            }
            else
            {
                var listeP = parameters.Split(';');
                foreach (var p in listeP)
                {
                    if (p.IsNullOrEmpty()) continue;
                    var v = new Variable();
                    CollectVariable(p, v, iLine);
                    if (v.Name != null)
                    {
                        rep.Add(v);
                    }
                }
            }

            //Add Variable Valeur de retour de la fonction
            var lastIndex = line.LastIndexOf(')');
            string returnVar = "";
            if (lastIndex + 1 < line.Length)
            {
                returnVar = line.Substring(lastIndex + 1);
            }
            if (returnVar.IsNotNullOrEmpty() && returnVar.Trim() != ";")
            {
                var tab = returnVar.Split(':', ';');
                if (tab != null)
                {
                    if (tab.Count() > 1)
                    {
                        if (tab[0].Trim().IsNotNullOrEmpty())
                        {
                            var v = new Variable();
                            v.Name = tab[0].Trim();
                            v.Type = VariableType.Other;
                            v.DefinitionLine = iLine;
                            rep.Add(v);
                        }
                    }
                }
            }


            return rep;
        }
        private void CollectVariable(string line, Variable variable, int iLine)
        {
            bool isTemp = false;
            isTemp = line.Contains("TEMPORARY ");
            line = line.Replace("VAR ", "").Replace("TEMPORARY ", "");
            var variableData = Regex.Split(line, @":|\@|;");
            if (variableData != null)
            {
                if (variableData.Length > 2)
                {
                    var name = variableData[0];
                    //if (name.Contains("TEMPORARY"))
                    //{
                    //    int i = 5;
                    //}
                    variable.Name = name.Trim();
                    variable.IsTempVariable = isTemp;
                    var typeData = variableData[2].Trim().Split(' ');

                    if (typeData.Length > 0)
                    {
                        var type1 = typeData[0].Trim();
                        variable.SetTypeFromText(type1);
                        if(variable.Type!= VariableType.Other)
                        {
                            if (typeData.Length > 1)
                            {
                                var strId = typeData[1].Trim();
                                int IdVar = 0;
                                Int32.TryParse(strId, out IdVar);
                                variable.ID = IdVar;
                            }
                        }
                    }
                    else
                    {
                        variable.Type = VariableType.Other;
                    }

                    //if (typeData.Length >= 2)
                    //{
                    //    var strType = typeData[typeData.Length - 2].Trim();
                    //    var Id = typeData[typeData.Length - 1].Trim();
                    //    int IdVar = 0;
                    //variable.SetTypeFromText(strType);
                    //    Int32.TryParse(Id, out IdVar);
                    //    variable.ID = IdVar;
                    //}
                    //else
                    //{
                    //    variable.Type = VariableType.Other;
                    //}
                    variable.DefinitionLine = iLine;
                }
            }
        }
        protected void ProcessSourceExpr(int i, string line, int RecId)
        {
            ProcessSourceExpr_DataSource("SourceExpr=",i, line, RecId);
        }
        protected void ProcessDataSource(int i, string line, int RecId)
        {
            ProcessSourceExpr_DataSource("DataSource=", i, line, RecId);
        }

        private void ProcessSourceExpr_DataSource(string marker,int i, string line, int RecId)
        {
            if (line.Contains(marker))
            {
                int j = line.IndexOf(marker);
                var startCol = j + marker.Length;
                var start = new Place(startCol, i);
                var end = new Place(line.Length, i);
                var r = new CodeRange(start, end);
                r.RecId = RecId;
                navObj.PlacesOfSourceExpr.Add(r);
            }
        }

        //    { 2   ;   ;Price Includes VAT  ;Boolean       ;OnValidate=VAR
        //                                                   OnValidate=BEGIN
        //                                                   OnValidate=VAR
        protected void ProcessCodeTrigger(ref bool isCodeLines, ref bool isLastCodeLine, ref bool isVarZone,
            int i, string line,
            ref CodeRange actualCodeRange, ref TextBlockCodeType actualCodeType, ref string actualCodeTriggerName,
            TextBlockCodeType codeType, string TextStartOfTrigger,
            string TextBegin_WhenVar, string TextEndOfTrigger,int RecId)
        {
            if (i == 616 && codeType== TextBlockCodeType.Page_OnDrillDown)
            {
                var p = 0;
            }

            if (line.EndsWith(@"=VAR"))
            {
                isVarZone = true;
            }
            //if (line.StartsWith(TextBegin_WhenVar))
            //{
            //    isVarZone = false;
            //}

            if (line.Contains(TextStartOfTrigger.Trim().Replace("BEGIN", "")))
            {
                actualCodeType = codeType;
            }
            var startTriggerCodeAfterVar = false;
            if(isVarZone && line.Trim() == "BEGIN" && actualCodeType == codeType)
            {
                startTriggerCodeAfterVar = true;
            }

            
            var txtMarker = TextStartOfTrigger.Replace("BEGIN", "");
            //if (line.StartsWith(TextStartOfTrigger) || line.Contains(";" + txtMarker.TrimStart()))
            if (line.Contains(TextStartOfTrigger.Trim()))//OnValidate=BEGIN
            {
                isCodeLines = true;
                isVarZone = false;
                actualCodeType = codeType;
                StartCodeRange(i, line, TextStartOfTrigger.Replace("BEGIN", ""),
                    ref actualCodeRange, actualCodeType, actualCodeTriggerName);
            }
            if (startTriggerCodeAfterVar)//BEGIN After OnValidate=VAR
            {
                startTriggerCodeAfterVar = false;
                isCodeLines = true;
                isVarZone = false;
                actualCodeType = codeType;
                string prefixBegin = "";
                for(int n=0;n< txtMarker.Length; n++) { prefixBegin += " "; }
                StartCodeRange(i, line, prefixBegin,
                    ref actualCodeRange, actualCodeType, actualCodeTriggerName);
            }

            if (actualCodeType == codeType)
            {
                if (isVarZone && !line.EndsWith(@"=VAR"))
                {
                    var variable = new Variable();
                    CollectVariable(line, variable, i);
                    if (!variable.Name.IsNullOrEmpty())
                    {
                        if (actualCodeRange != null)
                        {
                            actualCodeRange.LocalVariables.Add(variable);
                        }
                        else//Range not yet created (
                        {
                            //Exemple
                            /* OnOpenPage = VAR
                                CRMIntegrationManagement@1000 : Codeunit 5330;
                            BEGIN*/
                            
                            tempLocalVariables.Add(variable);
                        }
                    }
                }
                //if (line.StartsWith(TextBegin_WhenVar))
                //{
                //    isCodeLines = true;
                //    isVarZone = false;
                //    StartCodeRange(i, ref actualCodeRange, actualCodeType, actualCodeTriggerName, false);
                //}
                if (line.StartsWith(TextEndOfTrigger) && actualCodeType == codeType)
                {
                    isLastCodeLine = true;
                    isVarZone = false;
                    startTriggerCodeAfterVar = false;
                    CloseAndSaveCodeRange(i, line, TextEndOfTrigger, ref actualCodeRange, RecId);
                }
            }
        }

        protected void ProcessCodeFunction(ref bool isCodeLines, ref bool isLastCodeLine, int i,
            string line, ref bool isVarZone, ref Function actualFunction,
            List<Variable> GlobalVariables,
             ref CodeRange actualCodeRange,int RecId)
        {
            if (isVarZone)
            {
                var variable = new Variable();
                CollectVariable(line, variable, i);

                if (!variable.Name.IsNullOrEmpty())
                {
                    if (actualFunction == null)
                    {
                        GlobalVariables.Add(variable);
                    }
                    else
                    {
                        //if (actualCodeRange != null)
                        //{
                        //    actualCodeRange.LocalVariables.Add(variable);
                        //}

                        tempLocalVariables.Add(variable);
                    }
                }
            }

            if (IsStartOfFunctionLine(line))
            {
                actualFunction = new Function();
                actualFunction.StartingDefLine = i + 1;
                actualFunction.FunctionName = GetFunctionName(line);
                actualFunction.ObjectID = navObj.ID;
                actualFunction.ObjectType = navObj.Type;
                actualFunction.Private = IsPrivateFunction(line);
            }
            if (line.StartsWith(@"    END;")|| line.StartsWith(@"    END."))
            {
                isLastCodeLine = true;
                if (actualFunction != null)
                {
                    actualFunction.EndingDefLine = i + 1;
                    navObj.FunctionList.Add(actualFunction);
                    actualFunction = null;
                }
                CloseAndSaveCodeRange(i, line, @"    END;", ref actualCodeRange, RecId);
            }

            if (line.StartsWith(@"    VAR"))
            {
                isVarZone = true;
            }

            if (line.StartsWith(@"    BEGIN"))
            {
                isCodeLines = true;
                isVarZone = false;
                var functionName = "";
                if (actualFunction != null)
                {
                    functionName = actualFunction.FunctionName;
                }
                StartCodeRange(i, line, @"    ", ref actualCodeRange, true, functionName);
            }
        }
        private void CloseAndSaveCodeRange(int i, string line, string codeString,
            ref CodeRange actualCodeRange,int RecId)
        {
            int x = line.IndexOfLastChar(codeString);
            actualCodeRange.End = new Place(x, i);
            actualCodeRange.CanHaveReference = true;
            if (tempLocalVariables.Count>0)
            {
                actualCodeRange.LocalVariables.AddRange(tempLocalVariables);
            }
            AddCodeRange(actualCodeRange, RecId);
            actualCodeRange = null;
            tempLocalVariables = new List<Variable>();
        }
        protected void AddDocRange(int iLine, string LineText)
        {
            var r = new CodeRange(iLine, LineText);
            navObj.PlacesOfCode.Add(r);
        }
        protected void AddCodeRange(CodeRange r,int recId)
        {
            //var r = new CodeRange(iLine, LineText);
            r.RecId = recId;
            navObj.PlacesOfCode.Add(r);
        }
        private void StartCodeRange(int i, string line, string codeString,
            ref CodeRange actualCodeRange,
            TextBlockCodeType TriggerType, string actualCodeTriggerName)
        {
            var trigger = AddTrigger(i, TriggerType, actualCodeTriggerName);

            StartCodeRange(i, line, codeString, ref actualCodeRange, false, trigger.Name);
        }
        protected void CollectElementsObjectsProperties(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line,
            int currentLineOnActualZone)
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
                    //PlacesOfCode.Add(new CodeRange(i, line));
                    AddDocRange(i, line);
                }
            }
        }
        protected void CollectElementsObjectsProperties(int indexZone, string[] lines)
        {
            AddDocRange(indexZone, lines[indexZone]);
            AddDocRange(indexZone + 1, lines[indexZone + 1]);
            AddDocRange(indexZone + lines.Length - 1, lines[indexZone + lines.Length - 1]);

            //if (line.StartsWith(@"  BEGIN"))
            //{
            //    isCodeLines = true;
            //    AddDocRange(i, line);
            //}
            //if (line.StartsWith(@"  END"))
            //{
            //    isLastCodeLine = true;
            //    AddDocRange(i, line);
            //}
            //if (!isCodeLines)
            //{
            //    if (currentLineOnActualZone == 0)
            //    {
            //        //PlacesOfCode.Add(new CodeRange(i, line));
            //        AddDocRange(i, line);
            //    }
            //}
        }
        protected void UpdateActualZone(string line, ref TextBlockType actualZone1, ref bool isVarZone
            , ref int currentLineOnActualZone)
        {
            if (line.StartsWith(@"  OBJECT-PROPERTIES"))
            {
                actualZone1 = TextBlockType.ObjectsProperties;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  PROPERTIES"))
            {
                isVarZone = false;
                actualZone1 = TextBlockType.Properties;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  FIELDS"))
            {
                actualZone1 = TextBlockType.Fields;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  KEYS"))
            {
                actualZone1 = TextBlockType.Keys;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  FIELDGROUPS"))
            {
                actualZone1 = TextBlockType.FieldGroups;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  CODE"))
            {
                isVarZone = false;
                actualZone1 = TextBlockType.Code;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  CONTROLS"))
            {
                actualZone1 = TextBlockType.Controls;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  MENUNODES"))
            {
                actualZone1 = TextBlockType.MenuNodes;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  LABELS"))
            {
                actualZone1 = TextBlockType.Labels;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  RDLDATA"))
            {
                actualZone1 = TextBlockType.RdlData;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  EVENTS"))
            {
                actualZone1 = TextBlockType.XMLEvents;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  ELEMENTS"))
            {
                actualZone1 = TextBlockType.Elements;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  DATASET"))
            {
                actualZone1 = TextBlockType.DataSet;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"  REQUESTPAGE"))
            {
                actualZone1 = TextBlockType.RequestPage;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"    PROPERTIES"))
            {
                actualZone1 = TextBlockType.RequestPageProperties;
                currentLineOnActualZone = 0;
            }
            if (line.StartsWith(@"    CONTROLS"))
            {
                actualZone1 = TextBlockType.RequestPageControls;
                currentLineOnActualZone = 0;
            }

        }
        private Trigger AddTrigger(int i, TextBlockCodeType TriggerType, string actualCodeTriggerName)
        {
            Trigger item = new Trigger
            {
                TextBlockType = TriggerType,
                DefLine = i+1,
                Prefix = actualCodeTriggerName
            };
            navObj.Triggers.Add(item);
            return item;
        }
        private void StartCodeRange(int i, string line, string codeString, ref CodeRange actualCodeRange,
            bool isFunction, string triggerName)
        {
            actualCodeRange = new CodeRange();
            int x = line.IndexOfLastChar(codeString);
            actualCodeRange.Start = new Place(x, i);

            actualCodeRange.RangeName = triggerName;
            actualCodeRange.RangeType = isFunction ? TypeOfCodeRange.Function : TypeOfCodeRange.Trigger;
        }
        private void StartCodeRange2(int i, string line, ref CodeRange actualCodeRange,
            bool isFunction, string triggerName)
        {
            actualCodeRange = new CodeRange();
            //int x = line.IndexOfLastChar(codeString);
            actualCodeRange.Start = new Place(0, i);

            actualCodeRange.RangeName = triggerName;
            actualCodeRange.RangeType = isFunction ? TypeOfCodeRange.Function : TypeOfCodeRange.Trigger;
        }
        protected void CollectElementsCode(ref bool isCodeLines, ref bool isLastCodeLine, 
            ref Function actualFunction,
            ref CodeRange actualCodeRange, int currentLineOnActualZone,
            int i, string line, ref bool isVarZone, int RecId)
        {
            if (i == 174)
            {
                var p = 0;
            }
            if (line.IsNullOrEmpty()) return;

            if (line.StartsWith(@"    BEGIN"))
            {
                isVarZone = false;
            }

            ProcessCodeFunction(ref isCodeLines, ref isLastCodeLine, i, line,
                ref isVarZone, ref actualFunction, navObj.GlobalVariables, ref actualCodeRange, RecId);

            if (!isCodeLines)
            {
                if (currentLineOnActualZone <= 1)
                {
                    AddDocRange(i, line);//Ajout de la 1ere ligne (BEGIN)
                }
                if (IsStartOfFunctionLine(line))
                {
                    isVarZone = false;

                    CodeRange range = new CodeRange(i, line);
                    range.RangeName = actualFunction.FunctionName;
                    range.RangeType = TypeOfCodeRange.Function;
                    range.IsFunctionDefinition = true;
                    range.LocalVariables = GetAllParameters(line, i);

                    if (range.LocalVariables.Count > 0)
                    {
                        tempLocalVariables.AddRange(range.LocalVariables);
                    }

                    //PlacesOfCode.Add(range);
                    AddCodeRange(range, RecId);
                }
            }
            AddEndOfZoneCodeRange(i, line, RecId);
        }
        protected void AddEndOfZoneCodeRange(int i, string line,int RecId)
        {
            if (IsEndOfZone(line))
            {
                if (line.StartsWith(@"  END"))
                {
                    int x = line.IndexOfLastChar(@"  END");
                    var start = new Place(0, i);
                    var end = new Place(x, i);
                    AddCodeRange(new CodeRange(start, end), RecId);
                }
            }
        }
        protected void AddEndOfFileCodeRange(int fromLine)
        {
            var start = new Place(0, fromLine);
            var end = new Place(10, navObj.NosOfLines - 1);
            AddCodeRange(new CodeRange(start, end), 0);
            //PlacesOfCode.Add(new CodeRange(start, end));
        }
        protected bool IsEndOfZone(string line)
        {
            return line.StartsWith(@"  END");
        }
        private void StartCodeRange(int i, ref CodeRange actualCodeRange, TextBlockCodeType TriggerType
            , string actualCodeTriggerName, bool isFunction)
        {
            actualCodeRange = new CodeRange();
            actualCodeRange.Start = new Place(0, i);

            var trigger = AddTrigger(i, TriggerType, actualCodeTriggerName);

            actualCodeRange.RangeName = trigger.Name;
            actualCodeRange.RangeType = isFunction ? TypeOfCodeRange.Function : TypeOfCodeRange.Trigger;
        }

        protected void AddEndOfFileCodeRange(string[] lines)
        {
            int nbre = 0;
            for (int k = lines.Length - 1; k >= 0; k--)
            {
                nbre++;
                if (nbre > 10)
                {
                    throw new Exception("Cannot find end of file");
                }
                var line = lines[k];
                //if (line.StartsWith("    BEGIN"))
                if (line.StartsWith("  END"))
                    {
                    AddEndOfFileCodeRange(k);
                    break;
                }
            }
        }
        protected Place SaveOtherZone(ref Place startZone, int i, TypeOfCodeRange type)
        {
            Place endZone = new Place("  END".Length, i - 1);
            InsertZone(startZone, endZone, type);
            startZone = new Place(0, i);
            return endZone;
        }
        protected void InsertZone(Place startZone, Place endZone, TypeOfCodeRange type)
        {
            var r = new CodeRange(startZone, endZone);
            r.RangeType = type;
            navObj.OthersPlaces.Add(r);
        }
        protected void InsertGlobalVariable_DataItem(string line, ref Variable actualVariableElement)
        {
            if (line.Contains(";DataItem;"))
            {
                actualVariableElement = new Variable();
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
                if (tab.Length > 3)
                {
                    var itemName = tab[3].Trim();
                    if (itemName.IsNotNullOrEmpty())
                    {
                        newDataItem.Name = itemName;
                        actualVariableElement.Name = itemName;
                    }
                }
                navObj.DataItems.Add(newDataItem);

            }
            if (navObj.Type == ObjectType.Query)
            {
                if (line.Contains(";Filter") || line.Contains(";Column"))//Query only
                {
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
                    //        //actualVariableElement.Name = itemName;
                    //    }
                    //}
                    navObj.QueryColumnAndFilters.Add(newDataItem);

                }
                if (line.Contains("DataSource="))
                {
                    var tab = line.FormatEndOfElement().RemoveAtBorders("[", "]").Split('=');
                    if (tab.Length > 1)
                    {
                        var colStr = (tab[1].Trim());

                        var lastQueryFilter = navObj.QueryColumnAndFilters.LastOrDefault();
                        if (lastQueryFilter != null)
                        {
                            lastQueryFilter.Name = colStr;
                            var dernItem = navObj.DataItems.LastOrDefault();
                            if (dernItem != null)
                            {
                                lastQueryFilter.DataItemTable = dernItem.DataItemTable;
                            }
                        }

                    }

                }
            }
            if (line.Contains("DataItemTable="))
            {
                var tab = line.FormatEndOfElement().Split('=');
                if (tab.Length > 1)
                {
                    var idTable = tab[1].Replace("Table", "").Trim().FormatEndOfElement();
                    int idObj = 0;
                    int.TryParse(idTable, out idObj);

                    if (actualVariableElement != null)
                    {
                        actualVariableElement.ID = idObj;
                        actualVariableElement.Type = VariableType.Record;

                        var obj = navObj.NavProject.GetObject(idObj, ObjectType.Table);
                        if (obj != null)
                        {
                            if (actualVariableElement.Name.IsNullOrEmpty())
                            {
                                string objName = obj.Name;
                                if(navObj.Type== ObjectType.Query)
                                {
                                    objName = FormatNameQueryDataItem(objName);
                                }
                                actualVariableElement.Name = objName;
                            }
                        }
                    }


                    var dernItem = navObj.DataItems.LastOrDefault();
                    if (dernItem != null)
                    {
                        if (dernItem.Name.IsNullOrEmpty())
                        {
                            var obj = navObj.NavProject.GetObject(idObj, ObjectType.Table);
                            if (obj != null)
                            {
                                string objName = obj.Name;
                                if (navObj.Type == ObjectType.Query)
                                {
                                    objName = FormatNameQueryDataItem(objName);
                                }
                                dernItem.Name = objName;
                            }
                        }
                        dernItem.DataItemTable = idObj;
                    }
                }
            }     
            if (line.Contains("DataItemLinkReference="))
            {
                var dernItem = navObj.DataItems.LastOrDefault();
                if (dernItem != null)
                {
                    var expr = line.Trim().FormatEndOfElement();
                    var tab = expr.Split('=');
                    if (tab.Length > 1)
                    {
                        dernItem.DataItemLinkReference = tab[1].Trim();
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

        private string FormatNameQueryDataItem(string objName)
        {
            objName = objName.Replace(". ", "_").Replace(".", "_").Replace(" ", "_");
            return objName;
        }
    }
}
