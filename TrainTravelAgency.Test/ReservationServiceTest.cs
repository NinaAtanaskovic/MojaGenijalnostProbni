using System;
using NUnit.Framework;
using TrainTravelAgency.Exceptions;
using TrainTravelAgency.Fakes;
using TrainTravelAgency.Models;

namespace TrainTravelAgency.Test
{
    [TestFixture]
    public class ReservationServiceTest
    {
        private FakeUserService _fakeUserService;
        private FakeLoggerService _fakeLoggerService;
        private FakeDistanceCalculationService _fakeDistanceCalculationService;
        private ReservationService _reservationService;

        // =====================================================================
        // F1: RecommendTicketType — testovi pokretani pomocu PICT TestCaseSource
        // =====================================================================

        /// <summary>
        /// F1 - Testovi generisani iz PICT Results.txt fajla pomocu PictParser-a.
        /// Svaki test proverava ispravnu preporuku tipa karte.
        /// </summary>
        [TestCaseSource(typeof(PictParser), nameof(PictParser.GetTestCases))]
        public void RecommendTicketType_PictTestCases_ReturnsExpectedResult(
            SeatType seatType, double luggageWeight, bool beverage, int travelHour, TicketType? expectedResult)
        {
            // Arrange
            _fakeLoggerService = new FakeLoggerService();
            _fakeUserService = new FakeUserService(new User { Id = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183"), NumberOfTicketsPurchasedInTheLastMonth = 5 });
            _fakeDistanceCalculationService = new FakeDistanceCalculationService(500);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            // Act
            TicketType? result = _reservationService.RecommendTicketType(seatType, luggageWeight, beverage, travelHour);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // =====================================================================
        // F1: RecommendTicketType — dodatni direktni testovi
        // =====================================================================

        [SetUp]
        public void Setup()
        {
            _fakeLoggerService = new FakeLoggerService();
            _fakeUserService = new FakeUserService(new User
            {
                Id = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183"),
                FullName = "Test User",
                Email = "test@example.com",
                NumberOfTicketsPurchasedInTheLastMonth = 5
            });
            _fakeDistanceCalculationService = new FakeDistanceCalculationService(1000);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);
        }

        /// <summary>
        /// F1 - Kada je beverage=true i seatType=Regular, ocekuje se null (nije moguce ponuditi napitak).
        /// </summary>
        [Test]
        public void RecommendTicketType_BeverageTrueRegularSeat_ReturnsNull()
        {
            // Arrange
            SeatType seatType = SeatType.Regular;
            double luggageWeight = 10;
            bool beverage = true;
            int travelHour = 2;

            // Act
            TicketType? result = _reservationService.RecommendTicketType(seatType, luggageWeight, beverage, travelHour);

            // Assert
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// F1 - Kada je beverage=true i seatType=Romettes, ocekuje se FirstClass karta.
        /// </summary>
        [Test]
        public void RecommendTicketType_BeverageTrueRomettesSeat_ReturnsFirstClass()
        {
            // Arrange
            SeatType seatType = SeatType.Romettes;
            double luggageWeight = 10;
            bool beverage = true;
            int travelHour = 2;

            // Act
            TicketType? result = _reservationService.RecommendTicketType(seatType, luggageWeight, beverage, travelHour);

            // Assert
            Assert.That(result, Is.EqualTo(TicketType.FirstClass));
        }

        /// <summary>
        /// F1 - Kada je beverage=true i seatType=Table, ocekuje se FirstClass karta.
        /// </summary>
        [Test]
        public void RecommendTicketType_BeverageTrueTableSeat_ReturnsFirstClass()
        {
            // Arrange
            SeatType seatType = SeatType.Table;
            double luggageWeight = 15;
            bool beverage = true;
            int travelHour = 5;

            // Act
            TicketType? result = _reservationService.RecommendTicketType(seatType, luggageWeight, beverage, travelHour);

            // Assert
            Assert.That(result, Is.EqualTo(TicketType.FirstClass));
        }

        /// <summary>
        /// F1 - Kada je beverage=false, tezina prtljaga veca od 30 i travelHour izmedju 2 i 5 (ekskluzivno), ocekuje se null.
        /// </summary>
        [Test]
        public void RecommendTicketType_NoBeverageHeavyLuggagePeakHour_ReturnsNull()
        {
            // Arrange
            SeatType seatType = SeatType.Table;
            double luggageWeight = 50;
            bool beverage = false;
            int travelHour = 3; // >2 && <5

            // Act
            TicketType? result = _reservationService.RecommendTicketType(seatType, luggageWeight, beverage, travelHour);

            // Assert
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// F1 - Kada je beverage=false, tezina prtljaga veca od 30 i travelHour van peak zone, ocekuje se SecondClass.
        /// </summary>
        [Test]
        public void RecommendTicketType_NoBeverageHeavyLuggageOffPeakHour_ReturnsSecondClass()
        {
            // Arrange
            SeatType seatType = SeatType.Romettes;
            double luggageWeight = 31;
            bool beverage = false;
            int travelHour = 6; // nije >2 && <5

            // Act
            TicketType? result = _reservationService.RecommendTicketType(seatType, luggageWeight, beverage, travelHour);

            // Assert
            Assert.That(result, Is.EqualTo(TicketType.SecondClass));
        }

        /// <summary>
        /// F1 - Kada je beverage=false i tezina prtljaga manja ili jednaka 30, ocekuje se Economic karta.
        /// </summary>
        [Test]
        public void RecommendTicketType_NoBeverageLightLuggage_ReturnsEconomic()
        {
            // Arrange
            SeatType seatType = SeatType.Regular;
            double luggageWeight = 10;
            bool beverage = false;
            int travelHour = 1;

            // Act
            TicketType? result = _reservationService.RecommendTicketType(seatType, luggageWeight, beverage, travelHour);

            // Assert
            Assert.That(result, Is.EqualTo(TicketType.Economic));
        }

        // =====================================================================
        // F2: CalculateTicketPriceForUser — testovi cena karata
        // =====================================================================

        /// <summary>
        /// F2 - FirstClass karta sa distancom > 1500: cena = distance * 0.06 (popust).
        /// </summary>
        [Test]
        public void CalculateTicketPriceForUser_FirstClassLongDistance_ReturnsDiscountedPrice()
        {
            // Arrange
            double distance = 2000;
            TicketType ticketType = TicketType.FirstClass;
            Guid userId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            var user = new User { Id = userId, NumberOfTicketsPurchasedInTheLastMonth = 5 };
            _fakeUserService = new FakeUserService(user);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            double expectedPrice = distance * 0.06;

            // Act
            double result = _reservationService.CalculateTicketPriceForUser(distance, ticketType, userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPrice).Within(0.0001));
        }

        /// <summary>
        /// F2 - FirstClass karta sa vise od 10 kupljenih karata u poslednjem mesecu: cena = distance * 0.06 (popust).
        /// </summary>
        [Test]
        public void CalculateTicketPriceForUser_FirstClassFrequentBuyer_ReturnsDiscountedPrice()
        {
            // Arrange
            double distance = 500;
            TicketType ticketType = TicketType.FirstClass;
            Guid userId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            var user = new User { Id = userId, NumberOfTicketsPurchasedInTheLastMonth = 11 };
            _fakeUserService = new FakeUserService(user);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            double expectedPrice = distance * 0.06;

            // Act
            double result = _reservationService.CalculateTicketPriceForUser(distance, ticketType, userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPrice).Within(0.0001));
        }

        /// <summary>
        /// F2 - FirstClass karta bez popusta (distance <= 1500 i numberOfTickets <= 10): cena = distance * 0.1.
        /// </summary>
        [Test]
        public void CalculateTicketPriceForUser_FirstClassNoDiscount_ReturnsFullPrice()
        {
            // Arrange
            double distance = 1000;
            TicketType ticketType = TicketType.FirstClass;
            Guid userId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            var user = new User { Id = userId, NumberOfTicketsPurchasedInTheLastMonth = 5 };
            _fakeUserService = new FakeUserService(user);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            double expectedPrice = distance * 0.1;

            // Act
            double result = _reservationService.CalculateTicketPriceForUser(distance, ticketType, userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPrice).Within(0.0001));
        }

        /// <summary>
        /// F2 - SecondClass karta sa distancom > 1000 i >= 15 kupljenih karata: cena = distance * 0.04 (popust).
        /// </summary>
        [Test]
        public void CalculateTicketPriceForUser_SecondClassLongDistanceFrequentBuyer_ReturnsDiscountedPrice()
        {
            // Arrange
            double distance = 1500;
            TicketType ticketType = TicketType.SecondClass;
            Guid userId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            var user = new User { Id = userId, NumberOfTicketsPurchasedInTheLastMonth = 15 };
            _fakeUserService = new FakeUserService(user);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            double expectedPrice = distance * 0.04;

            // Act
            double result = _reservationService.CalculateTicketPriceForUser(distance, ticketType, userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPrice).Within(0.0001));
        }

