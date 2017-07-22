// ReSharper disable once CheckNamespace
namespace CodeBuilder.TemplateEngine
{
    public class TemplateData
    {
        private string _name;
        private string _language;
        private string _database;
        private string _package;
        private string _tablePrefix;
        private string _author;
        private string _version;
        private string _templateEngine;
        private string _templateName;
        private string _prefix;
        private string _suffix;
        private string _encoding;
        private string _templateFileName;
        private string _codeFileName;
        private bool _isOmitTablePrefix;
        private bool _isCamelCaseName;
        private object _modelObject;

        public TemplateData() { }

        public TemplateData(string name, string language, string database, string templateEngine, string package,
            string tablePrefix, string author, string version, string templateName, string prefix, string suffix,
            string encoding, string templateFileName, string codeFileName, bool isOmitTablePrefix, bool isCamelCaseName, object modelObject)
        {
            _name = name;
            _language = language;
            _database = database;
            _templateEngine = templateEngine;
            _package = package;
            _tablePrefix = tablePrefix;
            _author = author;
            _version = version;
            _templateName = templateName;
            _prefix = prefix;
            _suffix = suffix;
            _encoding = encoding;
            _templateFileName = templateFileName;
            _codeFileName = codeFileName;
            _isOmitTablePrefix = isOmitTablePrefix;
            _isCamelCaseName = isCamelCaseName;
            _modelObject = modelObject;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public string Package
        {
            get { return _package ?? string.Empty; }
            set { _package = value; }
        }

        public string TablePrefix
        {
            get { return _tablePrefix ?? string.Empty; }
            set { _tablePrefix = value; }
        }

        public string Author
        {
            get { return _author ?? string.Empty; }
            set { _author = value; }
        }

        public string Version
        {
            get { return _version ?? string.Empty; }
            set { _version = value; }
        }

        public string TemplateEngine
        {
            get { return _templateEngine; }
            set { _templateEngine = value; }
        }

        public string TemplateName
        {
            get { return _templateName; }
            set { _templateName = value; }
        }

        public string Prefix
        {
            get { return _prefix ?? string.Empty; }
            set { _prefix = value; }
        }

        public string Suffix
        {
            get { return _suffix ?? string.Empty; }
            set { _suffix = value; }
        }

        public string Encoding
        {
            get { return _encoding ?? "UTF-8"; }
            set { _encoding = value; }
        }

        public string TemplateFileName
        {
            get { return _templateFileName; }
            set { _templateFileName = value; }
        }

        public string CodeFileName
        {
            get { return _codeFileName; }
            set { _codeFileName = value; }
        }

        public bool IsOmitTablePrefix
        {
            get { return _isOmitTablePrefix; }
            set { _isOmitTablePrefix = value; }
        }

        public bool IsCamelCaseName
        {
            get { return _isCamelCaseName; }
            set { _isCamelCaseName = value; }
        }

        public object ModelObject
        {
            get { return _modelObject; }
            set { _modelObject = value; }
        }
    }
}
