using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class MoveFileSemanticUnit : SemanticUnit
    {
        public string To { set; get; }
        public string Extension { set; get; }
        public MoveFileSemanticUnit() { Name = "Moving File"; }
    }
}
