using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NavCodeViewer.Business
{
    public class ImportFileMgt
    {
        public List<NavObject> ImportFile(string fileName)
        {
            try
            {
                var enc = EncodingDetector.DetectTextFileEncoding(fileName);
                if (enc != null)
                    return OpenFile(fileName, enc);
                else
                    return OpenFile(fileName, Encoding.Default);
            }
            catch
            {
                throw;
            }
        }

        private List<NavObject> OpenFile(string fileName, Encoding enc)
        {
            var FielsList = new List<Field>();
            var FunctionList = new List<Function>();
            var ObjectList = new List<NavObject>();
            NavObject ActualObject=null;
            string ActualObjectSource = "";
            string transformLine = "";
            //var source = File.ReadAllText(fileName, enc);
            var lines = File.ReadLines(fileName,enc);
            ObjectType ActualObjectType;
            bool ReplaceNextLineAccolladeByBegin = false;
            TextBlockType ActualBlockType= TextBlockType.None;
            foreach (var line in lines)
            {
                transformLine = line;
                if (ReplaceNextLineAccolladeByBegin && Regex.IsMatch(line, @"\s*{"))
                {
                    transformLine = transformLine.Replace("{", "begin");
                    ReplaceNextLineAccolladeByBegin = false;
                }
                if(line == "  }")
                {
                    if(ActualBlockType== TextBlockType.ObjectsProperties ||
                        ActualBlockType == TextBlockType.Properties ||
                        ActualBlockType == TextBlockType.Controls ||
                        ActualBlockType == TextBlockType.Code)
                    {
                        transformLine = transformLine.Replace("}", "end");
                    }
                }
                if (line=="}")
                {
                    transformLine = transformLine.Replace("}", "end");
                }
                if (ActualBlockType== TextBlockType.Controls)
                {
                    transformLine = Regex.Replace(transformLine, @"(?<=.+\s)}(\r\n\r\n)", "end");
                    transformLine = Regex.Replace(transformLine, @"(?<=\s\s\s\s){", "begin");
                }
                if (Regex.IsMatch(line, @"\bOBJECT\b\s"))
                {
                    //Save old object
                    if (ActualObject != null)
                    {
                        //ActualObject.TextSource = ActualObjectSource;
                        ObjectList.Add(ActualObject);
                        //Save in bd
                    }
                    ActualObject = CreateNewObject(line);
                    ActualObjectType = ActualObject.Type;
                    ReplaceNextLineAccolladeByBegin = true;
                    ActualBlockType = TextBlockType.None;
                }
                if (Regex.IsMatch(line, @"\s\bOBJECT-PROPERTIES\b"))
                {
                    ActualBlockType =  TextBlockType.ObjectsProperties;
                    ReplaceNextLineAccolladeByBegin = true;
                }
                if (Regex.IsMatch(line, @"\s\bPROPERTIES\b"))
                {
                    ActualBlockType = TextBlockType.Properties;
                    ReplaceNextLineAccolladeByBegin = true;
                }
                if (Regex.IsMatch(line, @"\s\bCONTROLS\b"))
                {
                    ActualBlockType = TextBlockType.Controls;
                    ReplaceNextLineAccolladeByBegin = true;
                }
                if (Regex.IsMatch(line, @"\s\bCODE\b"))
                {
                    ActualBlockType = TextBlockType.Code;
                    ReplaceNextLineAccolladeByBegin = true;
                }

                ActualObjectSource += transformLine + Environment.NewLine;

            }
            return ObjectList;
        }

        private NavObject CreateNewObject(string line)
        {
            var obj = new NavObject();
            var words = line.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (i == 1) obj.Type = GetTypeFromText(words[i]);
                if (i == 2) obj.ID = Convert.ToInt32(words[i]);
                if (i > 2)
                {
                    if (obj.Name.IsNullOrEmpty())
                    {
                        obj.Name = words[i];
                    }
                    else
                    {
                        obj.Name += " " + words[i];
                    }
                }
            }
            return obj;
        }
        ObjectType GetTypeFromText(string type)
        {
            if (type == "Page") return  ObjectType.Page;
            if (type == "Codeunit") return ObjectType.CodeUnit;
            if (type == "MenuSuite ") return ObjectType.MenuSuite;
            if (type == "Query ") return ObjectType.Query;
            if (type == "Report ") return ObjectType.Report;
            if (type == "Table ") return ObjectType.Table;
            if (type == "XMLport  ") return ObjectType.XmlPort;
            return  ObjectType.Table;
        }

        private void ProcessTable()
        {

        }
        private void Process_ObjectProperties()
        {

        }

    }
}
