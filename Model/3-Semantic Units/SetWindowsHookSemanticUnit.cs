using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class SetWindowsHookSemanticUnit:SemanticUnit
    {
        public int HookMode { set; get; }
        public SetWindowsHookSemanticUnit() { Name = "SetWindowsHook"; }
    }
}
