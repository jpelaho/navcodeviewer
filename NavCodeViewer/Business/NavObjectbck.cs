using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NavCodeViewer.Business
{
    public class NavObject 
    {
        public ObjectType Type { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        
        public int NosOfLines;
        private int _SourceTableID;
        public Project NavProject { get; set; }
        public List<Field> FieldList { get; set; } = new List<Field>();
        public List<Function> FunctionList { get; set; } = new List<Function>();
        public List<Variable> GlobalVariables { get; set; } = new List<Variable>();
        public List<Trigger> Triggers { get; set; } = new List<Trigger>();
        public List<CodeRange> PlacesOfCode { get; set; } = new List<CodeRange>();
        public List<CodeRange> PlacesOfSourceExpr { get; set; } = new List<CodeRange>();
        public List<CodeRange> PlacesOfCodeWithRecId
        {
            get
            {
                return PlacesOfCode.Where(c => c.RecId != 0).ToList();
            }
        }
        public List<CodeRange> OthersPlaces { get; set; } = new List<CodeRange>();
        public List<Reference> References { get; set; } = new List<Reference>();
        public string ObjectTextSource { get; set; }
        public string[] Tb
        {
            get
            {
                return ObjectTextSource.SplitLines();
            }
        }
        public NavObject SourceTableObject
        {
            get
            {
                return (NavObject)NavProject.GetObject(SourceTableID, ObjectType.Table);
            }
        }
        public int SourceTableID 
        { 
            get
            {
                if (_SourceTableID == 0)
                {
                    if(Type== ObjectType.Table)
                    {
                        return ID;
                    }
                    //if (Tb.IsNotNullOrEmpty())
                    //{
                    //    foreach(var line in Tb)
                    //    {
                    //        if (line.Contains("SourceTable="))
                    //        {
                    //            var table = line.Trim().Replace(";", "").Replace("Table", "");
                    //            int idObj = 0;
                    //            int.TryParse(table, out idObj);
                    //            if (idObj > 0)
                    //            {
                    //                _SourceTableID = NavProject.GetObjectID(idObj, ObjectType.Table);
                    //                return _SourceTableID;
                    //            }
                    //        }
                    //    }
                    //}
                }
                return _SourceTableID;
            }
            set => _SourceTableID = value; }
        private List<Variable> tempLocalVariables = new List<Variable>();
        TableRelationRefMgt tableRelationRefMgt;
        OthersRefMgt othersRefMgt;
        FlowFieldRefMgt flowFieldRefMgt;
        CodeRefMgt codeRefMgt;
        
        public List<DataItem> DataItems = new List<DataItem>();
        public List<DataItem> ElementsItems = new List<DataItem>();
        private DataItem actualDataItem; private DataItem actualElementItem;

        private int activeTableIDForRunObject = 0;
        private bool RunPageView_IsCollect;
        private bool RunPageLink_IsCollect;
        private CodeRange activeRangeForRunPageView = null;
        private string activeExprForRunObjectRunPageView = "";
        private CodeRange activeRangeForRunPageLink = null;
        private string activeExprForRunObjectRunPageLink = "";
        private ObjectType menuSuite_RunObjectType = ObjectType.Page;


        public NavObject()
        {
            tableRelationRefMgt = new TableRelationRefMgt(this);
            othersRefMgt = new OthersRefMgt(this);
            flowFieldRefMgt = new FlowFieldRefMgt(this);
            codeRefMgt = new CodeRefMgt(this);
        }

        #region Format
        public string FormatObjectsProperties(string source)
        {
            source = Regex.Replace(source, @"(?<=\s\s){", "BEGIN");
            source = Regex.Replace(source, @"(?<=\s\s)}", "END");
            return source;
        }
        public string FormatProperties(string source)
        {
            source = Regex.Replace(source, @"(?<=\s\s){", "BEGIN");
            source = Regex.Replace(source, @"(?<=\s\s)}", "END");
            return source;
        }
        
        public string FormatCode(string source)
        {
            source = source.TrimEnd();
            string response = @"  CODE" + Environment.NewLine + "  BEGIN" + Environment.NewLine;
            List<string> varSource = new List<string>();
            string[] lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0 || i == 1) continue;
                var line = lines[i];
                response += line + Environment.NewLine;
            }
            response += @"  END" + Environment.NewLine;
            return response;
        }
    
        
        public Place SaveZone(ref Place startZone, int i, TypeOfCodeRange type)
        {
            Place endZone = new Place("  END".Length, i - 1);
            InsertZone(startZone, endZone, type);
            startZone = new Place(0, i);
            return endZone;
        }
        private void InsertZone(Place startZone, Place endZone, TypeOfCodeRange type)
        {
            var r = new CodeRange(startZone, endZone);
            r.RangeType = type;
            OthersPlaces.Add(r);
        }
        #endregion

        #region Collect Fields, Triggers, Functions, Variables Parameters
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
            var str = Regex.Matches(line, @"(?<=\b(PROCEDURE)\s+)(?<range>\w+?)\b", RegexOptions.Compiled);
            if (str.Count > 0)
            {
                return str[0].Value;
            }
            return "";
        }
        private string GetFunctionParametersString(string line)
        {
            //if (line.Contains("TempInvoicingSpecification@1000"))
            //{
            //    int p = 0;
            //}
            var str = Regex.Matches(line, @"(?<=.*)\(.+\)", RegexOptions.Compiled);
            if (str.Count > 0)
            {
                return str[0].Value;
            }
            return "";
        }
        protected List<Variable> GetAllParameters(string line, int iLine)
        {
            List<Variable> rep = new List<Variable>();
            var parameters = GetFunctionParametersString(line).Replace("(", "").Replace(")", "");
            if (parameters.IsNullOrEmpty())
            {
                return rep;
            }
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
            return rep;
            //var str = Regex.Matches(line, @"(?<=.*)\(.+\)", RegexOptions.Compiled);
            //if (str.Count > 0)
            //{
            //    return str[0].Value;
            //}
            //return "";
        }
        private void CollectVariable(string line, Variable variable, int iLine)
        {
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
                    variable.Name = name.Replace("VAR ", "").Trim();
                    //var typeData = variableData[2].Replace("TEMPORARY", "").Trim().Split(' ');
                    var typeData = variableData[2].Trim().Split(' ');
                    if (typeData.Length >= 2)
                    {
                        var strType = typeData[typeData.Length - 2].Trim();
                        var Id = typeData[typeData.Length - 1].Trim();
                        int IdVar = 0;
                        variable.SetTypeFromText(strType);
                        Int32.TryParse(Id, out IdVar);
                        variable.ID = IdVar;
                    }
                    else
                    {
                        variable.Type = VariableType.Other;
                    }
                    variable.DefinitionLine = iLine;
                }
            }
        }
        public void ProcessSourceExpr(int i, string line, int RecId)
        {
            ProcessSourceExpr_DataSource("SourceExpr=",i, line, RecId);
        }
        public void ProcessDataSource(int i, string line, int RecId)
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
                PlacesOfSourceExpr.Add(r);
            }
        }

        public void ProcessCodeTrigger(ref bool isCodeLines, ref bool isLastCodeLine, ref bool isVarZone,
            int i, string line,
            ref CodeRange actualCodeRange, ref TextBlockCodeType actualCodeType, ref string actualCodeTriggerName,
            TextBlockCodeType codeType, string TextStartOfTrigger,
            string TextBegin_WhenVar, string TextEndOfTrigger,int RecId)
        {
            if (i == 1897)
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
            var startTriggerCodeAfterVar = false;
            if(isVarZone && line.Trim() == "BEGIN" && actualCodeType == codeType)
            {
                startTriggerCodeAfterVar = true;
            }

            
            var txtMarker = TextStartOfTrigger.Replace("BEGIN", "");
            //if (line.StartsWith(TextStartOfTrigger) || line.Contains(";" + txtMarker.TrimStart()))
            if (line.StartsWith(TextStartOfTrigger))
            {
                isCodeLines = true;
                isVarZone = false;
                actualCodeType = codeType;
                StartCodeRange(i, line, TextStartOfTrigger.Replace("BEGIN", ""),
                    ref actualCodeRange, actualCodeType, actualCodeTriggerName);
            }
            if (startTriggerCodeAfterVar)
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
            if (line.Contains(TextStartOfTrigger.Replace("BEGIN", "")))
            {
                actualCodeType = codeType;
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

        public void ProcessCodeFunction(ref bool isCodeLines, ref bool isLastCodeLine, int i,
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
                actualFunction.ObjectID = ID;
                actualFunction.ObjectType = Type;
                actualFunction.Private = IsPrivateFunction(line);
            }
            if (line.StartsWith(@"    END;")|| line.StartsWith(@"    END."))
            {
                isLastCodeLine = true;
                if (actualFunction != null)
                {
                    actualFunction.EndingDefLine = i + 1;
                    FunctionList.Add(actualFunction);
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
        public void AddDocRange(int iLine, string LineText)
        {
            var r = new CodeRange(iLine, LineText);
            PlacesOfCode.Add(r);
        }
        public void AddCodeRange(CodeRange r,int recId)
        {
            //var r = new CodeRange(iLine, LineText);
            r.RecId = recId;
            PlacesOfCode.Add(r);
        }
        private void StartCodeRange(int i, string line, string codeString,
            ref CodeRange actualCodeRange,
            TextBlockCodeType TriggerType, string actualCodeTriggerName)
        {
            var trigger = AddTrigger(i, TriggerType, actualCodeTriggerName);

            StartCodeRange(i, line, codeString, ref actualCodeRange, false, trigger.Name);
        }
        public void CollectElementsObjectsProperties(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line,
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
        public void UpdateActualZone(string line, ref TextBlockType actualZone1, ref bool isVarZone
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
                DefLine = i,
                Prefix = actualCodeTriggerName
            };
            Triggers.Add(item);
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
        public void CollectElementsCode(ref bool isCodeLines, ref bool isLastCodeLine, 
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
                ref isVarZone, ref actualFunction, GlobalVariables, ref actualCodeRange, RecId);

            if (!isCodeLines)
            {
                if (currentLineOnActualZone <= 1)
                {
                    //PlacesOfCode.Add(new CodeRange(i, line));
                    AddDocRange(i, line);
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
        public void AddEndOfZoneCodeRange(int i, string line,int RecId)
        {
            if (IsEndOfZone(line))
            {
                if (line.StartsWith(@"  END"))
                {
                    int x = line.IndexOfLastChar(@"  END");
                    var start = new Place(0, i);
                    var end = new Place(x, i);
                    //PlacesOfCode.Add();
                    AddCodeRange(new CodeRange(start, end), RecId);
                }
            }
        }
        public bool IsEndOfZone(string line)
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
        protected void CreateNewObject(string line)
        {
            var words = line.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (i == 1) Type = GetTypeFromText(words[i]);
                if (i == 2) ID = Convert.ToInt32(words[i]);
                if (i > 2)
                {
                    if (Name.IsNullOrEmpty())
                    {
                        Name = words[i];
                    }
                    else
                    {
                        Name += " " + words[i];
                    }
                }
            }
        }
        ObjectType GetTypeFromText(string type)
        {
            if (type == "Page") return ObjectType.Page;
            if (type == "Codeunit") return ObjectType.CodeUnit;
            if (type == "MenuSuite") return ObjectType.MenuSuite;
            if (type == "Query") return ObjectType.Query;
            if (type == "Report") return ObjectType.Report;
            if (type == "Table") return ObjectType.Table;
            if (type == "XMLport") return ObjectType.XmlPort;
            return ObjectType.Table;
        }
        #endregion

        #region Collect References
        private Reference CreateRef()
        {
            return new Reference
            {
                RefBy_ObjetID = ID,
                RefBy_ObjetType = Type
            };
        }
        public void CollectReferences()
        {
            if (ObjectTextSource.IsNullOrEmpty())
            {
                throw new Exception("ObjectTextSource is null");
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

            new CodeRefMgt(this).FindReferencesInCode();
        }
        private void FindReferencesNotInCode()
        {
            for (int index = 0; index < OthersPlaces.Count; index++)
            {
                var range = OthersPlaces[index];

                if (range.RangeType == TypeOfCodeRange.PropertiesDef ||
                    range.RangeType == TypeOfCodeRange.FieldsDef ||
                    range.RangeType == TypeOfCodeRange.ControlsDef ||
                    range.RangeType == TypeOfCodeRange.MenuNodesDef)
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
            for (int index = 0; index < PlacesOfSourceExpr.Count; index++)
            {
                var range = PlacesOfSourceExpr[index];
                var rangeText = range.GetRangeText(Tb).FormatEndOfElement();

                codeRefMgt. FindReferenceInSourceExpr(rangeText,range, 0);                
            }
        }
        private void CollectRef_NonCodeRange(CodeRange range)
        {
            var rangeText = range.GetRangeText(Tb);
            var lines = rangeText.SplitLines();
            var start = new Place(0, 0);
            var end = new Place(0, 0);
            NonCodeReferenceType typeRef = NonCodeReferenceType.None;
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim Here
                var line = lines[i];

                SearchAllReferences(ref start, ref typeRef, i, line);

                if (typeRef != NonCodeReferenceType.None)
                {
                    if (line.Length >= start.iChar)
                    {
                        int k = line.IndexOfAny(new char[] { '}', ';' }, start.iChar);
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

            //report            
            SearchForReference(ref start, ref typeRef, i, line, "DataItemTable=", NonCodeReferenceType.DataItemTable);
            
            //Menusuite
            SearchForReference(ref start, ref typeRef, i, line, "RunObjectType=", NonCodeReferenceType.RunObjectType);
            SearchForReference(ref start, ref typeRef, i, line, "RunObjectID=", NonCodeReferenceType.RunObjectID);
        }
        private void CollectRef_DataItems(CodeRange rangeZone)
        {
            var rangeText = rangeZone.GetRangeText(Tb);
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
                if (line.Contains(";DataItem;"))
                {
                    
                    var newDataItem = new DataItem();
                    var tab = line.Split(';');

                    actualDataItem = DataItems.Where(c => c.IDRef == tab[0]).FirstOrDefault();

                    //string strNiveau = ""; int idNiveau = 0;
                    //if (tab.Length > 1)
                    //{
                    //    strNiveau = tab[1];
                    //    int.TryParse(strNiveau, out idNiveau);
                    //    newDataItem.Niveau = idNiveau;
                    //}
                    //if (tab.Length > 3)
                    //{
                    //    var itemName = tab[3].Trim();
                    //    if (itemName.IsNotNullOrEmpty())
                    //    {
                    //        newDataItem.Name = itemName;
                    //    }
                    //}
                    //DataItems.Add(newDataItem);
                }
                if (line.Contains("DataItemTable="))
                {
                    //var dernItem = actualDataItem;
                    var tab = line.FormatEndOfElement().Split('=');
                    string strTab = ""; int idTable = 0;
                    if (tab.Length > 1)
                    {
                        strTab = tab[1];
                        int.TryParse(strTab.Replace("Table",""), out idTable);
                        AddReference(idTable.ToString(), rangeZone.FromLine + i, RefType.ExternalObject, idTable, ObjectType.Table);
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

                    //    dernItem.DataItemTable = idTable;
                    //}
                }
                SearchForReference(ref start, ref typeRef, i, line, "DataItemTableView=", NonCodeReferenceType.DataItemTableView);
                SearchForReference(ref start, ref typeRef, i, line, "DataItemLinkReference=", NonCodeReferenceType.DataItemLinkReference);
                SearchForReference(ref start, ref typeRef, i, line, "DataItemLink=", NonCodeReferenceType.DataItemLink);
                SearchForReference(ref start, ref typeRef, i, line, "ReqFilterFields=", NonCodeReferenceType.ReqFilterFields);
                SearchForReference(ref start, ref typeRef, i, line, "CalcFields=", NonCodeReferenceType.CalcFields);
                SearchForReference(ref start, ref typeRef, i, line, "SourceExpr=", NonCodeReferenceType.SourceExpr);





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
                            //var dernItem = DataItems.LastOrDefault();
                            //if (dernItem != null)
                            //{
                            //    if (expr != "")
                            //    {
                            //        //if (typeRef != NonCodeReferenceType.SourceExpr)
                            //        {
                            //            expr = expr.FormatEndOfElement();
                            //        }
                            //        dernItem.DataItemLinkReference = expr;
                            //    }
                            //}
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
                                        AddReference(f, refLine, RefType.ExternalField,
                                           dernItem.DataItemTable, ObjectType.Table);
                                    }
                                    //dernItem.DataItemLinkReference = expr;
                                }
                            }
                        }
                        if (typeRef == NonCodeReferenceType.DataItemLink)
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
                                    var parent = DataItems.Where(c => c.Name == itemFils.DataItemLinkReference).LastOrDefault();
                                    if (parent != null)
                                    {
                                        IdItemTableParent = parent.DataItemTable;
                                    }
                                    else
                                    {
                                        var itemParent = DataItems.Where(c => c.Niveau == itemFils.Niveau - 1).LastOrDefault();
                                        if (itemParent != null)
                                        {
                                            IdItemTableParent = itemParent.DataItemTable;
                                        }
                                    }
                                }
                                else
                                {
                                    var itemParent = DataItems.Where(c => c.Niveau == itemFils.Niveau - 1).LastOrDefault();
                                    if (itemParent != null)
                                    {
                                        IdItemTableParent = itemParent.DataItemTable;
                                    }
                                }                               
                                othersRefMgt.InsertRefDataItemLink(expr, r, rangeZone, itemFils.DataItemTable,IdItemTableParent);
                            }
                        }
                        if (typeRef == NonCodeReferenceType.SourceExpr)
                        {
                            //var dernItem = DataItems.LastOrDefault();
                            //if (dernItem != null)
                            //{
                            //    if (expr != "")
                            //    {
                            //        expr = expr.Replace(";", "");
                            //        dernItem.DataItemLinkReference = expr;
                            //    }
                            //}
                            //codeRefMgt.FindReferenceInSourceExpr(expr, rangeZone, i);
                        }

                        typeRef = NonCodeReferenceType.None;
                    }
                }
            }
        }
        private void CollectRef_ElementsItems(CodeRange rangeZone)
        {
            var rangeText = rangeZone.GetRangeText(Tb);
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
                    actualElementItem = ElementsItems.Where(c => c.IDRef == tab[0]).FirstOrDefault();

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
                        AddReference(idTable.ToString(), rangeZone.FromLine + i, RefType.ExternalObject, idTable, ObjectType.Table);
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

                            var v = GlobalVariables.Where(c=>c.Name== expr).FirstOrDefault();
                            if (v != null)
                            {
                                int refLine = r.Start.iLine + rangeZone.FromLine;
                                AddReference(v.Name, refLine, RefType.ExternalField, v.ID);
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
                                        AddReference(fName, refLine, RefType.ExternalField,
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
                                    var parent = ElementsItems.Where(c => c.Name == itemFils.DataItemLinkReference).LastOrDefault();
                                    if (parent != null)
                                    {
                                        IdItemTableParent = parent.DataItemTable;
                                    }
                                    else
                                    {
                                        var itemParent = ElementsItems.Where(c => c.Niveau == itemFils.Niveau - 1).LastOrDefault();
                                        if (itemParent != null)
                                        {
                                            IdItemTableParent = itemParent.DataItemTable;
                                        }
                                    }
                                }
                                else
                                {
                                    var itemParent = ElementsItems.Where(c => c.Niveau == itemFils.Niveau - 1).LastOrDefault();
                                    if (itemParent != null)
                                    {
                                        IdItemTableParent = itemParent.DataItemTable;
                                    }
                                }
                                othersRefMgt.InsertRefElementLinkFields(expr, r, rangeZone, itemFils.DataItemTable, IdItemTableParent);
                            }
                        }


                        typeRef = NonCodeReferenceType.None;
                    }
                }
            }
        }
        private string GetFieldName(int tableID,string fieldNoStr)
        {
            var fieldId = Convert.ToInt32(fieldNoStr.FormatEndOfElement().Replace("Field", ""));
            var obj = NavProject.GetObject(tableID, ObjectType.Table);
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
                        SourceTableID = othersRefMgt.InsertRefSourceTable(expr, r, rangeZone);
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
                        flowFieldRefMgt.InsertRef_CalcFormula(expr, r, rangeZone);
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
                        if (SourceTableID == 0)
                        {
                            throw new Exception("Error on SourceTable");
                        }
                        othersRefMgt.InsertRefSourceTableView(expr, r, rangeZone, SourceTableID);
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
                        //othersRefMgt.InsertRef_InternalField(expr, r, rangeZone);
                        //codeRefMgt.FindReferenceInTextCode(expr,rangeZone);
                        //codeRefMgt.FindReferenceInSourceExpr(expr, rangeZone, r.Start.iLine);
                        break;
                    }
                case NonCodeReferenceType.SubPageLink:
                    {
                        var PagePartID_SourceTable = GetPagePartSourceTableID(rangeZone, r);
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
            for (int i = startLine; i < Tb.Length; i++)
            {
                var line = Tb[i];
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
                    string source = r2.GetRangeText(Tb);
                    var PageID = othersRefMgt.GetPageID_PagePartID(source);
                    if (PageID > 0)
                    {
                        var objPartPage = NavProject.GetObject(PageID, ObjectType.Page);
                        if (objPartPage != null)
                        {
                            PagePartID_SourceTable = ((NavObject)objPartPage).SourceTableID;
                            break;
                        }
                    }
                    break;
                }
            }

            return PagePartID_SourceTable;
        }
        private int GetElementSourceTableID(CodeRange rangeZone, CodeRange r)
        {
            int PagePartID_SourceTable = 0;
            int startLine = rangeZone.FromLine + r.Start.iLine;
            for (int i = startLine; i < Tb.Length; i++)
            {
                var line = Tb[i];
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
                    string source = r2.GetRangeText(Tb).FormatEndOfElement();
                    var var = GlobalVariables.Where(c => c.Name == source).FirstOrDefault();
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

         
        public void AddReference(string name, int refLine, RefType refType, 
            int objID = 0, ObjectType objType = ObjectType.Table, 
            CodeRange scope = null,string optName="")
        {
            if (objID == 2000000026) return;//Integer Table for Report DataItem

            var intRef = CreateRef();
            intRef.Name = name;
            intRef.ReferenceLine = refLine;
            intRef.ReferenceType = refType;
            intRef.Ref_ObjectID = objID;
            intRef.Ref_ObjectType = objType;
            intRef.Scope = scope;
            intRef.OptionName = optName;

            References.Add(intRef);
        }
        private void CollectObjectsRefAsLocalVars()
        {
            var rangesInLocalVar = PlacesOfCode.Where(c => c.IsFunctionDefinition == false).ToList();
            foreach (var p in rangesInLocalVar)
            {
                var objs = p.LocalVariables.Where(c => c.Type != VariableType.Other).ToList();
                foreach (var o in objs)
                {
                    var r = CreateRef();
                    r.ReferenceLine = o.DefinitionLine;
                    r.ReferenceType = RefType.ExternalObject;
                    r.Ref_ObjectType = o.GetObjectType;
                    r.Ref_ObjectID = o.ID;
                    r.Name = o.Name.RemoveQuotes();
                    //r.ReferenceScope = RefScope.Externe;

                    References.Add(r);
                }
            }
        }
        private void CollectObjectsRefAsGlobalVars()
        {
            var objs = GlobalVariables.Where(c => c.Type != VariableType.Other).ToList();
            foreach (var o in objs)
            {
                var r = CreateRef();
                r.ReferenceLine = o.DefinitionLine;
                r.ReferenceType = RefType.ExternalObject;
                r.Ref_ObjectType = o.GetObjectType;
                r.Ref_ObjectID = o.ID;
                r.Name = o.Name.RemoveQuotes();
                //r.ReferenceScope = RefScope.Externe;

                References.Add(r);
            }
        }
        private void CollectObjectsRefAsFunctionParameter()
        {
            var rangesWithParameters = PlacesOfCode.Where(c => c.IsFunctionDefinition).ToList();
            foreach (var p in rangesWithParameters)
            {
                var objs = p.LocalVariables.Where(c => c.Type != VariableType.Other).ToList();
                foreach (var o in objs)
                {
                    var r = CreateRef();
                    r.ReferenceLine = o.DefinitionLine;
                    r.ReferenceType = RefType.ExternalObject;
                    r.Ref_ObjectType = o.GetObjectType;
                    r.Ref_ObjectID = o.ID;
                    r.Name = o.Name.RemoveQuotes();
                    //r.ReferenceScope = RefScope.Externe;

                    References.Add(r);
                }
            }
        }
        public void InsertMarker(List<Marker> markers, string expr, int iLine)
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
        #endregion
    }
}
