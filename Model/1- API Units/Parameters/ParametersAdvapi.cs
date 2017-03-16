using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class CreateServiceParameter : ParameterBase
    {
        public string Name { set; get; }
        public string Path { set; get; }
        public uint Mode { set; get; }
        
    }
    [Serializable]
    public class RegCreateKeyParameter:ParameterBase
    {
        public IntPtr HKey { set; get; }
        public string SubKey { set; get; }
        public IntPtr Handle { set; get; }

        
    }
    [Serializable]
    public class RegOpenKeyParameter : ParameterBase
    {
        public IntPtr HKey { set; get; }
        public string SubKey { set; get; }
        public IntPtr Handle { set; get; }

       
    }
    [Serializable]
    public class RegSetValueParameter:ParameterBase
    {
        public IntPtr Handle { get; set; }
        public uint Type { set; get; }
        public string Value { set; get; }
        public string Data { set; get; }

       
    }
}
