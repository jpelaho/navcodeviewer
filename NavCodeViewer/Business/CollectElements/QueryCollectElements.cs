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
    public partial class QueryCollectElements : NavObjectCollectElements
    {
        private CodeRange actualCodeRange = null;
        private Function actualFunction=null;
        private int currentLineOnActualZone = 0;
        string actualCodeTriggerName = "";
        private Variable actualVariableElement;

        TextBlockType actualZone = TextBlockType.None;
        TextBlockCodeType actualCodeType = TextBlockCodeType.None;

        public QueryCollectElements(NavObject a) : base(a)
        {

        }
        //public string ProcessQuery(string source)
        //{           
        //    var updated = FormatQuery(source);
        //    var update2 = CollectElements(updated);
        //    return update2;
        //}
        public string CollectElements(string source)
        {
            string[] lines = source.SplitLines();
            navObj.NosOfLines = lines.Length;
            navObj.ObjectTextSource = source;

            if (lines.Length > 1)
            {
                //CreateNewObject(lines[0]);
                //PlacesOfCode.Add(new CodeRange(0, lines[0]));
                //PlacesOfCode.Add(new CodeRange(1, lines[1]));
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
                if (i == 604)
                {
                    var p = 4;
                }
                var line = lines[i];
                UpdateActualZone(line, ref actualZone,ref isVarZone, ref currentLineOnActualZone);

                CollectOthersZones(line, ref startZone, ref endZone, i);

                InsertGlobalVariable_DataItem(line, ref actualVariableElement);

                if (actualZone == TextBlockType.ObjectsProperties)
                {
                    CollectElementsObjectsProperties(ref isCodeLines, ref prevIsLastCodeLine, i, line,currentLineOnActualZone);
                }
                if (actualZone == TextBlockType.Properties)
                {
                    CollectElementsProperties(ref isCodeLines, ref prevIsLastCodeLine,
                        ref isVarZone,ref actualCodeTriggerName,  i, line,0);
                }
                if (actualZone == TextBlockType.Elements)
                {
                    CollectElementsElements(ref isCodeLines, ref prevIsLastCodeLine, i, line);
                }
                if (actualZone == TextBlockType.Code)
                {
                    CollectElementsCode(ref isCodeLines, ref prevIsLastCodeLine,ref actualFunction,ref actualCodeRange,
                        currentLineOnActualZone, i, line,ref isVarZone,0);
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
            if (line.StartsWith(@"  CODE"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.DataSetDef);

                //Collect code Start Zone
                Place startZone1 = new Place(0, i);
                InsertZone(startZone1, startZone1, TypeOfCodeRange.CodeDef);
            }
        }
        private void CollectElementsProperties(ref bool isCodeLines, ref bool isLastCodeLine, ref bool isVarZone,
            ref string actualcodetriggername, int i, string line,int RecId)
        {
            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine,ref isVarZone, i, line,
                ref actualCodeRange,ref actualCodeType,ref actualcodetriggername,
                TextBlockCodeType.Query_OnBeforeOpen,
                @"    OnBeforeOpen=BEGIN",
                @"                 BEGIN",
                @"                 END;",RecId);

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

        
        private void CollectElementsElements(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line)
        {
            if (!isCodeLines)
            {
                if (currentLineOnActualZone <= 1)
                {
                    AddDocRange(i, line);
                }
                AddEndOfZoneCodeRange(i, line,0);
            }
        }

    }
}
