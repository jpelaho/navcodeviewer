using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NavCodeViewer.Business
{
    public class FormatNavObject
    {
        //public List<CodeRange> ZonesPlaces { get; set; } = new List<CodeRange>();
        public FormatNavObject()
        {
        }

        protected string FormatObjectsProperties(string source)
        {
            source = Regex.Replace(source, @"(?<=\s\s){", "BEGIN");
            source = Regex.Replace(source, @"(?<=\s\s)}", "END");
            return source;
        }
        //protected string FormatProperties2(string source)
        //{
        //    source = Regex.Replace(source, @"(?<=\s\s){", "BEGIN");
        //    source = Regex.Replace(source, @"(?<=\s\s)}", "END");
        //    return source;
        //}
        protected string FormatReplaceBeginEnd(string source)
        {
            source=source.Replace("\r\r\n", "\r\n");
            //source = source.Replace("\n\r\n", "\r\n");
            source = source.ReplaceFirstOccurrence("{", "BEGIN");
            source = source.ReplaceLastOccurrence("}", "END");

            return source;
        }
        //protected string FormatReplaceBegin(string source)
        //{
        //    source = source.ReplaceFirstOccurrence("{", "BEGIN");
        //    return source;
        //}
        protected string FormatCode(string source)
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
        protected string GetStringBetween(string source,string marker1,string marker2)
        {
            int ind = source.IndexOf(marker1);
            int ind2 = source.IndexOf(marker2);
            return source.Substring(ind, ind2 - ind);
        }
        protected Place SaveZone(ref Place startZone, int i, TypeOfCodeRange type)
        {
            Place endZone = new Place("  END".Length, i - 1);
            InsertZone(startZone, endZone, type);
            startZone = new Place(0, i);
            return endZone;
        }
        private void InsertZone(Place startZone, Place endZone, TypeOfCodeRange type)
        {
            //var r = new CodeRange(startZone, endZone);
            //r.RangeType = type;
            //ZonesPlaces.Add(r);
        }
    }
}
