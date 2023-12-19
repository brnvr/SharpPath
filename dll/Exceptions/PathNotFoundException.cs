using System;

namespace SharpPath.Exceptions
{
    public class PathNotFoundException : Exception
    {
        const string DEFAULT_MESSAGE = "Path can't be found.";
        public PathNotFoundException(string message = DEFAULT_MESSAGE, Exception innerException = null) : base(message, innerException) { }
    }
}
