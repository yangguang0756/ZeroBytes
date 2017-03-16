using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Model;
using Analyzer;
using System.Runtime.Serialization.Formatters.Binary;
namespace Agent
{
    public partial class MainAgent : Form
    {
        TcpListener listener;
        public MainAgent()
        {
            InitializeComponent();
            listener = new TcpListener(IPAddress.Any, 5056);
            listener.Start();
            ThreadPool.QueueUserWorkItem(o => HandleConnection());
        }

        private void HandleConnection()
        {
            try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Report("Connected to " + client.Client.RemoteEndPoint.ToString());
                    NetworkStream ns = client.GetStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    Sample s = (Sample)formatter.Deserialize(ns);
                    s.FilePath = "c:\\test\\test.exe";
                    File.WriteAllBytes("c:\\test\\test.exe",s.ExecutableData);
                    DynamicAnalyzer.Analyze(ref s);
                    formatter.Serialize(ns,s);
                    ns.Close();
                    Report("Analysis Done");                    
                    client.Close();
                }
            catch(Exception ex)
            {
                Report(ex.Message);
                Report(ex.StackTrace);
            }
        }
        void Report(string rep)
        {
            this.Invoke(new Action( () => txtReport.Text += rep + "\r\n"));
        }
        private void localAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() != DialogResult.OK)
                return;
            ThreadPool.QueueUserWorkItem(o =>
            {
                Sample s = new Sample(of.FileName);
                DynamicAnalyzer.Analyze(ref s);
                var a1=s.APIs.Select(x=>x.ToString()).ToList();
                var a2 = s.SequenceUnits.Select(x => x.ToString()).ToList();
                var a3=s.SemanticUnits.Select(x=>x.ToString()).ToList();
                File.WriteAllLines("api.txt", a1);
                File.WriteAllLines("seq.txt", a2);
                File.WriteAllLines("sem.txt", a3);
            });

        }

    
    }
}
