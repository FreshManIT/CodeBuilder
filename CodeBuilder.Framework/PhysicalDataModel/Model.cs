// ReSharper disable once CheckNamespace
namespace CodeBuilder.PhysicalDataModel
{
    /// <summary>
    /// Represent the Physical Data Model of Database.
    /// </summary>
    public class Model
    {
        private string _database;
        private string _databaseName;
        private string _schema;
        private Tables _tables;
        private Views _views;

        public Model() { }

        public Model(Tables tables)
        {
            _tables = tables;
        }

        public Model(Views views)
        {
            _views = views;
        }

        public Model(Tables tables, Views views)
            : this(tables)
        {
            _views = views;
        }

        public Tables Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }

        public Views Views
        {
            get { return _views; }
            set { _views = value; }
        }

        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        public string Schema
        {
            get { return _schema; }
            set { _schema = value; }
        }
    }
}
