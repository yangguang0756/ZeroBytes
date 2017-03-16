using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class SocketConnectSemanticUnit:SemanticUnit
    {
        public int Port { set; get; }
        public string IP { set; get; }
        public SocketConnectSemanticUnit() { Name = "Connecting Socket"; }
    }
}
