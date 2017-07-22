using System;
using CodeBuilder.Framework.Configuration;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI.OptionsPages
{
    using Properties;
    using Util;

    public partial class TraceOptionsPage : BaseOptionsPage
    {
        public TraceOptionsPage()
        {
            InitializeComponent();
        }

        public TraceOptionsPage(string key)
            : base(key)
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            IsLoaded = true;
            traceLevelCombox.Text = ConfigManager.OptionSection.Options["Options.InternalTraceLevel"].Value;
            logDirectoryLabel.Text = ConfigManager.LogDirectory;
        }

        public override void ApplySettings()
        {
            try
            {
                ConfigManager.OptionSection.Options["Options.InternalTraceLevel"].Value = traceLevelCombox.Text;
                ConfigManager.RefreshOptions();
                ConfigManager.Save();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(Resources.SaveOptionsInternalTraceLevelFailure, ex);
            }

            InternalTraceLevel level = (InternalTraceLevel)Enum.Parse(InternalTraceLevel.Default.GetType(), traceLevelCombox.Text, true);
            InternalTrace.ReInitialize("CodeBuilder_%p.log", level);
        }
    }
}
