using System.Net;
using System.Net.Http.Json;
using VacationRental.Domain.Models.Rentals;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PostRentalTests
    {
        private readonly HttpClient _client;

        public PostRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
        {
            var request = new RentalRequest
            {
                Units = 25,
                PreparationTimeInDays = 2
            };

            CreateRentalResponse? postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadFromJsonAsync<CreateRentalResponse>();

                Assert.NotNull(postResult);
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult!.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadFromJsonAsync<GetRentalResponse>();

                Assert.NotNull(getResult);
                Assert.Equal(request.Units, getResult!.Units);
                Assert.Equal(request.PreparationTimeInDays, getResult.PreparationTimeInDays);
            }
        }
    }
}
