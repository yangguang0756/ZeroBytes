using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class ShellExecuteSemanticUnit:SemanticUnit
    {
        public string ProcessName { set; get; }
        public string Parameters { set; get; }
        public ShellExecuteSemanticUnit() { Name = "Shell Executing"; }
    }
}
