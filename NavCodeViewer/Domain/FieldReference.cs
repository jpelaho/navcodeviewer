using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Domain
{
    public class FieldReference
    {
        public ObjectType RefByObjetType { get; set; }
        public int RefByObjetID { get; set; }
        public ObjectType FieldObjectType { get; set; }
        public int FieldObjectID { get; set; }
        public string FieldName { get; set; }
        public int ReferenceLine { get; set; }
    }
}
