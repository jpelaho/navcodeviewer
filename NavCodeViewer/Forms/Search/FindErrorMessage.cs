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

namespace NavCodeViewer.Forms
{
    public partial class FindErrorMessage : Form
    {
        MainForm mainForm;
        public FindErrorMessage(MainForm main)
        {
            mainForm = main;
            InitializeComponent();
        }

        private void FindErrorMessage_Load(object sender, EventArgs e)
        {
            cmdFindAll.Text = Resources.String23;
            LblExample.Text = Resources.String39;
            Text = Resources.String38;
        }

        private async void cmdFindAll_Click(object sender, EventArgs e)
        {
            try
            {
                var refs = new List<Reference>();

                var pattern = txtSearch.Text.Replace(" ", @"\s");
                pattern=Regex.Replace(pattern, "\\%\\d+", ".*");

                RegexOptions opt = RegexOptions.IgnoreCase;
                //pattern = Regex.Escape(pattern);

                if (pattern.IsNullOrEmpty())
                {
                    GlobalUI.Error("S002", Resources.String35);
                }

                await SearchAllAsync(refs, pattern, opt);

                mainForm.ClearProgressInfos();
            }
            catch (Exception ex)
            {
                GlobalUI.ErrorMsg(ex.Message);
            }
        }
        private async Task SearchAllAsync(List<Reference> refs, string pattern, RegexOptions opt)
        {
            var listeObjs = mainForm.ActiveProject.AllObjects;
            await mainForm.SearchErrorMessagesInObjectsAsync(txtSearch.Text, pattern, listeObjs, opt);
        }

    }
}
