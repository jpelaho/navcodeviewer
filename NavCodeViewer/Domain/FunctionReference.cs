using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Domain
{
    public class FunctionReference
    {
        public ObjectType RefByObjetType { get; set; }
        public int RefByObjetID { get; set; }
        public ObjectType FunctionObjectType { get; set; }
        public int FunctionObjectID { get; set; }
        public string FunctionName { get; set; }
        public int ReferenceLine { get; set; }
    }
}
