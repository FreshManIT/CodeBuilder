using System.Windows.Forms;
using CodeBuilder.WinForm.Properties;
using CodeBuilder.WinForm.UI.OptionsPages;

namespace CodeBuilder.WinForm.UI
{
    /// <summary>
    /// OptionDialog
    /// </summary>
    public class OptionsDialog
    {
        /// <summary>
        /// Display option page.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="initialPage"></param>
        public static void Display(Form owner, string initialPage = null)
        {
            TreeOptionsDialog.Display(owner, initialPage,
                new RecentFilesOptionsPage(Resources.EnvironmentRecentFiles),
                new CodeGeneralOptionsPage(Resources.CodeGenerationGeneral),
                new DataSourceOptionsPage(Resources.DataSourceManagerDataSources),
                new TemplateOptionsPage(Resources.TemplateManagerTemplates),
                new TraceOptionsPage(Resources.AdvancedSettingsInternalTrace));
        }
    }
}
