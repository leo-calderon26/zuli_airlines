using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
namespace zuli_Data.Exceptions
{
    // Esta clase esta hecha para que la use el middleware
    public abstract class AppExeptions : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }
        public int ErrorId { get; }

        protected AppExeptions(string message, int statusCode, string errorCode, int errorId) : base(message) {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            ErrorId = errorId;
        }
    }

    public class ZuliNotFoundException : AppExeptions
    {
        public ZuliNotFoundException(string msg, int errorId = -1) 
        : base(msg, StatusCodes.Status404NotFound,"NOT_FOUND", errorId) { }
    }

    public class ZuliValidationException : AppExeptions
    {
        public IReadOnlyDictionary<string, List<string>> Errors { get; }

        public ZuliValidationException(Dictionary<string, List<string>> errors, int errorId = -1)
            : base("Validation failed.", StatusCodes.Status422UnprocessableEntity, "VALIDATION_ERROR", errorId)
            => Errors = errors;
        public ZuliValidationException(string errorKey, List<string> errorValue, int errorId = -1)
        : base("Validation failed.", StatusCodes.Status422UnprocessableEntity, "VALIDATION_ERROR", errorId)
            => Errors = new Dictionary<string, List<string>> { { errorKey, errorValue } };

        public ZuliValidationException(string errorKey, string errorValue, int errorId = -1)
            : base("Validation failed.", StatusCodes.Status422UnprocessableEntity, "VALIDATION_ERROR", errorId)
            => Errors = new Dictionary<string, List<string>> { { errorKey, new List<string> { errorValue } } };
    }

    public class ZuliUnauthorizedException : AppExeptions
    {
        public ZuliUnauthorizedException(string msg, int errorId = -1)
       : base(msg, StatusCodes.Status401Unauthorized, "FORBIDDEN", errorId) { }
    }
}
