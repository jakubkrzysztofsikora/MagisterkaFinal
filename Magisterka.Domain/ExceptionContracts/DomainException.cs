using System;

namespace Magisterka.Domain.ExceptionContracts
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
            
        }
    }
}
