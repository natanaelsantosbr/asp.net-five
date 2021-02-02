using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Natanael.Web;
using Natanael.Web.Contracts.V1;
using Natanael.Web.Contracts.V1.Requests;
using Natanael.Web.Contracts.V1.Responses;
using Natanael.Web.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Natanael.IntegrationTests
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;
        public IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });
                    });
                });

            this._serviceProvider = appFactory.Services;
            this._client = appFactory.CreateClient();

        }

        public void Dispose()
        {
            using var serviceScope = this._serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context.Database.EnsureDeleted();

        }

        protected async Task AuthenticateAsync()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }


        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
        {
            var response = await this._client.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
            return await response.Content.ReadAsAsync<PostResponse>();


        }


        private async Task<string> GetJwtAsync()
        {
            var response = await this._client.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "natanaelsantosbr@gmail.com",
                Password = "Natanael@1030!"
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            return registrationResponse.Token;

            
        }
    }
}
