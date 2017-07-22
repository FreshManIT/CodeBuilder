using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            SetAboutInfo();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void infoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(infoLinkLabel.Text);
            infoLinkLabel.LinkVisited = true;
        }

        private void SetAboutInfo()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string versionText = executingAssembly.GetName().Version.ToString();
            versionLabel.Text = versionText;

            dotNetVersionLabel.Text = string.Format(".Net Framework {0}", Environment.Version);
        }
    }
}
