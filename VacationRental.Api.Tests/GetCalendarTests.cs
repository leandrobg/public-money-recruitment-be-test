using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VacationRental.Domain.Models.Bookings;
using VacationRental.Domain.Models.Calendar;
using VacationRental.Domain.Models.Rentals;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class GetCalendarTests
    {
        private readonly HttpClient _client;
        public GetCalendarTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar()
        {
            var postRentalRequest = new RentalRequest
            {
                Units = 2,
                PreparationTimeInDays = 2
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

            CreateBookingResponse? postBooking1Result;
            using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
                postBooking1Result = await postBooking1Response.Content.ReadFromJsonAsync<CreateBookingResponse>();

                Assert.NotNull(postBooking1Result);
            }

            var postBooking2Request = new CreateBookingRequest
            {
                RentalId = postRentalResult.Id,
                Nights = 2,
                Start = new DateTime(2000, 01, 03)
            };

            CreateBookingResponse? postBooking2Result;
            using (var postBooking2Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBooking2Request))
            {
                Assert.True(postBooking2Response.IsSuccessStatusCode);
                postBooking2Result = await postBooking2Response.Content.ReadFromJsonAsync<CreateBookingResponse>();

                Assert.NotNull(postBooking2Result);
            }

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={postRentalResult.Id}&start=2000-01-01&nights=7"))
            {
                    Assert.True(getCalendarResponse.IsSuccessStatusCode);

                    var getCalendarResult = await getCalendarResponse.Content.ReadFromJsonAsync<GetCalendarResponse>();

                Assert.NotNull(getCalendarResult);
                Assert.NotNull(getCalendarResult!.Dates);
                Assert.True(getCalendarResult.Dates!.All(d => d.Bookings != null && d.PreparationTimes != null));

                Assert.Equal(postRentalResult.Id, getCalendarResult!.RentalId);
                Assert.Equal(7, getCalendarResult.Dates.Count);

                Assert.Equal(new DateTime(2000, 01, 01), getCalendarResult.Dates[0].Date);
                Assert.Empty(getCalendarResult.Dates[0].Bookings!);
                Assert.Empty(getCalendarResult.Dates[0].PreparationTimes!);

                Assert.Equal(new DateTime(2000, 01, 02), getCalendarResult.Dates[1].Date);
                Assert.Single(getCalendarResult.Dates[1].Bookings!);
                Assert.Contains(getCalendarResult.Dates[1].Bookings!, x => x.Id == postBooking1Result!.Id && x.Unit == 1);
                Assert.Empty(getCalendarResult.Dates[0].PreparationTimes!);

                Assert.Equal(new DateTime(2000, 01, 03), getCalendarResult.Dates[2].Date);
                Assert.Equal(2, getCalendarResult.Dates[2].Bookings!.Count);
                Assert.Contains(getCalendarResult.Dates[2].Bookings!, x => x.Id == postBooking1Result!.Id && x.Unit == 1);
                Assert.Contains(getCalendarResult.Dates[2].Bookings!, x => x.Id == postBooking2Result!.Id && x.Unit == 2);
                Assert.Empty(getCalendarResult.Dates[0].PreparationTimes!);

                Assert.Equal(new DateTime(2000, 01, 04), getCalendarResult.Dates[3].Date);
                Assert.Single(getCalendarResult.Dates[3].Bookings!);
                Assert.Contains(getCalendarResult.Dates[3].Bookings!, x => x.Id == postBooking2Result!.Id && x.Unit == 2);
                Assert.Single(getCalendarResult.Dates[3].PreparationTimes!);
                Assert.Contains(getCalendarResult.Dates[3].PreparationTimes!, x => x.Unit == 1);

                Assert.Equal(new DateTime(2000, 01, 05), getCalendarResult.Dates[4].Date);
                Assert.Empty(getCalendarResult.Dates[4].Bookings!);
                Assert.Equal(2, getCalendarResult.Dates[4].PreparationTimes!.Count);
                Assert.Contains(getCalendarResult.Dates[4].PreparationTimes!, x => x.Unit == 1);
                Assert.Contains(getCalendarResult.Dates[4].PreparationTimes!, x => x.Unit == 2);

                Assert.Equal(new DateTime(2000, 01, 06), getCalendarResult.Dates[5].Date);
                Assert.Empty(getCalendarResult.Dates[5].Bookings!);
                Assert.Single(getCalendarResult.Dates[5].PreparationTimes!);
                Assert.Contains(getCalendarResult.Dates[5].PreparationTimes!, x => x.Unit == 2);

                Assert.Equal(new DateTime(2000, 01, 07), getCalendarResult.Dates[6].Date);
                Assert.Empty(getCalendarResult.Dates[6].Bookings!);
                Assert.Empty(getCalendarResult.Dates[6].PreparationTimes!);
            }
        }
    }
}
