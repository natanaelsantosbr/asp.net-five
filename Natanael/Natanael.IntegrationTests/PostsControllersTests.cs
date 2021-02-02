using FluentAssertions;
using Natanael.Web.Contracts.V1;
using Natanael.Web.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Natanael.IntegrationTests
{
    public class PostsControllersTests : IntegrationTest
    {
        public PostsControllersTests()
        {

        }
        [Fact]
        public async Task GetAll_WithoutEmptyAnyPosts_ReturnEmptyResponse()
        {
            //Arrange
            await this.AuthenticateAsync();

            //Act
            var response = await this._client.GetAsync(ApiRoutes.Posts.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<List<Post>>()).Should().BeEmpty();

        }

        [Fact]
        public async Task Get_ReturnsPosts_WhenPostExistsInTheDatabase()
        {
            //Arrange
            await this.AuthenticateAsync();
            var createdPost = await this.CreatePostAsync(new Web.Contracts.V1.Requests.CreatePostRequest { Name = "Post Tests" });

            //Act
            var response = await this._client.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnPost = await response.Content.ReadAsAsync<Post>();
            returnPost.Id.Should().Be(createdPost.Id);
            returnPost.Name.Should().Be("Post Tests");
            
        }

    }
}
