using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class FindFirstFileSemanticUnit : SemanticUnit
    {
        public string Pattern { set; get; }

        public FindFirstFileSemanticUnit() { Name = "Searching for Files"; }
    }
}