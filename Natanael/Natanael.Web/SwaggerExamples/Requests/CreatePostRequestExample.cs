using Natanael.Contracts.V1.Requests;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.SwaggerExamples.Requests
{
    public class CreatePostRequestExample : IExamplesProvider<CreatePostRequest>
    {
        public CreatePostRequest GetExamples()
        {
            return new CreatePostRequest
            {
                Name = "insira o nome"

            };
        }
    }
}
