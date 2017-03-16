using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class CopyFileSemanticUnit:SemanticUnit
    {
        public string To { set; get;}
        public string Extension { set; get; }
        public CopyFileSemanticUnit() { Name = "Copying File"; }
    }
}
