using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class CreateServiceSemanticUnit:SemanticUnit
    {
        public string Service { set; get; }
        public uint Mode { set; get; }
        public CreateServiceSemanticUnit() { Name = "Creating Service"; }
    }
}
