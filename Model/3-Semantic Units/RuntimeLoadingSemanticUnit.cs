using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class RuntimeLoadingSemanticUnit:SemanticUnit
    {
        public string Module {  set; get; }
        public string Function {  set; get; }
        public RuntimeLoadingSemanticUnit() { Name = "Runtime API Loading "; }
    }
}
