using HotelBooking.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HotelBooking.UnitTests {
    public class BookingManager_CreateBookingTests {

        private readonly Mock<IRepository<Room>> _roomRepoMock;
        private readonly Mock<IRepository<Booking>> _bookingRepoMock;

        public BookingManager_CreateBookingTests() {
            _roomRepoMock = new Mock<IRepository<Room>>();
            _bookingRepoMock = new Mock<IRepository<Booking>>();
        }

        // Decision table bases testing
        // Testcase 1 (B B Y)
        public static IEnumerable<object[]> BeforeBooking() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(9), EndDate = DateTime.Today.AddDays(9), IsActive = false
                },
            };
        }
        // Testcase 2 (A A Y)
        public static IEnumerable<object[]> AfterBooking() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(21), EndDate = DateTime.Today.AddDays(21), IsActive = false
                },
            };
        }

        [Theory]
        [MemberData(nameof(BeforeBooking)), MemberData(nameof(AfterBooking))]
        public void CreateBooking_RequestedDatesAreFree_ShouldAddBooking(Booking booking) {
            // Arrange
            _bookingRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Booking>()
                {
                    new Booking
                    {
                        Id = 1, StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(20),
                        IsActive = true, CustomerId = 2, RoomId = 1
                    },
                });
            _roomRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Room>()
                {
                    new Room() {Description = "Room 1", Id = 1},
                });
            var manager = CreateInstance();

            // Act
            var result = manager.CreateBooking(booking);

            // Assert
            Assert.True(result);
            _bookingRepoMock.Verify(x => x.Add(It.IsAny<Booking>()), Times.Once);
        }

        // Testcase 3 (B A N)
        public static IEnumerable<object[]> Booking3() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(9), EndDate = DateTime.Today.AddDays(21), IsActive = false
                },
            };
        }

        // Testcase 4 (B O N)
        public static IEnumerable<object[]> Booking4() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(9), EndDate = DateTime.Today.AddDays(10), IsActive = false
                },
            };
        }

        // Testcase 5 (B O N) 2
        public static IEnumerable<object[]> Booking5() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(9), EndDate = DateTime.Today.AddDays(20), IsActive = false
                },
            };
        }

        // Testcase 6 (O A N) 
        public static IEnumerable<object[]> Booking6() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(21), IsActive = false
                },
            };
        }

        // Testcase 7 (O A N) 2
        public static IEnumerable<object[]> Booking7() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(20), EndDate = DateTime.Today.AddDays(21), IsActive = false
                },
            };
        }

        // Testcase 8 (O O N) 
        public static IEnumerable<object[]> Booking8() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(10), IsActive = false
                },
            };
        }

        // Testcase 9 (O O N) 2
        public static IEnumerable<object[]> Booking9() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(20), IsActive = false
                },
            };
        }

        // Testcase 10 (O O N) 3
        public static IEnumerable<object[]> Booking10() {
            yield return new object[] {
                new Booking() {
                    StartDate = DateTime.Today.AddDays(20), EndDate = DateTime.Today.AddDays(20), IsActive = false
                },
            };
        }

        [Theory]
        [MemberData(nameof(Booking3)), MemberData(nameof(Booking4)), MemberData(nameof(Booking5)), MemberData(nameof(Booking6)), MemberData(nameof(Booking7)), MemberData(nameof(Booking8)), MemberData(nameof(Booking9)), MemberData(nameof(Booking10))]
        public void CreateBooking_RequestedDatesAreOccupied_ShouldNotAddBooking(Booking booking) {
            // Arrange
            _bookingRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Booking>()
                {
                    new Booking
                    {
                        Id = 1, StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(20),
                        IsActive = true, CustomerId = 2, RoomId = 1
                    },
                });
            _roomRepoMock.Setup(repo => repo.GetAll())
                .Returns(new List<Room>()
                {
                    new Room() {Description = "Room 1", Id = 1},
                });
            var manager = CreateInstance();

            // Act
            var result = manager.CreateBooking(booking);

            // Assert
            Assert.False(result);
            //_bookingRepoMock.Verify(x => x.Add(It.IsAny<Booking>()), Times.Once);
        }

        public IBookingManager CreateInstance() {
            return new BookingManager(_bookingRepoMock.Object, _roomRepoMock.Object);
        }

    }
}
