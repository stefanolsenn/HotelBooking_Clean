using System;
using System.Collections.Generic;
using HotelBooking.Core;
using Moq;
using Xunit;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private readonly Mock<IRepository<Room>> _roomRepoMock;
        private readonly Mock<IRepository<Booking>> _bookingRepoMock;

        public BookingManagerTests()
        {
            _roomRepoMock = new Mock<IRepository<Room>>();
            _bookingRepoMock = new Mock<IRepository<Booking>>();
        }

        /// <summary>
        /// Black dates always fails
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> ConflictingDates()
        {
            yield return new object[] {DateTime.Today.AddDays(10), DateTime.Today};
        }

        public static IEnumerable<object[]> NonConclitingDates()
        {
            yield return new object[] {DateTime.Today.AddDays(5), DateTime.Today.AddDays(9)};
            yield return new object[] {DateTime.Today.AddDays(21), DateTime.Today.AddDays(25)};
        }

        public static IEnumerable<object[]> OverlappingDates()
        {
            yield return new object[] {DateTime.Today.AddDays(10), DateTime.Today.AddDays(20)};
        }

        [Theory]
        [MemberData(nameof(ConflictingDates))]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException(DateTime start, DateTime end)
        {
            // Arrange
            var manager = CreateInstance();
            //Assert
            Assert.Throws<ArgumentException>(() => manager.FindAvailableRoom(start, end));
        }

        [Theory]
        [MemberData(nameof(NonConclitingDates))]
        public void FindAvailableRoom_BookingPeriodeIsBeforeAlreadyBookedRooms_ShouldBookARoom(DateTime start,
            DateTime end)
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
        [MemberData(nameof(OverlappingDates))]
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

        [Theory]
        [MemberData(nameof(OverlappingDates))]
        public void FindAvailableRoom_NoRoomsAddedInHotel_ShouldNotBookARoom(DateTime start, DateTime end)
        {
            // Arrange
            _bookingRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Booking>());
            _roomRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Room>()
                {
                });
            var manager = CreateInstance();

            // Act
            var result = manager.FindAvailableRoom(start, end);

            // Assert
            Assert.Equal(-1, result);
        }

        public static IEnumerable<object[]> Bookings()
        {
            yield return new object[]
            {
                new Booking()
                {
                    RoomId = 3,
                    IsActive = false,
                    StartDate = DateTime.Today.AddDays(5),
                    EndDate = DateTime.Today.AddDays(9)
                },
            };
        }

        [Theory]
        [MemberData(nameof(Bookings))]
        public void CreateBooking_RoomIdIsThree_ShouldAddBooking(Booking booking)
        {
            // Arrange
            _bookingRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Booking>()
                {
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
            var result = manager.CreateBooking(booking);

            // Assert
            Assert.True(result);
            _bookingRepoMock.Verify(x => x.Add(It.IsAny<Booking>()), Times.Once);
        }

        public IBookingManager CreateInstance()
        {
            return new BookingManager(_bookingRepoMock.Object, _roomRepoMock.Object);
        }


        [Fact]
        public void GetFullyOccupiedDates_TwoRoomsTwoBookingsOneFullyOccupiedDate_ShouldReturnTheFullyOccupiedDate()
        {
            //arrange
            DateTime today = DateTime.Today;

            _bookingRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Booking>()
                {
                    new Booking
                    {
                        Id = 1, StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(2),
                        IsActive = true, CustomerId = 2, RoomId = 1
                    },
                    new Booking
                    {
                        Id = 2, StartDate = today, EndDate = DateTime.Today.AddDays(1),
                        IsActive = true, CustomerId = 2, RoomId = 2
                    },
                });

            _roomRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Room>()
                {
                    new Room() {Description = "room 1", Id = 1},
                    new Room() {Description = "room 2", Id = 2},
                });
            var manager = CreateInstance();

            //act
            var result = manager.GetFullyOccupiedDates(today, DateTime.Today.AddDays(2));

            var comparisonResult = DateTime.Compare(DateTime.Today.AddDays(1), result[0]);
            //assert
            Assert.True(result.Count == 1 && comparisonResult == 0);
        }
    }
}