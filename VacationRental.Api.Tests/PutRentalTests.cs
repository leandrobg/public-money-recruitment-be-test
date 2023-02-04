using System.Net;
using System.Net.Http.Json;
using VacationRental.Domain.Models.Bookings;
using VacationRental.Domain.Models.Rentals;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class PutRentalTests
    {
        private readonly HttpClient _client;

        public PutRentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAGetReturnsTheUpdatedRental()
        {
            var postRentalRequest = new RentalRequest
            {
                Units = 2,
                PreparationTimeInDays = 1
            };

            CreateRentalResponse? postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadFromJsonAsync<CreateRentalResponse>();

                Assert.NotNull(postRentalResult);
            }

            var postBooking1Request = new CreateBookingRequest
            {
                RentalId = postRentalResult!.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };

            CreateRentalResponse? postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadFromJsonAsync<CreateRentalResponse>();

                Assert.NotNull(postBooking1Result);
            }

            var postBooking2Request = new CreateBookingRequest
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 06)
            };

            CreateRentalResponse? postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking2Result = await postBooking2Response.Content.ReadFromJsonAsync<CreateRentalResponse>();

                Assert.NotNull(postBooking2Result);
            }

            var putRentalRequest = new UpdateRentalRequest
            {
                Units = 1,
                PreparationTimeInDays = 2
            };

            CreateRentalResponse? putRentalResult;
            using (var putRentalResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult!.Id}", putRentalRequest))
            {
                Assert.True(putRentalResponse.IsSuccessStatusCode);
                putRentalResult = await putRentalResponse.Content.ReadFromJsonAsync<CreateRentalResponse>();

                Assert.NotNull(putRentalResult);
            }

            using (var getRentalResponse = await _client.GetAsync($"/api/v1/rentals/{postRentalResult!.Id}"))
            {
                Assert.True(getRentalResponse.IsSuccessStatusCode);

                var getRentalResult = await getRentalResponse.Content.ReadFromJsonAsync<GetRentalResponse>();

                Assert.NotNull(getRentalResult);
                Assert.Equal(putRentalRequest.Units, getRentalResult!.Units);
                Assert.Equal(putRentalRequest.PreparationTimeInDays, getRentalResult.PreparationTimeInDays);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPutRental_ThenAPutReturnsErrorWhenThereIsOverlappingBookings()
        {
            var postRentalRequest = new RentalRequest
            {
                Units = 2,
                PreparationTimeInDays = 1
            };

            CreateRentalResponse? postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadFromJsonAsync<CreateRentalResponse>();

                Assert.NotNull(postRentalResult);
            }

            var postBooking1Request = new CreateBookingRequest
            {
                RentalId = postRentalResult!.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 02)
            };

            CreateRentalResponse? postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadFromJsonAsync<CreateRentalResponse>();

                Assert.NotNull(postBooking1Result);
            }

            var postBooking2Request = new CreateBookingRequest
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 05)
            };

            CreateRentalResponse? postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking2Result = await postBooking2Response.Content.ReadFromJsonAsync<CreateRentalResponse>();

                Assert.NotNull(postBooking2Result);
            }

            var putRentalRequest = new RentalRequest
            {
                Units = 1,
                PreparationTimeInDays = 2
            };

            using (var putRentalResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult!.Id}", putRentalRequest))
            {
                Assert.Equal(HttpStatusCode.InternalServerError, putRentalResponse.StatusCode);
            }
        }
    }
}