        /// <summary>
        /// F2 - SecondClass karta bez popusta (distance <= 1000): cena = distance * 0.05.
        /// </summary>
        [Test]
        public void CalculateTicketPriceForUser_SecondClassShortDistance_ReturnsFullPrice()
        {
            // Arrange
            double distance = 800;
            TicketType ticketType = TicketType.SecondClass;
            Guid userId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            var user = new User { Id = userId, NumberOfTicketsPurchasedInTheLastMonth = 20 };
            _fakeUserService = new FakeUserService(user);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            double expectedPrice = distance * 0.05;

            // Act
            double result = _reservationService.CalculateTicketPriceForUser(distance, ticketType, userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPrice).Within(0.0001));
        }

        /// <summary>
        /// F2 - SecondClass karta bez popusta (numberOfTickets < 15): cena = distance * 0.05.
        /// </summary>
        [Test]
        public void CalculateTicketPriceForUser_SecondClassFewTickets_ReturnsFullPrice()
        {
            // Arrange
            double distance = 2000;
            TicketType ticketType = TicketType.SecondClass;
            Guid userId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            var user = new User { Id = userId, NumberOfTicketsPurchasedInTheLastMonth = 10 };
            _fakeUserService = new FakeUserService(user);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            double expectedPrice = distance * 0.05;

            // Act
            double result = _reservationService.CalculateTicketPriceForUser(distance, ticketType, userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPrice).Within(0.0001));
        }

        /// <summary>
        /// F2 - Economic karta: cena = distance * 0.01.
        /// </summary>
        [Test]
        public void CalculateTicketPriceForUser_EconomicTicket_ReturnsEconomicPrice()
        {
            // Arrange
            double distance = 500;
            TicketType ticketType = TicketType.Economic;
            Guid userId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            var user = new User { Id = userId, NumberOfTicketsPurchasedInTheLastMonth = 3 };
            _fakeUserService = new FakeUserService(user,false);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            double expectedPrice = distance * 0.01;

            // Act
            double result = _reservationService.CalculateTicketPriceForUser(distance, ticketType, userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPrice).Within(0.0001));
        }

        /// <summary>
        /// F2 - Kada IUserService baci ExternalServiceErrorException, servis treba da loguje gresku i rethrow-uje izuzetak.
        /// </summary>
        [Test]
        public void CalculateTicketPriceForUser_UserServiceThrowsException_LogsErrorAndRethrows()
        {
            // Arrange
            double distance = 1000;
            TicketType ticketType = TicketType.FirstClass;
            Guid userId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            string errorMessage = "External user service error.";

            var throwingUserService = new FakeUserService(null, shouldThrow: true);
            _fakeLoggerService = new FakeLoggerService();
            _reservationService = new ReservationService(throwingUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            // Act & Assert
            Assert.That(
                (Action)(() => _reservationService.CalculateTicketPriceForUser(distance, ticketType, userId)),
                Throws.TypeOf<ExternalServiceErrorException>());

            Assert.That(_fakeLoggerService.LoggedMessages.Count, Is.EqualTo(1));
            Assert.That(_fakeLoggerService.LoggedMessages[0], Is.EqualTo(errorMessage));
        }

        // =====================================================================
        // F3: GetDistanceBetweenCities — testovi konverzije udaljenosti
        // =====================================================================

        /// <summary>
        /// F3 - Konvertuje udaljenost iz milja u kilometre: distanceKm = distanceMiles * 1.060.
        /// </summary>
        [Test]
        public void GetDistanceBetweenCities_ValidCities_ReturnsConvertedDistance()
        {
            // Arrange
            double distanceInMiles = 1000;
            double expectedDistanceInKm = distanceInMiles * 1.060;
            Guid cityFromId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            Guid cityToId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");

            _fakeDistanceCalculationService = new FakeDistanceCalculationService(distanceInMiles);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            // Act
            double result = _reservationService.GetDistanceBetweenCities(cityFromId, cityToId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDistanceInKm).Within(0.0001));
        }

        /// <summary>
        /// F3 - Konverzija za kratku udaljenost (10 milja).
        /// </summary>
        [Test]
        public void GetDistanceBetweenCities_ShortDistance_ReturnsCorrectKilometers()
        {
            // Arrange
            double distanceInMiles = 10;
            double expectedDistanceInKm = distanceInMiles * 1.060;
            Guid cityFromId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            Guid cityToId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");

            _fakeDistanceCalculationService = new FakeDistanceCalculationService(distanceInMiles);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            // Act
            double result = _reservationService.GetDistanceBetweenCities(cityFromId, cityToId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDistanceInKm).Within(0.0001));
        }

        /// <summary>
        /// F3 - Konverzija za veliku udaljenost (5000 milja).
        /// </summary>
        [Test]
        public void GetDistanceBetweenCities_LongDistance_ReturnsCorrectKilometers()
        {
            // Arrange
            double distanceInMiles = 5000;
            double expectedDistanceInKm = distanceInMiles * 1.060;
            Guid cityFromId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            Guid cityToId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");

            _fakeDistanceCalculationService = new FakeDistanceCalculationService(distanceInMiles);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            // Act
            double result = _reservationService.GetDistanceBetweenCities(cityFromId, cityToId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDistanceInKm).Within(0.0001));
        }

        /// <summary>
        /// F3 - Konverzija kada je udaljenost 0 milja — ocekuje se 0 km.
        /// </summary>
        [Test]
        public void GetDistanceBetweenCities_ZeroDistance_ReturnsZero()
        {
            // Arrange
            double distanceInMiles = 0;
            Guid cityFromId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");
            Guid cityToId = Guid.Parse("96bff8d0-3254-404c-9f50-51a8e135a183");

            _fakeDistanceCalculationService = new FakeDistanceCalculationService(distanceInMiles);
            _reservationService = new ReservationService(_fakeUserService, _fakeLoggerService, _fakeDistanceCalculationService);

            // Act
            double result = _reservationService.GetDistanceBetweenCities(cityFromId, cityToId);

            // Assert
            Assert.That(result, Is.EqualTo(0).Within(0.0001));
        }
    }
}
