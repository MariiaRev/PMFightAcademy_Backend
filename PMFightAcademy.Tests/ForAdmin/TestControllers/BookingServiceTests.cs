﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using PMFightAcademy.Admin.Contract;
using PMFightAcademy.Admin.DataBase;
using PMFightAcademy.Admin.Models;
using PMFightAcademy.Admin.Services;
using PMFightAcademy.Admin.Services.ServiceInterfaces;
using Xunit;

namespace PMFightAcademy.Tests.ForAdmin.TestControllers
{
    public class BookingServiceTests
    {
        private Mock<AdminContext> _clientContextMock;
        private IBookingService _testedService;

        private void Setup()
        {
            var options = new DbContextOptionsBuilder<AdminContext>().Options;
            _clientContextMock = new Mock<AdminContext>(options);
            _testedService = new BookingService(_clientContextMock.Object);
        }

        [Fact]
        public async Task TakeAllBooking()
        {
            Setup();

            var bookings = new List<Booking>() { new () };

            _clientContextMock.Setup(x => x.Bookings).ReturnsDbSet(bookings);

            _testedService = new BookingService(_clientContextMock.Object);

            var result = await _testedService.TakeAllBooking(CancellationToken.None);

            Assert.Single(result);
        }

        [Fact]
        public async Task TakeBookingForCoach_Success()
        {
            Setup();
            var slot = new Slot() {Id = 1, CoachId = 1};
            var bookings = new List<Booking>() { new() {Id = 1, Slot = slot } };

            _clientContextMock.Setup(x => x.Bookings).ReturnsDbSet(bookings);

            _testedService = new BookingService(_clientContextMock.Object);

            var result = await _testedService.TakeBookingForCoach(1, CancellationToken.None);

            Assert.Single(result);
        }

        [Fact]
        public async Task TakeBookingForCoach_Fail()
        {
            Setup();
            var slot = new Slot() { Id = 1, CoachId = 2 };
            var bookings = new List<Booking>() { new() { Id = 1, Slot = slot } };

            _clientContextMock.Setup(x => x.Bookings).ReturnsDbSet(bookings);

            _testedService = new BookingService(_clientContextMock.Object);

            var result = await _testedService.TakeBookingForCoach(1, CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task TakeBookingOnClient_Success()
        {
            Setup();
            var bookings = new List<Booking>()
            {
                new() { Id = 1, ClientId = 1},
                new() { Id = 2, ClientId = 2},
                new() { Id = 3, ClientId = 1}
            };

            _clientContextMock.Setup(x => x.Bookings).ReturnsDbSet(bookings);

            _testedService = new BookingService(_clientContextMock.Object);

            var result = await _testedService.TakeBookingOnClient(1, CancellationToken.None);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task TakeBookingOnClient_Fail()
        {
            Setup();
            var bookings = new List<Booking>()
            {
                new() { Id = 1, ClientId = 1},
                new() { Id = 2, ClientId = 2},
                new() { Id = 3, ClientId = 1}
            };

            _clientContextMock.Setup(x => x.Bookings).ReturnsDbSet(bookings);

            _testedService = new BookingService(_clientContextMock.Object);

            var result = await _testedService.TakeBookingOnClient(3, CancellationToken.None);

            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateBooking()
        {
            Setup();
            var bookings = new List<Booking>()
            {
                new() { Id = 1, ClientId = 1},
                new() { Id = 2, ClientId = 2},
                new() { Id = 3, ClientId = 1}
            };

            _clientContextMock.Setup(x => x.Bookings).ReturnsDbSet(bookings);

            _testedService = new BookingService(_clientContextMock.Object);

            var bookingDto = new BookingReturnContract()
            {
                Id = 1,
                ClientId = 1,
                ResultPrice = 500,
                ServiceId = 1,
                Slot = new SlotsReturnContract()
                {
                    Id =1, 
                    CoachId = 1, 
                    DateStart = "05.05.2021", 
                    TimeStart = "8:00",
                    Duration = "12:00"
                }
            };

            var result = await _testedService.UpdateBooking(bookingDto, CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task RemoveBooking_Success()
        {
            Setup();
            var bookings = new List<Booking>()
            {
                new() { Id = 1, ClientId = 1},
                new() { Id = 2, ClientId = 2},
                new() { Id = 3, ClientId = 1}
            };

            _clientContextMock.Setup(x => x.Bookings).ReturnsDbSet(bookings);

            _testedService = new BookingService(_clientContextMock.Object);

            var result = await _testedService.RemoveBooking(1, CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task RemoveBooking_Fail()
        {
            Setup();

            var bookings = new List<Booking>()
            {
                new() { Id = 1, ClientId = 1},
                new() { Id = 2, ClientId = 2},
                new() { Id = 3, ClientId = 1}
            };

            _clientContextMock.Setup(x => x.Bookings).ReturnsDbSet(bookings);

            _testedService = new BookingService(_clientContextMock.Object);

            var result = await _testedService.RemoveBooking(5, CancellationToken.None);

            Assert.False(result);
        }

    }
}
