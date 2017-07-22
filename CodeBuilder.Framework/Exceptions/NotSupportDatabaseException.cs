using System;
using CodeBuilder.Framework.Properties;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.Exceptions
{
    public class NotSupportDatabaseException : Exception
    {
        public NotSupportDatabaseException()
            : this(Resource.NotSupportDatabaseExceptionMessage)
        {
        }

        public NotSupportDatabaseException(string message)
            : base(message)
        {
        }
    }
}
