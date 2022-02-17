using NavCodeViewer.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NavCodeViewer
{
    public class GlobalUI
    {
        public static bool Confirm(string msg)
        {
            return MessageBox.Show(msg, GlobalApp.AppName, 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
        public static void Info(string msg)
        {
            MessageBox.Show(msg, GlobalApp.AppName,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void ErrorMsg(string msg)
        {
            MessageBox.Show(msg, GlobalApp.AppName,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void Error(string errCode,string msg)
        {
            throw new Exception(string.Format("{0} : {1}", errCode, msg));
        }
    }
}
