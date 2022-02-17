using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Domain
{
    public class ObjetReference
    {
        public ObjectType RefByObjetType { get; set; }
        public int RefByObjetID { get; set; }
        public ObjectType ObjectType { get; set; }
        public int ObjectID { get; set; }
        public int ReferenceLine { get; set; }
    }
}
