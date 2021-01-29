using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Contracts.V1.Responses
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Erros { get; set; }
    }
}
