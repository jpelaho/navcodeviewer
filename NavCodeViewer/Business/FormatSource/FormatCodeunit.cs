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
    public partial class FormatCodeunit: FormatNavObject
    {
        public string FormatCodeUnit(string source)
        {
            string response = "";
            source = source.TrimEnd();

            //Enlever les deux dernier } qui seront remplacés
            source = source.ReplaceLastOccurrence("}", "END");
            //source = source.ReplaceLastOccurrence("}", "END");
            //source.TrimEnd();


            //source = source.Replace("\r", "");
            var indFin = 0; var indDebut = 0; var updated = ""; var str = "";


            indDebut = 0;
            indFin = source.IndexOf(@"  OBJECT-PROPERTIES" + Environment.NewLine);
            str = source.Substring(indDebut, indFin);
            response += str.Replace("{", "BEGIN");
            indDebut = indFin;


            indFin = source.IndexOf(@"  PROPERTIES" + Environment.NewLine);
            str = source.Substring(indDebut, indFin - indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;
            indDebut = indFin;


            indFin = source.IndexOf(@"  CODE" + Environment.NewLine); ;
            str = source.Substring(indDebut, indFin - indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;
            indDebut = indFin;


            str = source.Substring(indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;


            return response;
        }
        //public string FormatCodeUnit22(string source)
        //{
        //    string response = "";
        //    source = source.TrimEnd();

        //    //Enlever les deux dernier } qui seront remplacés
        //    source = source.ReplaceLastOccurrence("}", "");
        //    source = source.ReplaceLastOccurrence("}", "");
        //    source.TrimEnd();

        //    string[] lines = source.SplitLines();
        //    response = lines[0] + Environment.NewLine + "BEGIN" + Environment.NewLine;

        //    //actualIndex = 2;

        //    var sourceZone = "";
        //    var startZone = new Place(0, 2);
        //    var endZone = new Place(0, 0);

        //    for (int i = 0; i < lines.Length; i++)
        //    {

        //        if (i == 0 || i == 1) continue;
        //        var line = lines[i];

        //        if (line.StartsWith(@"  PROPERTIES"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatObjectsProperties(sourceZone);
        //                response = response + updated ;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.ObjectsPropertiesDef);
        //            }
        //        }
        //        if (line.StartsWith(@"  CODE"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatReplaceBeginEnd(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);
        //            }
        //        }
        //        if (i == lines.Length - 1)
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatReplaceBeginEnd(sourceZone);
        //                response = response + updated ;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.CodeDef);
        //            }
        //        }
        //        else
        //        {
        //            sourceZone = sourceZone + line + Environment.NewLine;
        //        }
        //    }
        //    response += @"END";
        //    return response;
        //}
  
    }
}
