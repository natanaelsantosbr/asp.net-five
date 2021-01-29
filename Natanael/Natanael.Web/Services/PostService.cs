using Natanael.Web.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Services
{
    public class PostService : IPostService
    {
        private List<Post> _posts;

        public PostService()
        {
            _posts = new List<Post>();
            for (int i = 0; i < 5; i++)
            {
                _posts.Add(new Post
                {
                    Id = Guid.NewGuid(),
                    Name = $"Post name {i}"
                });
            }
        }

        public bool DeletePost(Guid postId)
        {
            var post = this.GetPostById(postId);

            if (post == null)
                return false;

            _posts.Remove(post);

            return true;

        }

        public Post GetPostById(Guid postId)
        {
            var post = _posts.SingleOrDefault(a => a.Id == postId);

            return post;
        }

        public List<Post> GetPosts()
        {
            return _posts;
        }

        public bool UpdatePost(Post postToUpdate)
        {
            var exists = this.GetPostById(postToUpdate.Id) != null;

            if (!exists)
                return false;

            var index = _posts.FindIndex(a => a.Id == postToUpdate.Id);
            _posts[index] = postToUpdate;

            return true;
        }
    }
}
