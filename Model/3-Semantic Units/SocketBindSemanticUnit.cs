using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class SocketBindSemanticUnit:SemanticUnit
    {
         public int Port { set; get; }
         public SocketBindSemanticUnit() { Name = "Creating Server Socket"; }
    }
}
