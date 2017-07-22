using System.Linq;
using System.Windows.Forms;
using CodeBuilder.WinForm.UI.Forms;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.WinForm.UI
{
    public partial class TreeOptionsDialog : BaseOptionsDialog
    {
        /// <summary>
        /// Current choose option page.
        /// </summary>
        private BaseOptionsPage _current;

        /// <summary>
        /// enter in option page name
        /// </summary>
        private static string _initialPage;

        /// <summary>
        /// struck function
        /// </summary>
        public TreeOptionsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// show which one page.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="initialPageName"></param>
        /// <param name="pages"></param>
        public static void Display(Form owner, string initialPageName, params BaseOptionsPage[] pages)
        {
            _initialPage = initialPageName;
            using (TreeOptionsDialog dialog = new TreeOptionsDialog())
            {
                owner.Site.Container?.Add(dialog);
                dialog.Font = owner.Font;
                dialog.OptionsPages.AddRange(pages);
                dialog.ShowDialog();
            }
        }

        #region TreeView Event Handlers

        /// <summary>
        /// select show one option page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string key = e.Node.FullPath;
            BaseOptionsPage page = OptionsPages[key];

            if (page != null && page != _current)
            {
                pagePanel.Controls.Clear();
                pagePanel.Controls.Add(page);
                page.Dock = DockStyle.Fill;
                _current = page;
            }
        }

        /// <summary>
        /// Tree view is expand event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = e.Node.SelectedImageIndex = 1;
        }

        /// <summary>
        /// Tree view is collapse event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionTreeView_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.ImageIndex = e.Node.SelectedImageIndex = 0;
        }

        #endregion

        /// <summary>
        /// Tree optiondialog load function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeOptionsDialog_Load(object sender, System.EventArgs e)
        {
            foreach (BaseOptionsPage page in OptionsPages)
                AddBranchToTree(optionTreeView.Nodes, page.Key);

            if (optionTreeView.VisibleCount >= optionTreeView.GetNodeCount(true))
                optionTreeView.ExpandAll();
            //select one page.
            SelectInitialPage();
            optionTreeView.Select();
        }

        /// <summary>
        /// select init page,if the page name is null or empty choose first
        /// </summary>
        private void SelectInitialPage()
        {
            if (_initialPage != null)
                SelectPage(_initialPage);
            else if (optionTreeView.Nodes.Count > 0)
                SelectFirstPage(optionTreeView.Nodes);
        }

        /// <summary>
        /// select page name
        /// </summary>
        /// <param name="pageName"></param>
        private void SelectPage(string pageName)
        {
            TreeNode node = FindNode(optionTreeView.Nodes, pageName);
            if (node != null)
                optionTreeView.SelectedNode = node;
            else
                SelectFirstPage(optionTreeView.Nodes);
        }

        /// <summary>
        /// Find one node show.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private TreeNode FindNode(TreeNodeCollection nodes, string key)
        {
            int dot = key.IndexOf('.');
            string tail = null;

            if (dot >= 0)
            {
                tail = key.Substring(dot + 1);
                key = key.Substring(0, dot);
            }

            return (from TreeNode node in nodes where node.Text == key select tail == null ? node : FindNode(node.Nodes, tail)).FirstOrDefault();
        }

        /// <summary>
        /// choose first node.
        /// </summary>
        /// <param name="nodes"></param>
        private void SelectFirstPage(TreeNodeCollection nodes)
        {
            if (nodes[0].Nodes.Count == 0)
                optionTreeView.SelectedNode = nodes[0];
            else
            {
                nodes[0].Expand();
                SelectFirstPage(nodes[0].Nodes);
            }
        }

        /// <summary>
        /// Add all of child node to parent node.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="key"></param>
        private void AddBranchToTree(TreeNodeCollection nodes, string key)
        {
            int dot = key.IndexOf('.');
            if (dot < 0)
            {
                nodes.Add(new TreeNode(key, 2, 2));
                return;
            }

            string name = key.Substring(0, dot);
            key = key.Substring(dot + 1);

            TreeNode node = FindOrAddNode(nodes, name);

            AddBranchToTree(node.Nodes, key);
        }

        /// <summary>
        /// Find or add node.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private TreeNode FindOrAddNode(TreeNodeCollection nodes, string name)
        {
            foreach (TreeNode node in nodes)
                if (node.Text == name) return node;

            TreeNode newNode = new TreeNode(name, 0, 0);
            nodes.Add(newNode);
            return newNode;
        }
    }
}
