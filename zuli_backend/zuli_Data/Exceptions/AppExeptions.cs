using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace zuli_Data.Exceptions
{
    // Esta clase esta hecha para que la use el middleware
    public abstract class AppExeptions : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        protected AppExeptions(string message, int statusCode, string errorCode) : base(message) {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }

    public class NotFoundException : AppExeptions
    {
        public NotFoundException(string msg) 
        : base(msg, StatusCodes.Status404NotFound,"NOT_FOUND") { }
    }
}
