using FastColoredTextBoxNS;
using NavCodeViewer.Business;
using NavCodeViewer.Domain;
using NavCodeViewer.Properties;
using System;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NavCodeViewer.Forms
{
    public partial class SourceViewer : DockContent
    {
        private NavObject _navObject;
        private bool activateComboTrigger = false;
        private MainForm _Mainform;
        private bool isQuickView;

        public NavObject NavObject
        {
            get => _navObject; set
            {
                _navObject = value;
                this.fctb.Navobject = value;
                this.fctb.SourceView = this;
            }
        }
        public bool ActivateComboTrigger { get => activateComboTrigger; set => activateComboTrigger = value; }
        public bool IsQuickView { get => isQuickView; set
            {
                isQuickView = value;
                if (NavObject != null)
                {
                    if (value)
                    {
                        Text = "(*) " + NavObject.ObjectName;
                    }
                    else
                    {
                        Text = NavObject.ObjectName;
                    }
                }
            }
        }
        public string ViewID
        {
            get
            {
                if (NavObject != null)
                {
                    return NavObject.ObjectName;
                }
                return "";
            }
        }
        public SourceViewer()
        {
            InitializeComponent();
        }
        public NavFCTB CurrentTB
        {
            get
            {
                return fctb;
            }
        }
        public MainForm Mainform
        {
            get
            {
                return _Mainform;
            }
            set
            {
                _Mainform = value;
                this.fctb.Zoom = _Mainform.ZoomVal;
            }
        }
        //public FastColoredTextBox GetFastTB()
        //{
        //    return fctb;
        //}

        public ToolStripComboBox GetCmbFunctions()
        {
            return cmbFunctions;
        }

        public ToolStripComboBox GetCmbElements()
        {
            return cmbElements;
        }

        public ToolStripComboBox GetCmbTriggers()
        {
            return cmbTriggers;
        }

        private void SourceViewer_Load(object sender, EventArgs e)
        {
        }

        private void SourceViewer_ResizeEnd(object sender, EventArgs e)
        {
        }
        public void GoToLine(int iLine)
        {
            fctb.GoToLine(iLine);
        }
        public void SetZoom(int val)
        {
            fctb.Zoom = (val);
        }
        public void SelectAll()
        {
            fctb.SelectAll();
        }
        private void fctb_SizeChanged(object sender, EventArgs e)
        {
            int size = fctb.Width;
            cmbElements.Width = Convert.ToInt32(size / 3);
            cmbTriggers.Width = Convert.ToInt32(size / 3);
            cmbFunctions.Width = size - cmbElements.Width - cmbTriggers.Width;
        }

        private void cmbFunctions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!activateComboTrigger) return;
            var it = cmbFunctions.SelectedItem;
            if (it != null)
            {
                int str = ((Function)it).StartingDefLine;
                GoToLine(str);
            }
        }

        private void cmbTriggers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!activateComboTrigger) return;
            var it = cmbTriggers.SelectedItem;
            if (it != null)
            {
                int str = ((Trigger)it).DefLine;
                GoToLine(str);
            }
        }

        private void cmbElements_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!activateComboTrigger) return;
            var it = cmbElements.SelectedItem;
            if (it != null)
            {
                int str = ((Field)it).StartingDefLine;
                GoToLine(str);
            }
        }
        public void ShowSource(NavObject obj)
        {
            if (_navObject != null)
            {
                if (obj.Type == _navObject.Type && obj.ID == _navObject.ID)
                {
                    return;
                }
            }

            if (IsQuickView)
            {
                Text = "(*) " + obj.ObjectName;
            }
            else
            {
                Text = obj.ObjectName;
            }

            fctb.Poc.Clear();
            foreach (CodeRange cr in obj.PlacesOfCode)
            {
                fctb.Poc.Add(new Range(fctb, cr.Start, cr.End));
            }
            fctb.Text = obj.ObjectTextSource;


            cmbFunctions.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFunctions.ComboBox.DisplayMember = "FunctionName";
            cmbFunctions.ComboBox.ValueMember = "FunctionName";
            cmbFunctions.ComboBox.DataSource = obj.FunctionList.OrderBy(c => c.FunctionName).ToArray();


            cmbTriggers.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTriggers.ComboBox.DisplayMember = "Name";
            cmbTriggers.ComboBox.ValueMember = "Name";
            cmbTriggers.ComboBox.DataSource = obj.Triggers.ToArray();


            cmbElements.ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbElements.ComboBox.DisplayMember = "FieldName";
            cmbElements.ComboBox.ValueMember = "FieldName";
            cmbElements.ComboBox.DataSource = obj.FieldList.ToArray();


            ActivateComboTrigger = true;
            NavObject = obj;
        }
        public void ShowFieldReferences(string fieldName)
        {
            Mainform.ShowFieldReferences(NavObject, fieldName);
        }
        public void ShowFunctionReferences(string fieldName)
        {
            Mainform.ShowFunctionReferences(NavObject, fieldName);
        }
        public void Copy()
        {
            fctb.Copy();
        }
        public void AddBookMark()
        {
            if (CurrentTB == null)
                return;
            CurrentTB.BookmarkLine(CurrentTB.Selection.Start.iLine);
        }
        public void RemoveBookMark()
        {
            if (CurrentTB == null)
                return;
            CurrentTB.UnbookmarkLine(CurrentTB.Selection.Start.iLine);
        }

        public void CollapseSelectedBlock()
        {
            fctb.CollapseBlock(fctb.Selection.Start.iLine, fctb.Selection.End.iLine);
        }
        public void CollapseAllObjectBlocks()
        {
            //for (int iLine = 0; iLine < fctb.LinesCount; iLine++)
            //{
            //    if (fctb[iLine].FoldingStartMarker == @"#region\b")//marker @"#region\b" was used in SetFoldingMarkers()
            //        fctb.CollapseFoldingBlock(iLine);
            //}
            var zones = NavObject.OthersPlaces;
            foreach (var pl in zones)
            {
                fctb.CollapseCodeRange(pl);
            }
        }

        public void CollapseAllObjectFunctions(bool collapse)
        {
            var zonesCode = NavObject.PlacesOfCode.Where(c => c.RangeType == TypeOfCodeRange.Function
              || c.RangeType == TypeOfCodeRange.Trigger).ToList();

            var othersZones = NavObject.OthersPlaces.Where(c => c.RangeType == TypeOfCodeRange.KeysDef
              || c.RangeType == TypeOfCodeRange.FieldGroupDef || c.RangeType == TypeOfCodeRange.ObjectsPropertiesDef
              || c.RangeType == TypeOfCodeRange.RDLDataDef).ToList();

            var zones = zonesCode.Union(othersZones);

            foreach (var p in zones)
            {
                if (p.IsFunctionDefinition) continue;
                var iLine = p.Start.iLine;
                if (fctb[iLine].FoldingStartMarker.IsNotNullOrEmpty())
                {
                    ExpandOrCollapseBlock(collapse, iLine);
                }
                else
                {
                    if (iLine + 1 < fctb.LinesCount)
                    {
                        if (fctb[iLine + 1].FoldingStartMarker.IsNotNullOrEmpty())
                        {
                            ExpandOrCollapseBlock(collapse, iLine + 1);
                        }
                    }
                }
            }
        }

        private void ExpandOrCollapseBlock(bool collapse, int iLine)
        {
            if (collapse)
            {
                fctb.CollapseFoldingBlock(iLine);
            }
            else
            {
                fctb.ExpandFoldedBlock(iLine);
            }
        }

        public void ExpandAllObjectBlocks()
        {
            var zones = NavObject.OthersPlaces;
            foreach (var p in zones)
            {
                var iLine = p.Start.iLine + 1;
                if (fctb[iLine].FoldingStartMarker.IsNotNullOrEmpty())
                {
                    fctb.ExpandFoldedBlock(iLine);
                }
            }
        }
    }
}