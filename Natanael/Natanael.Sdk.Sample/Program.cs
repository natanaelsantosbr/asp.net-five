using Natanael.Contracts.V1.Requests;
using Refit;
using System;
using System.Threading.Tasks;

namespace Natanael.Sdk.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cachedToken = string.Empty;

            var identityApi = RestService.For<IIdentityApi>("https://localhost:5001");
            var natanaelApi = RestService.For<INatanaelApi>("https://localhost:5001", new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)

            });

            var registerResponse = await identityApi.RegisterAsync(new UserRegistrationRequest
            {
                Email = "nat2an2s2@gmail.com",
                Password = "Teste@123"
            });

            var loginResponse = await identityApi.LoginAsync(new UserLoginRequest
            {
                Email = "natan2s2@gmail.com",
                Password = "Teste@123"
            });

            cachedToken = loginResponse.Content.Token;

            var allPosts = await natanaelApi.GetAllAsync();

            var createPost = await natanaelApi.CreateAsync(new CreatePostRequest
            {
                Name = "Natanael utilizando SDK"
            });

            var retrievedPost = await natanaelApi.GetAsync(createPost.Content.Id);

            var updatePosts = await natanaelApi.UpdateAsync(createPost.Content.Id, new UpdatePostRequest
            {
                Name = "Natanael update utilizando SDK"
            });

            var deletePost = await natanaelApi.DeleteAsync(createPost.Content.Id);











        }
    }
}
