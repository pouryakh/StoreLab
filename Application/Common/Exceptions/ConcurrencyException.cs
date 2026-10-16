using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(
        string message,
        Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
