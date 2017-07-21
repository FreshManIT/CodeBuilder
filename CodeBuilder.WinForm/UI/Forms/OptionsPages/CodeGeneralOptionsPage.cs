using System;
using System.IO;
using System.Windows.Forms;
using CodeBuilder.Framework.Configuration;
using CodeBuilder.WinForm.Properties;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI.OptionsPages
{
    public partial class CodeGeneralOptionsPage : BaseOptionsPage
    {
        public CodeGeneralOptionsPage()
        {
            InitializeComponent();
        }

        public CodeGeneralOptionsPage(string key)
            : base(key)
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            isLoaded = true;
            ouputPathTxtbox.Text = ConfigManager.GenerationCodeOuputPath;
            templatePathTxtbox.Text = ConfigManager.TemplatePath;
        }

        public override void ApplySettings()
        {
            try
            {
                string templatePath = templatePathTxtbox.Text;
                string ouputPath = ouputPathTxtbox.Text;

                if(!Directory.Exists(templatePath)) Directory.CreateDirectory(templatePath);
                if(!Directory.Exists(ouputPath)) Directory.CreateDirectory(ouputPath);

                ConfigManager.OptionSection.Options["CodeGeneration.General.TemplatePath"].Value = templatePath;
                ConfigManager.OptionSection.Options["CodeGeneration.General.OutputPath"].Value = ouputPath;
                ConfigManager.RefreshOptions();
                ConfigManager.Save();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(Resources.SaveOptionsCodeGenerationGeneralFailure, ex);
            }
        }

        private void ouputPathBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                ouputPathTxtbox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void templatePathBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                templatePathTxtbox.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
