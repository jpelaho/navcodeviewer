using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NavCodeViewer.Forms.Controls
{
    public partial class ReferenceHint : UserControl
    {
        SourceViewer view;
        string functionName;
        public ReferenceHint(SourceViewer viewer,string funcName1)
        {
            InitializeComponent();
            view = viewer;
            functionName = funcName1;
        }
        public void SetLinkText(string txt)
        {
            linkLabel1.Text = txt;
        }
        public void SetFont(Font f)
        {
            linkLabel1.Font = f;
        }

        private void ReferenceHint_Click(object sender, EventArgs e)
        {
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            view.ShowFunctionReferences(functionName);
        }
    }
}
