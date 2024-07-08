using System;
using System.Collections;

namespace KVA.Cinema.Exceptions
{
    internal class FailedToCreateEntityException : Exception
    {
        public IEnumerable Errors { get; set; }

        public FailedToCreateEntityException(IEnumerable errors)
        {
            Errors = errors;
        }
    }
}
