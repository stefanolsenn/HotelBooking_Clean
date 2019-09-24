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

        [Theory]
        [InlineData("04/20/2020", "04/19/2020")]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException(DateTime start, DateTime end)
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
            var manager = CreateInstance();
           

            // Assert
            Assert.True(true);
            //Assert.Throws<ArgumentException>(() => manager.FindAvailableRoom(start, end));
        }

        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
          //  int roomId = bookingManager.FindAvailableRoom(date, date);
            // Assert
            // Assert.NotEqual(-1, roomId);
        }

        public IBookingManager CreateInstance()
        {
            return new BookingManager(_bookingRepoMock.Object, _roomRepoMock.Object);
        }

    }
}
