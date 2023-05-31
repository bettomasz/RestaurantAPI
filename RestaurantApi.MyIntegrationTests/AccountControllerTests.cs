using Microsoft.AspNetCore.Mvc.Testing;
using RestaurantAPI;
using Xunit;
using FluentAssertions;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization.Policy;
using RestaurantApi.MyIntegrationTests.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using RestaurantAPI.Services;

namespace RestaurantApi.MyIntegrationTests
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {

        private HttpClient _client;
        private Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();

        private WebApplicationFactory<Startup> _factory;
        public AccountControllerTests(WebApplicationFactory<Startup> factory)
        {

            _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
                    services.Remove(dbContextOptions);
                    services.AddSingleton<IAccountService>(_accountServiceMock.Object);
                    services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                }
                );

            }
            ).CreateClient();

        }

        [Fact]
        public async Task RegisterUser_ForValidModel_ReturnsOK()
        {
            // arrange

            var registerUser = new RegisterUserDto()
            { 
              Email ="www3243423424@wp.pl", Password="12222111", ConfirmPassword= "12222111"

            };

           var httpContent = registerUser.ToJsonHttpContent();

            // act
            var response = await  _client.PostAsync("/api/account/register", httpContent);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        }

        [Fact]
        public async Task RegisterUser_ForInValidModel_ReturnsBadRequest()
        {
            // arrange

            var registerUser = new RegisterUserDto()
            {
                Email = "www3243423424@wp.pl",
                Password = "122221111",
                ConfirmPassword = "1222211112"

            };

            var httpContent = registerUser.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/account/register", httpContent);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
           
        }

        [Fact]
        public async Task Login_ForRegisteredUser_ReturnsOK()
        {

            //arrrange

            _accountServiceMock
                .Setup(e => e.GenerateJwt(It.IsAny<LoginDto>()))
                .Returns("jwt");



            var loginDto = new LoginDto() { Email = "test@wp.pl", Password = "12121212122" }  ;
        
            var httpContent = loginDto.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync("/api/account/login", httpContent);

            //assert
            //
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }


    }
}
