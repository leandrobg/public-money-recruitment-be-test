using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using VacationRental.Domain.Models.Rentals;
using VacationRental.Domain.Models.Bookings;

namespace VacationRental.Api.Tests
{
    namespace VacationRental.Api.Tests
    {
        [Collection("Integration")]
        public class PostBookingTests
        {
            private readonly HttpClient _client;

            public PostBookingTests(IntegrationFixture fixture)
            {
                _client = fixture.Client;
            }

            [Fact]
            public async Task GivenCompleteRequest_WhenPostBooking_ThenAGetReturnsTheCreatedBooking()
            {
                var postRentalRequest = new RentalRequest
                {
                    Units = 4,
                    PreparationTimeInDays = 1
                };

                CreateRentalResponse? postRentalResult;
                using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
                {
                    Assert.True(postRentalResponse.IsSuccessStatusCode);
                    postRentalResult = await postRentalResponse.Content.ReadFromJsonAsync<CreateRentalResponse>();

                    Assert.NotNull(postRentalResult);
                }

                var postBookingRequest = new CreateBookingRequest
                {
                    RentalId = postRentalResult!.Id,
                    Nights = 3,
                    Start = new DateTime(2001, 01, 01)
                };

                CreateBookingResponse? postBookingResult;
                using (var postBookingResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
                {
                    Assert.True(postBookingResponse.IsSuccessStatusCode);
                    postBookingResult = await postBookingResponse.Content.ReadFromJsonAsync<CreateBookingResponse>();

                    Assert.NotNull(postRentalResult);
                }

                using (var getBookingResponse = await _client.GetAsync($"/api/v1/bookings/{postBookingResult!.Id}"))
                {
                    Assert.True(getBookingResponse.IsSuccessStatusCode);

                    var getBookingResult = await getBookingResponse.Content.ReadFromJsonAsync<GetBookingResponse>();

                    Assert.NotNull(getBookingResult);
                    Assert.Equal(postBookingRequest.RentalId, getBookingResult!.RentalId);
                    Assert.Equal(postBookingRequest.Nights, getBookingResult.Nights);
                    Assert.Equal(postBookingRequest.Start, getBookingResult.Start);
                }
            }

            [Fact]
            public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbooking()
            {
                var postRentalRequest = new RentalRequest
                {
                    Units = 1,
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
                    Nights = 3,
                    Start = new DateTime(2002, 01, 01)
                };

                using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
                {
                    Assert.True(postBooking1Response.IsSuccessStatusCode);
                }

                var postBooking2Request = new CreateBookingRequest
                {
                    RentalId = postRentalResult.Id,
                    Nights = 1,
                    Start = new DateTime(2002, 01, 02)
                };

                using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
                {
                    Assert.Equal(HttpStatusCode.InternalServerError, postBooking2Response.StatusCode);
                }
            }

            [Fact]
            public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsConflictingPreparationTime()
            {
                var postRentalRequest = new RentalRequest
                {
                    Units = 1,
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
                    Nights = 3,
                    Start = new DateTime(2002, 01, 01)
                };

                using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
                {
                    Assert.True(postBooking1Response.IsSuccessStatusCode);
                }

                var postBooking2Request = new CreateBookingRequest
                {
                    RentalId = postRentalResult.Id,
                    Nights = 1,
                    Start = new DateTime(2002, 01, 04)
                };

                using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
                {
                    Assert.Equal(HttpStatusCode.InternalServerError, postBooking2Response.StatusCode);
                }
            }
        }
    }
}
