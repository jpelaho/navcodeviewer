using FastColoredTextBoxNS;
using NavCodeViewer.Business;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace navtest1
{
    public partial class Form1 : Form
    {
        //Color currentLineColor = Color.FromArgb(100, 210, 210, 255);


        
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Project pr = new Project();

            //main.Maximum = 100;
            //progressBar1.Step = 1;

            //var progress = new Progress<int>(v =>
            //{
            //    // This lambda is executed in context of UI thread,
            //    // so it can safely update form controls
            //    progressBar1.Value = v;
            //});

            //pr.LoadFileAndCollectReferences(@"D:\WinProject\NavEditor\TestsFiles\TestNavEditor06.TXT");
            //OpenTextFile(pr.Menusuites[0]);
        }
        private void OpenTextFile(NavObject rec)
        {
            //var tomodified = File.ReadAllText(@"D:\WinProject\NavEditor\TestsFiles\TestNavEditor06.TXT",
            //    Encoding.GetEncoding(850));
            //var rec = new MenusuiteMgt();
            //var result = rec.ProcessMenu(tomodified);

            foreach (CodeRange cr in rec.PlacesOfCode)
            {
                fctb.Poc.Add(new Range(fctb, cr.Start, cr.End));
            }
            var z = rec.PlacesOfCodeWithRecId;

            //rec.CollectReferences(result);
            var refs = rec.NavProject.References;
            
            var localfunctions = refs.Where(c => c.ReferenceType == RefType.InternalFunction).ToList();
            var extfunctions = refs.Where(c => c.ReferenceType == RefType.ExternalFunction).ToList();
            var localFiels = refs.Where(c => c.ReferenceType == RefType.InternalField).ToList();
            var extFiels = refs.Where(c => c.ReferenceType == RefType.ExternalField).ToList();
            var pars = refs.Where(c => c.ReferenceType == RefType.Parameter).ToList();
            var locvars = refs.Where(c => c.ReferenceType == RefType.LocalVariable).ToList();
            var glovars = refs.Where(c => c.ReferenceType == RefType.GlobalVariable).ToList();
            //var codes = table.PlacesOfCode.Where(c => c.CanHaveReference).ToList();
            fctb.Text = rec.ObjectTextSource;
            //File.WriteAllText(@"D:\WinProject\NavEditor\TestsFiles\TestNavEditor07.TXT", result, Encoding.UTF8);
            //fctb.OpenFile(@"D:\WinProject\NavEditor\TestsFiles\TestNavEditor07.TXT");
        }

        void TestSynthaxHighLigth()
        {
            var tomodified = File.ReadAllText(@"D:\WinProject\NavEditor\TestsFiles\TestNavEditor06.TXT",
                Encoding.GetEncoding(850));
            string[] lines = tomodified.SplitLines();
            var lineDeb = lines[0];
            var lineFin = lines[lines.Length - 1];
            fctb.Poc.Add(new Range(fctb,
                new Place(0,0),
                new Place(lineFin.Length, lines.Length - 1)));
            fctb.Text = tomodified;
        }
        private void fastColoredTextBox1_TextChangedDelayed(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            ////delete all markers
            //fctb.Range.ClearFoldingMarkers();

            //var currentIndent = 0;
            //var lastNonEmptyLine = 0;

            //for (int i = 0; i < fctb.LinesCount; i++)
            //{
            //    var line = fctb[i];
            //    var spacesCount = line.StartSpacesCount;
            //    if (spacesCount == line.Count) //empty line
            //        continue;

            //    if (currentIndent < spacesCount)
            //        //append start folding marker
            //        fctb[lastNonEmptyLine].FoldingStartMarker = "m" + currentIndent;
            //    else
            //    if (currentIndent > spacesCount)
            //        //append end folding marker
            //        fctb[lastNonEmptyLine].FoldingEndMarker = "m" + spacesCount;

            //    currentIndent = spacesCount;
            //    lastNonEmptyLine = i;
            //}

        }
        private string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
        private void fctb_FileOpened(object sender, EventArgs e)
        {

            ////Text = TransformCALText();
            //var modified = fctb.Text;
            //modified = Regex.Replace(modified, @"(?<=\bOBJECT\b\s.*\r\n){", "BEGIN");
            //modified = Regex.Replace(modified, @"(?<=\bOBJECT-PROPERTIES\b\r\n\s\s){", "BEGIN");
            //modified = Regex.Replace(modified, @"(?<=\s\s\s\s\s\s\bVersion\sList=.*\r\n\s\s)}", "END");
            //modified = Regex.Replace(modified, @"(?<=\s*\bPROPERTIES\b\r\n\s*){", "BEGIN");

            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bCODE\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bCODE\b\r\n\s\s){", "BEGIN");


            ////table
            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bFIELDS\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bFIELDS\b\r\n\s\s){", "BEGIN");
            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bKEYS\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bKEYS\b\r\n\s\s){", "BEGIN");
            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bFIELDGROUPS\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bFIELDGROUPS\b\r\n\s\s){", "BEGIN");

            ////page
            //modified = Regex.Replace(modified, @"}(?=\r\n\s*\bCONTROLS\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bCONTROLS\b\r\n\s*){", "BEGIN");

            ////report
            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bDATASET\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bDATASET\b\r\n\s\s){", "BEGIN");
            //modified = Regex.Replace(modified, @"}(?=\r\n\s*\bREQUESTPAGE\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bREQUESTPAGE\b\r\n\s*){", "BEGIN");
            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bLABELS\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bLABELS\b\r\n\s\s){", "BEGIN");
            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bRDLDATA\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bRDLDATA\b\r\n\s\s){", "BEGIN {");

            ////query
            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bELEMENTS\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bELEMENTS\b\r\n\s\s){", "BEGIN");

            ////xmlports
            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bEVENTS\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bEVENTS\b\r\n\s\s){", "BEGIN");
            //modified = Regex.Replace(modified, @"}(?=\r\n\s*\bEND\b\r\n\s*\bCODE\b\r\n)", "END");
            ////modified = Regex.Replace(modified, @"(?<=\bCONTROLS\b\r\n\s\s){", "BEGIN");

            ////menusuite
            //modified = Regex.Replace(modified, @"}(?=\r\n\s\s\bMENUNODES\b\r\n)", "END");
            //modified = Regex.Replace(modified, @"(?<=\bMENUNODES\b\r\n\s\s){", "BEGIN");
            


            //modified = ReplaceLastOccurrence(modified, "}", "END");
            //modified = ReplaceLastOccurrence(modified, "}", "END");
            //modified = ReplaceLastOccurrence(modified, "END_OF_RDLDATA", "} ");


            //fctb.Text = modified;
            ////(?<=\s\s\s\s\s\s\bVersion\sList=.*\r\n\s\s)}
            ////(?<=\bOBJECT-PROPERTIES\b\r\n\s\s){
            ////(?<=\s\s\bPROPERTIES\b\r\n\s\s){
            ////}(?=\r\n\s\s\bCODE\b\r\n)
        }

        //private string TransformCALText()
        //{
        //    //s = Regex.Replace(s, @"\bwest\b", "something");
        //}
    }
}