using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Model
{
    [Serializable]
    public class Sample
    {
        public string FilePath { set; get;}
        public string FileName { set; get; }
        public string MD5 { set; get; }
        public bool IsMalicious { set; get; }
        public bool IsAnalyzed { get { return APIs.Count > 0; } }
        public byte[] ExecutableData { set; get; }
        public Dictionary<string, byte[]> AccessedFiles;
        public List<APIUnit> APIs { set; get; }
        public List<SequenceUnit> SequenceUnits { set; get; }
        public List<SemanticUnit> SemanticUnits { set; get; }

        private Sample()
        {
            AccessedFiles = new Dictionary<string, byte[]>();
            this.APIs = new List<APIUnit>();
            this.SequenceUnits = new List<SequenceUnit>();
            this.SemanticUnits = new List<SemanticUnit>();
        }
        public Sample(string fileName)
            : this()
        {
            FilePath = fileName;
            FileName = Path.GetFileName(fileName);
            MD5 = fileName.MD5();
            ExecutableData = File.ReadAllBytes(fileName);
        }
        public byte[] ToByteArray()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            return ms.ToArray();
        }
        public static Sample FromByteArray(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryFormatter bf = new BinaryFormatter();
            return (Sample)bf.Deserialize(ms);
        }
    }
}
