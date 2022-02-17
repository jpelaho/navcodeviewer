using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace NavCodeViewer.Business
{
    public partial class FormatTable : FormatNavObject
    {
        public string FormatTableSource(string source)
        {
            string response = "";
            source = source.TrimEnd();

            //Enlever les deux dernier } qui seront remplacés
            source = source.ReplaceLastOccurrence("}", "END");
            //source = source.ReplaceLastOccurrence("}", "END");
            //source.TrimEnd();


            //source = source.Replace("\r", "");
            var indFin = 0; var indDebut = 0; var updated = "";var str = "";


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


            indFin = source.IndexOf(@"  FIELDS" + Environment.NewLine);
            str = source.Substring(indDebut, indFin - indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;
            indDebut = indFin;

            indFin = source.IndexOf(@"  KEYS" + Environment.NewLine);
            str = source.Substring(indDebut, indFin - indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;
            indDebut = indFin;

            indFin = source.IndexOf(@"  FIELDGROUPS" + Environment.NewLine);
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

        //public string FormatTableSourceOld(string source)
        //{
        //    string response = "";
        //    source = source.TrimEnd();

        //    //Enlever les deux dernier } qui seront remplacés
        //    source = source.ReplaceLastOccurrence("}", "");
        //    //source = source.ReplaceLastOccurrence("}", "");
        //    source.TrimEnd();

        //    string[] lines = source.SplitLines();
        //    response = lines[0] + Environment.NewLine + "BEGIN" + Environment.NewLine;

        //    //actualIndex = 2;

        //    var startZone = new Place(0, 2);
        //    var endZone = new Place(0, 0);

        //    var sourceZone = "";
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        //Verifiez que les 2 prémieres lignes collent ici
        //        if (i == 0 || i == 1) continue;

        //        //if (i == 1895)
        //        //{
        //        //    var p = 0;
        //        //}

        //        var line = lines[i];
        //        if (line.Contains("\r"))
        //        {
        //            var p = 1;
        //        }

        //        if (line.StartsWith(@"  PROPERTIES"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatObjectsProperties(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.ObjectsPropertiesDef);
        //            }
        //        }
        //        if (line.StartsWith(@"  FIELDS"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatReplaceBeginEnd(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);
        //            }
        //        }
        //        if (line.StartsWith(@"  KEYS"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatFields(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.FieldsDef);
        //            }
        //        }
        //        if (line.StartsWith(@"  FIELDGROUPS"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatKeys(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.KeysDef);
        //            }
        //        }
        //        if (line.StartsWith(@"  CODE"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatFieldGroups(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.FieldGroupDef);
        //            }
        //        }
        //        if (i == lines.Length - 1)
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatCode(sourceZone);
        //                response = response + updated;
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

        //private List<string> SplitSource(string source, ref string objectsproperties,ref string properties,ref string keys,ref string fieldgroups,
        //    ref string code,ref string fields)
        //{
        //    int ind = source.IndexOf("  OBJECT-PROPERTIES");
        //    int ind2 = source.IndexOf("  PROPERTIES");
        //    objectsproperties = GetStringBetween;
        //}
        private string FormatKeys(string source)
        {
            source = source.TrimEnd();
            string response = @"  KEYS
  BEGIN" + Environment.NewLine;
            string updatedcolSource = "";
            string[] lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0 || i == 1 || i == lines.Length - 1) continue;
                var line = lines[i];
                if (line.StartsWith(@"    {"))
                {
                    updatedcolSource = FormatFieldSource(line);
                    response += updatedcolSource + Environment.NewLine;
                }
            }
            response += @"  END" + Environment.NewLine;
            return response;
        }
        private string FormatFieldGroups(string source)
        {
            string response = @"  FIELDGROUPS
  BEGIN" + Environment.NewLine;
            string updatedcolSource = "";
            string[] lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0 || i == 1 || i == lines.Length - 1) continue;
                var line = lines[i];
                if (line.StartsWith(@"    {"))
                {
                    updatedcolSource = FormatFieldSource(line);
                    response += updatedcolSource + Environment.NewLine;
                }
            }
            response += @"  END" + Environment.NewLine;
            return response;
        }
        private string FormatFields(string source)
        {
            string line = "";
            source = source.TrimEnd();
            string response = @"  FIELDS
  BEGIN" + Environment.NewLine;
            string colSource = "", updatedcolSource = "";
            string[] lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0 || i == 1 || i == lines.Length - 1) continue;
                line = lines[i];
                if (line.StartsWith(@"    {"))
                {
                    if (colSource != "")
                    {
                        updatedcolSource = FormatFieldSource(colSource);
                        response += updatedcolSource;
                        colSource = "";
                    }
                }
                colSource += line + Environment.NewLine;
            }

            if (colSource != "")
            {
                var removeLastNEwLine = colSource.ReplaceLastOccurrence(Environment.NewLine, "");
                updatedcolSource = FormatFieldSource(removeLastNEwLine);
                response += updatedcolSource;
                colSource = "";
            }

            response += Environment.NewLine + @"  END" + Environment.NewLine;
            return response;
        }
        //private string FormatAndCollectField(string oneFieldSource, Field field)
        //{
        //    string[] lines = oneFieldSource.SplitLines();
        //    oneFieldSource = oneFieldSource.ReplaceFirstOccurrence("{", "BEGIN");
        //    oneFieldSource = oneFieldSource.ReplaceLastOccurrence("}", "END");
        //    return oneFieldSource;
        //}
        private string FormatFieldSource(string oneFieldSource)
        {
            //var replaced = oneFieldSource.ReplaceFirstOccurrence("{", "");
            //replaced = replaced.ReplaceLastOccurrence("}", "");
            return oneFieldSource;
        }
    }
}
