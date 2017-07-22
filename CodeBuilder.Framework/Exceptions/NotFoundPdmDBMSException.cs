using System;
using CodeBuilder.Framework.Properties;

namespace CodeBuilder.Exceptions
{
    public class NotFoundPdmDBMSException : Exception
    {
        public NotFoundPdmDBMSException()
            : this(Resource.NotFoundPdmDBMSExceptionMessage)
        {
        }

        public NotFoundPdmDBMSException(string message)
            : base(message)
        {
        }
    }
}
