using Microsoft.EntityFrameworkCore;
using Natanael.Web.Data;
using Natanael.Web.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Services
{
    public class PostService : IPostService
    { 

        private readonly DataContext _dataContext;

        public PostService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            this._dataContext.Posts.Add(post);
            var created = await this._dataContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await this._dataContext.Posts.ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            var post = await this._dataContext.Posts.SingleOrDefaultAsync(a => a.Id == postId);

            return post;
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            this._dataContext.Posts.Update(postToUpdate);
            var updated = await this._dataContext.SaveChangesAsync();

            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await this.GetPostByIdAsync(postId);

            if (post == null)
                return false;

            this._dataContext.Posts.Remove(post);
            var deleted = await this._dataContext.SaveChangesAsync();

            return deleted > 0;

        }

        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await this._dataContext.Posts.SingleOrDefaultAsync(a => a.Id == postId);

            if (post == null)
                return false;

            if (post.UserId != userId)
                return false;

            return true;
        }

        public async Task<Post> GetPostByNameAsync(string name)
        {
            var post = await this._dataContext.Posts.SingleOrDefaultAsync(a => a.Name == name);

            return post;
        }
    }
}
