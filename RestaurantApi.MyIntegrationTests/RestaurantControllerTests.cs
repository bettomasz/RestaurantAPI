using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using RestaurantAPI;
using FluentAssertions;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization.Policy;
using RestaurantApi.MyIntegrationTests.Helpers;

namespace RestaurantApi.MyIntegrationTests
{
    public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;

         private WebApplicationFactory<Startup> _factory;
       public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
        {

            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
                        services.Remove(dbContextOptions);
                        services.AddSingleton<IPolicyEvaluator, FakePolisyEvaluator>();
                        services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));
                        services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                    }
                    );

                }
                );
               _client = _factory.CreateClient();

        }

        [Theory]
        [InlineData("pageSize=5&pageNumber=1")]
        [InlineData("pageSize=15&pageNumber=1")]   
        public async Task GetAll_WithQueryParameters_ReturnsOkResult(string queryParams)
        {
            //arrange



            //act
         var response = await  _client.GetAsync("/api/restaurant?" + queryParams);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
   
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("pageSize=115&pageNumber=1")]
        public async Task GetAll_WithInvalidQueryParameters_ReturnsBadRequest(string queryParams)
        {
            //arrange


            //act

            var response = await _client.GetAsync("/api/restaurant?" + queryParams);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateRestaurant_WithValidModel_ReturnsCreatedStatus()
        { 
            //arrange
            var model = new CreateRestaurantDto() { Name= "test restaurant", City="Warszawa", Street ="LEśna" };

            var httpContent = model.ToJsonHttpContent();

            //act
          var response = await  _client.PostAsync("/api/restaurant", httpContent);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

        }

        [Fact]
        public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
        {
            //arrange
            var model = new CreateRestaurantDto() { Description = "test restaurant", City = "Warszawa", Street = "LEśna" };

            var httpContent = model.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync("/api/restaurant", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);


        }

        [Fact]
        public async Task Delete_ForNonExiststingRestaurant_ReturnsNotFound()
        {

            //act
            var response = await _client.DeleteAsync("/api/restaurant/123");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        private void SeedRestaurant (Restaurant restaurant)
        {

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

        }



        [Fact]
        public async Task Delete_ForOwnerRestaurant_ReturnsNoContent()
        {
            //arrange
            var restaurant = new Restaurant() { CreatedById =1, Name="Test" };

            //seed
            SeedRestaurant(restaurant);

            //act
            var response = await _client.DeleteAsync("/api/restaurant/" + restaurant.Id);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        }
        [Fact]
        public async Task Delete_ForNonOwnerRestaurant_ReturnsForbidden()
        {
            //arrange
            var restaurant = new Restaurant() { CreatedById = 1003, Name = "Test" };

            //seed
            SeedRestaurant(restaurant);

            //act
            var response = await _client.DeleteAsync("/api/restaurant/" + restaurant.Id);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);

        }

    }
}
