using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Results
{
    public class SimpleResult : IResult
    {
        public SimpleResult(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public string? Error { get; }


        public static SimpleResult Success()
        {
            return new SimpleResult(true, null);
        }

        public static SimpleResult Failure(string error)
        {
            return new SimpleResult(false, error);
        }
    }
}
