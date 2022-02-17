using NavCodeViewer.Business;
using NavCodeViewer.Domain;
using NavCodeViewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NavCodeViewer.Forms
{
    public partial class ObjectStructure :  DockContent
    {
        private const string WhereUsedObjKey = "0001";
        private const string WhereInsertedObjKey = "0002";
        private const string WhereDeletedObjKey = "0003";
        private const string WhereUpdateObjKey = "0004";
        private const string WhereFilterObjKey = "0005";
        private const string OptionMarker = "OptionStringDescr999";
        private const string FunctionMarker = "function ";
        private const string FieldMarker = "field ";
        private int mClick = 0;

        NavObject navObject;
        private TreeNode SelectedNod = null;
        ObjectReferences mainObjRefsDockPanel;
        public MainForm Mainform
        {
            get; set;
        }
        public ObjectStructure()
        {
            InitializeComponent();
            Text = Resources.String7;
            ImportImages();
        }
        public void SetNavObject(NavObject obj, ObjectReferences refwindows)
        {
            if (navObject != null)
            {
                if (obj.ID == navObject.ID && obj.Type == navObject.Type)
                {
                    return;
                }
            }

            navObject = obj;
            mainObjRefsDockPanel = refwindows;
            //mainObjRefsDockPanel.mainForm = Mainform;
            LoadMenu();
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

            TVimageList.Images.Add("objectDeleted", Resources.objectDeleted);
            TVimageList.Images.Add("objectFiltered", Resources.objectFiltered);
            TVimageList.Images.Add("objectInserted", Resources.objectInserted);
            TVimageList.Images.Add("objectUpdated", Resources.objectUpdated);
            TVimageList.Images.Add("objectUsed", Resources.objectUsed);
        }
        public void LoadMenu()
        {
            var tab = new List<TreeNode>();

            this.Text = string.Format(Resources.String4, navObject.LongName);

            mainObjRefsDockPanel.Reset();

            TreeNode node1;

            TreeNode obj1 = GetTreeNode(WhereUsedObjKey, Resources.String10, "objectUsed");
            tab.Add(obj1);

            if (navObject.Type == ObjectType.Table)
            {
                TreeNode objInsert = GetTreeNode(WhereInsertedObjKey, Resources.String11, "objectInserted");
                TreeNode objDeleted = GetTreeNode(WhereDeletedObjKey, Resources.String12, "objectDeleted");
                TreeNode objUpdated = GetTreeNode(WhereUpdateObjKey, Resources.String13, "objectUpdated");
                //TreeNode objFiltered = GetTreeNode(WhereFilterObjKey, Resources.String16, "objectFiltered");

                tab.Add(objInsert); 
                tab.Add(objUpdated);
                tab.Add(objDeleted);
                //tab.Add(objFiltered);
            }

            node1 = BuildObjectFieldsNodes();
            if (node1 != null) tab.Add(node1);

            node1 = BuildObjectFunctionNodes();
            if (node1 != null) tab.Add(node1);



            //node1 = BuildObjectNodes();
            //if (node1 != null) tab.Add(node1);

            //this.BeginInvoke(new InvokeDelegate(InvokeMethod(tab)));
            this.Invoke((Action)(() => InvokeMethod(tab)));
        }
        TreeNode BuildObjectFieldsNodes()
        {
            var list = new List<TreeNode>();
            foreach (var f in navObject.FieldList)
            {
                TreeNode item = GetTreeNode(f);
                list.Add(item);
            }
            if (list.Count > 0)
            {
                return GetTreeNode("00076", Resources.String14, "group", list.ToArray());
            }
            return null;
        }
        TreeNode BuildObjectFunctionNodes()
        {
            var list = new List<TreeNode>();
            foreach (var f in navObject.FunctionList)
            {
                string itemName = f.FunctionName;
                TreeNode item = GetTreeNode(FunctionMarker + f.StartingDefLine.ToString(), itemName, "function");
                list.Add(item);
            }
            if (list.Count > 0)
            {
                return GetTreeNode("00077", Resources.String15, "group", list.ToArray());
            }
            return null;
        }
        //TreeNode BuildObjectFunctionNodes()
        //{
        //    var list = new List<TreeNode>();
        //    TreeNode obj = GetTreeNode(WhereUsedObjKey, Resources.String10, "group");
        //    list.Add(obj);

        //    if (navObject.Type == ObjectType.Table)
        //    {
        //        TreeNode obj2 = GetTreeNode(WhereInsertedObjKey, Resources.String11, "group");
        //        TreeNode obj3 = GetTreeNode(WhereDeletedObjKey, Resources.String12, "group");
        //        TreeNode obj4 = GetTreeNode(WhereUpdateObjKey, Resources.String13, "group");
        //        list.Add(obj2); list.Add(obj3); list.Add(obj4);
        //    }
        //    return GetTreeNode("0005", navObject.LongName, "group", list.ToArray());
        //}
        public void InvokeMethod(List<TreeNode> tab)
        {
            this.tvObjectStructure.Nodes.Clear();
            this.tvObjectStructure.Nodes.AddRange(tab.ToArray());
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

        private TreeNode GetTreeNode(Field f)
        {

            List<TreeNode> Options = new List<TreeNode>();
            if (f.OptionString.IsNotNullOrEmpty())
            {
                var tab = f.OptionString.Split(',');
                for(int i = 0; i < tab.Length; i++)
                {
                    string optionName = tab[i].IsNullOrEmpty() ? i.ToString() : tab[i];
                    string strName = OptionMarker + "," + f.FieldName + "," + optionName;
                    Options.Add(GetTreeNode(strName, optionName, "function"));
                }
            }


            TreeNode node = new TreeNode(f.FieldID.ToString() + " " + f.FieldName, Options.ToArray());
            node.Name = FieldMarker + f.StartingDefLine.ToString();
            node.ImageKey = "field";
            node.SelectedImageKey = "field";
            node.ToolTipText = f.FieldType;
            return node;
        }

        private void tvObjectStructure_AfterSelect(object sender, TreeViewEventArgs e)
        {
            e.Node.Toggle();


            //TreeNode node = tvObjectStructure.SelectedNode;
            //SelectNode(node);
            //SelectedNod = node;
        }

        private void SelectNode(TreeNode node,bool quickView)
        {
            if (node != null)
            {
                if (node.Name.StartsWith(FunctionMarker) || node.Name.StartsWith(FieldMarker))
                {
                    var idStr = node.Name.Replace(FieldMarker, "").Replace(FunctionMarker, "");
                    OpenSourceObj(quickView, idStr);
                }
                if (node.Name.StartsWith(OptionMarker))
                {
                    var tab = node.Name.Split(',');
                    if (tab.Length < 3) return;
                    var fieldName = tab[1];
                    var f = navObject.FieldList.Where(c => c.FieldName == fieldName).FirstOrDefault();
                    if (f != null)
                    {
                        OpenSourceObj(quickView, f.StartingDefLine.ToString());
                    }
                }

                mainObjRefsDockPanel.Reset();

                LoadMenuOptionObjectRefs(node);

                if (node.Name.StartsWith(FieldMarker))
                {
                    LoadFieldsRefs(node);
                }
                if (node.Name.StartsWith(FunctionMarker))
                {
                    LoadFunctionsRefs(node);
                }
            }
        }

        private void OpenSourceObj(bool quickView, string iLine)
        {
            var v = Mainform.GetSourceViewer(navObject.ObjectName);
            if (v != null)
            {
                v.GoToLine(Convert.ToInt32(iLine));
            }
            else
            {
                Mainform.ShowObjectSource(navObject, quickView, Convert.ToInt32(iLine));
            }
        }

        private void LoadFunctionsRefs(TreeNode node)
        {
            string functionName = node.Text;
            Mainform.ShowFunctionReferences(navObject, functionName);
        }

        private void LoadFieldsRefs(TreeNode node)
        {
            string fieldNameMod = RemoveIDPart(node.Text);
            Mainform.ShowFieldReferences(navObject, fieldNameMod);
        }



        private void LoadMenuOptionObjectRefs(TreeNode node)
        {
            if (node.Name.StartsWith(OptionMarker))
            {
                var tab = node.Name.Split(',');
                if (tab.Length < 3) return;
                var refs = navObject.GetOptionReferences(tab[1],tab[2]);
                mainObjRefsDockPanel.SetNavObject(navObject.NavProject, refs, navObject.ObjectName,false);
            }
            if (node.Name.Equals(WhereUsedObjKey))
            {
                var refs = navObject.GetWhereObjectIsReferenced();
                mainObjRefsDockPanel.SetNavObject(navObject.NavProject, refs, navObject.ObjectName,false);
            }
            if (node.Name.Equals(WhereDeletedObjKey))
            {
                var refs = navObject.GetObjectFuncRecordedReferences(Reference.strDeleteRef);
                mainObjRefsDockPanel.SetNavObject(navObject.NavProject, refs, navObject.ObjectName, false);
            }
            //if (node.Name.Equals(WhereFilterObjKey))
            //{
            //    var refs = navObject.GetObjectFuncRecordedReferences(Reference.strFilterRef);
            //    mainObjRefsDockPanel.SetNavObject(navObject, refs, navObject.ObjectName);
            //}
            if (node.Name.Equals(WhereInsertedObjKey))
            {
                var refs = navObject.GetObjectFuncRecordedReferences(Reference.strInsertRef);
                mainObjRefsDockPanel.SetNavObject(navObject.NavProject, refs, navObject.ObjectName, false);
            }
            if (node.Name.Equals(WhereUpdateObjKey))
            {
                var refs = navObject.GetObjectFuncRecordedReferences(Reference.strUpdateRef);
                mainObjRefsDockPanel.SetNavObject(navObject.NavProject, refs, navObject.ObjectName, false);
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
        private void tvObjectStructure_Click(object sender, EventArgs e)
        {
            //TreeNode node = tvObjectStructure.SelectedNode;
            //if (node == SelectedNod)
            //{
            //    tvObjectStructure.SelectedNode = null;
            //}
        }
        private TreeNode GetTreeNode(string name, string descr, string imagekey, TreeNode[] children)
        {
            TreeNode node = new TreeNode(descr, children);
            node.Name = name;
            node.ImageKey = imagekey;
            node.SelectedImageKey = imagekey;
            return node;
        }

        private void tvObjectStructure_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show("popo");
        }

        private void NodeDoubleClicTimer_Tick(object sender, EventArgs e)
        {
            NodeDoubleClicTimer.Enabled = false;

            if (mClick == 1)
            {
                TreeNode node = tvObjectStructure.SelectedNode;
                SelectNode(node,true);
                SelectedNod = node;
            }
            if (mClick == 2)
            {
                TreeNode node = tvObjectStructure.SelectedNode;
                SelectNode(node, false) ;
                SelectedNod = node;
            }
            mClick = 0;
        }

        private void tvObjectStructure_MouseDown(object sender, MouseEventArgs e)
        {
            mClick++;
            NodeDoubleClicTimer.Enabled = true;
        }
    }
}
