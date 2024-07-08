using System;

namespace KVA.Cinema.Exceptions
{
    internal class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message)
        {

        }
    }
}
