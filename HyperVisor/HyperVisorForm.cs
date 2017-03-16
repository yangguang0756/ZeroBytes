using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HyperVisor
{
    public partial class HyperVisorForm : Form
    {
        Manager manager;
        public HyperVisorForm()
        {
            InitializeComponent();
            manager = new Manager();
            bndSamples.CurrentChanged += bndSamples_CurrentChanged;
            bndSamples.DataSource = manager.Dataset;
            
        }

        void bndSamples_CurrentChanged(object sender, EventArgs e)
        {
            if(bndSamples.Current==null)
            {
                rdAPI.Enabled = false;
                rdSemantic.Enabled = false;
            }
            else
            {
                rdAPI.Enabled = true;
                rdSemantic.Enabled = true;
                Sample s = bndSamples.Current as Sample;
                if (s == null)
                    return;
                if (rdAPI.Checked)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("API Count {0}\r\n", s.APIs.Count);
                    sb.AppendFormat("Semantic Count {0} \r\n\r\n", s.SemanticUnits.Count);
                    foreach (var a in s.APIs)
                        sb.AppendLine(a.ToString());
                    txtReport.Text = sb.ToString();
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("API Count {0}\r\n", s.APIs.Count);
                    sb.AppendFormat("Semantic Count {0} \r\n\r\n", s.SemanticUnits.Count);
                    foreach (var a in s.SemanticUnits)
                        sb.AppendLine(a.ToString());
                    txtReport.Text = sb.ToString();

                }
            }
        }

        private void btnAddDataset_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() != DialogResult.OK)
                return;
            bool isMalicious=cmbType.SelectedIndex == 0;
            manager.Analyze(of.FileName,isMalicious );
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            if (sf.ShowDialog() != DialogResult.OK)
                return;
            manager.SaveDataset(sf.FileName);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() != DialogResult.OK)
                return;
            manager.LoadDataset(of.FileName);
            bndSamples.ResetBindings(false);
        }


        private void rdAPI_CheckedChanged(object sender, EventArgs e)
        {
            Sample s = bndSamples.Current as Sample;
            if (s == null)
                return;
            if (rdAPI.Checked)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("API Count {0}\r\n", s.APIs.Count);
                sb.AppendFormat("Semantic Count {0} \r\n\r\n", s.SemanticUnits.Count);
                foreach (var a in s.APIs)
                    sb.AppendLine(a.ToString());
                txtReport.Text = sb.ToString();
            }
        }

        private void rdSemantic_CheckedChanged(object sender, EventArgs e)
        {
            Sample s = bndSamples.Current as Sample;
            if (s == null)
                return;
            if (rdSemantic.Checked)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("API Count {0}\r\n", s.APIs.Count);
                sb.AppendFormat("Semantic Count {0} \r\n\r\n", s.SemanticUnits.Count);
                foreach (var a in s.SemanticUnits)
                    sb.AppendLine(a.ToString());
                txtReport.Text = sb.ToString();
            }
        }

    }
}
