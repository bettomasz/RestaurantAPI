using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantApi.MyIntegrationTests.Helpers;
using RestaurantAPI;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantApi.MyIntegrationTests
{
    public class DishControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {

        private HttpClient _client;

        private WebApplicationFactory<Startup> _factory;
        public DishControllerTests(WebApplicationFactory<Startup> factory)
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
        [InlineData("1")]
        public async Task GetAllDishes_ForExisttingRestaurant_ReturnsOK( string restaurantId)
        {

            //act
           var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish");


            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        

        }
        [Theory]
        [InlineData("4")]
        public async Task GetAllDishes_ForNonExisttingRestaurant_ReturnsNotFound(string restaurantId)
        {

            //act
            var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish");


            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);


        }

        [Theory]
        [InlineData("1","1")]
        public async Task GetDishId_ForExisttingDishForRestaurant_ReturnsOK(string restaurantId, string dishId)
        {

            //act
            var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);


        }

        [Theory]
        [InlineData("1", "10")]
        [InlineData("2", "1")]
        [InlineData("10", "1")]
        public async Task GetDishId_ForNonExistingDishInExistingRestaurantOrNonExistingRestaurant_ReturnsNotFound(string restaurantId, string dishId)
        {

            //act
            var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");


            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        [Theory]
        [InlineData("1")]

        public async Task CreateDish_ForExistingRestaurant_ReturnsOK(string restaurantId)
        {
            //arrange
            var dishModel = new CreateDishDto() { Name = "Testowa" };

            var httpContent = dishModel.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync($"/api/restaurant/{restaurantId}/dish", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Theory]
        [InlineData("10")]

        public async Task CreateDish_ForNonExistingRestaurant_ReturnsNotFound(string restaurantId)
        {
            //arrange
            var dishModel = new CreateDishDto() { Name = "Testowa" };

            var httpContent = dishModel.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync($"/api/restaurant/{restaurantId}/dish", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("1")]
        public async Task CreateDish_WithInvalidModel_ReturnsBadRequest(string restaurantId)
        {
            //arrange
            var dishModel = new CreateDishDto() { Description = "Testowa" };

            var httpContent = dishModel.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync($"/api/restaurant/{restaurantId}/dish", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        }

        
        [Theory]
        [InlineData("1")]
        public async Task RemoveAllDishes_ForExistingRestaurant_ReturnsNoContent(string restaurantId)
        {


             //act
             var response = await _client.DeleteAsync($"/api/restaurant/{restaurantId}/dish/");
            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Theory]
        [InlineData("19")]
        public async Task RemoveAllDishes_ForNonExistingRestaurant_ReturnsNotFound(string restaurantId)
        {
        

            //act
            var response = await _client.DeleteAsync($"/api/restaurant/{restaurantId}/dish/");
            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

    }
}

