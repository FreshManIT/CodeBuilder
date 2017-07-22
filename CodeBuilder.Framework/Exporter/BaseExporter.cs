using CodeBuilder.PhysicalDataModel;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.DataSource.Exporter
{
    public abstract class BaseExporter : IExporter
    {
        public abstract Model Export(string connectionString);
    }
}
