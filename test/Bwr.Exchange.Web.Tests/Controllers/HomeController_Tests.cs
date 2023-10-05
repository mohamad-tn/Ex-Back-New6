using System.Threading.Tasks;
using Bwr.Exchange.Models.TokenAuth;
using Bwr.Exchange.Web.Controllers;
using Shouldly;
using Xunit;

namespace Bwr.Exchange.Web.Tests.Controllers
{
    public class HomeController_Tests: ExchangeWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}