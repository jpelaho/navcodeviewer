using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavCodeViewer.Domain
{
    public class ErrorMessageRef
    {
        public ObjectType RefByObjetType { get; set; }
        public int RefByObjetID { get; set; }
        public string ErrorMessage { get; set; }
        public int ReferenceLine { get; set; }
    }
}
