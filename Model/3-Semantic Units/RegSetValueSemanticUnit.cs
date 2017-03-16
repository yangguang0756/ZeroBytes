using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class RegSetValueSemanticUnit:SemanticUnit
    {
        public string Value{set;get;}
        public string Data{set;get;}
        public RegSetValueSemanticUnit() { Name = "Setting Registry Value"; }
    }
}
