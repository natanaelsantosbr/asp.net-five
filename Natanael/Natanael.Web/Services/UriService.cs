using Microsoft.AspNetCore.WebUtilities;
using Natanael.Contracts.V1;
using Natanael.Contracts.V1.Requests.Queries;
using System;

namespace Natanael.Web.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            this._baseUri = baseUri;
        }

        public Uri GetPostUri(string postId)
        {
            return new Uri(this._baseUri + ApiRoutes.Posts.Get.Replace("{postId}", postId));
        }

        public Uri GetAllPostsUri(PaginationQuery pagination = null)
        {
            var uri = new Uri(this._baseUri);

            if (pagination == null)
                return uri;

            var modifiedUri = QueryHelpers.AddQueryString(this._baseUri, "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(this._baseUri, "pageSize", pagination.PageSize.ToString());

            return new Uri(modifiedUri);
        }


    }
}