using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
namespace HyperVisor
{
    static class RemoteAgent
    {
        public static Sample Analyze(string path)
        {
            Sample sample1= new Sample(path);
            TcpClient client = new TcpClient("vm", 5056);
            var stream = client.GetStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, sample1);
            stream.Flush();
            var sample2 = (Sample)bf.Deserialize(stream);
            sample2.FilePath = sample1.FilePath;
           
            client.Close();
            return sample2;
        }
    }
}
