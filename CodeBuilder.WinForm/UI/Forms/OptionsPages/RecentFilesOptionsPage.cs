using System;
using CodeBuilder.Framework.Configuration;
using CodeBuilder.Util;
using CodeBuilder.WinForm.Properties;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI.OptionsPages
{
    public partial class RecentFilesOptionsPage : BaseOptionsPage
    {
        public RecentFilesOptionsPage()
        {
            InitializeComponent();
        }

        public RecentFilesOptionsPage(string key)
            : base(key)
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            isLoaded = true;
            recentFilesCountTextBox.Text = ConfigManager.OptionSection.Options["Environment.RecentFiles.MaxFiles"].Value;
            string value = ConfigManager.OptionSection.Options["Environment.RecentFiles.IsCheckFileExist"].Value;
            checkFilesExistCheckBox.Checked = ConvertHelper.GetBoolean(value);
        }

        public override void ApplySettings()
        {
            try
            {
                ConfigManager.OptionSection.Options["Environment.RecentFiles.MaxFiles"].Value = recentFilesCountTextBox.Text;
                ConfigManager.OptionSection.Options["Environment.RecentFiles.IsCheckFileExist"].Value = checkFilesExistCheckBox.Checked.ToString();
                ConfigManager.RefreshOptions();
                ConfigManager.Save(); 
            }
            catch (Exception ex)
            {
                throw new ApplicationException(Resources.SaveEnvironmentRecentFilesFailure, ex);
            }
        }
    }
}
