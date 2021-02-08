using Natanael.Contracts.V1.Responses;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.SwaggerExamples.Responses
{
    public class PostResponseExample : IExamplesProvider<PostResponse>
    {
        public PostResponse GetExamples()
        {
            return new PostResponse
            {
                Name = "novo nome"
            };
        }
    }
}
