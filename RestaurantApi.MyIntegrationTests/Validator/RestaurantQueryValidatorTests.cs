using FluentValidation.TestHelper;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RestaurantApi.MyIntegrationTests.Validator
{
    public class RestaurantQueryValidatorTests
    {
        public static IEnumerable<object[]> GetSampleValidData()
        {

            var list = new List<RestaurantQuery>() 
            {

                new RestaurantQuery()
                {
                     PageNumber=1,
                      PageSize=10,
                       SortBy = nameof(Restaurant.Name)

                },
                 new RestaurantQuery()
                  {
                      PageNumber = 1,
                      PageSize = 5,
                      SortBy = nameof(Restaurant.Category)

                  },
                  new RestaurantQuery()
                  {
                        PageNumber = 1,
                        PageSize = 15,


                  }
              };

            return list.Select(q => new object[] {q});
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {

            var list = new List<RestaurantQuery>()
            {

                new RestaurantQuery()
                {
                     PageNumber=0,
                      PageSize=10,
                       SortBy = nameof(Restaurant.Name)

                },
                 new RestaurantQuery()
                  {
                      PageNumber = 1,
                      PageSize = 115,
                      SortBy = nameof(Restaurant.Category)

                  },
                  new RestaurantQuery()
                  {
                        PageNumber = 1,
                        PageSize = 15,
                        SortBy = nameof(Restaurant.ContactNumber)

                  }
              };

            return list.Select(q => new object[] { q });
        }


        [Theory]
        [MemberData(nameof(GetSampleValidData))]

        public void Validate_ForCorrrectModel_ReturnsOK(RestaurantQuery model)
        {
            //arrange
            var validator = new RestaurantQueryValidator();

            //act
          var result =  validator.TestValidate(model);

            //assert

            result.ShouldNotHaveAnyValidationErrors();

        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]

        public void Validate_ForIncorrrectModel_ReturnsFailure(RestaurantQuery model)
        {
            //arrange
            var validator = new RestaurantQueryValidator();

            //act
            var result = validator.TestValidate(model);

            //assert

            result.ShouldHaveAnyValidationError();

        }
    }
}
