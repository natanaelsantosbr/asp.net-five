using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Domain
{
    public class AuthenticationResult
    {
        public string Token { get; set; }

        public bool Sucess { get; set; }

        public IEnumerable<string> Erros { get; set; }

    }
}
