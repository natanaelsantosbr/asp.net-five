﻿using Natanael.Web.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Services
{
    public interface IPostService
    {
        List<Post> GetPosts();

        Post GetPostById(Guid postId);

        bool UpdatePost(Post postToUpdate);

        bool DeletePost(Guid postId);
    }
}
