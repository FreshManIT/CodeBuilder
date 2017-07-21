using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBuilder.Configuration;
using CodeBuilder.Framework.Configuration;
using CodeBuilder.Util;
using CodeBuilder.WinForm.Properties;
using CodeBuilder.WinForm.UI;

namespace CodeBuilder.WinForm
{
    public partial class MainForm : Form
    {
        #region [1 Init main window]
        private static readonly Logger Logger = InternalTrace.GetLogger(typeof(MainForm));
        private string _currentGenerationSettingsFile;

        public MainForm()
        {
            InitializeComponent();
            InitializeUiData();
        }

        /// <summary>
        /// init ui data info.
        /// </summary>
        private void InitializeUiData()
        {
            SetComboBoxItems();
            SetStatusItems();
        }
        #endregion

        #region [2 Menu Handlers]

        #region File
        private void fileOpenMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = Resources.OpenGenerationSettingsFile;
            openFileDialog.Filter = Resources.GenerationSettingsFileExt;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string xmlFileName = openFileDialog.FileName;
                try
                {
                    SetGenerationSettings(xmlFileName);
                }
                catch (Exception ex)
                {
                    Logger.Error(Resources.OpenGenerationSettingsFile, ex);
                    MessageBoxHelper.DisplayFailure(Resources.InvalidGenerationSettingsFile);
                    return;
                }

