using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBuilder.Util;

namespace CodeBuilder.WinForm.UI.Forms
{
    public partial class BaseOptionsDialog : Form
    {
        private static readonly Logger Logger = InternalTrace.GetLogger(typeof(BaseOptionsDialog));

        public BaseOptionsDialog()
        {
            InitializeComponent();
            optionsPages = new OptionsPageList(6);
        }

        #region Properties

        private OptionsPageList optionsPages;
        public OptionsPageList OptionsPages
        {
            get { return optionsPages; }
        }

        #endregion

        #region Public Methods

        public void ApplySettings()
        {
            foreach (BaseOptionsPage page in optionsPages)
            {
                try
                {
                    if (page.IsLoaded) page.ApplySettings();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    MessageBoxHelper.DisplayFailure(ex.Message);
                }
            }
        }

        #endregion

        #region Event Handlers

        private void BaseOptionsDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ApplySettings();
            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion

        #region Nested SettingsPageCollection Class

        public class OptionsPageList : List<BaseOptionsPage>
        {
            public OptionsPageList()
            { }

            public OptionsPageList(int capacity)
                : base(capacity)
            {
            }

            public BaseOptionsPage this[string key]
            {
                get
                {
                    return this.FirstOrDefault(x => x.Key == key);
                }
            }
        }
        #endregion
    }
}
