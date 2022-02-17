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
    public partial class TableCollectElements : NavObjectCollectElements
    {
        private CodeRange actualCodeRange = null;
        private Function actualFunction=null;
        
        private int currentLineOnActualZone = 0;

        TextBlockType actualZone = TextBlockType.None;
        TextBlockCodeType actualCodeType = TextBlockCodeType.None;
        string actualCodeTriggerName = "";

        public TableCollectElements(NavObject a) : base(a)
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
                var line = lines[i];

                if (i == 1897)
                {
                    var p = 0;
                }

                UpdateActualZone(line, ref actualZone, ref isVarZone, ref currentLineOnActualZone);
                CollectOthersZones(line, ref startZone, ref endZone, i);

                if (actualZone == TextBlockType.ObjectsProperties)
                {
                    CollectElementsObjectsProperties(ref isCodeLines, ref prevIsLastCodeLine, i, line, currentLineOnActualZone);
                }
                if (actualZone == TextBlockType.Properties)
                {
                    CollectElementsProperties(ref isCodeLines, ref prevIsLastCodeLine, i, line, ref isVarZone, navObj.ID);
                }
                if (actualZone == TextBlockType.Fields)
                {
                    CollectElementsFields(ref isCodeLines, ref prevIsLastCodeLine, ref isVarZone, i, line, navObj.ID);
                }
                if (actualZone == TextBlockType.Keys)
                {
                    CollectElementsKeys(ref isCodeLines, ref prevIsLastCodeLine, i, line);
                }
                if (actualZone == TextBlockType.FieldGroups)
                {
                    CollectElementsFieldGroups(ref isCodeLines, ref prevIsLastCodeLine, i, line);
                }
                if (actualZone == TextBlockType.Code)
                {
                    CollectElementsCode(ref isCodeLines, ref prevIsLastCodeLine, ref actualFunction, ref actualCodeRange,
                        currentLineOnActualZone, i, line, ref isVarZone, navObj.ID);
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
            if (line.StartsWith(@"  FIELDS"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);
            }
            if (line.StartsWith(@"  KEYS"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.FieldsDef);
            }
            if (line.StartsWith(@"  FIELDGROUPS"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.KeysDef);
            }
            if (line.StartsWith(@"  CODE"))
            {
                //FIELDGROUPS
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.FieldGroupDef);

                //Collect code Start Zone
                Place startZone1 = new Place(0, i);
                InsertZone(startZone1, startZone1, TypeOfCodeRange.CodeDef);
            }
        }
        private Field CollectFieldInfos(string line, int iLine)
        {
            var field = new Field();
            var ColValues = line.Split(';');
            string colID = ColValues[0].Replace("{", "").Trim();
            field.FieldID = Convert.ToInt32(colID);
            field.FieldName = ColValues[2].Trim();
            field.FieldType = ColValues[3].Trim();
            field.ObjectID = navObj.ID;
            field.ObjectType = ObjectType.Table;
            field.StartingDefLine = iLine + 1;
            return field;
        }

        private void CollectElementsProperties(ref bool isCodeLines, 
            ref bool isLastCodeLine, int i, string line, ref bool isVarZone,int RecId)
        {

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Table_OnInsert,
                @"    OnInsert=BEGIN",
                @"             BEGIN",
                @"             END;",RecId);

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Table_OnModify,
                @"    OnModify=BEGIN",
                @"             BEGIN",
                @"             END;", RecId);

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Table_OnDelete,
                @"    OnDelete=BEGIN",
                @"             BEGIN",
                @"             END;", RecId);

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Table_OnRename,
                @"    OnRename=BEGIN",
                @"             BEGIN",
                @"             END;", RecId);

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
        
        private void CollectElementsFields(ref bool isCodeLines, 
            ref bool isLastCodeLine, ref bool isVarZone, int i, string line,int RecId)
        {
            if (line.Length > 5)
            {
                if (System.Char.IsNumber(line[6]))
                {
                    var f = CollectFieldInfos(line, i);
                    actualCodeTriggerName = f.FieldName;
                    navObj.FieldList.Add(f);
                }
                if (line.Contains("OptionString="))
                {
                    var lineStr = line.Replace("OptionString=", "").FormatEndOfElement().Trim();
                    lineStr = lineStr.RemoveAtBorders("[", "]");
                    var lastField = navObj.FieldList.LastOrDefault();
                    if (lastField != null) lastField.OptionString = lineStr;
                }
            }

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Table_OnLookUp,
                @"                                                   OnLookup=BEGIN",
                @"                                                            BEGIN",
                @"                                                            END;",RecId);

            ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, ref isVarZone, i, line,
                ref actualCodeRange, ref actualCodeType, ref actualCodeTriggerName, TextBlockCodeType.Table_OnValidate,
                @"                                                   OnValidate=BEGIN",
                @"                                                              BEGIN",
                @"                                                              END;",RecId);

            
            if (!isCodeLines)
            {
                if (currentLineOnActualZone <= 1)
                {
                    AddDocRange(i, line);
                }
                AddEndOfZoneCodeRange(i, line,RecId);
            }
        }
        private void CollectElementsKeys(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line)
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
        private void CollectElementsFieldGroups(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line)
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
    }
}
