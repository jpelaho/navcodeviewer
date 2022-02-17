using NavCodeViewer.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Domain
{
    public class Function
    {
        public ObjectType ObjectType { get; set; }
        public int ObjectID { get; set; }
        public string FunctionName { get; set; }
        public int StartingDefLine { get; set; }
        public int EndingDefLine { get; set; }
        public bool Private { get; set; }
        public string LongName
        {
            get
            {
                return string.Format("{0} {1}", Resources.String6, FunctionName);
            }
        }
    }
}
