using Natanael.Contracts.V1.Requests.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Services
{
    public interface IUriService 
    {
        Uri GetPostUri(string postId);

        Uri GetAllPostsUri(PaginationQuery pagination = null); 

    }
}
