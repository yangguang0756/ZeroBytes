using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
namespace Model
{
    static class Extension
    {
        public static string MD5(this string fileName)
        {
            Stream s = File.OpenRead(fileName);
            byte[] hv = HashAlgorithm.Create("MD5").ComputeHash(s);
            s.Close();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hv)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}
