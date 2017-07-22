using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CodeBuilder.Configuration;
using CodeBuilder.Framework.Configuration;
using CodeBuilder.Util;
using CodeBuilder.WinForm.Properties;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI.OptionsPages
{
    public partial class TemplateOptionsPage : BaseOptionsPage
    {
        private static Logger logger = InternalTrace.GetLogger(typeof(TemplateOptionsPage));
        private Dictionary<string, TemplateItem> listBoxItems = new Dictionary<string, TemplateItem>(20);

        public TemplateOptionsPage()
        {
            InitializeComponent();
        }

        public TemplateOptionsPage(string key)
            : base(key)
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            IsLoaded = true;
            SetComboBoxItems();
            ListTemplateItems();
        }

        public override void ApplySettings()
        {
            try
            {
                SaveChanged();
                listBoxItems.Clear();
                ConfigManager.RefreshTemplates();
                ConfigManager.Save();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(Resources.SaveTemplateItemsFailure, ex);
            }
        }

        #region Event Handlers

        private void templateListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (templateListbox.SelectedItem == null) return;
            SelectedListBoxItem(templateListbox.SelectedItem.ToString());
        }

        private void openFileDialogBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileNameTextbox.Text = openFileDialog.FileName;
            }
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            if (fileNameTextbox.Text.Trim().Length == 0) return;

            try
            {
                Process.Start("Notepad.exe", fileNameTextbox.Text);
            }
            catch (Exception ex)
            {
                logger.Error(Resources.EditTemplateFileFailure, ex);
                MessageBoxHelper.DisplayFailure(Resources.EditTemplateFileFailure);
            }
        }

        private void removeBtn_Click(object sender, EventArgs e)
        {

            object selectedItem = templateListbox.SelectedItem;
            if (selectedItem == null) return;

            string name = selectedItem.ToString().Trim().ToLower();
            listBoxItems[name].Status = TemplateItemStatus.Deleted;

            try
            {
                templateListbox.Items.RemoveAt(templateListbox.SelectedIndex);
            }
            catch (Exception ex)
            {
                logger.Error(Resources.RemoveTemplateItemFailure, ex);
                MessageBoxHelper.DisplayFailure(Resources.RemoveTemplateItemFailure);
            }
        }

        private void newsaveBtn_Click(object sender, EventArgs e)
        {
            string language = languageCombox.Text.Trim();
            string engine = engineCombox.Text.Trim();
            string fileName = fileNameTextbox.Text.Trim();
            string displayName = displayNameTxtbox.Text.Trim();
            string prefix = prefixTxtBox.Text.Trim();
            string suffix = suffixTxtBox.Text.Trim();

            if (displayName.Length == 0 || fileName.Length == 0)
            {
                MessageBoxHelper.Display(Resources.DisplayNameOrFileNameCanntSetEmpty); return;
            }

            if (!File.Exists(fileName))
            {
                MessageBoxHelper.Display(Resources.TemplateFileNotFound); return;
            }

            try
            {
                string name = GetTemplateUniqueName(language, engine, displayName);
                if (listBoxItems.ContainsKey(name))
                {
                    listBoxItems[name].Name = name;
                    listBoxItems[name].Language = language;
                    listBoxItems[name].Engine = engine;
                    listBoxItems[name].FileName = fileName;
                    listBoxItems[name].DisplayName = displayName;
                    listBoxItems[name].Prefix = prefix;
                    listBoxItems[name].Suffix = suffix;
                    if (listBoxItems[name].Status != TemplateItemStatus.New)
                        listBoxItems[name].Status = TemplateItemStatus.Edit;

                    return;
                }

                listBoxItems.Add(name, new TemplateItem(name, language,
                    engine, fileName, displayName, prefix, suffix, TemplateItemStatus.New));
                templateListbox.Items.Add(name);
            }
            catch (Exception ex)
            {
                logger.Error(Resources.SaveOrNewTemplateFileFailure, ex);
                MessageBoxHelper.DisplayFailure(Resources.SaveOrNewTemplateFileFailure);
            }
        }

        private void getItFromOnlineBtn_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Helper methods for modifying the UI display

        private void SetComboBoxItems()
        {
            languageCombox.Items.Clear();
            engineCombox.Items.Clear();

            foreach (LanguageElement language in ConfigManager.SettingsSection.Languages)
            {
                languageCombox.Items.Add(language.Name);
            }

            foreach (TemplateEngineElement templateEngine in ConfigManager.SettingsSection.TemplateEngines)
            {
                engineCombox.Items.Add(templateEngine.Name);
            }

            languageCombox.Text = languageCombox.Items[0].ToString();
            engineCombox.Text = engineCombox.Items[0].ToString();
        }

        private void ListTemplateItems()
        {
            templateListbox.Items.Clear();
            listBoxItems.Clear();

            foreach (TemplateElement template in ConfigManager.TemplateSection.Templates)
            {
                string name = template.Name.Trim();
                string fileName = Path.Combine(ConfigManager.TemplatePath, template.FileName);
                if (!listBoxItems.ContainsKey(name))
                {
                    templateListbox.Items.Add(name);
                    listBoxItems.Add(name, new TemplateItem(template.Name, template.Language,
                        template.Engine, fileName, template.DisplayName, template.Prefix, template.Suffix));
                }
            }
        }

        private void SelectedListBoxItem(string selectedItem)
        {
            if (selectedItem == null) return;

            string name = selectedItem;
            if (!listBoxItems.ContainsKey(name)) return;

            languageCombox.Text = listBoxItems[name].Language;
            engineCombox.Text = listBoxItems[name].Engine;
            displayNameTxtbox.Text = listBoxItems[name].DisplayName;
            fileNameTextbox.Text = listBoxItems[name].FileName;
            prefixTxtBox.Text = listBoxItems[name].Prefix;
            suffixTxtBox.Text = listBoxItems[name].Suffix;
        }

        private void SaveChanged()
        {
            foreach (var item in listBoxItems)
            {
                if (item.Value.Status == TemplateItemStatus.None) continue;
                if (item.Value.Status == TemplateItemStatus.Deleted)
                {
                    ConfigManager.TemplateSection.Templates.Remove(item.Value.Name);
                    continue;
                }

                item.Value.FileName = GetTemplateReleatedFileName(item.Value);
                if (string.IsNullOrEmpty(item.Value.FileName)) continue;

                if (item.Value.Status == TemplateItemStatus.New)
                {
                    AddTemplate(item.Value);
                    continue;
                }

                if (item.Value.Status == TemplateItemStatus.Edit)
                {
                    EditTemplate(item.Value);
                }
            }
        }

        private void AddTemplate(TemplateItem item)
        {
            TemplateElement element = new TemplateElement();
            element.Name = item.Name;
            element.Language = item.Language;
            element.Engine = item.Engine;
            element.FileName = item.FileName;
            element.DisplayName = item.DisplayName;
            element.Prefix = item.Prefix;
            element.Suffix = item.Suffix;
            element.Url = item.Url;
            element.Description = item.Description;
            ConfigManager.TemplateSection.Templates.Add(element);
        }

        private void EditTemplate(TemplateItem item)
        {
            ConfigManager.TemplateSection.Templates[item.Name].Name = item.Name;
            ConfigManager.TemplateSection.Templates[item.Name].Language = item.Language;
            ConfigManager.TemplateSection.Templates[item.Name].Engine = item.Engine;
            ConfigManager.TemplateSection.Templates[item.Name].FileName = item.FileName;
            ConfigManager.TemplateSection.Templates[item.Name].DisplayName = item.DisplayName;
            ConfigManager.TemplateSection.Templates[item.Name].Prefix = item.Prefix;
            ConfigManager.TemplateSection.Templates[item.Name].Suffix = item.Suffix;
            ConfigManager.TemplateSection.Templates[item.Name].Url = item.Url;
            ConfigManager.TemplateSection.Templates[item.Name].Description = item.Description;
        }

        private string GetTemplateReleatedFileName(TemplateItem item)
        {
            string languageAlais = ConfigManager.SettingsSection.Languages[item.Language].Alias;
            string fileName = CopyTemplateFile(item.DisplayName.ToLower(), languageAlais, item.Engine, item.FileName);
            return fileName.Replace(ConfigManager.TemplatePath, "").TrimStart('\\', '/');
        }

        private string CopyTemplateFile(string displayName, string language, string engine, string srcFileName)
        {
            string destFileName = string.Empty;

            try
            {
                if (File.Exists(srcFileName))
                {
                    string path = Path.Combine(ConfigManager.TemplatePath, language, engine);
                    string enginext = ConfigManager.SettingsSection.TemplateEngines[engine].Extension;
                    destFileName = Path.Combine(path, displayName + enginext);

                    if (destFileName.Equals(srcFileName,
                        StringComparison.CurrentCultureIgnoreCase)) return srcFileName;

                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    File.Copy(srcFileName, destFileName, true);
                }
            }
            catch (Exception ex)
            {
                destFileName = string.Empty;
                logger.Error(string.Format(Resources.SaveTemplateFileFailure, srcFileName), ex);
            }

            return destFileName;
        }

        private string GetTemplateUniqueName(string language, string engineName, string displayName)
        {
            string langext = ConfigManager.SettingsSection.Languages[language].Extension;
            string enginext = ConfigManager.SettingsSection.TemplateEngines[engineName].Extension;
            return $"{displayName}{langext}{enginext}".ToLower();
        }


        #endregion	

        private class TemplateItem
        {
            public TemplateItem(string name, string language, string engine,
                string fileName, string displayName, string prefix, string suffix, TemplateItemStatus status = TemplateItemStatus.None)
                : this(name, language, engine, fileName, displayName, prefix, suffix, "", "", status)
            {
            }

            public TemplateItem(string name, string language, string engine,
                string fileName, string displayName, string prefix, string suffix, string url, string desc, TemplateItemStatus status)
            {
                Name = name;
                Language = language;
                Engine = engine;
                FileName = fileName;
                DisplayName = displayName;
                Prefix = prefix;
                Suffix = suffix;
                Url = url;
                Description = desc;
                Status = status;
            }

            public string Name { get; set; }
            public string Language { get; set; }
            public string Engine { get; set; }
            public string FileName { get; set; }
            public string DisplayName { get; set; }
            public string Prefix { get; set; }
            public string Suffix { get; set; }
            public string Url { get; set; }
            public string Description { get; set; }
            public TemplateItemStatus Status { get; set; }
        }

        private enum TemplateItemStatus
        {
            None,
            Edit,
            New,
            Deleted
        }
    }
}
