using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.PhysicalDataModel
{
    public class Columns : Dictionary<string, Column>
    {
        public Columns()
        {
        }

        public Columns(int capacity)
            : base(capacity)
        {
        }
    }
}
