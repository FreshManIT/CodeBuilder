using CodeBuilder.PhysicalDataModel;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.DataSource.Exporter
{
    public interface IExporter
    {
        Model Export(string connectionString);
    }
}
