using FastColoredTextBoxNS;
using NavCodeViewer.Business;
using NavCodeViewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace NavCodeViewer.Forms
{
    public partial class ObjectViewer : DockContent
    {
        public delegate void InvokeDelegate();
        //MainForm mainForm;
        Project navproject;
        WeifenLuo.WinFormsUI.Docking.DockPanel mainSourceDockPanel;
        ObjectStructure mainObjStructureDockPanel;
        ObjectReferences mainObjRefsDockPanel;
        private int mClick = 0;

        private TreeNode SelectedNod = null;

        public MainForm Mainform
        {
            get; set;
        }
        public ObjectViewer()
        {
            InitializeComponent();
            Text = Resources.String9;
            ImportImages();
        }
        public void SetProject(Project pro)
        {
            navproject = pro;
        }
        public void SetSourceDockPanel(WeifenLuo.WinFormsUI.Docking.DockPanel objectList,
            ObjectStructure structure, ObjectReferences refs)
        {
            mainSourceDockPanel = objectList;
            mainObjStructureDockPanel = structure;
            mainObjRefsDockPanel = refs;
        }
        private void ObjectViewer_Load(object sender, EventArgs e)
        {
        }
        public void LoadMenu()
        {
            var tab = new List<TreeNode>();
            if (navproject.Tables.Count > 0)
            {
                TreeNode tnTables = BuildMenuType(navproject.Tables.Cast<NavObject>().ToList(), "Table",
                    "table", "Table", "table", "table");
                tab.Add(tnTables);
            }
            if (navproject.Pages.Count > 0)
            {
                TreeNode tnPages = BuildMenuType(navproject.Pages.Cast<NavObject>().ToList(), "Page",
                    "page", "Page", "page", "page");
                tab.Add(tnPages);
            }
            if (navproject.CodeUnits.Count > 0)
            {
                TreeNode tnCU = BuildMenuType(navproject.CodeUnits.Cast<NavObject>().ToList(), "Codeunit",
                    "codeunit", "Codeunit", "codeunit", "codeunit");
                tab.Add(tnCU);
            }
            if (navproject.XMLPorts.Count > 0)
            {
                TreeNode tnXMLPorts = BuildMenuType(navproject.XMLPorts.Cast<NavObject>().ToList(), "XMLport",
                    "xmlport", "XmlPort", "xmlport", "xmlport");
                tab.Add(tnXMLPorts);
            }
            if (navproject.Reports.Count > 0)
            {
                TreeNode tnReports = BuildMenuType(navproject.Reports.Cast<NavObject>().ToList(), "Report",
                    "report", "Report", "report", "report");
                tab.Add(tnReports);
            }
            if (navproject.Querys.Count > 0)
            {
                TreeNode tnQuerys = BuildMenuType(navproject.Querys.Cast<NavObject>().ToList(), "Query",
                    "query", "Query", "query", "query");
                tab.Add(tnQuerys);
            }
            if (navproject.Menusuites.Count > 0)
            {
                TreeNode tnMenus = BuildMenuType(navproject.Menusuites.Cast<NavObject>().ToList(), "MenuSuite",
                "menusuite", "MenuSuite", "menusuite", "menusuite");
                tab.Add(tnMenus);
            }

            //this.BeginInvoke(new InvokeDelegate(InvokeMethod(tab)));
            this.Invoke((Action)(() => InvokeMethod(tab)));
        }
        public void InvokeMethod(List<TreeNode> tab)
        {
            this.TViewObjViewer.Nodes.AddRange(tab.ToArray());
        }
        string plageName(int deb,int fin)
        {
            return deb + " - " + fin;
        }
        private TreeNode BuildMenuType(List<NavObject> objs, string objType, string groupName, string groupCaption, string mainImage, string itemImage)
        {
            var tab = new List<TreeNode>();
            int deb = 0, fin = deb + 999; int i = 1;
            while (fin < 9999)
            {
                AddPlageObj(objs, objType, itemImage, tab, deb, fin, i);
                deb = fin + 1;
                fin = deb + 999;
                i++;
            }

            AddPlageObj(objs, objType, itemImage, tab, 10000, 49999, i++);
            AddPlageObj(objs, objType, itemImage, tab, 50000, 99999, i++);
            AddPlageObj(objs, objType, itemImage, tab, 100000, 999999999, i++);

            TreeNode groupMenu = GetTreeNode(groupName, groupCaption, mainImage, tab.ToArray());
            return groupMenu;
        }
        private void AddPlageObj(List<NavObject> objs, string objType, string itemImage, List<TreeNode> tab, int deb, int fin, int i)
        {
            var plage = objs.Where(c => c.ID >= deb && c.ID < fin).ToList();
            if (plage.Count > 0)
            {
                TreeNode item = BuildDetailGroup(i.ToString(), objType, plageName(deb, fin), plage, itemImage);
                tab.Add(item);
            }
        }
        private TreeNode BuildDetailGroup(string groupName,string objType,string groupCaption,List<NavObject> objs,string image)
        {
            var tab = new List<TreeNode>();
            foreach (var obj in objs)
            {
                string itemName = obj.ID.ToString() + " " + obj.Name;
                string itemCode = objType + obj.ID.ToString();
                TreeNode item = GetTreeNode(itemCode, itemName, image);
                tab.Add(item);
            }
            TreeNode detailGroup = GetTreeNode(groupName, groupCaption, "group", tab.ToArray());
            return detailGroup;
        }
        private TreeNode GetTreeNode(string name, string descr, string imagekey)
        {
            TreeNode node = new TreeNode(descr);
            node.Name = name;
            node.ImageKey = imagekey;
            node.SelectedImageKey = imagekey;
            return node;
        }
        private TreeNode GetTreeNode(string name, string descr, string imagekey, TreeNode[] children)
        {
            TreeNode node = new TreeNode(descr, children);
            node.Name = name;
            node.ImageKey = imagekey;
            node.SelectedImageKey = imagekey;
            return node;
        }
        private TreeNode GetTreeNode(string name, string descr)
        {
            TreeNode node = new TreeNode(descr);
            node.Name = name;
            return node;
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
        }
        private void TViewObjViewer_Click(object sender, EventArgs e)
        {
            //TreeNode node = TViewObjViewer.SelectedNode;
            //if (node == SelectedNod)
            //{
            //    TViewObjViewer.SelectedNode = null;
            //}
        }
        private void TViewObjViewer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            e.Node.Toggle();



        }
        
        

        

        private void NodeDoubleClicTimer_Tick(object sender, EventArgs e)
        {
            NodeDoubleClicTimer.Enabled = false;

            if (mClick == 1)
            {
                TreeNode node = TViewObjViewer.SelectedNode;
                if (node != null)
                {
                    var idStr = node.Name;
                    var obj = navproject.GetRunObject(idStr);
                    if (obj != null)
                    {
                        //NavObject rec = (NavObject)obj;
                        Mainform.ShowObjectSource(obj, true);
                        //mainObjStructureDockPanel.SetNavObject(rec, mainObjRefsDockPanel);
                        //OpenObjectSourceQuickView(rec);
                    }
                }
                SelectedNod = node;
            }
            if (mClick == 2)
            {
                TreeNode node = TViewObjViewer.SelectedNode;
                if (node != null)
                {
                    var idStr = node.Name;
                    var obj = navproject.GetRunObject(idStr);
                    if (obj != null)
                    {
                        //NavObject rec = (NavObject)obj;
                        //mainObjStructureDockPanel.SetNavObject(rec, mainObjRefsDockPanel);
                        //OpenObjectSource(rec);
                        Mainform.ShowObjectSource(obj, false);
                    }
                }
                SelectedNod = node;
            }
            mClick = 0;
        }

        private void TViewObjViewer_MouseDown(object sender, MouseEventArgs e)
        {
            mClick++;
            NodeDoubleClicTimer.Enabled = true;
        }
    }
}
