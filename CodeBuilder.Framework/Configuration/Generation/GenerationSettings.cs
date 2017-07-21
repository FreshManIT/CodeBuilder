using System;
using System.Xml.Serialization;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.Configuration
{
    [Serializable,
    XmlRoot("GenerationSettings")]
    public class GenerationSettings
    {
        /// <summary>
        /// language
        /// </summary>
        private string _language;

        /// <summary>
        /// package name
        /// </summary>
        private string _package;

        /// <summary>
        /// table prefix
        /// </summary>
        private string _tablePrefix;

        /// <summary>
        /// author
        /// </summary>
        private string _author;

        /// <summary>
        /// version number
        /// </summary>
        private string _version;

        /// <summary>
        /// template engine
        /// </summary>
        private string _templateEngine;

        /// <summary>
        /// template names,include more one template.
        /// </summary>
        private string[] _templateNames;

        /// <summary>
        /// file encodeing 
        /// </summary>
        private string _encoding;

        /// <summary>
        /// is omit table prefix
        /// </summary>
        private bool _isOmitTablePrefix;

        /// <summary>
        /// Came case field name
        /// </summary>
        private bool _isCamelCaseName;

        /// <summary>
        /// struck function
        /// </summary>
        public GenerationSettings() { }

        /// <summary>
        /// struck function
        /// </summary>
        /// <param name="language"></param>
        /// <param name="templateEngine"></param>
        public GenerationSettings(string language, string templateEngine)
        {
            _language = language;
            _templateEngine = templateEngine;
        }

        /// <summary>
        /// struck function
        /// </summary>
        /// <param name="language"></param>
        /// <param name="templateEngine"></param>
        /// <param name="package"></param>
        /// <param name="tablePrefix"></param>
        /// <param name="author"></param>
        /// <param name="version"></param>
        /// <param name="templateNames"></param>
        /// <param name="encoding"></param>
        /// <param name="isOmitTablePrefix"></param>
        /// <param name="isCamelCaseName"></param>
        public GenerationSettings(string language, string templateEngine, string package,
            string tablePrefix, string author, string version, string[] templateNames, string encoding,
            bool isOmitTablePrefix, bool isCamelCaseName)
        {
            _language = language;
            _templateEngine = templateEngine;
            _package = package;
            _tablePrefix = tablePrefix;
            _author = author;
            _version = version;
            _templateNames = templateNames;
            _encoding = encoding;
            _isOmitTablePrefix = isOmitTablePrefix;
            _isCamelCaseName = isCamelCaseName;
        }

        /// <summary>
        /// Language
        /// </summary>
        [XmlElement("Language")]
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        /// <summary>
        /// Package
        /// </summary>
        [XmlElement("Package")]
        public string Package
        {
            get { return _package ?? string.Empty; }
            set { _package = value; }
        }

        /// <summary>
        /// TablePrefix
        /// </summary>
        [XmlElement("TablePrefix")]
        public string TablePrefix
        {
            get { return _tablePrefix ?? string.Empty; }
            set { _tablePrefix = value; }
        }

        /// <summary>
        /// Author
        /// </summary>
        [XmlElement("Author")]
        public string Author
        {
            get { return _author ?? string.Empty; }
            set { _author = value; }
        }

        /// <summary>
        /// Version
        /// </summary>
        [XmlElement("Version")]
        public string Version
        {
            get { return _version ?? string.Empty; }
            set { _version = value; }
        }

        /// <summary>
        /// TemplateEngine
        /// </summary>
        [XmlElement("TemplateEngine")]
        public string TemplateEngine
        {
            get { return _templateEngine; }
            set { _templateEngine = value; }
        }

        /// <summary>
        /// TemplatesNames
        /// </summary>
        [XmlArray("TemplatesNames"),
        XmlArrayItem("TemplatesName")]
        public string[] TemplatesNames
        {
            get { return _templateNames; }
            set { _templateNames = value; }
        }

        /// <summary>
        /// Encoding
        /// </summary>
        [XmlElement("Encoding")]
        public string Encoding
        {
            get { return _encoding ?? "UTF-8"; }
            set { _encoding = value; }
        }

        /// <summary>
        /// IsOmitTablePrefix
        /// </summary>
        [XmlElement("IsOmitTablePrefix")]
        public bool IsOmitTablePrefix
        {
            get { return _isOmitTablePrefix; }
            set { _isOmitTablePrefix = value; }
        }

        /// <summary>
        /// IsCamelCaseName
        /// </summary>
        [XmlElement("IsCamelCaseName")]
        public bool IsCamelCaseName
        {
            get { return _isCamelCaseName; }
            set { _isCamelCaseName = value; }
        }
    }
}
