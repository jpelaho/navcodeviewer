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
    public partial class MenusuiteCollectElements : NavObjectCollectElements
    {
        private int currentLineOnActualZone = 0;
        TextBlockType actualZone = TextBlockType.None;
        TextBlockCodeType actualCodeType = TextBlockCodeType.None;

        public MenusuiteCollectElements(NavObject a) : base(a)
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
                var line = lines[i];
                UpdateActualZone(line,ref actualZone,ref isVarZone,ref currentLineOnActualZone);

                CollectOthersZones(lines, ref startZone, ref endZone, i);

                if (actualZone == TextBlockType.ObjectsProperties)
                {
                    CollectElementsObjectsProperties(ref isCodeLines, ref prevIsLastCodeLine, i, line,currentLineOnActualZone);
                }
                if (actualZone== TextBlockType.Properties)
                {
                    CollectElementsProperties(ref isCodeLines, ref prevIsLastCodeLine, i, line);
                }
                if (actualZone == TextBlockType.MenuNodes)
                {
                    //CollectElementsMenuNodes(ref isCodeLines, ref prevIsLastCodeLine, i, line);
                    if (line.StartsWith(@"  BEGIN"))
                    {
                        isCodeLines = true;
                        AddDocRange(i, line);

                        AddDocRange(lines.Length - 1, "          ");

                        //if (i == lines.Length - 1)
                        {
                            endZone = SaveOtherZone(ref startZone, lines.Length - 1, TypeOfCodeRange.MenuNodesDef);
                        }
                        break;
                    }
                }
                currentLineOnActualZone++;
            }

            return source;
        }
        private void CollectOthersZones(string[] lines, ref Place startZone, ref Place endZone, int i)
        {
            var line = lines[i];
            if (line.StartsWith(@"  PROPERTIES"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.ObjectsPropertiesDef);
            }
            if (line.StartsWith(@"  MENUNODES"))
            {
                endZone = SaveOtherZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);

                //Collect code Start Zone
                Place startZone1 = new Place(0, i);
                InsertZone(startZone1, startZone1, TypeOfCodeRange.MenuNodesDef);
            }

        }
        private void CollectElementsMenuNodes(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line)
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
        
        private void CollectElementsProperties(ref bool isCodeLines, ref bool isLastCodeLine, int i, string line)
        {

            //ProcessCodeTrigger(ref isCodeLines, ref isLastCodeLine, i, line, TextBlockCodeType.CodeUnit_OnRun,
            //     @"    OnRun=BEGIN",
            //     @"          BEGIN",
            //     @"          END;");

            if (!isCodeLines)
            {
                if (currentLineOnActualZone == 0)
                {
                    AddDocRange(i, line);
                    AddDocRange(i + 1, line);
                }
                AddEndOfZoneCodeRange(i, line, 0);
            }
        }
        
    }
}
