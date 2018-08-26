using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StolotoParser
{
    public interface IMainForm
    {
        event EventHandler Download;

        bool EnableBtn { set; }
        string NameChecked { get; }
        string SelectToPath { get; }
        string GetMaximum { get; }

        string SetMessageProgress { set; }
        int SetMessageMaxValue { set; }
        int SetMessageValue { set; }
    }

    public partial class MainForm : Form, IMainForm
    {
        public event EventHandler Download;

        public MainForm()
        {
            InitializeComponent();

            btnTo.Click += BtnTo_Click;
            btnDovnload.Click += BtnDovnload_Click;
        }

        private void BtnDovnload_Click(object sender, EventArgs e)
        {
            if (Download != null)
            {
                Download(this, EventArgs.Empty);
            }
        }

        public string NameChecked
        {
            get
            {
                var control = Controls.OfType<RadioButton>()
                    .FirstOrDefault(r => r.Checked);

                if (control != null)
                {
                    return control.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string SelectToPath
        {
            get { return tbTo.Text; }
        }

        public bool EnableBtn
        {
            set
            {
                var settextAction = new Action(() => { btnDovnload.Enabled = value; });

                if (btnDovnload.InvokeRequired)
                    btnDovnload.Invoke(settextAction);
                else
                    settextAction();
            }
        }

        public string GetMaximum
        {
            get
            {
                return textMaximum.Text;
            }
        }

        public string SetMessageProgress
        {
            set
            {
                var settextAction = new Action(() => { labelProgress.Text = value; });

                if (labelProgress.InvokeRequired)
                    labelProgress.Invoke(settextAction);
                else
                    settextAction();
            }
        }

        public int SetMessageMaxValue
        {
            set
            {
                var settextAction = new Action(() => { progressBar.Maximum = value; });

                if (progressBar.InvokeRequired)
                    progressBar.Invoke(settextAction);
                else
                    settextAction();
            }
        }

        public int SetMessageValue
        {
            set
            {
                var settextAction = new Action(() => { progressBar.Value = value; });

                if (progressBar.InvokeRequired)
                    progressBar.Invoke(settextAction);
                else
                    settextAction();
            }
        }

        private void BtnTo_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbTo.Text = dlg.SelectedPath;
            }
        }
    }
}

