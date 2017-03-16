using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class CreateFileSemanticUnit:SemanticUnit
    {
        public string Directory {  set; get; }
        public string Extension {  set; get; }
        public uint Access {  set; get; }
        public uint Mode {  set; get; }
        public CreateFileSemanticUnit() { Name = "Create File "; }
    }
}
