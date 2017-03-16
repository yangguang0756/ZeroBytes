using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class ShellExecuteParameter : ParameterBase
    {
        public string Name { set; get; }
        public string Directory { set; get; }
        public string Parameters { set; get; }

        
    }

}
