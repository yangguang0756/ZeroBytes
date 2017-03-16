using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
   public class ConnectionParameter:ParameterBase
   {
       public ushort Port { set; get; }
       public string IP { set; get; }
       public bool Server { set; get; }

      
   }
}
