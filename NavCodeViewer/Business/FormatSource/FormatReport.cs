using FastColoredTextBoxNS;
using NavCodeViewer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace NavCodeViewer.Business
{
    public partial class FormatReport : FormatNavObject
    {
        public string FormatReportSource(string source)
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


            indFin = source.IndexOf(@"  DATASET" + Environment.NewLine);
            str = source.Substring(indDebut, indFin - indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;
            indDebut = indFin;

            indFin = source.IndexOf(@"  REQUESTPAGE" + Environment.NewLine);
            str = source.Substring(indDebut, indFin - indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;
            indDebut = indFin;

            indFin = source.IndexOf(@"  LABELS" + Environment.NewLine);
            str = source.Substring(indDebut, indFin - indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;
            indDebut = indFin;

            indFin = source.IndexOf(@"  CODE" + Environment.NewLine); 
            str = source.Substring(indDebut, indFin - indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;
            indDebut = indFin;


            indFin = source.IndexOf(@"  RDLDATA" + Environment.NewLine); 
            str = source.Substring(indDebut, indFin - indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;
            indDebut = indFin;


            var marker = @"  END_OF_RDLDATA" + Environment.NewLine;
            indFin = source.IndexOf(marker);
            if (indFin >= 0)
            {
                indFin = indFin + marker.Length;
                str = source.Substring(indDebut, indFin - indDebut);
                updated = FormatReplaceBeginEnd(str);
                response += updated;
                indDebut = indFin;
            }


            str = source.Substring(indDebut);
            updated = FormatReplaceBeginEnd(str);
            response += updated;


            return response;
        }
        //public string FormatReportSource55(string source)
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

        //    var sourceZone = "";
        //    var startZone = new Place(0, 2);
        //    var endZone = new Place(0, 0);

        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        //if(i==562)
        //        //{
        //        //    var p = 1;
        //        //}
        //        if (i == 0 || i == 1) continue;
        //        var line = lines[i].Replace("\r","");


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
        //        if (line.StartsWith(@"  DATASET"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatReplaceBeginEnd(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.PropertiesDef);
        //            }
        //        }
        //        if (line.StartsWith(@"  REQUESTPAGE"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatDataSet(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.DataSetDef);
        //            }
        //        }
        //        if (line.StartsWith(@"  LABELS"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatRequestPage(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.RequestPage);
        //            }
        //        }
        //        if (line.StartsWith(@"  CODE"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatLabels(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.LabelsDef);
        //            }
        //        }
        //        if (line.StartsWith(@"  RDLDATA"))
        //        {
        //            if (sourceZone != "")
        //            {
        //                var updated = FormatCode(sourceZone);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, i, TypeOfCodeRange.CodeDef);
        //            }

        //            string rdlText = "";
        //            var index = source.IndexOf(@"  RDLDATA");
        //            var index2 = source.IndexOf("{", index);
        //            rdlText = source.Substring(index2);
        //            if (rdlText != "")
        //            {
        //                var updated = FormatRDLData(rdlText);
        //                response = response + updated;
        //                sourceZone = "";

        //                endZone = SaveZone(ref startZone, lines.Length - 1, TypeOfCodeRange.RDLDataDef);
        //            }
        //            break;
        //        }
        //        //if (i == lines.Length - 1)
        //        //{
        //        //    if (sourceZone != "")
        //        //    {
        //        //        var updated = FormatRDLData(sourceZone);
        //        //        response = response + updated;
        //        //        sourceZone = "";

        //        //        endZone = SaveZone(ref startZone, i, TypeOfCodeRange.RDLDataDef);
        //        //    }
        //        //}
        //        else
        //        {
        //            sourceZone = sourceZone + line + Environment.NewLine; 
        //        }
        //    }
        //    response += @"END";
        //    return response;
        //}
        
        //private void SplitSource(string source,ref string )
        private new string FormatCode(string source)
        {
            source = source.TrimEnd();
            string response = @"  CODE
  BEGIN" + Environment.NewLine;
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
        private string FormatLabels(string source)
        {
            string line = "";
            source = source.TrimEnd();
            string response = @"  LABELS
  BEGIN" + Environment.NewLine;
            string colSource = "", updatedcolSource = "";
            string[] lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0 || i == 1 || i == lines.Length - 1) continue;
                line = lines[i];
                //colSource += line;
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
        private string FormatRDLData(string source)
        {
            string line = "";
            source = source.TrimEnd();

            string response = @"  RDLDATA
  BEGIN" + Environment.NewLine;
            //string response = "";
            //string[] lines = source.SplitLines();
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    if (i == 0 || i == 1) continue;
            //    line = lines[i];

            //    response += line + Environment.NewLine;
            //}
            response = response + source + Environment.NewLine;
            response +=  @"  END" + Environment.NewLine;
            return response;
        }
        private string FormatDataSet(string source)
        {
            string line = "";
            source = source.TrimEnd();
            string response = @"  DATASET
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
        private string FormatRequestPage(string source)
        {
            string line = "";
            source = source.TrimEnd(); string sourceZone = "";
            string response = @"  REQUESTPAGE
  BEGIN" + Environment.NewLine;
            //string colSource = "";
            string[] lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0 || i == 1 || i == lines.Length - 1) continue;
                line = lines[i];

                if (line.StartsWith(@"    CONTROLS"))
                {
                    
                    if (sourceZone != "")
                    {
                        var updated = FormatRequestPageProperties(sourceZone);
                        response = response + updated;
                        sourceZone = "";
                    }
                }

                sourceZone += line + Environment.NewLine;
            }

            if (sourceZone != "")
            {
                var updated = FormatRequestPageControls(sourceZone);
                response = response + updated;
                sourceZone = "";
            }

            response += Environment.NewLine + @"  END" + Environment.NewLine;
            return response;
        }
        private string FormatRequestPageProperties(string source)
        {
            string line = "";
            source = source.TrimEnd();
            string response = @"    PROPERTIES
    BEGIN" + Environment.NewLine;
            //string colSource = "", updatedcolSource = "";
            string[] lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0 || i == 1 || i == lines.Length - 1) continue;
                line = lines[i];

                response += line + Environment.NewLine;
            }
            response +=  @"    END" + Environment.NewLine;
            return response;
        }
        private string FormatRequestPageControls(string source)
        {
            string line = "";
            source = source.TrimEnd();
            string response = @"    CONTROLS
    BEGIN" + Environment.NewLine;
            string colSource = "", updatedcolSource = "";
            string[] lines = source.SplitLines();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0 || i == 1 || i == lines.Length - 1) continue;
                line = lines[i];
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

            response += @"    END" + Environment.NewLine;
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
