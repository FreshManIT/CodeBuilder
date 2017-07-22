using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.PhysicalDataModel
{
    public class Tables : Dictionary<string, Table>
    {
        public Tables()
        {
        }

        public Tables(int capacity)
            : base(capacity)
        {
        }
    }
}
