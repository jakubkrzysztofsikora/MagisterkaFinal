using System;

namespace Magisterka.Domain.ExceptionContracts
{
    public class DomainException : Exception
    {
        public eErrorTypes ErrorType { get; set; }
        public DomainException(string message) : base(message)
        {
            
        }
    }
}
