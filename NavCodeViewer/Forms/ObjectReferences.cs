using NavCodeViewer.Business;
using NavCodeViewer.Domain;
using NavCodeViewer.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NavCodeViewer.Forms
{
    public partial class ObjectReferences : DockContent
    {
        //private NavObject navObject;
        private Project navProject;
        private TreeNode SelectedNod = null;
        private string RefofObjName;
        private int mClick = 0;

        public MainForm mainForm
        {
            get; set;
        }

        public List<Reference> References
        {
            get; set;
        }

        public ObjectReferences(MainForm main)
        {
            mainForm = main;
            InitializeComponent();
            Text = Resources.String8;
            ImportImages();
        }

        public void SetNavObject(Project obj, List<Reference> References1, string refobj,bool isSearch)
        {
            RefofObjName = refobj;
            navProject = obj;
            References = References1;
            LoadMenu();
            if (isSearch)
            {
                Text = string.Format("({0}) {1} : '{2}'", References.Count, Resources.String36, RefofObjName);
            }
            else
            {
                Text = string.Format("({0}) {1} : '{2}'", References.Count, Resources.String17, RefofObjName);
            }
        }

        public void Reset()
        {
            References = null;
            tvObjectRefs.Nodes.Clear();
            Text = Resources.String17;
        }

        private void ImportImages()
        {
            TVimageList.Images.Add("codeunit", Resources.codeunit);
            TVimageList.Images.Add("page", Resources.page);
            TVimageList.Images.Add("group", Resources.group);
            TVimageList.Images.Add("menusuite", Resources.menusuite);
            TVimageList.Images.Add("query", Resources.query);
            TVimageList.Images.Add("report", Resources.report);
            TVimageList.Images.Add("table", Resources.table);
            TVimageList.Images.Add("xmlport", Resources.xmlport);
            TVimageList.Images.Add("field", Resources.field);
            TVimageList.Images.Add("function", Resources.function);
            TVimageList.Images.Add("Refe", Resources.Refe);
        }

        public void LoadMenu()
        {
            var tab = new List<TreeNode>();
            if (References == null) return;

            var listeObj = References.GroupBy(x => new { x.RefBy_ObjetType, x.RefBy_ObjetID },
                (key, group) => new
                {
                    TypeGr = key.RefBy_ObjetType,
                    IdGr = key.RefBy_ObjetID,
                    NbreRef = group.Count()
                });

            foreach (var gr in listeObj)
            {
                var rec = navProject.GetObject(gr.IdGr, gr.TypeGr) as NavObject;
                if (rec == null) continue;
                List<Reference> childs = References.Where(c => c.RefBy_ObjetID == gr.IdGr && c.RefBy_ObjetType == gr.TypeGr).ToList();
                TreeNode item = BuildDetailGroup(rec, childs, "Refe");
                tab.Add(item);
            }

            //this.BeginInvoke(new InvokeDelegate(InvokeMethod(tab)));
            try
            {
                this.Invoke((Action)(() => InvokeMethod(tab)));
            }
            catch { }
        }

        private TreeNode BuildDetailGroup(NavObject refbyObj, List<Reference> objs, string image)
        {
            var tab = new List<TreeNode>();
            var lignes = refbyObj.ObjectTextSource.SplitLines();
            foreach (var obj in objs)
            {
                string itemName = lignes[obj.ReferenceLine - 1].Trim();
                string itemCode = obj.ReferenceLine.ToString() + "|" + refbyObj.ObjectName;
                TreeNode item = GetTreeNode(itemCode, itemName, image);
                tab.Add(item);
            }
            string parentName = string.Format("{0} {1}  ({2})", refbyObj.ObjectName, refbyObj.Name, tab.Count);
            TreeNode detailGroup = GetTreeNode(refbyObj.ObjectName, parentName, refbyObj.GetImageString(), tab.ToArray());
            return detailGroup;
        }

        private TreeNode GetTreeNode(string name, string descr, string imagekey, TreeNode[] children)
        {
            TreeNode node = new TreeNode(descr, children);
            node.Name = name;
            node.ImageKey = imagekey;
            node.SelectedImageKey = imagekey;
            return node;
        }

        public void InvokeMethod(List<TreeNode> tab)
        {
            this.tvObjectRefs.Nodes.Clear();
            this.tvObjectRefs.Nodes.AddRange(tab.ToArray());
        }

        private TreeNode GetTreeNode(string name, string descr, string imagekey)
        {
            TreeNode node = new TreeNode(descr);
            node.Name = name;
            node.ImageKey = imagekey;
            node.SelectedImageKey = imagekey;
            //node.
            return node;
        }

        //private TreeNode GetTreeNode(Field f)
        //{
        //    TreeNode node = new TreeNode(f.FieldID.ToString() + " " + f.FieldName);
        //    node.Name = "field" + f.StartingDefLine.ToString();
        //    node.ImageKey = "field";
        //    node.SelectedImageKey = "field";
        //    node.ToolTipText = f.FieldType;
        //    return node;
        //}

        private void tvObjectStructure_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //e.Node.Toggle();
        }

        private void OpenReferenceSource(TreeNode node, bool quickview)
        {
            var tab = node.Name.Split('|');
            if (tab.Count() > 1)
            {
                int idLine = 0;
                int.TryParse(tab[0], out idLine);
                var refbyObj = navProject.GetRunObject(tab[1]);
                if (refbyObj != null)
                {
                    mainForm.ShowObjectSource(refbyObj,quickview, idLine);
                }
            }
        }

       

        private string RemoveIDPart(string text)
        {
            int ind = text.IndexOf(" ");
            if (ind >= 0)
            {
                return text.Substring(ind).Trim();
            }
            return text;
        }

        private void tvObjectRefs_Click(object sender, EventArgs e)
        {
            //TreeNode node = tvObjectRefs.SelectedNode;
            //if (node == SelectedNod)
            //{
            //    tvObjectRefs.SelectedNode = null;
            //}

            //TreeNode node = tvObjectRefs.SelectedNode;
            //if (node != null)
            //{
            //    OpenReferenceSource(node, true);
            //}
            //SelectedNod = node;
        }

        private void tvObjectRefs_DoubleClick(object sender, EventArgs e)
        {
        }

        private void tvObjectRefs_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //TreeNode node = tvObjectRefs.SelectedNode;
            //if (node != null)
            //{
            //    OpenReferenceSource(node, false);
            //}
            //SelectedNod = node;
        }

        private void tvObjectRefs_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //TreeNode node = tvObjectRefs.SelectedNode;
            //if (node != null)
            //{
            //    OpenReferenceSource(node, true);
            //}
            //SelectedNod = node;
        }

        private void tvObjectRefs_MouseDown(object sender, MouseEventArgs e)
        {
            mClick++;
            NodeDoubleClicTimer.Enabled = true;
        }

        private void NodeDoubleClicTimer_Tick(object sender, EventArgs e)
        {
            NodeDoubleClicTimer.Enabled = false;

            if (mClick == 1)
            {
                TreeNode node = tvObjectRefs.SelectedNode;
                if (node != null)
                {
                    OpenReferenceSource(node, true);
                }
                SelectedNod = node;
            }
            if (mClick == 2)
            {
                TreeNode node = tvObjectRefs.SelectedNode;
                if (node != null)
                {
                    OpenReferenceSource(node, false);
                }
                SelectedNod = node;
            }
            mClick = 0;
        }
    }
}