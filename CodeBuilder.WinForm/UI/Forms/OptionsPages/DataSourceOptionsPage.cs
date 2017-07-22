using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using CodeBuilder.Configuration;
using CodeBuilder.Framework.Configuration;
using CodeBuilder.Util;
using CodeBuilder.WinForm.Properties;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI.OptionsPages
{
    public partial class DataSourceOptionsPage : BaseOptionsPage
    {
        private static readonly Logger Logger = InternalTrace.GetLogger(typeof(DataSourceOptionsPage));
        private readonly Dictionary<string, DataSourceItem> _listBoxItems = new Dictionary<string, DataSourceItem>(10);

        public DataSourceOptionsPage()
        {
            InitializeComponent();
        }

        public DataSourceOptionsPage(string key)
            : base(key)
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            IsLoaded = true;
            ListExporterItems();
            ListDataSourceItems();
        }

        public override void ApplySettings()
        {
            try
            {
                SaveChanged();
                _listBoxItems.Clear();
                ConfigManager.RefreshDataSources();
                ConfigManager.Save();  
            }
            catch (Exception ex)
            {
                throw new ApplicationException(Resources.SaveDataSourceItemsFailure, ex);
            }
        }

        #region Event Handlers

        private void datasourceListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (datasourceListbox.SelectedItem == null) return;
            SelectedListBoxItem(datasourceListbox.SelectedItem.ToString());
        }

        private void removeBtn_Click(object sender, EventArgs e)
        {
            object selectedItem = datasourceListbox.SelectedItem;
            if (selectedItem == null) return;

            string name = selectedItem.ToString().Trim().ToLower();
            _listBoxItems[name].Status = DataSourceItemStatus.Deleted;

            try
            {
                datasourceListbox.Items.RemoveAt(datasourceListbox.SelectedIndex);
            }
            catch (Exception ex)
            {
                Logger.Error(Resources.SaveDataSourceItemsFailure, ex);
                MessageBoxHelper.DisplayFailure(Resources.SaveDataSourceItemsFailure);
            }
        }

        private void newsaveBtn_Click(object sender, EventArgs e)
        {
            string name = nameTxtbox.Text.Trim();
            string connString = connstrTxtbox.Text.Trim();
            string exporter = exporterCombox.Text;

            if (name.Trim().Length == 0 ||
                connString.Trim().Length == 0)
            {
                MessageBoxHelper.Display(Resources.NameOrConnectionstringCanntSetEmpty);
                return;
            }

            if (_listBoxItems.ContainsKey(name.ToLower()))
            {
                _listBoxItems[name.ToLower()].Name = name;
                _listBoxItems[name.ToLower()].ConnectionString = connString;
                _listBoxItems[name.ToLower()].Exporter = exporter;
                if (_listBoxItems[name.ToLower()].Status != DataSourceItemStatus.New)
                    _listBoxItems[name.ToLower()].Status = DataSourceItemStatus.Edit;
                return;
            }

            _listBoxItems.Add(name.ToLower(), new DataSourceItem(name, connString, exporter, DataSourceItemStatus.New));
            datasourceListbox.Items.Add(name);
        }

        #endregion

        #region Helper methods for modifying the UI display

        private void ListExporterItems()
        {
            exporterCombox.Items.Clear();
            foreach (ExporterElement exporter in ConfigManager.SettingsSection.Exporters)
            {
                exporterCombox.Items.Add(exporter.Name);
            }

            exporterCombox.Text = exporterCombox.Items[0].ToString();
        }

        private void ListDataSourceItems()
        {
            datasourceListbox.Items.Clear();
            _listBoxItems.Clear();

            foreach (DataSourceElement dataSource in ConfigManager.DataSourceSection.DataSources)
            {
                datasourceListbox.Items.Add(dataSource.Name);
                string name = dataSource.Name.Trim().ToLower();
                if (!_listBoxItems.ContainsKey(name)) { 
                    _listBoxItems.Add(name, new DataSourceItem(dataSource.Name, dataSource.ConnectionString, dataSource.Exporter));
                }
            }
        }

        private void SelectedListBoxItem(string selectedItem)
        {
            string name = selectedItem.Trim().ToLower();
            if (!_listBoxItems.ContainsKey(name)) return;

            nameTxtbox.Text = _listBoxItems[name].Name;
            connstrTxtbox.Text = _listBoxItems[name].ConnectionString;
            exporterCombox.Text = _listBoxItems[name].Exporter;
        }

        private void SaveChanged()
        {
            foreach (var item in _listBoxItems)
            {
                if (item.Value.Status == DataSourceItemStatus.None) continue;
                if (item.Value.Status == DataSourceItemStatus.Deleted)
                {
                    ConfigManager.DataSourceSection.DataSources.Remove(item.Value.Name);
                    continue;
                }

                if (item.Value.Status == DataSourceItemStatus.New)
                {
                    DataSourceElement element = new DataSourceElement
                    {
                        Name = item.Value.Name,
                        ConnectionString = item.Value.ConnectionString,
                        Exporter = item.Value.Exporter
                    };
                    ConfigManager.DataSourceSection.DataSources.Add(element);
                    continue;
                }
                if (item.Value.Status == DataSourceItemStatus.Edit)
                {
                    ConfigManager.DataSourceSection.DataSources[item.Value.Name].Name = item.Value.Name;
                    ConfigManager.DataSourceSection.DataSources[item.Value.Name].ConnectionString = item.Value.ConnectionString;
                    ConfigManager.DataSourceSection.DataSources[item.Value.Name].Exporter = item.Value.Exporter;
                }
            }
        }

        #endregion	

        private class DataSourceItem
        {
            public DataSourceItem(string name, string connectionString, string exporter, DataSourceItemStatus status = DataSourceItemStatus.None)
            {
                Name = name;
                ConnectionString = connectionString;
                Status = status;
                Exporter = exporter;
            }

            public string Name { get; set; }
            public string ConnectionString { get; set; }
            public string Exporter { get; set; }
            public DataSourceItemStatus Status { get; set; }
        }

        private enum DataSourceItemStatus
        {
            None,
            Edit,
            New,
            Deleted,
        }

        private void connStrRefSiteLbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(connStrRefSiteLbl.Text);
            connStrRefSiteLbl.LinkVisited = true;
        }
    }
}
