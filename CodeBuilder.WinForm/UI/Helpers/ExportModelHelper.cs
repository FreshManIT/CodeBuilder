using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeBuilder.DataSource.Exporter;
using CodeBuilder.Framework.Configuration;
using CodeBuilder.PhysicalDataModel;
using CodeBuilder.WinForm.Properties;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI
{
    public static class ExportModelHelper
    {
        public static TreeNode ExportPdm(string connectionString, TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(connectionString, 1, 1);
            treeView.Nodes.Add(rootNode);

            Export(new PowerDesigner12Exporter(), connectionString, rootNode);

            return rootNode;
        }

        /// <summary>
        /// export table info.
        /// </summary>
        /// <param name="dataSourceName"></param>
        /// <param name="treeView"></param>
        /// <returns></returns>
        public static TreeNode Export(string dataSourceName, TreeView treeView)
        {
            TreeNode rootNode = new TreeNode(dataSourceName, 1, 1);
            treeView.Nodes.Add(rootNode);

            string connectionString = ConfigManager.DataSourceSection.DataSources[dataSourceName].ConnectionString;
            string exporterName = ConfigManager.DataSourceSection.DataSources[dataSourceName].Exporter;
            string typeName = ConfigManager.SettingsSection.Exporters[exporterName].Type;
            var type = Type.GetType(typeName);
            if (type == null) return rootNode;
            IExporter exporter = (IExporter)Activator.CreateInstance(type);
            Export(exporter, connectionString, rootNode);

            return rootNode;
        }

        /// <summary>
        /// Do export
        /// </summary>
        /// <param name="exporter"></param>
        /// <param name="connectionString"></param>
        /// <param name="rootNode"></param>
        private static void Export(IExporter exporter, string connectionString, TreeNode rootNode)
        {
            Model model = exporter.Export(connectionString);

            rootNode.Tag = model.Database;
            AddBranchToTree(model, rootNode);

            ModelManager.Add(rootNode.Index.ToString(), model);
        }

        /// <summary>
        /// Add one node.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parentNode"></param>
        private static void AddBranchToTree(Model model, TreeNode parentNode)
        {
            AddBranchToTree(model.Tables, parentNode);
            AddBranchToTree(model.Views, parentNode);
        }

        /// <summary>
        /// check node,if the node is parent node,checked his child nodes.
        /// </summary>
        /// <param name="tn"></param>
        public static void CheckedTreeNode(TreeNode tn)
        {
            foreach (TreeNode childNode in tn.Nodes)
            {
                if (childNode.Checked != tn.Checked) childNode.Checked = tn.Checked;
                CheckedTreeNode(childNode);
            }
        }

        /// <summary>
        /// Add branch to tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="parentNode"></param>
        private static void AddBranchToTree<T>(Dictionary<string, T> objects, TreeNode parentNode) where T : BaseTable
        {
            if (objects == null ||
                objects.Count == 0) return;

            string text = typeof(T).Name.Equals("Table") ? Resources.Tables : Resources.Views;
            TreeNode childNode = new TreeNode(text, 1, 1);
            foreach (T baseTable in objects.Values)
            {
                TreeNode newNode = new TreeNode
                {
                    Tag = baseTable.Id,
                    Text = baseTable.DisplayName,
                    ToolTipText = baseTable.DisplayName,
                    ImageIndex = 2,
                    SelectedImageIndex = 2
                };
                childNode.Nodes.Add(newNode);
            }
            parentNode.Nodes.Add(childNode);
        }
    }
}
