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

        public Post GetPostById(Guid postId)
        {
            var post = _posts.SingleOrDefault(a => a.Id == postId);

            return post;
        }

        public List<Post> GetPosts()
        {
            return _posts;
        }
    }
}
