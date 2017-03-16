using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class SetWindowsHookParameter : ParameterBase 
    {
        public int HookType { set; get; }

       
    }
    [Serializable]
    public class GetAsyncKeyStateParameter : ParameterBase
    {
        
    }
    [Serializable]
    public class GetMessageParameter : ParameterBase
    {
       
    }
}
