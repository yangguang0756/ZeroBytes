using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class CreateProcessSemanticUnit:SemanticUnit
    {
        public string ProcessName { set; get; }
        public bool Dropped { set; get; }
        public bool Injected { set; get; }
        public CreateProcessSemanticUnit() { Name = "Create Process"; }
    }
}
