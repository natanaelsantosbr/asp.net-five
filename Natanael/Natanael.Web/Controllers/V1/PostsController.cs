using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Natanael.Web.Contracts.V1;
using Natanael.Web.Contracts.V1.Requests;
using Natanael.Web.Contracts.V1.Responses;
using Natanael.Web.Domain;
using Natanael.Web.Extensions;
using Natanael.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Controllers.V1
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await this._postService.GetPostsAsync());
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            var userOwnsPost = await this._postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if(!userOwnsPost)
                return BadRequest(new { error = "You do not own this post" });

            var post = await  this._postService.GetPostByIdAsync(postId);
            post.Name = request.Name;


            var updated = await this._postService.UpdatePostAsync(post);

            if (updated)
                return Ok(post);

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await this._postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
                return BadRequest(new { error = "You do not own this post" });

            var deleted = await this._postService.DeletePostAsync(postId);

            if (deleted)
                return NoContent();

            return NotFound();
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await this._postService.GetPostByIdAsync(postId);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpGet(ApiRoutes.Posts.GetByName)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get([FromRoute] string name)
        {
            var post = await this._postService.GetPostByNameAsync(name);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post 
            { 
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId()
            };
            var id = Guid.Empty;

            await this._postService.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var response = new PostResponse { Id = post.Id };

            return Created(locationUri, response);
        }


    }
}
