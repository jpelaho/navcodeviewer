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
    public partial class FormatPage : FormatNavObject
    {
        public string FormatPageSource(string source)
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


            indFin = source.IndexOf(@"  CONTROLS" + Environment.NewLine);
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
        //public string FormatPageSource11(string source)
        //{
        //    string response = "";
        //    source = source.TrimEnd();

        //    //Enlever les deux dernier } qui seront remplacés
        //    source = source.ReplaceLastOccurrence("}", "");
        //    source = source.ReplaceLastOccurrence("}", "");
        //    source.TrimEnd();

        //    string[] lines = source.SplitLines();
        //    response = lines[0] + Environment.NewLine + "BEGIN" + Environment.NewLine;

        //    var sourceZone = "";
        //    var startZone = new Place(0, 2);
        //    var endZone = new Place(0, 0);

        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        if (i == 562)
        //        {
        //            var p = 1;
        //        }
        //        if (i == 0 || i == 1) continue;
        //        var line = lines[i].Replace("\r", "");


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
        //        if (line.StartsWith(@"  CONTROLS"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatProperties(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);
        //            }
        //        }
        //        if (line.StartsWith(@"  CODE"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatControls(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.ControlsDef);
        //            }
        //        }
        //        if (i == lines.Length - 1)
        //        {
        //            sourceZone = sourceZone + line + Environment.NewLine;
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
        
        private string FormatControls(string source)
        {
            string line = "";
            source = source.TrimEnd();
            string response = @"  CONTROLS
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
                        updatedcolSource = RemoveParentheses(colSource);
                        response += updatedcolSource;
                        colSource = "";
                    }
                }
                colSource += line + Environment.NewLine;
            }

            if (colSource != "")
            {
                var removeLastNEwLine = colSource.ReplaceLastOccurrence(Environment.NewLine, "");
                updatedcolSource = RemoveParentheses(removeLastNEwLine);
                response += updatedcolSource;
                colSource = "";
            }

            response += Environment.NewLine + @"  END" + Environment.NewLine;
            return response;
        }

        private new string FormatProperties(string source)
        {
            string line = "";
            source = source.TrimEnd();
            string response = @"  PROPERTIES
  BEGIN" + Environment.NewLine;
            string colSource = "", updatedcolSource = "";
            string[] lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0 || i == 1 || i == lines.Length - 1) continue;
                line = lines[i];


                if (line.StartsWith(@"    {"))
                {
                    line = line.Replace("{", "BEGIN");
                }
                if (line.StartsWith(@"    }"))
                {
                    line = line.Replace("}", "END");
                }

                if (line.StartsWith(@"      {"))
                {
                    if (colSource != "")
                    {
                        updatedcolSource = RemoveParentheses(colSource);
                        response += updatedcolSource;
                        colSource = "";
                    }
                }
                colSource += line + Environment.NewLine;
            }

            if (colSource != "")
            {
                var removeLastNEwLine = colSource.ReplaceLastOccurrence(Environment.NewLine, "");
                updatedcolSource = RemoveParentheses(removeLastNEwLine);
                response += updatedcolSource;
                colSource = "";
            }

            response += Environment.NewLine + @"  END" + Environment.NewLine;
            return response;
        }

        private string RemoveParentheses(string oneFieldSource)
        {
            //var replaced = oneFieldSource.ReplaceFirstOccurrence("{", "");
            //replaced = replaced.ReplaceLastOccurrence("}", "");
            return oneFieldSource;
        }
    }
}
