using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class Process32FirstSemanticUnit : SemanticUnit
    {
        public bool enumProcesses { set; get; }
        public Process32FirstSemanticUnit() { Name = "Enumerating Processes"; enumProcesses = true; }
    }
}
