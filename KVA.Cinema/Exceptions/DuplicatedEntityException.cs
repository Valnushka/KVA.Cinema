using System;

namespace KVA.Cinema.Exceptions
{
    internal class DuplicatedEntityException : Exception
    {
        public DuplicatedEntityException(string message) : base(message)
        {

        }
    }
}