                string menuItemText = string.Format(Resources.SaveFormat, Path.GetFileName(xmlFileName));
                fileSaveMenuItem.Text = menuItemText;
                saveGenSettingCtxMenuItem.Text = menuItemText;
                _currentGenerationSettingsFile = xmlFileName;
                statusBarReady.Text = string.Format(Resources.OpenFormat, xmlFileName);
            }
        }

        /// <summary>
        /// Save new generation setting file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileSaveMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckParameters()) return;

            GenerationSettings settings = GetGenerationSettings();
            string xmlFileName = _currentGenerationSettingsFile;
            string cmdText = Resources.Modified;
            if (string.IsNullOrEmpty(xmlFileName))
            {
                cmdText = Resources.Saved;
                saveFileDialog.Filter = Resources.GenerationSettingsFileExt;
                saveFileDialog.DefaultExt = ".xml";
                saveFileDialog.FileName = "New_GenerationSettings.xml";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    xmlFileName = saveFileDialog.FileName;
                else return;
            }

            try
            {
                SerializeHelper.XmlSerialize(settings,xmlFileName);
            }
            catch (Exception ex)
            {
                Logger.Error(Resources.SaveGenerationSettingsFile, ex);
                MessageBoxHelper.DisplayFailure(ex.Message);
                return;
            }

            statusBarReady.Text = string.Format("{0} {1}", cmdText, xmlFileName);
            _currentGenerationSettingsFile = xmlFileName;
        }

        /// <summary>
        /// export pdm file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileExportPdmMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Title = Resources.OpenPowerDesignerPDMFile;
            openFileDialog.Filter = Resources.PhysicalDataModelFileExt;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string pdmFileName = openFileDialog.FileName;

                try
                {
                    TreeNode rootNode = ExportModelHelper.ExportPDM(pdmFileName, treeView);
                    rootNode.ExpandAll();
                    treeView.SelectedNode = rootNode;
                }
                catch (Exception ex)
                {
                    if (treeView.Nodes.Count > 0) 
                        treeView.Nodes[treeView.Nodes.Count - 1].Remove();

                    Logger.Error(Resources.ExportPDMFileFailure, ex);
                    MessageBoxHelper.Display(ex.Message);
                    return;
                }

                clearCtxMenuItem.Enabled = true;
                statusBarReady.Text = string.Format(Resources.ExportFormat, pdmFileName);
            }
        }

        /// <summary>
        /// Load data source menu items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileExportDataSourceMenuItem_MouseHover(object sender, EventArgs e)
        {
            AddDataSourceMenuItems(fileExportDataSourceMenuItem);
        }

        /// <summary>
        /// click data source menu item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileExportDataSourceMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            try
            {
                if (menuItem != null)
                {
                    TreeNode rootNode = ExportModelHelper.Export(menuItem.Text, treeView);
                    rootNode.ExpandAll();
                    treeView.SelectedNode = rootNode;
                }
            }
            catch (Exception ex)
            {
                if (treeView.Nodes.Count > 0)
                    treeView.Nodes[treeView.Nodes.Count - 1].Remove();

                Logger.Error(Resources.ExportDataSourceFailure, ex);
                MessageBoxHelper.Display(ex.Message);
                return;
            }

            clearCtxMenuItem.Enabled = true;
            if (menuItem != null) statusBarReady.Text = string.Format(Resources.ExportDataSourceFormat, menuItem.Text);
        }

        /// <summary>
        /// Exit application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Tools

        private void toolsOptionsMenuItem_Click(object sender, EventArgs e)
        {
            OptionsDialog.Display(this);
        }

        private void toolsDSConfigMenuItem_Click(object sender, EventArgs e)
        {
            OptionsDialog.Display(this,Resources.DataSourceManagerDataSources);
        }

        private void toolsTemplatesMenuItem_Click(object sender, EventArgs e)
        {
            OptionsDialog.Display(this, Resources.TemplateManagerTemplates);
        }
        #endregion

        #region Help

        private void helpF1MenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(ConfigManager.HelpUrl);
        }

        private void helpFeedbackMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(ConfigManager.HelpUrl);
        }

        private void helpAboutMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox aboutBox = new AboutBox())
            {
                aboutBox.ShowDialog();
            }
        }

        #endregion

        #endregion

        #region [3 Context Menu Handlers]

        #region TreeView
        private void exportPDMCtxMenuItem_Click(object sender, EventArgs e)
        {
            fileExportPdmMenuItem_Click(sender, e);
        }

        private void exportDataSourceCtxMenuItem_MouseHover(object sender, EventArgs e)
        {
            AddDataSourceMenuItems(exportDataSourceCtxMenuItem);
        }

        /// <summary>
        /// Clrea click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearCtxMenuItem_Click(object sender, EventArgs e)
        {
            ModelManager.Clear();
            treeView.Nodes.Clear();
            clearCtxMenuItem.Enabled = false;
            statusBarDatabase.Text = string.Empty;
            statusBarReady.Text = Resources.Ready;
        }
        #endregion

        #region Generation SettingsSection
        private void openGenSettingsCtxMenuItem_Click(object sender, EventArgs e)
        {
            fileOpenMenuItem_Click(sender, e);
        }

        private void saveGenSettingCtxMenuItem_Click(object sender, EventArgs e)
        {
            fileSaveMenuItem_Click(sender, e);
        }

        private void generateCtxMenuItem_Click(object sender, EventArgs e)
        {
            generateBtn_Click(sender, e);
        }
   
        #endregion

        #endregion

        #region [4 TreeView Handlers]

        private void treeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 0;
            e.Node.SelectedImageIndex = 0;
        }

        private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = 1;
            e.Node.SelectedImageIndex = 1;
        }

        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            ExportModelHelper.CheckedTreeNode(e.Node);
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null && e.Node != null)
            {
                databaseNameLbl.Text = e.Node.Tag.ToString();
                statusBarDatabase.Text = databaseNameLbl.Text;
            }
        }
        #endregion

        #region [5 Generation Settings Handlers]

        private void saveSettingsBtn_Click(object sender, EventArgs e)
        {
            fileSaveMenuItem_Click(sender, e);
        }

        private void generateBtn_Click(object sender, EventArgs e)
        {   
            if (!CheckParameters()) return;
            var generationObjects = GenerationHelper.GetGenerationObjects(treeView);
            int genObjectCount = generationObjects.Sum(x => x.Value.Count);
            if (genObjectCount == 0)
            {
                MessageBoxHelper.DisplayInfo(Resources.ShouldCheckedTreeNode);
                return;
            }

            int fileCount = genObjectCount * templateListBox.SelectedItems.Count;
            generateBtn.Enabled = false;
            completedLbl.Visible = false;
            totalFileCountLbl.Text = fileCount.ToString();
            genProgressBar.Maximum = fileCount;

            GenerationParameter parameter = new GenerationParameter(
                ModelManager.Clone(),
                GenerationHelper.GetGenerationObjects(treeView),
                GetGenerationSettings());

            try
            {
                codeGeneration.GenerateAsync(parameter, Guid.NewGuid().ToString());
            }
            catch (Exception ex)
            {
                Logger.Error(Resources.GenerateFailure, ex);
            }
        }

        private void codeGeneration_Completed(object sender, GenerationCompletedEventArgs args)
        {
            generateBtn.Enabled = true;
            completedLbl.Visible = true;
            statusBarReady.Text = completedLbl.Text;
            SetGenFileNameLabel();
        }

        private void codeGeneration_ProgressChanged(GenerationProgressChangedEventArgs args)
        {
            genProgressBar.Value = args.ProgressPercentage;
            genFileCountLbl.Text = args.GeneratedCount.ToString();
            errorFileCountLbl.Text = args.ErrorCount.ToString();
            currentGenFileNameLbl.Text = args.CurrentFileName;
            statusBarReady.Text = args.CurrentFileName;
        }

        private void languageCombx_SelectedIndexChanged(object sender, EventArgs e)
        {
            statusBarLanguage.Text = languageCombx.Text;
            ChangeTemplateListBox(languageCombx.Text, templateEngineCombox.Text);
        }

        private void codeFileEncodingCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            statusBarEncoding.Text = codeFileEncodingCombox.Text;
        }

        private void templateEngineCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTemplateListBox(languageCombx.Text, templateEngineCombox.Text);
        }

        private void currentGenFileNameLbl_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentGenFileNameLbl.Text)) return;

            try
            {
                string folder = Path.GetDirectoryName(currentGenFileNameLbl.Text);
                Process.Start("explorer.exe", folder);
            }
            catch (Exception ex)
            {
                Logger.Error(Resources.OpenCodeGenerationFolderFailure, ex);
                MessageBoxHelper.DisplayInfo(Resources.OpenCodeGenerationFolderFailure);
            }
        }

        #endregion

        #region [6 Helper methods for modifying the UI display]

        /// <summary>
        /// set combox item resource.
        /// </summary>
        private void SetComboBoxItems()
        {
            //language combox
            languageCombx.Items.Clear();
            //encode combox
            templateEngineCombox.Items.Clear();
            //file code combox
            codeFileEncodingCombox.Items.Clear();

            foreach (LanguageElement language in ConfigManager.SettingsSection.Languages)
            {
                languageCombx.Items.Add(language.Name);
            }

            foreach (TemplateEngineElement templateEngine in ConfigManager.SettingsSection.TemplateEngines)
            {
                templateEngineCombox.Items.Add(templateEngine.Name);
            }

            foreach (var encodingInfo in Encoding.GetEncodings())
            {
                codeFileEncodingCombox.Items.Add(encodingInfo.Name.ToUpper());
            }

            languageCombx.Text = languageCombx.Items[0].ToString();
            templateEngineCombox.Text = templateEngineCombox.Items[0].ToString();
            codeFileEncodingCombox.Text = @"UTF-8";
        }

        /// <summary>
        /// set window state tool bar.
        /// </summary>
        private void SetStatusItems()
        {
            statusBarDatabase.Text = databaseNameLbl.Text;
            statusBarEncoding.Text = codeFileEncodingCombox.Text;
            statusBarLanguage.Text = languageCombx.Text;
        }

        /// <summary>
        /// set file lable
        /// </summary>
        private void SetGenFileNameLabel()
        {
            if (!string.IsNullOrEmpty(currentGenFileNameLbl.Text))
            {
                currentGenFileNameLbl.Cursor = Cursors.Hand;
                toolTip.SetToolTip(currentGenFileNameLbl, Resources.OpenCodeGenerationFolder);
            }
        }

        /// <summary>
        /// Add data source config items.
        /// </summary>
        /// <param name="parent"></param>
        private void AddDataSourceMenuItems(ToolStripMenuItem parent)
        {
            parent.DropDownItems.Clear();
            foreach (DataSourceElement dataSource in ConfigManager.DataSourceSection.DataSources)
            {
                ToolStripMenuItem ds = new ToolStripMenuItem(dataSource.Name);
                ds.Click += fileExportDataSourceMenuItem_Click;
                parent.DropDownItems.Add(ds);
            }
        }

        /// <summary>
        /// Change template item.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="engine"></param>
        private void ChangeTemplateListBox(string language, string engine)
        {
            templateListBox.Items.Clear();
            foreach (TemplateElement template in ConfigManager.TemplateSection.Templates)
            {
                if (template.Language.Equals(language, StringComparison.CurrentCultureIgnoreCase) &&
                    template.Engine.Equals(engine, StringComparison.CurrentCultureIgnoreCase))
                {

                    templateListBox.Items.Add(new TemplateListBoxItem(template.Name, template.DisplayName));
                    templateListBox.DisplayMember = "DisplayName";
                }
            }
        }

        /// <summary>
        /// get set file content
        /// </summary>
        /// <param name="xmlFileName"></param>
        private void SetGenerationSettings(string xmlFileName)
        {
            GenerationSettings settings = SerializeHelper.XmlDeserialize<GenerationSettings>(xmlFileName);
            languageCombx.Text = settings.Language;
            templateEngineCombox.Text = settings.TemplateEngine;
            packageTxtBox.Text = settings.Package;
            tablePrefixTxtBox.Text = settings.TablePrefix;
            authorTxtBox.Text = settings.Author;
            versionTxtBox.Text = settings.Version;
            codeFileEncodingCombox.Text = settings.Encoding;
            isOmitTablePrefixChkbox.Checked = settings.IsOmitTablePrefix;
            isStandardizeNameChkbox.Checked = settings.IsCamelCaseName;

            templateListBox.Items.Clear();
            foreach (string templateName in settings.TemplatesNames)
            {
                TemplateElement template = ConfigManager.TemplateSection.Templates[templateName];
                TemplateListBoxItem item = new TemplateListBoxItem(template.Name, template.DisplayName);
                templateListBox.Items.Add(item);
                templateListBox.SelectedItems.Add(item);
                templateListBox.DisplayMember = "DisplayName";
            }
        }

        private GenerationSettings GetGenerationSettings()
        {
            GenerationSettings settings = new GenerationSettings(languageCombx.Text,
                templateEngineCombox.Text, packageTxtBox.Text, tablePrefixTxtBox.Text,
                authorTxtBox.Text, versionTxtBox.Text,
                templateListBox.SelectedItems.Cast<TemplateListBoxItem>().Select(x=>x.Name).ToArray(),
                codeFileEncodingCombox.Text, isOmitTablePrefixChkbox.Checked, isStandardizeNameChkbox.Checked);
            return settings;
        }

        public bool CheckParameters()
        {
            if (!GenerationHelper.IsValidPackageName(packageTxtBox.Text))
            {
                MessageBoxHelper.DisplayInfo(Resources.PackageNameInvalid);
                packageTxtBox.Focus();
                return false;
            }

            if (templateListBox.SelectedItems.Count == 0)
            {
                MessageBoxHelper.DisplayInfo(Resources.ShouldSelectOneTemplate);
                templateListBox.Focus();
                return false;
            }

            return true;
        }

        #endregion	

        #region [7 TemplateListBox Item Inner Class]

        /// <summary>
        /// Template list box item
        /// </summary>
        private class TemplateListBoxItem
        {
            public TemplateListBoxItem(string name,string displayName)
            {
                Name = name;
                DisplayName = displayName;
            }

            /// <summary>
            /// name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// display name
            /// </summary>
            public string DisplayName { get; set; }
        }

        #endregion
    }
}
