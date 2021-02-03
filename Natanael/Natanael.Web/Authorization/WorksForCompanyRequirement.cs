using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Authorization
{
    public class WorksForCompanyRequirement : IAuthorizationRequirement
    {
        public string DomainMain { get; set; }

        public WorksForCompanyRequirement(string domainName)
        {
            this.DomainMain = domainName;
        }
    }
}
