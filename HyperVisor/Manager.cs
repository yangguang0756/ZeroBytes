using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HyperVisor
{
    class Manager
    {
        public string VMPath
        {
            set
            {
                Properties.Settings.Default.VMPath = value;
                Properties.Settings.Default.Save();
            }
            get
            {
                return Properties.Settings.Default.VMPath;
            }
        }
        public List<Sample> Dataset { set; get; }
        public Manager()
        {
            Dataset = new List<Sample>();
            VMControl.VMPath = VMPath;
        }
        public  void Analyze(string path, bool isMalicious)
        {
            //VMControl.PowerOnWithReset();
            Sample s = RemoteAgent.Analyze(path);
            s.IsMalicious = isMalicious;
            Dataset.Add(s);
        }
        public void SaveDataset(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();
            var s = File.Create(path);
            bf.Serialize(s, Dataset);
            s.Close();
        }
        public void LoadDataset(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();
            var s = File.OpenRead(path);
            var ls = (List<Sample>)bf.Deserialize(s);
            Dataset.AddRange(ls.ToArray());
            s.Close();
        }
    }
}
