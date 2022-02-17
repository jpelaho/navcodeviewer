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

        #region Private Fields

        private int _SourceTableID;

        #endregion Private Fields

        #region Public Fields

        public string[] _Tb;
        public List<DataItem> DataItems = new List<DataItem>();
        public List<DataItem> ElementsItems = new List<DataItem>();
        public int NosOfLines;
        public List<DataItem> PageParts = new List<DataItem>();
        public List<DataItem> QueryColumnAndFilters = new List<DataItem>();

        #endregion Public Fields

        #region Public Properties

        public List<Field> FieldList { get; set; } = new List<Field>();
        public List<Function> FunctionList { get; set; } = new List<Function>();
        public List<Variable> GlobalVariables { get; set; } = new List<Variable>();
        public int ID { get; set; }
        public string LongName
        {
            get
            {
                return string.Format("{0} {1}", ObjectName, Name);
            }
        }

        public string Name { get; set; }
        public Project NavProject { get; set; }
        //public string GetFunctionLineDef(string funcName)
        //{
        //    string rep = "";
        //    var func = PlacesOfCode.Where(c => c.RangeName == funcName&& c.IsFunctionDefinition).FirstOrDefault();
        //    if (func != null)
        //    {
        //        rep = rep + lines[i] + Environment.NewLine;
        //    }
        //    return rep;
        //}
        public string ObjectName
        {
            get
            {
                switch (Type)
                {
                    case ObjectType.Table:
                        return string.Format("{0} {1}", "Table", ID);
                    case ObjectType.Page:
                        return string.Format("{0} {1}", "Page", ID);
                    case ObjectType.Report:
                        return string.Format("{0} {1}", "Report", ID);
                    case ObjectType.CodeUnit:
                        return string.Format("{0} {1}", "Codeunit", ID);
                    case ObjectType.Query:
                        return string.Format("{0} {1}", "Query", ID);
                    case ObjectType.XmlPort:
                        return string.Format("{0} {1}", "XmlPort", ID);
                    case ObjectType.MenuSuite:
                        return string.Format("{0} {1}", "MenuSuite", ID);
                    default:
                        return string.Format("{0} {1}", "Table", ID);
                }
            }
        }

        public string ObjectTextSource { get; set; }
        public List<CodeRange> OthersPlaces { get; set; } = new List<CodeRange>();
        public List<CodeRange> PlacesOfCode { get; set; } = new List<CodeRange>();
        public List<CodeRange> PlacesOfCodeWithRecId
        {
            get
            {
                return PlacesOfCode.Where(c => c.RecId != 0).ToList();
            }
        }

        public List<CodeRange> PlacesOfSourceExpr { get; set; } = new List<CodeRange>();
        public int SourceTableID
        {
            get
            {
                if (_SourceTableID == 0)
                {
                    if (Type == ObjectType.Table)
                    {
                        return ID;
                    }
                }
                return _SourceTableID;
            }
            set => _SourceTableID = value;
        }

        public string[] Tb
        {
            get
            {
                if (_Tb == null)
                {
                    _Tb = ObjectTextSource.SplitLines();
                }
                return _Tb;
            }
        }

        public List<Trigger> Triggers { get; set; } = new List<Trigger>();
        public ObjectType Type { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public NavObject()
        {
        }

        #endregion Public Constructors

        #region Public Methods
        public string GetExternalVarDefinition(string varName, Reference refe, ref string fieldTitle)
        {
            const int MaxLineLengthToShow = 80;
            string fieldDef = "";
            //var field = GlobalVariables.Where(c => c.Name == varName).FirstOrDefault();
            if (refe != null)
            {
                NavObject navObject = NavProject.GetObject(refe.Ref_ObjectID, refe.Ref_ObjectType);
                if (navObject != null)
                {
                    fieldTitle = fieldTitle + navObject.ObjectName;
                }
                fieldDef = Tb[refe.ReferenceLine-1];
                if (fieldDef.Length > MaxLineLengthToShow)
                {
                    fieldDef = fieldDef.Substring(0, MaxLineLengthToShow) + "[...]";
                }
            }

            return fieldDef;
        }
        public string GetFieldDefinition(string funcName)
        {
            const int MaxLineLengthToShow = 80;

            string fieldDef = "";
            var field = FieldList.Where(c => c.FieldName == funcName).FirstOrDefault();
            if (field != null)
            {
                var lines = this.ObjectTextSource.SplitLines();
                fieldDef = lines[field.StartingDefLine - 1];
                if (fieldDef.Length > MaxLineLengthToShow)
                {
                    fieldDef = fieldDef.Substring(0, MaxLineLengthToShow) + "[...]";
                }
            }

            return fieldDef;
        }
        public string GetGlobalVarDefinition(string varName, ref string fieldTitle)
        {
            const int MaxLineLengthToShow = 80;

            string fieldDef = "";
            var field = GlobalVariables.Where(c => c.Name == varName).FirstOrDefault();
            if (field != null)
            {
                NavObject navObject = NavProject.GetObject(field.ID, field.Type);
                if (navObject != null)
                {
                    fieldTitle = fieldTitle + navObject.ObjectName;
                }
                var lines = this.ObjectTextSource.SplitLines();
                fieldDef = lines[field.DefinitionLine];
                if (fieldDef.Length > MaxLineLengthToShow)
                {
                    fieldDef = fieldDef.Substring(0, MaxLineLengthToShow) + "[...]";
                }
            }

            return fieldDef;
        }
        public string GetLocalVarDefinition(string varName,int defLine,CodeRange scope, ref string fieldTitle)
        {
            const int MaxLineLengthToShow = 80;

            string fieldDef = "";
            var field = scope.LocalVariables.Where(c => c.Name == varName).FirstOrDefault();
            if (field != null)
            {
                if(field.Type!= VariableType.Other)
                {

                    NavObject navObject = NavProject.GetObject(field.ID, field.Type);
                    if (navObject != null)
                    {
                        fieldTitle = fieldTitle + navObject.ObjectName;
                    }
                }
                fieldDef = Tb[field.DefinitionLine];
                if (fieldDef.Length > MaxLineLengthToShow)
                {
                    fieldDef = fieldDef.Substring(0, MaxLineLengthToShow) + "[...]";
                }
            }

            return fieldDef;
        }

        public List<Reference> GetFieldReferences(string fieldName)
        {
            return NavProject.GetFieldReferences(this, fieldName);
        }

        public List<Reference> GetFunctionReferences(string functionName)
        {
            return NavProject.GetFunctionReferences(this, functionName);
        }

        public string GetFunctionText(string funcName, ref string functionTitle)
        {
            string rep = "";
            var func = FunctionList.Where(c => c.FunctionName == funcName).FirstOrDefault();
            if (func != null)
            {
                var lines = this.ObjectTextSource.SplitLines();

                var funcDefRange = PlacesOfCode.Where(c => c.RangeName == funcName && c.IsFunctionDefinition).FirstOrDefault();
                if (funcDefRange != null)
                {
                    var str = lines[funcDefRange.Start.iLine];
                    functionTitle = str;

                }
                int TotalFunLines = func.EndingDefLine - func.StartingDefLine;

                int maxNosLine = Math.Min(TotalFunLines, lines.Length - 1);
                const int MaxLineToShow = 40;
                const int MaxLineLengthToShow = 80;
                maxNosLine = Math.Min(maxNosLine, MaxLineToShow);
                int maxLineLength = 0;
                for (int i = func.StartingDefLine; i < func.StartingDefLine + maxNosLine + 1; i++)
                {
                    rep = rep + lines[i] + Environment.NewLine;
                    var l = lines[i].Length;
                    if (l > maxLineLength)
                    {
                        maxLineLength = l - 10;
                    }
                }
                //maxLineLength = Math.Min(maxLineLength, MaxLineLengthToShow);
                if (functionTitle.Length > MaxLineLengthToShow)
                {
                    functionTitle = functionTitle.Substring(0, MaxLineLengthToShow) + "[...]";
                }
                else
                {
                    if (maxLineLength > 0)
                    {
                        if (functionTitle.Length > maxLineLength)
                        {
                            functionTitle = functionTitle.Substring(0, maxLineLength) + "[...]";
                        }
                    }
                }

                if (TotalFunLines > MaxLineToShow)
                {
                    rep = rep + "[...]";
                }
            }

            return rep;
        }

        public string GetImageString()
        {
            switch (Type)
            {
                case ObjectType.Table:
                    return "table";
                case ObjectType.Page:
                    return "page";
                case ObjectType.Report:
                    return "report";
                case ObjectType.CodeUnit:
                    return "codeunit";
                case ObjectType.Query:
                    return "query";
                case ObjectType.XmlPort:
                    return "xmlport";
                case ObjectType.MenuSuite:
                    return "menusuite";
                default: return "table";
            }
        }

        public List<Reference> GetObjectFuncRecordedReferences(string typeRecorded)
        {
            return NavProject.GetObjectFuncRecordedReferences(this, typeRecorded);
        }

        public List<Reference> GetWhereObjectIsReferenced()
        {
            return NavProject.GetWhereObjectIsReferenced(this);
        }
        /// <summary>
        /// Obtient tous les elements qu'un objet donné référence
        /// </summary>
        /// <returns></returns>
        public List<Reference> AllObjectsReferenced
        {
            get { return NavProject.GetAllObjectsReferenced(this); }
        }
        public List<Reference> GetOptionReferences(string fieldName,string optionName)
        {
            return NavProject.GetOptionReferences(this, fieldName, optionName);
        }
        public bool IsLocalField(string expr)
        {
            return FieldList.Where(c => c.FieldName == expr).FirstOrDefault() != null;
        }

        //private bool IsLocalFunction(string expr,int iChar)
        //{
        //    return GetObjectsReferencedBy().Where(re)
        //    //return FunctionList.Where(c => c.FunctionName == expr && c.).FirstOrDefault() != null;
        //}
        public Reference GetRef(string expr, int iCol, int iLine)
        {
            var r = AllObjectsReferenced.Where(c => c.Name == expr
                    && c.ReferenceColumn == iCol && c.ReferenceLine == iLine)
                    .FirstOrDefault();
            if (r != null)
            {
                return r;
            }


            var r2 = AllObjectsReferenced.Where(c => c.Name == expr && c.ReferenceLine == iLine)
                .FirstOrDefault();
            if (r2 != null)
            {
                return r2;
            }


            var r3 = AllObjectsReferenced.Where(c => c.Name == expr)
                .FirstOrDefault();
            return r3;

        }

        public string GetDefinitionOfExpr(string expr, int iChar,ref string funcTionDef, int iLine)
        {
            var refe = GetRef(expr, iChar,iLine+1);
            if (refe != null)
            {
                switch (refe.ReferenceType)
                {
                    case RefType.Parameter:
                        {
                            return GetLocalVarDefinition(expr, refe.ReferenceLine, refe.Scope,ref funcTionDef);
                        }
                    case RefType.GlobalVariable:
                        {
                            return GetGlobalVarDefinition(expr, ref funcTionDef);
                        }
                    case RefType.LocalVariable:
                        {
                            return GetLocalVarDefinition(expr,refe.ReferenceLine,refe.Scope, ref funcTionDef);
                        }
                    case RefType.ExternalObject:
                        {
                            return GetExternalVarDefinition(expr, refe, ref funcTionDef);
                        }
                    case RefType.InternalFunction:
                        {
                            return GetFunctionText(expr,ref funcTionDef);
                        }
                    case RefType.ExternalFunction:
                        break;
                    case RefType.InternalField:
                        {
                            return GetFieldDefinition(expr);
                        }
                    case RefType.ExternalField:
                        break;
                    case RefType.OptionVariable:
                        break;
                    case RefType.RecordedFunction:
                        break;
                    case RefType.None:
                        break;
                }
                
            }
            return "";
        }
        #endregion Public Methods

    }
}
