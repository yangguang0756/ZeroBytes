using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
namespace Model
{
    [Serializable]
    public abstract class ParameterBase
    {
        public  string MD5()
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            bf.Serialize(ms, this); 
            ms.Position = 0;
            byte[] hv = System.Security.Cryptography.HashAlgorithm.Create("MD5").ComputeHash(ms);
            return BitConverter.ToString(hv);
        }
        
        public string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                Type t = this.GetType();
                PropertyInfo[] pInfo = t.GetProperties();
                for (int i = 0; i < pInfo.Length - 1; i++)
                    if(pInfo[i].PropertyType==typeof(IntPtr))
                        sb.AppendLine(String.Format("{0}\t{1}", pInfo[i].Name,((IntPtr)pInfo[i].GetValue(this, null)).ToString("X2")));
                    else
                        sb.AppendLine(String.Format("{0}\t{1}", pInfo[i].Name, pInfo[i].GetValue(this, null)));
                sb.AppendLine();
                return sb.ToString();                
            }
        }
    }
   
}
