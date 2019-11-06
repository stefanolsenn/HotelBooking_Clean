using HotelBooking.Core;
using Moq;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Xunit;

namespace SpecflowTests
{
    [Binding]
    public class BookSteps
    {

        IBookingManager _bookingManager;

        Mock<IRepository<Room>> _roomRepoMock;
        Mock<IRepository<Booking>> _bookingRepoMock;

        static DateTime today = DateTime.Today;
        DateTime _firstDayInMonth = new DateTime(today.Year, today.Month, 1).AddMonths(1);
        DateTime _tenthDayInMonth = new DateTime(today.Year, today.Month, 10).AddMonths(1);
        int _customerId;
        DateTime _startDate;
        DateTime _endDate;
        bool _resultOfBooking;


        public BookSteps()
        {

            _bookingRepoMock = new Mock<IRepository<Booking>>();

            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1, CustomerId = 1, RoomId = 1, IsActive = true,
                    StartDate = _firstDayInMonth,
                    EndDate = _tenthDayInMonth
                },
                new Booking
                {
                    Id = 2, CustomerId = 2, RoomId = 2, IsActive = true,
                    StartDate = _firstDayInMonth,
                    EndDate = _tenthDayInMonth
                }
            };


            _bookingRepoMock.Setup(x => x.GetAll()).Returns(bookings);



            _bookingRepoMock.Setup(x => x.Add(It.IsAny<Booking>()));
            

            _roomRepoMock = new Mock<IRepository<Room>>();

            var rooms = new List<Room>
            {
                new Room { Id=1, Description="First Room" },
                new Room { Id=2, Description="Second Room" },
            };

            // Implement fake GetAll() method.
            _roomRepoMock.Setup(x => x.GetAll()).Returns(rooms);

            _bookingManager = new BookingManager(_bookingRepoMock.Object, _roomRepoMock.Object);
        }



        [Given(@"I have entered a start date '(.*)'")]
        public void GivenIHaveEnteredAStartDateForTheBooking(string startDate)
        {
            _startDate = DateTime.Parse(startDate);
        }

        [Given(@"I have entered an end date '(.*)'")]
        public void GivenIHaveEnteredAnEndDateForTheSameBooking(string endDate)
        {
            _endDate = DateTime.Parse(endDate);
        }

        [Given(@"I have entered a customer ID (.*)")]
        public void GivenIHaveEnteredACustomerId(int customerId)
        {
            _customerId = customerId;
        }
        
        [When(@"I press the create booking button")]
        public void WhenIPressTheCreateBookingButton()
        {
            var booking = new Booking
            {
                CustomerId = _customerId,
                StartDate = _startDate,
                EndDate = _endDate
            };
            try
            {
                _resultOfBooking = _bookingManager.CreateBooking(booking);
            }
            catch 
            {
                _resultOfBooking = false;
            }
        }

        [Then(@"The result should be (.*)")]
        public void ThenTheResultShouldBeEitherTrueOgFalseIfIsPossible(bool result)
        {
            Assert.Equal(result, _resultOfBooking);
        }
    }
}
