using System;
using System.Windows.Forms;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI
{
    public partial class BaseOptionsPage : UserControl
    {
        /// <summary>
        /// option page is loaded
        /// </summary>
        protected bool isLoaded;

        public BaseOptionsPage()
        {
            InitializeComponent();
        }

        public BaseOptionsPage(string key)
            : this()
        {
            Key = key;
            Title = key;
            int dot = key.LastIndexOf('.');
            if (dot >= 0) Title = key.Substring(dot + 1);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode) this.LoadSettings();
        }

        #region Properties

        /// <summary>
        /// option page key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// option page title
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// option page is loaded
        /// </summary>
        public bool IsLoaded => isLoaded;

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
