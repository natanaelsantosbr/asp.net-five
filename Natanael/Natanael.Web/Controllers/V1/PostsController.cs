using Microsoft.AspNetCore.Mvc;
using Natanael.Web.Contracts.V1;
using Natanael.Web.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Controllers.V1
{
    public class PostsController : Controller
    {
        private List<Post> _posts;

        public PostsController()
        {
            _posts = new List<Post>();
            for (int i = 0; i < 5; i++)
            {
                _posts.Add(new Post { Id = Guid.NewGuid().ToString() });
            }
        }

        [Route(ApiRoutes.Posts.GetAll)]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_posts);
        }
    }
}
