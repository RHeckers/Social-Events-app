using System;
using System.Net;

namespace Application.Errors
{
    public class RestException : Exception
    {
        private readonly HttpStatusCode _code;
        private readonly object _errors;
        public RestException(HttpStatusCode code, object errors = null)
        {
            _errors = errors;
            _code = code;
        }

        public HttpStatusCode Code { get; }
        public object Errors { get; }
    }
}