using System;
using System.Collections.Generic;
using System.Text;

namespace Natanael.Contracts.V1.Responses
{
    public class Response<T>
    {
        public Response() { }

        public Response(T response)
        {
            this.Data = response;
        }

        public T Data { get; set; }

    }
}
