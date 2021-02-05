using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Contracts.V1.Requests
{
    public class CreatePostRequest
    {
        public string Name { get; set; }
    }
}
