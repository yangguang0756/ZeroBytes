using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Model
{
    [Serializable]
    public class SemanticUnit
    {
        public string Name { set; get; }
        public string MD5
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Position = 0;
                byte[] hv = HashAlgorithm.Create("MD5").ComputeHash(ms);
                return BitConverter.ToString(hv);
            }
        }
        public byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            return ms.ToArray();
        }
        public static SemanticUnit FromByteArray(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            ms.Position = 0;
            BinaryFormatter bf = new BinaryFormatter();
            return (SemanticUnit)bf.Deserialize(ms);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Type t = this.GetType();
            PropertyInfo[] pInfo = t.GetProperties();
            for (int i = 0; i < pInfo.Length - 1; i++)
                sb.AppendLine(String.Format("{0}\t{1}", pInfo[i].Name, pInfo[i].GetValue(this, null)));
            return sb.ToString();
        }
    }
}
