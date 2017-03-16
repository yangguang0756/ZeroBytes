using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class InternetOperationSemanticUnit:SemanticUnit
    {
        public string URL { set; get; }
        public string Operation { set; get; }
        public string FilePath { set; get; }
        public string FileDirectory { set; get; }
        public InternetOperationSemanticUnit()
        {
            Name = "Internet Operation";
        }
    }
}
