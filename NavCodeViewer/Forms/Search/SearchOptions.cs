using FastColoredTextBoxNS;
using NavCodeViewer.Business;
using NavCodeViewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NavCodeViewer.Forms
{
    public partial class SearchOptions :  DockContent
    {
        bool firstSearch = true;
        Place startPlace;
        //Place endPlace;
        MainForm mainForm;
        NavFCTB tb
        {
            get
            {
                if (mainForm != null)
                {
                    return mainForm.CurrentSourceViewer.CurrentTB;
                }
                return null;
            }
        }
        public SearchOptions(MainForm main)
        {
            mainForm = main;

            InitializeComponent();

            Init();
        }

        public MainForm MainForm { get => mainForm; set => mainForm = value; }

        private void Init()
        {
            lblSearchIn.Text = Resources.String28;
            cbMatchCasse.Text = Resources.String25;
            cbMatchWord.Text = Resources.String27;
            cbRegex.Text = Resources.String26;
            btnSearchAll.Text = Resources.String23;
            btnSearchPrec.Text = Resources.String22;
            btnSearchSuiv.Text = Resources.String21;
            Text = Resources.String37;

            cmbSearchIn.Items.Add(new SearchInItem
            {
                Type= SearchInType.CurrentDocument,
                Caption=Resources.String32
            });
            cmbSearchIn.Items.Add(new SearchInItem
            {
                Type = SearchInType.AllOpenDocs,
                Caption = Resources.String29
            });
            cmbSearchIn.Items.Add(new SearchInItem
            {
                Type = SearchInType.AllSameTypeDocs,
                Caption = Resources.String30
            });
            cmbSearchIn.Items.Add(new SearchInItem
            {
                Type = SearchInType.AllDocs,
                Caption = Resources.String31
            });
            cmbSearchIn.DisplayMember = nameof(SearchInItem.Caption);
            cmbSearchIn.ValueMember = nameof(SearchInItem.Type);
            cmbSearchIn.SelectedIndex = 0;
        }

        public virtual void Find(string pattern,bool IsNext)
        {
            try
            {
                RegexOptions opt = cbMatchCasse.Checked ? RegexOptions.None : RegexOptions.IgnoreCase;
                if (!cbRegex.Checked)
                    pattern = Regex.Escape(pattern);

                if (cbMatchWord.Checked)
                    pattern = "\\b" + pattern + "\\b";


                //Récupere la sélection (peut etre vide ou alors le mot trouvé encours)
                Range range = tb.Selection.Clone();
                range.Normalize();
                //

                if (firstSearch)
                {
                    startPlace = range.Start;
                    firstSearch = false;
                    //endPlace = range.End;
                }

                mainForm.AddFindWord(pattern);

                //
                if (IsNext)
                {
                    range.Start = range.End;
                    if (range.Start >= startPlace)
                        range.End = new Place(tb.GetLineLength(tb.LinesCount - 1), tb.LinesCount - 1);
                    else
                        range.End = startPlace;

                    foreach (var r in range.GetRangesByLines(pattern, opt))
                    {
                        tb.Selection = r;
                        tb.DoSelectionVisible();
                        tb.Invalidate();
                        return;
                    }

                    //Si aucune selection alors rechercher à partir du début
                    if (range.Start >= startPlace && startPlace > Place.Empty)
                    {
                        tb.Selection.Start = new Place(0, 0);
                        Find(pattern, IsNext);
                        return;
                    }
                }
                else
                {
                    Place saveEnd = range.Start;
                    //if (range.End <= endPlace)
                        range.Start = new Place(0, 0);
                    //else
                    //    range.Start = endPlace;
                    range.End = saveEnd;

                    foreach (var r in range.GetRangesByLinesReversed(pattern, opt))
                    {
                        tb.Selection = r;
                        tb.DoSelectionVisible();
                        tb.Invalidate();
                        return;
                    }
                }

                GlobalUI.Info(Resources.String33);
            }
            catch (Exception ex)
            {
                GlobalUI.ErrorMsg(ex.Message);
            }
        }

        private void btnSearchSuiv_Click(object sender, EventArgs e)
        {
            Find(cmbSearchText.Text,true);
        }

        private void cmbSearchText_DropDown(object sender, EventArgs e)
        {
            cmbSearchText.Items.Clear();
            foreach (string str in mainForm.FindWorlds)
            {
                cmbSearchText.Items.Add(str);
            }
        }
        void ResetSearch()
        {
            firstSearch = true;
        }

        private void SearchOptions_Activated(object sender, EventArgs e)
        {

        }
        protected override void OnActivated(EventArgs e)
        {
            cmbSearchText.Focus();
            //ResetSerach();
        }

        private void cbMatchWord_CheckedChanged(object sender, EventArgs e)
        {
            ResetSearch();
        }

        private void cbMatchCasse_CheckedChanged(object sender, EventArgs e)
        {
            ResetSearch();
        }

        private void cbRegex_CheckedChanged(object sender, EventArgs e)
        {
            ResetSearch();
        }

        private void cmbSearchText_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetSearch();
        }

        private void cmbSearchText_TextChanged(object sender, EventArgs e)
        {
            ResetSearch();
        }

        private void btnSearchPrec_Click(object sender, EventArgs e)
        {
            Find(cmbSearchText.Text, false);
        }

        private async void btnSearchAll_Click(object sender, EventArgs e)
        {
            try
            {
                var refs = new List<Reference>();

                var pattern = cmbSearchText.Text;

                RegexOptions opt = cbMatchCasse.Checked ? RegexOptions.None : RegexOptions.IgnoreCase;
                if (!cbRegex.Checked)
                    pattern = Regex.Escape(pattern);

                if (cbMatchWord.Checked)
                    pattern = "\\b" + pattern + "\\b";

                if (pattern.IsNullOrEmpty())
                {
                    GlobalUI.Error("S002", Resources.String35);
                }

                await SearchAllAsync(refs, pattern, opt);

                MainForm.ClearProgressInfos();
            }
            catch(Exception ex)
            {
                GlobalUI.ErrorMsg(ex.Message);
            }
        }

        private async Task SearchAllAsync(List<Reference> refs, string pattern, RegexOptions opt)
        {
            var listeObjs = new List<NavObject>();
            SearchInItem typeRech = ((SearchInItem)cmbSearchIn.SelectedItem);
            if (typeRech.Type == SearchInType.CurrentDocument)
            {
                if (mainForm.CurrentSourceViewer == null)
                {
                    GlobalUI.Error("S001", Resources.String34);
                }
                listeObjs.Add(mainForm.CurrentSourceViewer.NavObject);
            }
            if (typeRech.Type == SearchInType.AllOpenDocs)
            {
                foreach(var view in mainForm.GetListViewers) 
                {
                    listeObjs.Add(view.NavObject);
                }
            }
            if (typeRech.Type == SearchInType.AllSameTypeDocs)
            {
                if (mainForm.CurrentSourceViewer == null)
                {
                    GlobalUI.Error("S001", Resources.String34);
                }
                listeObjs = mainForm.CurrentSourceViewer.NavObject.NavProject.AllObjects.Where(c => c.Type == mainForm.CurrentSourceViewer.NavObject.Type).ToList();
            }
            if (typeRech.Type == SearchInType.AllDocs)
            {
                listeObjs = mainForm.ActiveProject.AllObjects;
            }


            await mainForm.SearchInObjectsAsync(cmbSearchText.Text,pattern, listeObjs,opt);
            //mainForm.ShowSearchReferences()

        }

        private void cmbSearchIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchInItem typeRech = ((SearchInItem)cmbSearchIn.SelectedItem);
            btnSearchPrec.Enabled = typeRech.Type == SearchInType.CurrentDocument;
            btnSearchSuiv.Enabled = typeRech.Type == SearchInType.CurrentDocument;
        }
    }
    public class SearchInItem
    {
        public SearchInType Type { get; set; }
        public string Caption { get; set; }
    }
    public enum SearchInType
    {
        CurrentDocument,
        AllOpenDocs,
        AllSameTypeDocs,
        AllDocs
    }
}
