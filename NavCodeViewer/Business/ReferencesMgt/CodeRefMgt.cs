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
    public class CodeRefMgt
    {
        Stack<EndBeginItem> PileEndBegin = new Stack<EndBeginItem>();
        NavObject _navObject;
        NavObjectCollectRefs _collectRefs;
        private string[] _CalKeywords = null;
        private bool containsWith = false;
        private string[] CalKeywords
        {
            get
            {
                if (_CalKeywords == null)
                {
                    //Added : Rec ; xRec
                    var arr = @"OnRun|VAR|Record|LOCAL|WITH|DO|BEGIN|IF|THEN|EXIT|END|CASE|
                    |repeat|until|AssertError|DownTo|Else|Event|False|For|Local|true|while|to|Rec|xRec";
                    _CalKeywords = arr.Split('|');
                }
                return _CalKeywords;
            }
        }
        private string[] _CalSystemFunction = null;
        //Remove setfilter setrange find insert delete deleteall modify modifyall
        private string[] CalSystemFunction
        {
            get
            {
                if (_CalSystemFunction == null)
                {
                    var arr = @"Type|Abs|Activate|Active|Addlink|Addtext|Applicationpath|ArrayLen|Ascending|Beep|Break|CalcDate|CalcField|CalcSum|CalcSums|ChangeCompany|CheckLicenseFile|
                        |Class|Clear|ClearAll|ClearLastError|ClearMarks|ClientType|Close|ClosingDate|CodeCoverageLog|CommandLine|Commit|CompanyName|CompressArray|Confirm|
                        |Consistent|ContextURL|ConvertStr|Copy|CopyArray|CopyFilter|CopyFilters|CopyLinks|CopyStr|CopyStream|Count|CountApprox|Create|CreateDateTime|CreateGUID|
                        |CreateInstream|CreateOutstream|CreateTempFile|CurrentClientType|CurrentDateTime|CurrentExecutionMode|CurrentKey|CurrentKeyIndex|CurrentTransactionType|
                        |Database|Date2DMY|Date2DWY|DaTi2Variant|Debugger|DecimalPlacesMax|DecimalPlacesMin|DefaultClientType|DelChr|Delete|DeleteAll|DeleteLink|DeleteLinks|
                        |DelStr|DMY2Date|Download|DownloadFromStream|DT2Date|DT2Time|Duplicate|DWY2Date|Environ|EOS|Erase|Error|Evaluate|Exists|Export|ExportObjects|Field|
                        |FieldActive|FieldCaption|FieldCount|FieldError|FieldExist|FieldIndex|FieldName|FieldNo|FilterGroup|Find|FindFirst|FindLast|FindSet|Get|GetFilter|GetFilters|
                        |GetLastErrorText|GetPosition|GetRangeMax|GetRangeMin|GetRecord|GetStamp|GetSubtext|GetTable|GetURL|GetView|GlobalLanguage|GUIAllowed|HasFilter|HasLinks|
                        |HasValue|Hyperlink|ImportObjects|IncStr|Init|Input|Insert|InsStr|IsAction|IsAutomation|IsBinary|IsBoolean|IsChar|IsClear|IsCode|IsCodeunit|IsDate|
                        |IsDateFormula|IsDecimal|IsEmpty|IsFile|IsInstream|IsInteger|IsNullGUID|IsOption|IsOutstream|IsRecord|IsText|IsTime|IsTransactionType|KeyCount|KeyGroupDisable|
                        |KeyGroupEnable|KeyGroupEnabled|KeyIndex|Language|Len|Length|LockTable|LockTimeout|Lowercase|Mark|MarkedOnly|MaxStrLen|Message|Modify|ModifyAll|Next|
                        |NormalDate|Number|ObjectType|Open|OSVersion|PadStr|PAGENO|PAPERSOURCE|Pos|Power|Preview|QueryReplace|Quit|Random|Randomize|Read|ReadConsistency|
                        |ReadPermission|ReadText|RecordLevelLocking|Relation|Rename|Reset|Round|RoundDateTime|Run|RunModal|SaveAsExcel|SaveAsHTML|SaveAsPDF|SaveAsXML|SaveRecord|
                        |Seek|SelectLatestVersion|SelectStr|SerialNumber|SetAutoCalcFields|SetCurrentKey|SetFilter|SetPermissionFilter|SetPosition|SetRange|SetRecFilter|SetRecord|
                        |SetSelectionFilter|SetStamp|SetTable|SetTableView|SetView|Shell|SID|Skip|Sleep|STARTSESSION|STOPSESSION|StrCheckSum|StrLen|StrMenu|StrPos|StrSubstNo|
                        |SynchronizeAllLogins|SynchronizeSingleLogin|TableCaption|TableName|TemporaryPath|TestField|TextEncoding|TextMode|TextPos|Today|TOTALSCAUSEDBY|TransferFields|
                        |Trunc|Update|UpdateControls|UpdateEditable|UpdateFontBold|UpdateForeColor|UpdateIndent|UpdateSelected|Upload|UploadIntoStream|Uppercase|UserID|
                        |Validate|Value|VariableActive|Variant2Date|Variant2Time|WindowsLanguage|WordDate|Write|WriteMode|WritePermission|WriteText|Yield";
                    _CalSystemFunction = arr.Split('|');
                }
                return _CalSystemFunction;
            }
        }
        private string activeWithRecord="";
        public CodeRefMgt(NavObject _navObject1, NavObjectCollectRefs _collectRefs1)
        {
            _navObject = _navObject1;
            _collectRefs = _collectRefs1;
        }
        public class EndBeginItem
        {
            public string Marker { get; set; }
            public string WithTableName { get; set; }
        }
        public void FindReferencesInCode()
        {
            List<CodeRange> codePlaces = _navObject.PlacesOfCode.Where(c => c.CanHaveReference).ToList();
            for (int index = 0; index < codePlaces.Count; index++)
            {
                //if (index == 34)
                //{
                //    int p = 0;
                //}
                if (_navObject.Type == ObjectType.Page && _navObject.ID == 563)
                {
                    int p = 0;
                }
                var range = codePlaces[index];
                var rangeText = range.GetRangeText(_navObject.Tb);
                if(rangeText.Contains(@"IF GLEntry.READPERMISSION THEN BEGIN"))
                {
                    int m = 0;
                }
                rangeText = RemoveComments(rangeText);
                InitPileEndBegin();
                FindReferenceInTextCode(rangeText, range);
            }
        }

        private void InitPileEndBegin()
        {
            PileEndBegin.Clear();
            containsWith = false;
            activeWithRecord = "";
        }

        public void FindReferenceInSourceExpr(string rangeText, CodeRange ZoneRange,int iLine)
        {
            if (_navObject.Type == ObjectType.XmlPort && _navObject.ID == 1)
            {
                int p = 0;
            }

            var cr = new CodeRange(0, rangeText);
            var r = ZoneRange.CreateSubRange(cr.Start, cr.End);
            var expr = rangeText.FormatEndOfElement();

            var markers = GetLineMarkers(expr);

            for (int k = 0; k < markers.Count; k++)
            {
                var m = markers[k];
                if (isKeyword(m.Name) || isSystemFunction(m.Name)) continue;
                if (IsPertinentOperator(m.Name)) continue;
                //int fromLine = ZoneRange.FromLine + iLine + m.Order;
                int fromLine = ZoneRange.FromLine + iLine;
                processMarker(ZoneRange, r, markers, m, fromLine,k);
            }
        }

        private void processMarker(CodeRange ZoneRange, CodeRange r, 
            List<Marker> markers, Marker m, int fromLine,int markerPos)
        {
            string varName = ""; string indexZone = "";
            if (IsArrayVariable(m.Name, ref varName, ref indexZone))
            {
                var mItem = new Marker
                {
                    Name = varName,
                    Order = 0
                };
                processMarker_normalExpr(r, new List<Marker>() { mItem }, mItem, ZoneRange, fromLine, 0);

                var indices = GetLineMarkers_line(indexZone, fromLine, false);
                foreach (Marker m2 in indices)
                {
                    processMarker_normalExpr(r, new List<Marker>() { m2 }, m2, ZoneRange, fromLine, 0);
                }
            }
            else
            {
                processMarker_normalExpr(r, markers, m, ZoneRange, fromLine, markerPos);
            }
        }

        private bool IsArrayVariable(string expr,ref string varName,ref string indexZone)
        {
            if(Regex.IsMatch(expr, @"(.*)\[.*\]"))
            {
                int ind = expr.IndexOf('[');
                varName = expr.Substring(0, ind);
                indexZone= expr.Substring(ind);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void FindReferenceInTextCode(string rangeText, CodeRange ZoneRange)
        {
            var lines = rangeText.SplitLines();
            var start = new Place(0, 0);
            var end = new Place(0, 0);
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim Here
                var line = lines[i];
                //int j = line.IndexOfAny(new char[] { ';', '}' });
                int j = line.IndexOfAvoidQuotes(';');
                if (j < 0) continue;
                while (j >= 0 && j < line.Length)
                {
                    //if (line[j] == ';')
                    {
                        end = new Place(j, i);//include ;
                        var r = ZoneRange.CreateSubRange(start, end);
                        var expr = r.GetRangeText(lines);
                        expr = expr.RemoveIfStartWith(";");

                        if (expr.Contains(@"IF GLEntry.READPERMISSION THEN BEGIN"))
                        {
                            int p = 0;
                        }
                        
                        var markers = GetLineMarkers(expr);

                        for (int k = 0; k < markers.Count; k++)
                        {
                            var m = markers[k];
                            ProcessPileBeginEnd(markers, k, m);
                            if (isKeyword(m.Name) || isSystemFunction(m.Name))
                            {
                                if (!isRecordedFunction(m.Name))
                                {
                                    if (m.Name.ToLower() != "rec" && m.Name.ToLower() != "xrec")
                                    {
                                        continue;
                                    }
                                }
                            }
                            if (IsPertinentOperator(m.Name)) continue;
                            int fromLine = ZoneRange.FromLine + start.iLine + m.Order;
                            processMarker(ZoneRange, r, markers, m, fromLine,k);
                        }
                        start = new Place(j, end.iLine);
                    }
                    j = line.IndexOfAvoidQuotes(';', j + 1);
                }
            }
        }

        private void ProcessPileBeginEnd(List<Marker> markers, int k, Marker m)
        {
            if (m.Name.Equals("with", StringComparison.OrdinalIgnoreCase))
            {
                if (k + 1 < markers.Count)
                {
                    activeWithRecord = markers[k + 1].Name; 
                    containsWith = true;
                }
            }
            if (m.Name.Equals("begin", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Equals("repeat", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Equals("case", StringComparison.OrdinalIgnoreCase))
            {
                EndBeginItem item = new EndBeginItem
                {
                    Marker = m.Name,
                };
                if (activeWithRecord.IsNotNullOrEmpty())
                {
                    item.WithTableName = activeWithRecord;
                    //activeWithRecord = "";
                }
                PileEndBegin.Push(item);
                containsWith = true;
            }
            if (m.Name.Equals("end", StringComparison.OrdinalIgnoreCase) ||
               m.Name.Equals("until", StringComparison.OrdinalIgnoreCase))
            {
                PileEndBegin.Pop();
                if (activeWithRecord.IsNotNullOrEmpty())
                {
                    if (PileEndBegin.ToList().Where(c => c.WithTableName.IsNotNullOrEmpty()).Count() == 0)
                    {
                        activeWithRecord = "";
                    }
                }
            }
        }

        private void processMarker_normalExpr(CodeRange range, List<Marker> markers,
            Marker m, CodeRange scope, int fromLine,int markerPos)
        {

            Function func = null;
            Field field = null;
            RefType natureRef = RefType.None;

            int colOfRef = m.ICol;

            if(m.Name== "CollectIBAN")
            {
                int p = 0;
            }

            Variable variable = GetNatureMarker(markers, m.Name, range, ref natureRef,
                ref func, ref field, markerPos,true);

            //Rec ou xRec
            if (m.Name.ToLower() == "rec" || m.Name.ToLower() == "xrec")
            {
                
                _collectRefs.AddReference(m.Name.RemoveQuotes(), fromLine, colOfRef, RefType.ExternalObject,
                    range.RecId, ObjectType.Table, null);
                return;
            }

            //Recorded function
            if (natureRef == RefType.RecordedFunction)
            {
                if (variable != null)
                {
                    if (variable.Type == VariableType.Record && !variable.IsTempVariable)
                    {
                        ProcessRecordedFunction(m, fromLine, natureRef, variable, colOfRef);
                    }
                }
            }

            //Parameter - Variable 
            if (natureRef == RefType.Parameter
            || natureRef == RefType.LocalVariable
            || natureRef == RefType.GlobalVariable)
            {
                //Ajouter la référence interne
                if (variable.Type == VariableType.Other)
                {
                    //Ajouter la référence interne
                    _collectRefs.AddReference(m.Name.RemoveQuotes(), fromLine, colOfRef, natureRef,
                        0, ObjectType.Table, scope);
                }
                else
                {
                    //Ajouter la référence interne
                    _collectRefs.AddReference(m.Name.RemoveQuotes(), fromLine, colOfRef, natureRef,
                        variable.ID, variable.GetObjectType, scope);

                    //Si objet, alors ajouter la référnce externe
                    _collectRefs.AddReference(m.Name.RemoveQuotes(), fromLine, colOfRef, RefType.ExternalObject,
                        variable.ID, variable.GetObjectType, scope);
                }
            }

            //Fonction 
            if (natureRef == RefType.InternalFunction || natureRef == RefType.ExternalFunction)
            {
                //AddInCodeReference_Function(range, fromLine,m, func, natureRef);
                _collectRefs.AddReference(m.Name.RemoveQuotes(), fromLine, colOfRef, natureRef, func.ObjectID, func.ObjectType);
            }

            //Field 
            if (natureRef == RefType.InternalField || natureRef == RefType.ExternalField)
            {
                //AddInCodeReference_Field(range, fromLine,m, field, natureRef);
                _collectRefs.AddReference(m.Name.RemoveQuotes(), fromLine, colOfRef, natureRef, field.ObjectID, field.ObjectType);
            }

            //Autres reference objets (ex : DATABASE::Employee ...)
            if (natureRef == RefType.ExternalObject)
            {
                //AddInCodeReference_Scope(fromLine, m, variable, natureRef);
                _collectRefs.AddReference(m.Name.RemoveQuotes(), fromLine, colOfRef, natureRef,
                    variable.ID, variable.GetObjectType);
            }

            if(natureRef== RefType.OptionVariable)
            {
                //Cte."Mode de calcul"::Individuelle
                _collectRefs.AddReference(variable.Name.RemoveQuotes(), fromLine, colOfRef, natureRef,
                    variable.ID,  ObjectType.Table,null,variable.OptionName);
            }

            if(natureRef== RefType.None)
            {
                if (IsCommitFunction(m.Name))
                {
                    _collectRefs.AddReference(m.Name, fromLine, colOfRef, RefType.RecordedFunction);
                }
            }
        }

        private void ProcessRecordedFunction(Marker m, int fromLine, RefType natureRef, Variable variable,int iColRef)
        {
            string typeFunction = Reference.strInsertRef;
            if (isModifyFunction(m.Name))
            {
                typeFunction = Reference.strUpdateRef;
            }
            if (isDeleteFunction(m.Name))
            {
                typeFunction = Reference.strDeleteRef;
            }
            if (isFilterFunction(m.Name))
            {
                typeFunction = Reference.strFilterRef;
            }
            _collectRefs.AddReference(typeFunction, fromLine, iColRef, natureRef, variable.ID, variable.GetObjectType);
        }

        private bool isRecordedFunction(string name)
        {
            //return name.ToLower().Equals("insert") || name.ToLower().Equals("delete") ||
            //    name.ToLower().Equals("deleteall") || name.ToLower().Equals("modify") ||
            //    name.ToLower().Equals("modifyall") || name.ToLower().Equals("setrange") ||
            //    name.ToLower().Equals("setfilter") || name.ToLower().Equals("findset") ||
            //    name.ToLower().Equals("findfirst") || name.ToLower().Equals("findlast") ||
            //    name.ToLower().Equals("find");
            return isInsertFunction(name) || isModifyFunction(name) ||IsCommitFunction(name)
                || isDeleteFunction(name);
        }
        private bool isInsertFunction(string name)
        {
            return name.ToLower().Equals("insert");
        }
        private bool isDeleteFunction(string name)
        {
            return name.ToLower().Equals("delete") || name.ToLower().Equals("deleteall");
        }
        private bool isModifyFunction(string name)
        {
            return name.ToLower().Equals("modify") || name.ToLower().Equals("modifyall");
        }
        private bool isFilterFunction(string name)
        {
            return name.ToLower().Equals("setrange") ||
                name.ToLower().Equals("setfilter") || name.ToLower().Equals("findset") ||
                name.ToLower().Equals("findfirst") || name.ToLower().Equals("findlast") ||
                name.ToLower().Equals("find");
        }
        private bool IsScopeReference(List<Marker> markers, int i, ref VariableType type)
        {
            if (i - 1 >= 0)
            {
                var mPrec = markers[i - 1];
                if (mPrec.Name == "::")
                {
                    if (i - 2 >= 0)
                    {
                        //Comme DATABASE::Employee REPORT:: PAGE::  CODEUNIT:: XMLPORT:: QUERY::
                        var mPrec2 = markers[i - 2];
                        if (mPrec2.Name.Equals("DATABASE", StringComparison.OrdinalIgnoreCase))
                        {
                            type = VariableType.Record;
                            return true;
                        }
                        if (mPrec2.Name.Equals("REPORT", StringComparison.OrdinalIgnoreCase))
                        {
                            type = VariableType.Report;
                            return true;
                        }
                        if (mPrec2.Name.Equals("PAGE", StringComparison.OrdinalIgnoreCase))
                        {
                            type = VariableType.Page;
                            return true;
                        }
                        if (mPrec2.Name.Equals("CODEUNIT", StringComparison.OrdinalIgnoreCase))
                        {
                            type = VariableType.CodeUnit;
                            return true;
                        }
                        if (mPrec2.Name.Equals("XMLPORT", StringComparison.OrdinalIgnoreCase))
                        {
                            type = VariableType.XMLPort;
                            return true;
                        }
                        if (mPrec2.Name.Equals("QUERY", StringComparison.OrdinalIgnoreCase))
                        {
                            type = VariableType.Query;
                            return true;
                        }

                        type = VariableType.Option;
                        return true;
                        //GetNatureMarker()
                    }
                }
            }
            return false;
        }
        private bool IndexIsCorrectInArray(List<Marker> exprList, int i)
        {
            return i < exprList.Count && i >= 0;
        }
        private bool IsFieldFunction(string name)
        {
            return name.ToLower().Equals("validate") || name.ToLower().Equals("testfield") ||
                name.ToLower().Equals("fielderror") || name.ToLower().Equals("transferfields") ||
                name.ToLower().Equals("fieldname") || name.ToLower().Equals("fieldcaption") ||
                name.ToLower().Equals("fieldactive") || name.ToLower().Equals("fieldno") ||
                name.ToLower().Equals("setrange") || name.ToLower().Equals("setfilter") ||
                name.ToLower().Equals("getfilter") || name.ToLower().Equals("getrangemin") ||
                name.ToLower().Equals("copyfilter") || name.ToLower().Equals("getrangemax") ||
                name.ToLower().Equals("setcurrentkey") || name.ToLower().Equals("setascending") ||
                name.ToLower().Equals("getascending") || name.ToLower().Equals("calcfields") ||
                name.ToLower().Equals("calcsums") || name.ToLower().Equals("calcfields") ||
                name.ToLower().Equals("modifyall") || 
                name.ToLower().Equals("relation");
        }
        private bool IsRunFunction(string name)
        {
            return name.ToLower().Equals("run") || name.ToLower().Equals("runmodal") || name.ToLower().Equals("export")
                || name.ToLower().Equals("import") || name.ToLower().Equals("saveascsv")
                || name.ToLower().Equals("saveasxml");
        }
        private bool IsCommitFunction(string name)
        {
            return name.ToLower().Equals("commit");
        }
        bool IsExternalExpression(List<Marker> exprList, int i, ref string externalobject,ref bool isrec,
            ref bool isSubPageFonction,ref bool IsWithZone,ref bool isRunObject,ref Variable runObj)
        {
            var rep = false;
            if (!IndexIsCorrectInArray(exprList, i)) { return false; }
            var expr = exprList[i];
            var exprSuiv = "";
            if (i + 1 < exprList.Count)
            {
                exprSuiv = exprList[i + 1].Name;
            }

            if (IndexIsCorrectInArray(exprList, i - 1))
            {
                var exprPrec = exprList[i - 1].Name;
                if (exprPrec == ".")
                {
                    if (!IndexIsCorrectInArray(exprList, i - 2)) { return false; }
                    var exprPrec2 = exprList[i - 2].Name;
                    if (exprPrec2.ToLower() == "rec" || exprPrec2.ToLower() == "xrec")
                    {
                        isrec = true;
                        return true;
                    }
                    else
                    {
                        if (exprPrec2.ToLower() == "page" && _navObject.Type == ObjectType.Page)
                        {
                            if (IndexIsCorrectInArray(exprList, i - 4))
                            {
                                externalobject = exprList[i - 4].Name;
                                isSubPageFonction = true;
                                return true;
                            }
                        }
                        else
                        {
                            string varName = exprPrec2; string indiceZone = "";
                            if (IsArrayVariable(exprPrec2, ref varName, ref indiceZone))
                            {
                                externalobject = varName;
                            }
                            else
                            {
                                externalobject = exprPrec2;
                            }
                            return true;
                        }
                    }
                }

                //AccountingPeriod2.SETRANGE("New Fiscal Year",TRUE);
                //SETRANGE("New Fiscal Year", TRUE);
                if (exprPrec == "(" && exprSuiv!=".")
                {
                    bool isCustomFunction = false;
                    if (IndexIsCorrectInArray(exprList, i - 2))
                    {
                        var func = exprList[i - 2].Name;
                        if (IsFieldFunction(func))
                        {
                            //GLEntry3.SETRANGE("Entry No.", Rec."Entry No.");
                            doProcessFieldFunction(exprList, i, ref isCustomFunction, ref externalobject);
                        }
                        if (IsRunFunction(func))
                        {
                            //REPORT.RUN(50082, TRUE, FALSE, GLEntry3);
                            Variable varRunObj = null;
                            doProcessRunFunction(exprList, i, ref isCustomFunction, 
                                ref externalobject, expr.Name,ref varRunObj);
                            if (varRunObj != null)
                            {
                                isRunObject = true;
                                runObj = varRunObj;
                                return true;
                            }
                        }
                        if (!isCustomFunction)
                        {
                            //SETRANGE("New Fiscal Year", TRUE);
                            if (IsInBlock_WITH(ref externalobject, ref rep, ref IsWithZone))
                            {
                                return true;
                            }
                        }
                    }
                }

                //SETCURRENTKEY(Status,"Prod. Order No.","Prod. Order Line No.","Item No.","Line No.");
                if (exprPrec == "," && exprSuiv != ".")
                {
                    bool found = false; int indexFound = 0;
                    //expr dans liste de champs precede d'une fonction, rechercher le debut de la fonction
                    for (int k = i; k >= 0; k--)
                    {
                        if (exprList[k].Name == "(")
                        {
                            found = true;
                            indexFound = k;
                            break;
                        }
                    }
                    if (found)
                    {
                        if (IndexIsCorrectInArray(exprList, indexFound - 1))
                        {
                            var exprPrec1 = exprList[indexFound - 1].Name;
                            if (exprPrec1.Equals("SETCURRENTKEY", StringComparison.OrdinalIgnoreCase) ||
                                exprPrec1.Equals("CALCFIELDS", StringComparison.OrdinalIgnoreCase) ||
                                exprPrec1.Equals("CALCSUMS", StringComparison.OrdinalIgnoreCase) ||
                                exprPrec1.Equals("SETAUTOCALCFIELDS", StringComparison.OrdinalIgnoreCase))
                            {
                                if (IndexIsCorrectInArray(exprList, indexFound - 2))
                                {
                                    var exprPrec3 = exprList[indexFound - 2].Name;
                                    if (exprPrec3 == ".")
                                    {
                                        if (IndexIsCorrectInArray(exprList, indexFound - 3))
                                        {
                                            var exprPrec4 = exprList[indexFound - 3].Name;
                                            if (exprPrec4.ToLower() != "rec" && exprPrec4.ToLower() != "xrec")
                                            {
                                                string varName = exprPrec4; string indiceZone = "";
                                                if (IsArrayVariable(exprPrec4, ref varName, ref indiceZone))
                                                {
                                                    externalobject = varName;
                                                }
                                                else
                                                {
                                                    externalobject = exprPrec4;
                                                }
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Si inclus dans bloc WITH
            if(IsInBlock_WITH(ref externalobject, ref rep, ref IsWithZone) && exprSuiv != ".")
            {
                return true;
            }
            return rep;
        }

        private void doProcessFieldFunction(List<Marker> exprList,int i, 
            ref bool isCustomFunction, ref string externalobject)
        {
            if (IndexIsCorrectInArray(exprList, i - 3))
            {
                var exprPrec3 = exprList[i - 3].Name;
                if (exprPrec3 == ".")
                {
                    if (IndexIsCorrectInArray(exprList, i - 4))
                    {
                        var exprPrec4 = exprList[i - 4].Name;
                        if (exprPrec4.ToLower() != "rec" && exprPrec4.ToLower() != "xrec")
                        {
                            string varName = exprPrec4; string indiceZone = "";
                            if (IsArrayVariable(exprPrec4, ref varName, ref indiceZone))
                            {
                                externalobject = varName;
                            }
                            else
                            {
                                externalobject = exprPrec4;
                            }
                            isCustomFunction = true;
                        }
                    }
                }
            }
        }
        private void doProcessRunFunction(List<Marker> exprList, int i,
            ref bool isCustomFunction, ref string externalobject,string marker,ref Variable varObjToRun)
        {
            int idObj = 0;
            int.TryParse(marker, out idObj);
            if (idObj <= 0)
            {
                return;
            }
            varObjToRun = new Variable();
            if (IndexIsCorrectInArray(exprList, i - 3))
            {
                var exprPrec3 = exprList[i - 3].Name;
                if (exprPrec3 == ".")
                {
                    if (IndexIsCorrectInArray(exprList, i - 4))
                    {
                        if (idObj == 50082)
                        {
                            int p = 1;
                        }
                        var exprPrec4 = exprList[i - 4].Name;
                        if (exprPrec4.ToLower() == "report")
                        {
                            varObjToRun.Type = VariableType.Report;
                            varObjToRun.ID = idObj;
                            isCustomFunction = true;
                        }
                        if (exprPrec4.ToLower() == "codeunit")
                        {
                            varObjToRun.Type = VariableType.CodeUnit;
                            varObjToRun.ID = idObj;
                            isCustomFunction = true;
                        }
                        if (exprPrec4.ToLower() == "page")
                        {
                            varObjToRun.Type = VariableType.Page;
                            varObjToRun.ID = idObj;
                            isCustomFunction = true;
                        }
                        if (exprPrec4.ToLower() == "xmlport")
                        {
                            varObjToRun.Type = VariableType.XMLPort;
                            varObjToRun.ID = idObj;
                            isCustomFunction = true;
                        }
                    }
                }
            }
        }
        private bool IsInBlock_WITH(ref string externalobject, ref bool rep,ref bool IsWithZone)
        {
            if (containsWith)
            {
                //if (PileEndBegin.Count > 0)
                if(activeWithRecord.IsNotNullOrEmpty())
                {
                    externalobject = activeWithRecord;
                    IsWithZone = true;
                    return true;
                    //var arrList = PileEndBegin.ToList();
                    //for (int j = arrList.Count - 1; j >= 0; j--)
                    //{
                    //    if (arrList[j].WithTableName.IsNotNullOrEmpty())
                    //    {
                    //        externalobject = arrList[j].WithTableName;
                    //        return true;
                    //    }
                    //}
                }
            }
            return false;
        }

        private Variable GetNatureMarker(List<Marker> markers, string marker,
            CodeRange range, ref RefType typeReference, ref Function returnFunction,
            ref Field returnField, int i,bool IsFirstCall)
        {

            if (i < 0 || marker.Trim() == "") return null;
            
            if (i >= markers.Count)
            {
                int p = 0;
            }

            var m = markers[i];
            var mText = m.Name;

            if(mText.ToLower()=="rec"|| mText.ToLower() == "xrec")
            {
                return null;
            }

            string varName = "";string indexZone = "";
            if(IsArrayVariable(mText,ref varName,ref indexZone))
            {
                mText = varName;
            }

            if (mText.IsNullOrEmpty()) return null;
            var name = mText.RemoveQuotes();

            if(name== "AccountNo")
            {
                int p = 0;
            }


            //Référence externe objet avec scope
            //Comme DATABASE::Employee REPORT:: PAGE::  CODEUNIT:: XMLPORT:: QUERY::
            VariableType typeObj = VariableType.Other;
            if (IsScopeReference(markers, i, ref typeObj))
            {
                //Cte."Mode de calcul"::Individuelle
                if (typeObj == VariableType.Option) 
                {
                    int idRec = 0;string fieldName = "";string optionName = "";
                    optionName = markers[i].Name;
                    if (i - 2 >= 0)
                    {
                        fieldName = markers[i - 2].Name;
                        if (i >= 3)
                        {
                            if(markers[i - 3].Name == ".")
                            {
                                //var strtable = markers[i - 4].Name;
                                var strtable = markers[i - 4].Name;
                                RefType TypeRefer = RefType.None; var func = new Function(); var fiel = new Field();

                                //List<Marker> markers1 = new List<Marker> {
                                //    new Marker(){
                                //        Name=strtable,
                                //        Order=0
                                //    }
                                //};
                                //var refByObjVar = GetNatureMarker(markers1, strtable, range, ref TypeRefer, ref func, ref fiel,0,false);

                                var refByObjVar = DetermineVariable(range,ref TypeRefer, strtable);

                                if (strtable.ToLower() == "rec"|| strtable.ToLower() == "xrec") 
                                {
                                    idRec = _navObject.SourceTableID;
                                } 
                                else
                                {
                                    //Possible si base objets non complete
                                    if (refByObjVar == null)
                                    {
                                        Console.WriteLine("Ref not found {0} in {1} {2}", strtable, _navObject.Type, _navObject.ID);
                                    }
                                    else
                                    {
                                        idRec = refByObjVar.ID;
                                    }
                                }
                            }
                            else
                            {
                                idRec = range.RecId;
                            }
                        }
                        else
                        {
                            idRec = range.RecId;
                        }
                        typeReference = RefType.OptionVariable;
                        return new Variable
                        {
                            Type =  VariableType.Option,
                            Name = fieldName,
                            ID = idRec,
                            OptionName=optionName.RemoveQuotes()
                        };
                    }
                }
                else
                {
                    typeReference = RefType.ExternalObject;
                    int ObjId = _navObject.NavProject.GetIDObjectFromVariable(name, typeObj);
                    return new Variable
                    {
                        Type = typeObj,
                        Name = name,
                        ID = ObjId
                    };
                }
            }

            var refByObj = "";bool isRec = false; bool isSubPageFunc = false;bool IsWithZone = false;
            bool isRunObj = false;Variable varRunObj = null;
            if (IsExternalExpression(markers, i, ref refByObj,ref isRec,ref isSubPageFunc,ref IsWithZone,ref isRunObj,ref varRunObj))
            {
                RefType TypeRefer = RefType.None;
                var func = new Function(); var fiel = new Field();

                if (isRec)
                {
                    var recObj = _navObject.NavProject.GetObject(_navObject.SourceTableID, ObjectType.Table);
                    if (recObj != null)
                    {
                        returnField = ((NavObject)recObj).FieldList.Where(c => c.FieldName == name).FirstOrDefault();
                        if (returnField != null)
                        {
                            typeReference = RefType.ExternalField;
                            return null;
                        }

                        //Fonction externe
                        returnFunction = ((NavObject)recObj).FunctionList.Where(c => c.FunctionName == name).FirstOrDefault();
                        if (returnFunction != null)
                        {
                            typeReference = RefType.ExternalFunction;
                            return null;
                        }
                    }
                }

                if (isRecordedFunction(m.Name))
                {
                    typeReference = RefType.RecordedFunction;
                    return DetermineVariable(range, ref TypeRefer, refByObj); 
                }

                if (isSubPageFunc)
                {
                    //ex= CurrPage.WorkflowStatusLine.PAGE.SetFilterOnWorkflowRecord(RECORDID);
                    Variable refByObjVar = DetermineVariable(range, ref TypeRefer, refByObj); 
                    if (refByObjVar != null)
                    {
                        //Champ externe - exclude DotNet variable
                        if (refByObjVar.Type != VariableType.Other)
                        {
                            var obj = _navObject.NavProject.GetObject(refByObjVar.ID, refByObjVar.GetObjectType);
                            if (obj != null)
                            {
                                //Fonction externe definie sur une SubPage
                                returnFunction = ((NavObject)obj).FunctionList.Where(c => c.FunctionName == name).FirstOrDefault();
                                if (returnFunction != null)
                                {
                                    typeReference = RefType.ExternalFunction;
                                    return null;
                                }
                            }
                        }
                    }
                }

                if (isRunObj)
                {
                    typeReference = RefType.ExternalObject;
                    return varRunObj;
                }

                if (!IsFirstCall)
                {
                    //TODO Logger ici car erreur est sortir sinon recursion infinie
                    int e = 2;
                }

                if (!isRec&&!isSubPageFunc)
                {
                    Variable refByObjVar = DetermineVariable(range, ref TypeRefer, refByObj);

                    if (refByObjVar != null)
                    {
                        //Champ externe - 
                        if (refByObjVar.Type != VariableType.Other)//exclude DotNet variable
                        {
                            var obj = _navObject.NavProject.GetObject(refByObjVar.ID, refByObjVar.GetObjectType);
                            if (obj != null)
                            {
                                returnField = ((NavObject)obj).FieldList.Where(c => c.FieldName == name).FirstOrDefault();
                                if (returnField != null)
                                {
                                    typeReference = RefType.ExternalField;
                                    return null;
                                }

                                //Fonction externe
                                returnFunction = ((NavObject)obj).FunctionList.Where(c => c.FunctionName == name).FirstOrDefault();
                                if (returnFunction != null)
                                {
                                    typeReference = RefType.ExternalFunction;
                                    return null;
                                }
                            }
                            if (IsWithZone)
                            {
                                //Bloc WITH mais l'expression n'appartient pas a la table du WITH, rechercher dans la table principale aussi
                                //Champ interne à la table de l'objet
                                var Rec = _navObject.NavProject.GetObject(range.RecId, ObjectType.Table);
                                if (Rec != null)
                                {
                                    returnField = ((NavObject)Rec).FieldList.Where(c => c.FieldName == name).FirstOrDefault();
                                    if (returnField != null)
                                    {
                                        typeReference = RefType.InternalField;
                                        return null;
                                    }

                                    //Fonction de la table de l'objet
                                    returnFunction = ((NavObject)Rec).FunctionList.Where(c => c.FunctionName == name).FirstOrDefault();
                                    if (returnFunction != null)
                                    {
                                        typeReference = RefType.InternalFunction;
                                        return null;
                                    }
                                }


                                //Cas d'un objet trouvé variable dans un bloc With
                                //Retourner l'objet
                                //return refByObjVar;
                                return DetermineVariable(range, ref typeReference, name);
                            }
                        }
                    }
                }
            }
            else
            {

                Variable rVar = null;RefType rType = RefType.None;
                Function rfun = null;
                var found = GetNatureVariable(range, ref rType, name, ref rVar, ref rfun);
                if (found)
                {
                    if(rType== RefType.LocalVariable)
                    {
                        typeReference = RefType.LocalVariable;
                        return rVar;
                    }
                    if (rType == RefType.Parameter)
                    {
                        typeReference = RefType.Parameter;
                        return rVar;
                    }
                    if (rType == RefType.InternalFunction)
                    {
                        typeReference = RefType.InternalFunction;
                        returnFunction = rfun;
                        return null;
                    }
                    if (rType == RefType.GlobalVariable)
                    {
                        typeReference = RefType.GlobalVariable;
                        return rVar;
                    }
                }

                if (isRecordedFunction(m.Name))
                {
                    typeReference = RefType.RecordedFunction;
                    return new Variable { ID = range.RecId, Type = VariableType.Record };
                }


                //Champ interne à la table de l'objet
                var Rec = _navObject.NavProject.GetObject(range.RecId, ObjectType.Table);
                if (Rec != null)
                {
                    returnField = ((NavObject)Rec).FieldList.Where(c => c.FieldName == name).FirstOrDefault();
                    if (returnField != null)
                    {
                        typeReference = RefType.InternalField;
                        return null;
                    }

                    //Fonction de la table de l'objet
                    returnFunction = ((NavObject)Rec).FunctionList.Where(c => c.FunctionName == name).FirstOrDefault();
                    if (returnFunction != null)
                    {
                        typeReference = RefType.InternalFunction;
                        return null;
                    }
                }
            }

            return null;
        }

        private Variable DetermineVariable(CodeRange range, ref RefType typeReference, string refByObj)
        {
            Variable rVar = null; RefType rType = RefType.None;
            Function rfun = null; Variable refByObjVar = null;
            var found = GetNatureVariable(range, ref rType, refByObj, ref rVar, ref rfun);
            if (found)
            {
                if (rType == RefType.LocalVariable)
                {
                    typeReference = RefType.LocalVariable;
                    refByObjVar = rVar;
                }
                if (rType == RefType.Parameter)
                {
                    typeReference = RefType.Parameter;
                    refByObjVar = rVar;
                }
                if (rType == RefType.InternalFunction)
                {
                    //typeReference = RefType.InternalField;
                    //returnFunction = rfun;
                    //return null;
                }
                if (rType == RefType.GlobalVariable)
                {
                    typeReference = RefType.GlobalVariable;
                    refByObjVar = rVar;
                }
            }

            return refByObjVar;
        }

        private bool GetNatureVariable(CodeRange range,ref RefType typeReference,string name,
            ref Variable returnVar, ref Function returnFunction)
        {
            name = name.RemoveQuotes();
            //Parametre
            if (range != null)
            {
                if (range.RangeType == TypeOfCodeRange.Function)
                {
                    var parameters = GetRangeParameters(range.RangeName);
                    if (parameters != null)
                    {
                        var p = parameters.Where(c => c.Name == name).FirstOrDefault();
                        if (p != null)
                        {
                            typeReference = RefType.Parameter;
                            returnVar = p;
                            return true;
                        }
                    }
                }

                //Variable locale           
                var localVars = range.LocalVariables;
                if (localVars != null)
                {
                    var p = localVars.Where(c => c.Name == name).FirstOrDefault();
                    if (p != null)
                    {
                        typeReference = RefType.LocalVariable;
                        returnVar = p;
                        return true;
                    }
                }
            }

            //Variable globale           
            var globalVars = _navObject.GlobalVariables;
            if (globalVars != null)
            {
                var p = globalVars.Where(c => c.Name == name).FirstOrDefault();
                if (p != null)
                {
                    typeReference = RefType.GlobalVariable;
                    returnVar= p;
                    return true;
                }
            }

            //fonction interne à l'objet           
            var functions = _navObject.FunctionList;
            if (functions != null)
            {
                returnFunction = functions.Where(c => c.FunctionName == name).FirstOrDefault();
                if (returnFunction != null)
                {
                    typeReference = RefType.InternalFunction;
                    return true;
                }
            }
            return false;
        }
        private List<Variable> GetRangeParameters(string functionname)
        {
            var def = _navObject.PlacesOfCode.Where(c => c.RangeType == TypeOfCodeRange.Function
            && c.RangeName == functionname && c.IsFunctionDefinition).FirstOrDefault();
            if (def != null)
            {
                return def.LocalVariables;
            }
            return null;
        }
        public List<Marker> GetLineMarkers(string expression)
        {
            var markers = new List<Marker> { };
            var lines = expression.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                //DO NOT Trim or RemoveQuotes
                var line = lines[i];
                if (line.Trim() == "") continue;
                var m = GetLineMarkers_line(line, i, true);
                markers.AddRange(m);
            }
            return markers;
        }
        public List<Marker> GetLineMarkers_line(string expression, int iLine,bool CanProcessingArray)
        {


            //var pattern =
            //    @"""|'|,|\/\/|\s|:=|\.|\[|\]|\(|\)|\:\:|\.\.|\+|-|\*|\/|\bdiv\b|\bmod\b|=|>|>=|<|<=|<>|\bin\b|\band\b|\bor\b|\bnot\b|\bxor\b|\@|;|\r\n|\r|\n";
            var pattern =
                @"""|'|,|\s|:=|\.|\[|\]|\(|\)|\:\:|\.\.|\+|-|\*|\/|\bdiv\b|\bmod\b|=|>|>=|<|<=|<>|\bin\b|\band\b|\bor\b|\bnot\b|\bxor\b|\@|;|\:";

            var updateExpr = expression.Trim();
            return GetMarkersWithPattern(updateExpr, iLine, CanProcessingArray, pattern,expression.StartSpacesCount());
        }

        public List<Marker> GetMarkersWithPattern(string expression, int iLine, bool CanProcessingArray, 
            string pattern,int iStartingCol)
        {
            var markers = new List<Marker> { };
            var stack = new Stack<string>();
            var start = 0;
            var end = 0;
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(expression);
            string previousMarker = null;
            bool canSplit = true; int lastMatchLength = 0; bool isINZone = false;
            while (m.Success)
            {
                if (m.Value == "[" && CanProcessingArray)
                {
                    bool doNotSplit = false;
                    if (previousMarker != null)
                    {
                        if (previousMarker.Equals("in", StringComparison.OrdinalIgnoreCase) == false)
                        {
                            doNotSplit = true;
                        }
                        else
                        {
                            int p = 1;
                            isINZone = true;
                        }
                    }

                    {
                        if (stack.Count == 0)
                        {
                            stack.Push(m.Value);
                            if (doNotSplit) canSplit = false;
                        }
                    }
                }
                if (m.Value == "]" && CanProcessingArray)
                {

                    if (stack.Count == 0)
                    {
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



                if (CanProcessingArray && !isINZone)
                {
                    if (canSplit && m.Value != "\"" && m.Value != "\'" && m.Value != "]")
                    {
                        start = collectExpr(expression, iLine, markers, start, out end, m, out lastMatchLength,iStartingCol);
                    }
                }
                else
                {
                    if (canSplit && m.Value != "\"" && m.Value != "\'")
                    {
                        start = collectExpr(expression, iLine, markers, start, out end, m, out lastMatchLength,iStartingCol);
                    }
                }

                if (isINZone && m.Value == "]")
                {
                    isINZone = false;
                }
                if (m.Value.Trim().IsNotNullOrEmpty())
                {
                    previousMarker = m.Value;
                }

                m = m.NextMatch();
            }

            if (canSplit)
            {
                var expr = expression.Substring(end + lastMatchLength);
                _collectRefs.InsertMarker(markers, expr, iLine,end);
            }

            return markers;
        }

        private int collectExpr(string expression, int iLine, List<Marker> markers, 
            int start, out int end, Match m, out int lastMatchLength,int iStartingCol)
        {
            end = m.Index;
            var expr = expression.Substring(start, (end - start));
            //if (markers.Count > 0)
            //{
            //    if (markers[markers.Count-1].Name.Equals("in", StringComparison.OrdinalIgnoreCase))
            //    {
            //        if (expr.StartsWith("[")) expr = expr.ReplaceFirstOccurrence("[", "");
            //        if (expr.EndsWith("]")) expr = expr.ReplaceLastOccurrence("]", "");
            //    }
            //}
            _collectRefs.InsertMarker(markers, expr, iLine,start+ iStartingCol);

            var isPlage = false;
            isPlage = DetectPlage(expression, m, isPlage);
            //insert operator
            string strVal = m.Value;
            if (IsPertinentOperator(strVal) && !isPlage)
            {
                var expr2 = expression.Substring(end, m.Length);
                _collectRefs.InsertMarker(markers, expr2, iLine,end+ iStartingCol);
            }
            start = end + m.Length;
            lastMatchLength = m.Length;
            return start;
        }

        private  bool DetectPlage(string expression, Match m, bool isPlage)
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

        private bool IsPertinentOperator(string strVal)
        {
            return strVal == "." || strVal == "::" || strVal == ":=" || 
                strVal == "(" || strVal == "," || strVal.ToLower() == "in";
        }     
        private string RemoveComments(string source)
        {
            var stack = new Stack<string>();
            string returnedString = "";
            bool isLineComment = false, isBlocComment = false;

            for (int i = 0; i < source.Length; i++)
            {
                var c = source[i];

                if ((isBlocComment || isLineComment))
                    //if ((isBlocComment || isLineComment) && (@"}".Contains(c.ToString()) == false))
                {
                    if (c == '\r')
                    {
                        returnedString += c.ToString();
                        continue;
                    }
                    if (c == '\n')
                    {
                        isLineComment = false;
                        returnedString += c.ToString();
                        continue;
                    }
                    if ((c == '}'|| c == '{') && !isLineComment)
                    {
                        //isBlocComment = false;
                        //continue;
                    }
                    else
                    {
                        //if (c != '}')
                        {
                            continue;
                        }
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
                                returnedString = returnedString.ReplaceLastOccurrence("/", "");
                                returnedString = returnedString.ReplaceLastOccurrence("/", "");
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
                        //if (!isBlocComment)
                        //{
                        //    returnedString += c.ToString();
                        //}

                        var stackPeek = stack.Peek();
                        if (stackPeek == "{")
                        {
                            stack.Push(c.ToString());
                        }
                        else
                        {
                            if (!isBlocComment)
                            {
                                returnedString += c.ToString();
                            }
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
                            var stackPeek = stack.Peek();
                            if (stackPeek == "{")
                            {
                                stack.Pop();
                            }
                            else
                            {
                                if (!isBlocComment)
                                {
                                    returnedString += c.ToString();
                                }
                            }
                        }
                    }
                }
            }

            return returnedString;
        }
        private bool isKeyword(string tosearch)
        {
            return CalKeywords.Contains(tosearch, StringComparer.InvariantCultureIgnoreCase);
        }
        private bool isSystemFunction(string tosearch)
        {
            return CalSystemFunction.Contains(tosearch, StringComparer.InvariantCultureIgnoreCase);
        }       
    }
}
