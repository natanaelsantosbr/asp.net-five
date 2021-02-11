using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Natanael.Contracts.V1;
using Natanael.Contracts.V1.Requests;
using Natanael.Contracts.V1.Requests.Queries;
using Natanael.Contracts.V1.Responses;
using Natanael.Web.Cache;
using Natanael.Web.Domain;
using Natanael.Web.Extensions;
using Natanael.Web.Helpers;
using Natanael.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Controllers.V1
{
    [Authorize]
    [Produces("application/json")]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public PostsController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            this._postService = postService;
            this._mapper = mapper;
            this._uriService = uriService;
        }

        /// <summary>
        /// Retorna todos os posts 
        /// </summary>
        /// <response code="200">Retorna todos os posts </response>
        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery paginationQuery)
        {
            var pagination = this._mapper.Map<PaginationFilter>(paginationQuery);

            var posts = await this._postService.GetPostsAsync(pagination);
            var postsResponses = this._mapper.Map<List<PostResponse>>(posts);

            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<PostResponse>(postsResponses));

            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(this._uriService, pagination, postsResponses);

            return Ok(paginationResponse);
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
                return Ok(new Response<PostResponse>(this._mapper.Map<PostResponse>(post)));

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

            return Ok(new Response<PostResponse>(this._mapper.Map<PostResponse>(post)));
        }

        [HttpGet(ApiRoutes.Posts.GetByName)]

        public async Task<IActionResult> Get([FromRoute] string name)
        {
            var post = await this._postService.GetPostByNameAsync(name);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        /// <summary>
        /// Cria um post no sistema
        /// </summary>
        /// <remarks>
        ///     Exemplo de uma  **requisição**:
        ///     
        ///         POST /api/v1/posts
        ///         {
        ///             "name": "Some name"
        ///         }
        /// </remarks>
        /// <response code="201">Cria um post no sistema</response>
        /// /// <response code="400">Unable to create the tag due to validation error</response>
        [HttpPost(ApiRoutes.Posts.Create)]
        [ProducesResponseType(typeof(PostResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            if(!ModelState.IsValid)
            {

            }

            var post = new Post 
            { 
                Name = postRequest.Name,
                UserId = HttpContext.GetUserId()
            };

            var created = await this._postService.CreatePostAsync(post);

            if(!created)
            {
                return BadRequest(

                    new ErrorResponse
                    {
                        Erros = new List<ErrorModel>
                        {
                            new ErrorModel
                            {
                                Message = "Unable to create post"
                            }
                        }
                    });
            }

            var locationUri = this._uriService.GetPostUri(post.Id.ToString());


            return Created(locationUri, new Response<PostResponse>(this._mapper.Map<PostResponse>(post)));
        }


    }
}
