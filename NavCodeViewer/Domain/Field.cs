using NavCodeViewer.Properties;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Domain
{
    public class Field
    {
        public ObjectType ObjectType { get; set; }
        public int ObjectID { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string OptionString { get; set; }
        public int FieldID { get; set; }
        public int StartingDefLine { get; set; }
        public string LongName { 
            get
            {
                return string.Format("{0} {1}", Resources.String5, FieldName);
            }
        }
        public override string ToString()
        {
            return string.Concat("ObjectType:", this.ObjectType, "\t",
                "ObjectID:", this.ObjectID, "\t",
                "FieldID:", this.FieldID, "\t",
                "FieldName:", this.FieldName, "\t",
                "StartingDefLine:", this.StartingDefLine, "\t",
                "FieldType:", this.FieldType);
        }
    }
}
