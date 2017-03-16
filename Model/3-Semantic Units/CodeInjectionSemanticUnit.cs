using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class CodeInjectionSemanticUnit:SemanticUnit
    {
        public bool Injecting { set; get; }
        public CodeInjectionSemanticUnit() { Name = "Injecting Code"; Injecting = true; }
    }
}
