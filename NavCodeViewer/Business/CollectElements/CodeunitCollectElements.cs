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
    public partial class CodeunitCollectElements : NavObjectCollectElements
    {
        private CodeRange actualCodeRange = null;
        private Function actualFunction = null;
        private int currentLineOnActualZone = 0;
        TextBlockType actualZone = TextBlockType.None;
        string actualCodeTriggerName = "";
        TextBlockCodeType actualCodeType = TextBlockCodeType.None;

        public CodeunitCollectElements(NavObject a) : base(a)
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
            //for(int k = lines.Length - 1; k >= 0; k--)
            //{
            //    var line = lines[k];
            //    if (line.StartsWith("    END;"))
            //    {
            //        AddEndOfFileCodeRange(k);
            //        break;
            //    }
            //}
            AddEndOfFileCodeRange(lines);

            var startZone = new Place(0, 2);
            var endZone = new Place(0, 0);


            bool isCodeLines = false; bool prevIsLastCodeLine = false;bool isVarZone = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (prevIsLastCodeLine)
                {
                    isCodeLines = false;
                    prevIsLastCodeLine = false;
                   actualCodeType = TextBlockCodeType.None;
                }
                var line = lines[i];

                FindSourceTableID(line);

                UpdateActualZone(line,ref actualZone,ref isVarZone,ref currentLineOnActualZone);

                CollectOthersZones(line, ref startZone, ref endZone, i);

                if (actualZone == TextBlockType.ObjectsProperties)
                {
                    CollectElementsObjectsProperties(ref isCodeLines, ref prevIsLastCodeLine, i, line,currentLineOnActualZone);
                }
                if (actualZone== TextBlockType.Properties)
                {
                    CollectElementsProperties(ref isCodeLines, ref prevIsLastCodeLine,
                        ref actualCodeTriggerName,ref isVarZone, i, line, navObj.SourceTableID);
                }
                if (actualZone == TextBlockType.Code)
                {
                    CollectElementsCode(ref isCodeLines, ref prevIsLastCodeLine,ref actualFunction,
                        ref actualCodeRange,currentLineOnActualZone, i, line,ref isVarZone, navObj.SourceTableID);
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
            if (line.StartsWith(@"  CODE"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);

                //Collect code Start Zone
                Place startZone1 = new Place(0, i);
                InsertZone(startZone1, startZone1, TypeOfCodeRange.CodeDef);
            }
        }
        private void FindSourceTableID(string line)
        {
            if (line.Contains("TableNo="))
            {
                var expr = line.Trim().FormatEndOfElement();
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
        private void CollectElementsProperties(ref bool isCodeLines, ref bool isLastCodeLine, ref string actualcodetriggerName,
            ref bool isVarZone, int i, string line, int RecId)
        {

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualcodetriggerName, TextBlockCodeType.CodeUnit_OnRun,
                 @"    OnRun=BEGIN",
                 @"          BEGIN",
                 @"          END;",RecId);

            if (!isCodeLines)
            {
                if (currentLineOnActualZone == 0)
                {
                    AddDocRange(i, line);
                    AddDocRange(i + 1, line);
                }
                AddEndOfZoneCodeRange(i, line, RecId);
            }
        } 
    }
}
