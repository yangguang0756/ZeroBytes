using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class CreateToolHelpSemanticUnit:SemanticUnit
    {
        public uint Flags { set; get; }
        public CreateToolHelpSemanticUnit() { Name = "Create Toolhelp"; }
    }
}
