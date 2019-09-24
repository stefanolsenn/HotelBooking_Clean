using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {

        private readonly Mock<IRepository<Room>> _roomRepoMock;
        private readonly Mock<IRepository<Booking>> _bookingRepoMock;

        public BookingManagerTests(){
            _roomRepoMock = new Mock<IRepository<Room>>();
            _bookingRepoMock = new Mock<IRepository<Booking>>();
        }

        /// <summary>
        /// Black dates always fails
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetBlackDates() {
            yield return new object[] { DateTime.Today.AddDays(10), DateTime.Today };
        }
        
        public static IEnumerable<object[]> GetWhiteDates() {
            yield return new object[] { DateTime.Today.AddDays(5), DateTime.Today.AddDays(9) };
            yield return new object[] { DateTime.Today.AddDays(21), DateTime.Today.AddDays(25) };
        }

        public static IEnumerable<object[]> GetBrownDates() {
            yield return new object[] { DateTime.Today.AddDays(10), DateTime.Today.AddDays(20) };
        }
        
        [Theory]
        [MemberData(nameof(GetBlackDates))]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException(DateTime start, DateTime end)
        {
            // Arrange
            var manager = CreateInstance();
            //Assert
            Assert.Throws<ArgumentException>(() => manager.FindAvailableRoom(start, end));
        }
        
        [Theory]
        [MemberData(nameof(GetWhiteDates))]
        public void FindAvailableRoom_BookingPeriodeIsBeforeAlreadyBookedRooms_ShouldBookARoom(DateTime start, DateTime end)
        {
            // Arrange
            _bookingRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Booking>()
                {

                    new Booking
                    {
                        Id = 1, StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(20),
                        IsActive = true, CustomerId = 1, RoomId = 1
                    },
                    new Booking
                    {
                        Id = 2, StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(20),
                        IsActive = true, CustomerId = 2, RoomId = 2
                    },
                });
            _roomRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Room>()
                {
                    new Room() {Description = "hejhej", Id = 3},
                });
            var manager = CreateInstance();
            
            // Act
            var result = manager.FindAvailableRoom(start, end);
            
            // Assert
            Assert.Equal(3, result);
        }
        
        [Theory]
        [MemberData(nameof(GetBrownDates))]
        public void FindAvailableRoom_RoomIsBookedInPeriod_ShouldNotBookARoom(DateTime start, DateTime end)
        {
            // Arrange
            _bookingRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Booking>()
                {

                    new Booking
                    {
                        Id = 1, StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(20),
                        IsActive = true, CustomerId = 1, RoomId = 1
                    },
                    new Booking
                    {
                        Id = 2, StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(20),
                        IsActive = true, CustomerId = 2, RoomId = 2
                    },
                });
            _roomRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Room>()
                {
                    new Room() {Description = "hejhej", Id = 2},
                    new Room() {Description = "hejhej", Id = 1},
                });
            var manager = CreateInstance();
            
            // Act
            var result = manager.FindAvailableRoom(start, end);
            
            // Assert
            Assert.Equal(-1, result);
        }

        public IBookingManager CreateInstance()
        {
            return new BookingManager(_bookingRepoMock.Object, _roomRepoMock.Object);
        }

    }
}
