using System;
using System.Windows.Forms;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI
{
    public partial class BaseOptionsPage : UserControl
    {
        public BaseOptionsPage()
        {
            InitializeComponent();
        }

        public BaseOptionsPage(string key)
            : this()
        {
            Key = key;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode) LoadSettings();
        }

        #region Properties

        /// <summary>
        /// option page key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// option page is loaded
        /// </summary>
        public bool IsLoaded { get; set; }

        #endregion

        #region Public Methods
        /// <summary>
        /// load setting
        /// </summary>
        public virtual void LoadSettings()
        {
        }

        /// <summary>
        /// ApplySetting
        /// </summary>
        public virtual void ApplySettings()
        {
        }

        #endregion
    }
}
