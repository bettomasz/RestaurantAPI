using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using System.Collections.Generic;
using Xunit;

namespace RestaurantApi.MyIntegrationTests.Validator
{
    public class RegisterUserDtoValidatorTests
    {
        private RestaurantDbContext _dbContext;

        public RegisterUserDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<RestaurantDbContext>();
            builder.UseInMemoryDatabase("TestDb");

            _dbContext = new RestaurantDbContext(builder.Options);

            Seed();
        }

        private void Seed()
        { 
            var testUsers = new List<User>()
            { 
               new User { Email="test2@test.pl"} ,
               new User { Email="test3@test.pl"}
            };   

            _dbContext.Users.AddRange(testUsers);
            _dbContext.SaveChanges();
        
        }

        [Fact]
        public void Validate_ForValidModel_ReturnsSoccess()
        {
            
  
            //arrange

            var model = new RegisterUserDto()
            { Email = "test@test.pl", Password = "123456", ConfirmPassword = "123456" };

            var validator = new RegisterUserDtoValidator(_dbContext);

            //act

             var result = validator.TestValidate(model);

            //assert

            result.ShouldNotHaveAnyValidationErrors();
        }
        [Fact]
        public void Validate_ForvInalidModel_ReturnsFailiure()
        {

            //arrange

            var model = new RegisterUserDto()
            { Email = "test2@test.pl", Password = "123456", ConfirmPassword = "123456" };


            var validator = new RegisterUserDtoValidator(_dbContext);


            //act

            var result = validator.TestValidate(model);

            //assert

            result.ShouldHaveAnyValidationError();
        }

        [Theory]
        [InlineData("test@test.pl", "123456", "123456")]
        [InlineData("test4@test.pl", "123456", "123456")]
        public void TheoryValidate_ForValidModel_ReturnsSoccess(string email, string password, string confirmPassword)
        {


            //arrange

            var model = new RegisterUserDto()
            { Email = email, Password = password, ConfirmPassword = confirmPassword };


            var validator = new RegisterUserDtoValidator(_dbContext);


            //act

            var result = validator.TestValidate(model);

            //assert

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
