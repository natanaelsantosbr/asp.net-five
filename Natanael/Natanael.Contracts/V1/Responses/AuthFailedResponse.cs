using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Contracts.V1.Responses
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Erros { get; set; }
    }
}
