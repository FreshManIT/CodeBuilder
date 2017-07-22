using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBuilder.DataSource.Exporter
{
    using PhysicalDataModel;

    public class Oracle8iExporter : BaseExporter
    {
        #region IExporter Members

        public override Model Export(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            Model model = new Model {Database = "Oracle8i"};

            return model;
        }

        #endregion

        #region Private Members

        private Tables GetTables()
        {
            return null;
        }

        private Views GetViews()
        {
            return null;
        }

        private Columns GetColumns()
        {
            return null;
        }

        private Columns GetKeys()
        {
            return null;
        }

        private Columns GetPrimaryKeys()
        {
            return null;
        }

        #endregion
    }
}
