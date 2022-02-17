using NavCodeViewer.Domain;
using NavCodeViewer.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public class Project
    {
        //private int NbreIterationsTotal = 0;
        public List<NavObject> AllObjects { get; set; } = new List<NavObject>();
        public List<Reference> References { get; set; } = new List<Reference>();
        public List<NavObject> Tables
        {
            get
            {
                return AllObjects.Where(c => c.Type == ObjectType.Table).ToList();
            }
        }
        public List<NavObject> Pages
        {
            get
            {
                return AllObjects.Where(c => c.Type == ObjectType.Page).ToList();
            }
        }
        public List<NavObject> CodeUnits
        {
            get
            {
                return AllObjects.Where(c => c.Type == ObjectType.CodeUnit).ToList();
            }
        }
        public List<NavObject> Reports
        {
            get
            {
                return AllObjects.Where(c => c.Type == ObjectType.Report).ToList();
            }
        }
        public List<NavObject> XMLPorts
        {
            get
            {
                return AllObjects.Where(c => c.Type == ObjectType.XmlPort).ToList();
            }
        }
        public List<NavObject> Querys
        {
            get
            {
                return AllObjects.Where(c => c.Type == ObjectType.Query).ToList();
            }
        }
        public List<NavObject> Menusuites
        {
            get
            {
                return AllObjects.Where(c => c.Type == ObjectType.MenuSuite).ToList();
            }
        }
        public string Name { get; set; }
        public string FileName { get; set; }
        public NavObject GetObject(string ObjName, ObjectType Type)
        {
            switch (Type)
            {
                case ObjectType.Table:
                    return Tables.FirstOrDefault(c => c.Name == ObjName);
                case ObjectType.Page:
                    return Pages.FirstOrDefault(c => c.Name == ObjName);
                case ObjectType.Report:
                    return Reports.FirstOrDefault(c => c.Name == ObjName);
                case ObjectType.CodeUnit:
                    return CodeUnits.FirstOrDefault(c => c.Name == ObjName);
                case ObjectType.Query:
                    return Querys.FirstOrDefault(c => c.Name == ObjName);
                case ObjectType.XmlPort:
                    return XMLPorts.FirstOrDefault(c => c.Name == ObjName);
                case ObjectType.MenuSuite:
                    return Menusuites.FirstOrDefault(c => c.Name == ObjName);
                default:return null;
            }
        }
        public NavObject GetObject(string ObjName, VariableType Type)
        {
            switch (Type)
            {
                case VariableType.Record:
                    return Tables.FirstOrDefault(c => c.Name == ObjName);
                case VariableType.Page:
                    return Pages.FirstOrDefault(c => c.Name == ObjName);
                case VariableType.Report:
                    return Reports.FirstOrDefault(c => c.Name == ObjName);
                case VariableType.CodeUnit:
                    return CodeUnits.FirstOrDefault(c => c.Name == ObjName);
                case VariableType.Query:
                    return Querys.FirstOrDefault(c => c.Name == ObjName);
                case VariableType.XMLPort:
                    return XMLPorts.FirstOrDefault(c => c.Name == ObjName);
                case VariableType.MenuSuite:
                    return Menusuites.FirstOrDefault(c => c.Name == ObjName);
                default: return null;
            }
        }
        public NavObject GetObject(int ObjID, ObjectType Type)
        {
            switch (Type)
            {
                case ObjectType.Table:
                    return Tables.FirstOrDefault(c => c.ID == ObjID);
                case ObjectType.Page:
                    return Pages.FirstOrDefault(c => c.ID == ObjID);
                case ObjectType.Report:
                    return Reports.FirstOrDefault(c => c.ID == ObjID);
                case ObjectType.CodeUnit:
                    return CodeUnits.FirstOrDefault(c => c.ID == ObjID);
                case ObjectType.Query:
                    return Querys.FirstOrDefault(c => c.ID == ObjID);
                case ObjectType.XmlPort:
                    return XMLPorts.FirstOrDefault(c => c.ID == ObjID);
                case ObjectType.MenuSuite:
                    return Menusuites.FirstOrDefault(c => c.ID == ObjID);
                default: return null;
            }
        }
        public NavObject GetObject(int ObjID, VariableType Type)
        {
            switch (Type)
            {
                case VariableType.Record:
                    return Tables.FirstOrDefault(c => c.ID == ObjID);
                case VariableType.Page:
                    return Pages.FirstOrDefault(c => c.ID == ObjID);
                case VariableType.Report:
                    return Reports.FirstOrDefault(c => c.ID == ObjID);
                case VariableType.CodeUnit:
                    return CodeUnits.FirstOrDefault(c => c.ID == ObjID);
                case VariableType.Query:
                    return Querys.FirstOrDefault(c => c.ID == ObjID);
                case VariableType.XMLPort:
                    return XMLPorts.FirstOrDefault(c => c.ID == ObjID);
                case VariableType.MenuSuite:
                    return Menusuites.FirstOrDefault(c => c.ID == ObjID);
                default: return null;
            }
        }
        /// <summary>
        /// TableData568
        /// </summary>
        /// <param name="TableDataExpr"></param>
        /// <returns></returns>
        public NavObject GetTableData(string TableDataExpr)
        {
            var expr = TableDataExpr.Replace("TableData", "");
            int objID = 0;
            if (int.TryParse(expr, out objID))
            {
                return GetObject(objID, ObjectType.Table);
            }
            return null;
        }

        /// <summary>
        /// RunObject=Page 452;
        /// </summary>
        /// <param name="TableDataExpr"></param>
        /// <returns></returns>
        public NavObject GetRunObject(string RunObjectExpr)
        {
            int objID = 0;
            if (RunObjectExpr.StartsWith("Page"))
            {
                RunObjectExpr = RunObjectExpr.Replace("Page", "");
                if (int.TryParse(RunObjectExpr, out objID))
                {
                    return GetObject(objID, ObjectType.Page);
                }
            }
            if (RunObjectExpr.StartsWith("Report"))
            {
                RunObjectExpr = RunObjectExpr.Replace("Report", "");
                if (int.TryParse(RunObjectExpr, out objID))
                {
                    return GetObject(objID, ObjectType.Report);
                }
            }
            if (RunObjectExpr.StartsWith("Codeunit"))
            {
                RunObjectExpr = RunObjectExpr.Replace("Codeunit", "");
                if (int.TryParse(RunObjectExpr, out objID))
                {
                    return GetObject(objID, ObjectType.CodeUnit);
                }
            }
            if (RunObjectExpr.StartsWith("XMLport"))
            {
                RunObjectExpr = RunObjectExpr.Replace("XMLport", "");
                if (int.TryParse(RunObjectExpr, out objID))
                {
                    return GetObject(objID, ObjectType.XmlPort);
                }
            }
            if (RunObjectExpr.StartsWith("Table"))
            {
                RunObjectExpr = RunObjectExpr.Replace("Table", "");
                if (int.TryParse(RunObjectExpr, out objID))
                {
                    return GetObject(objID, ObjectType.Table);
                }
            }
            if (RunObjectExpr.StartsWith("Query"))
            {
                RunObjectExpr = RunObjectExpr.Replace("Query", "");
                if (int.TryParse(RunObjectExpr, out objID))
                {
                    return GetObject(objID, ObjectType.Query);
                }
            }
            if (RunObjectExpr.StartsWith("MenuSuite"))
            {
                RunObjectExpr = RunObjectExpr.Replace("MenuSuite", "");
                if (int.TryParse(RunObjectExpr, out objID))
                {
                    return GetObject(objID, ObjectType.MenuSuite);
                }
            }
            return null;
        }
        public int GetTableDataID(string TableDataExpr)
        {
            var obj = GetTableData(TableDataExpr);
            if (obj != null)
            {
                return ((NavObject)obj).ID;
            }
            return 0;
        }
        public int GetObjectID(string ObjName, ObjectType Type)
        {
            var obj = GetObject(ObjName, Type);
            if (obj != null)
            {
                return ((NavObject)obj).ID;
            }
            return 0;
        }
        public int GetObjectID(int ObjID, ObjectType Type)
        {
            var obj = GetObject(ObjID, Type);
            if (obj != null)
            {
                return ((NavObject)obj).ID;
            }
            return 0;
        }        
        public Object GetObjectFromText(string refObj, ObjectType type)
        {
            int iResult = 0;
            if (int.TryParse(refObj, out iResult))
            {
                var obj = GetObject(iResult, type);
                return obj;
            }
            else
            {
                var obj = GetObject(refObj, type);
                return obj;
            }
        }
        public int GetIDObjectFromVariable(string variableName, VariableType typeObj)
        {
            var obj = GetObjectFromText(variableName, Variable.ConvertToObjectType(typeObj));
            int ObjId = 0;
            if (obj != null)
            {
                ObjId = ((NavObject)obj).ID;
            }

            return ObjId;
        }
        public void LoadFileAndCollectReferences(string filename, IProgress<ProgressBarInfo> progress)
        {

            var allText = File.ReadAllText(filename,
                Encoding.GetEncoding(850));

            CollectAndFormatObjects(progress, allText);
            //CollectReferences(progress);
        }
        public void CollectAllReferencesPages(IProgress<ProgressBarInfo> progress)
        {

            //var allText = File.ReadAllText(filename,
            //    Encoding.GetEncoding(850));

            //CollectAndFormatObjects(progress, allText);
            CollectReferencesPages(progress);
        }
        private void CollectAndFormatObjects(IProgress<ProgressBarInfo> progress, string allText)
        {
            if (progress != null)
                progress.Report(new ProgressBarInfo { Value=0,Text=""});

            //var allLines = allText.SplitLines();
            var objSource = "";
            string firstLine = "";

            var tab = Regex.Split(allText, @"^\bOBJECT\b\s", RegexOptions.Multiline);

            for (int i = 1; i < tab.Length; i++)
            {
                objSource = "OBJECT " + tab[i];

                var m = Regex.Match(objSource,
                    @"(^\bOBJECT\b\s)(.*)$", RegexOptions.Multiline);

                if (m.Success)
                {
                    if (progress != null)
                    {
                        firstLine = m.Value.Trim();
                        NavObject obj = GetObjectInfo(firstLine);

                        var info = new ProgressBarInfo
                        {
                            Value = Convert.ToInt32((i + 1) * 100 / tab.Length),
                            Text = string.Format("{0} {1} {2} {3}", Resources.String2, obj.Type.ToString(), obj.ID, obj.Name)
                        };
                        progress.Report(info);
                    }

                    
                    LoadObject(firstLine, objSource);
                }
            }
        }
        public void CollectReferencesPages(IProgress<ProgressBarInfo> progress)
        {
            for (int i = 0; i < this.Pages.Count; i++)
            {
                var obj = Pages[i];
                CollectRefObject(progress, i, obj, this.Pages.Count);
            }
        }

        private void UpdateProgress(IProgress<ProgressBarInfo> progress, int i, NavObject obj,int TotalObj)
        {
            if (progress != null)
            {
                var info = new ProgressBarInfo
                {
                    Value = Convert.ToInt32((i + 1) * 100 / TotalObj),
                    Text = string.Format("{0} {1} {2} {3}", Resources.String1, obj.Type.ToString(), obj.ID, obj.Name)
                };
                progress.Report(info);
            }
        }

        public void CollectReferencesReports(IProgress<ProgressBarInfo> progress)
        {
            for (int i = 0; i < this.Reports.Count; i++)
            {
                var obj = Reports[i];
                CollectRefObject(progress, i, obj, this.Reports.Count);
            }
        }

        private void CollectRefObject(IProgress<ProgressBarInfo> progress, int i, NavObject obj,int TotalObj)
        {
            var refMgt = new NavObjectCollectRefs(obj);
            UpdateProgress(progress, i, obj, TotalObj);
            refMgt.CollectReferences();
        }

        public void CollectReferencesTables(IProgress<ProgressBarInfo> progress)
        {
            for (int i = 0; i < this.Tables.Count; i++)
            {
                var obj = Tables[i];
                CollectRefObject(progress, i, obj, this.Tables.Count);
            }
        }
        public void CollectReferencesCodeunits(IProgress<ProgressBarInfo> progress)
        {
            for (int i = 0; i < this.CodeUnits.Count; i++)
            {
                var obj = CodeUnits[i];
                CollectRefObject(progress, i, obj, this.CodeUnits.Count);
            }
        }
        public void CollectReferencesQuerys(IProgress<ProgressBarInfo> progress)
        {
            for (int i = 0; i < this.Querys.Count; i++)
            {
                var obj = Querys[i];
                CollectRefObject(progress, i, obj, this.Querys.Count);
            }
        }
        public void CollectReferencesXmlports(IProgress<ProgressBarInfo> progress)
        {
            for (int i = 0; i < this.XMLPorts.Count; i++)
            {
                var obj = XMLPorts[i];
                CollectRefObject(progress, i, obj, this.XMLPorts.Count);
            }
        }
        public void CollectReferencesMenusuites(IProgress<ProgressBarInfo> progress)
        {
            for (int i = 0; i < this.Menusuites.Count; i++)
            {
                var obj = Menusuites[i];
                CollectRefObject(progress, i, obj, this.Menusuites.Count);
            }
        }
        private void LoadObject(string objectDefinition, string objSource)
        {
            var obj = CreateNewObject(objectDefinition);
            FormatObject(obj, objSource);
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
            throw new Exception("Cannot find Object Type : " + type);
        }
        private NavObject CreateNewObject(string line)
        {
            NavObject navObj = GetObjectInfo(line);
            navObj.NavProject = this;
            this.AllObjects.Add(navObj);
            return navObj;
        }
        private NavObject GetObjectInfo(string line)
        {
            var navObj = new NavObject();
            var words = line.Split(' ');
            bool nomEntoureParentheses = false;
            var strName = "";
            for (int i = 0; i < words.Length; i++)
            {
                if (i == 1)
                {
                    var strType = words[i];
                    if (strType.StartsWith("["))
                    {
                        strType = strType.ReplaceFirstOccurrence("[", "");
                        nomEntoureParentheses = true;
                    }
                    navObj.Type = GetTypeFromText(strType);
                }
                if (i == 2) navObj.ID = Convert.ToInt32(words[i]);
                if (i > 2)
                {

                    if (strName.IsNullOrEmpty())
                    {
                        strName = words[i];
                    }
                    else
                    {
                        strName += " " + words[i];
                    }
                }
            }
            navObj.Name = strName;
            if (nomEntoureParentheses && navObj.Name.TrimEnd().EndsWith("]"))
            {
                navObj.Name = navObj.Name.ReplaceLastOccurrence("]", "").Trim();
            }

            return navObj;
        }
        private string FormatObject(NavObject obj, string source)
        {
            var rep = "";

            switch (obj.Type)
            {
                case ObjectType.Table:
                    {
                        var formatObjMgt = new FormatTable();
                        var collectELtsMgt = new TableCollectElements(obj);
                        rep = formatObjMgt.FormatTableSource(source);
                        //obj.OthersPlaces.AddRange(formatObjMgt.ZonesPlaces);
                        collectELtsMgt.CollectElements(rep);
                        break;
                    }
                case ObjectType.Page:
                    {
                        var formatObjMgt = new FormatPage();
                        var collectELtsMgt = new PageCollectElements(obj);
                        rep = formatObjMgt.FormatPageSource(source);
                        //obj.OthersPlaces.AddRange(formatObjMgt.ZonesPlaces);
                        collectELtsMgt.CollectElements(rep);
                        break;
                    }
                case ObjectType.Report:
                    {
                        var formatObjMgt = new FormatReport();
                        var collectELtsMgt = new ReportCollectElements(obj);
                        rep = formatObjMgt.FormatReportSource(source);
                        //obj.OthersPlaces.AddRange(formatObjMgt.ZonesPlaces);
                        collectELtsMgt.CollectElements(rep);
                        break;
                    }
                case ObjectType.CodeUnit:
                    {
                        var formatObjMgt = new FormatCodeunit();
                        var collectELtsMgt = new CodeunitCollectElements(obj);
                        rep = formatObjMgt.FormatCodeUnit(source);
                        //obj.OthersPlaces.AddRange(formatObjMgt.ZonesPlaces);
                        collectELtsMgt.CollectElements(rep);
                        break;
                    }
                case ObjectType.Query:
                    {
                        var formatObjMgt = new FormatQuery();
                        var collectELtsMgt = new QueryCollectElements(obj);
                        rep = formatObjMgt.FormatQuerySource(source);
                        //obj.OthersPlaces.AddRange(formatObjMgt.ZonesPlaces);
                        collectELtsMgt.CollectElements(rep);
                        break;
                    }
                case ObjectType.XmlPort:
                    {
                        var formatObjMgt = new FormatXmlPort();
                        var collectELtsMgt = new XmlPortCollectElements(obj);
                        rep = formatObjMgt.FormatXMLPortSource(source);
                        //obj.OthersPlaces.AddRange(formatObjMgt.ZonesPlaces);
                        collectELtsMgt.CollectElements(rep);
                        break;
                    }
                case ObjectType.MenuSuite:
                    {
                        var formatObjMgt = new FormatMenusuite();
                        var collectELtsMgt = new MenusuiteCollectElements(obj);
                        rep = formatObjMgt.FormatMenu(source);
                        //obj.OthersPlaces.AddRange(formatObjMgt.ZonesPlaces);
                        collectELtsMgt.CollectElements(rep);
                        break;
                    }
            }
            return rep;
        
        }
        /// <summary>
        /// Tous les endroits ou l'objet est référencé
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public List<Reference> GetWhereObjectIsReferenced(NavObject rec)
        {
            try
            {
                var r = References;
                return References.Where(c => c.Ref_ObjectID == rec.ID
                    && c.Ref_ObjectType == rec.Type
                    && c.ReferenceType == RefType.ExternalObject).ToList();
            }
            catch
            {
                return new List<Reference>();
            }
        }
        /// <summary>
        /// Obtient tous les elements qu'un objet donné référence
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public List<Reference> GetAllObjectsReferenced(NavObject rec)
        {
            try
            {
                var r = References;
                return References.Where(c => c.RefBy_ObjetID == rec.ID
                    && c.RefBy_ObjetType == rec.Type).ToList();
            }
            catch
            {
                return new List<Reference>();
            }
        }
        public List<Reference> GetObjectFuncRecordedReferences(NavObject rec,string typeRecorded)
        {
            try
            {
                var r = References;
                return References.Where(c => c.Ref_ObjectID == rec.ID
                    && c.Ref_ObjectType == rec.Type
                    && c.Name == typeRecorded).ToList();
            }
            catch
            {
                return new List<Reference>();
            }
        }
        public List<Reference> GetOptionReferences(NavObject rec, string fieldName,string option)
        {
            try
            {
                var r = References;
                return References.Where(c => c.Ref_ObjectID == rec.ID
                    && c.Ref_ObjectType == rec.Type
                    && c.OptionName == option
                    && c.Name==fieldName).ToList();
            }
            catch
            {
                return new List<Reference>();
            }
        }
        public List<Reference> GetFieldReferences(NavObject rec,string fieldName)
        {
            try
            {
                var r = References;
                return References.Where(c => c.Ref_ObjectID == rec.ID
                    && c.Ref_ObjectType == rec.Type
                    && (c.ReferenceType == RefType.ExternalField || c.ReferenceType == RefType.InternalField)
                    && c.Name == fieldName).ToList();
            }
            catch
            {
                return new List<Reference>();
            }
        }
        public List<Reference> GetFunctionReferences(NavObject rec, string fieldName)
        {
            try
            {
                return References.Where(c => c.Ref_ObjectID == rec.ID
                    && c.Ref_ObjectType == rec.Type
                    && (c.ReferenceType == RefType.ExternalFunction || c.ReferenceType == RefType.InternalFunction)
                    && c.Name == fieldName).ToList();
            }
            catch
            {
                return new List<Reference>();
            }
        }
        //public List<Reference> GetFunctionReferences(NavObject rec, string fieldName)
        //{
        //    try
        //    {
        //        return References.Where(c => c.Ref_ObjectID == rec.ID
        //            && c.Ref_ObjectType == rec.Type
        //            && (c.ReferenceType == RefType.ExternalFunction || c.ReferenceType == RefType.InternalFunction)
        //            && c.Name == fieldName).ToList();
        //    }
        //    catch
        //    {
        //        return new List<Reference>();
        //    }
        //}
        public List<Reference> SearchText(string pattern,List<NavObject> objs, 
            IProgress<ProgressBarInfo> progress,RegexOptions opt)
        {
            var refs = new List<Reference>();
            for (int i = 0; i < objs.Count; i++)
            {
                var obj = objs[i];
                UpdateProgress(progress, i, obj, objs.Count);
                var objLines = obj.Tb;
                for (int j = 0; j < objLines.Length; j++)
                {
                    var line = objLines[j];
                    if (Regex.IsMatch(line, pattern, opt))
                    {
                        Reference toRef = new Reference
                        {
                            //RefBy_ObjetID = mainForm.CurrentSourceViewer.NavObject.ID,
                            //RefBy_ObjetType = mainForm.CurrentSourceViewer.NavObject.Type,
                            ReferenceLine = j + 1,
                            RefBy_ObjetID = obj.ID,
                            RefBy_ObjetType = obj.Type,
                            LineText=line,
                        };

                        if (refs.Where(c => c.RefBy_ObjetID == toRef.RefBy_ObjetID
                            && c.RefBy_ObjetType == toRef.RefBy_ObjetType
                            && c.ReferenceLine == toRef.ReferenceLine).Count() == 0)
                        {
                            refs.Add(toRef);
                        }
                    }
                }
            }
            return refs;
        }
    }
}