using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class DeleteFileSemanticUnit : SemanticUnit
    {
        public string File { set; get; }
        public string Extension { set; get; }
        public DeleteFileSemanticUnit() { Name = "Deleting File"; }
    }
}
