using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TrainTravelAgency;
using TrainTravelAgency.Models;
using TrainTravelAgency.Services;
using TrainTravelAgency.Fakes;
using TrainTravelAgency.Exceptions;
using System.Net.Sockets;

namespace TrainTravelAgency.Test
{
    [TestFixture]
    public class mojaGenijalnost
    {
        //propertiji

        private ReservationService servis;

        //F1-TACNE CENE


        //ako upada u prvi if (firstclass) i ipunava uslov za dist>1500
        [TestCase(1501, TicketType.FirstClass, 5, 000000000-0000-0000-0000-000000000001, 1501 * 0.06)]
        //ako ne upada u if a first class je 
        [TestCase(1000, TicketType.FirstClass, 5, 000000000-0000-0000-0000-000000000001, 1000 * 0.1)]
        //second class i ispunjava oba uslova
        [TestCase(1001, TicketType.SecondClass, 15, 000000000-0000-0000-0000-000000000001, 1001 * 0.04)]
        //second class i ne ispunjava uslove
        [TestCase(999, TicketType.SecondClass, 14, 00000000-0000-0000-0000-000000000001, 999 * 0.05)]
        //ne upada nigde (economy)
        [TestCase(500, TicketType.Economic, 0, 00000000-0000-0000-0000-000000000001, 500 * 0.01)]


        //ne prolaze zbog Guid-a(umesto ovog stringa samo ne prolsedimo nista i stavimo za ID new guid)
        public void CalculateTicketPrice_TacnaCena(double distance, TicketType tipKarte, int brKarataPrMesec, Guid id, double exp)
        {
            User user = new User
            {
                Id = id,
                NumberOfTicketsPurchasedInTheLastMonth = brKarataPrMesec
            };
            Fus fus = new Fus(user);
            ReservationService servis = new ReservationService(fus); //napravih konst koji prima samo to
            double cena = servis.CalculateTicketPriceForUser(distance, tipKarte, id);
            Assert.That(cena, Is.EqualTo(exp).Within(0.0001));

        }

        [TestCase(1000, TicketType.FirstClass, 11, 1000 * 0.06)]
        public void CalculateTicketPrice_TacnaCena2(double distance, TicketType tipKarte, int brKarataPrMesec,  double exp)
        {
            Guid id = new Guid();
            User korisnik = new User { Id =id, NumberOfTicketsPurchasedInTheLastMonth = brKarataPrMesec };
            Fus fakeUserIn = new Fus(korisnik);
            ReservationService servis = new ReservationService(fakeUserIn);
            Assert.That(servis.CalculateTicketPriceForUser(distance, tipKarte,id ), Is.EqualTo(exp).Within(0.0001));

        }


        [Test]
        public void CalculateTicketPrice_ExeptionTip()
        {
            User user = new User { Id = Guid.NewGuid(), NumberOfTicketsPurchasedInTheLastMonth = 5 };
            Fus fus = new Fus(user, true); //postavljamo shouldThrow na true da on zna da treba da baci exception
            Fls fls = new Fls();
            ReservationService servis = new ReservationService(fus, fls);
            Assert.That(
               (Action)(() => servis.CalculateTicketPriceForUser(1000, TicketType.FirstClass, user.Id)),
               Throws.TypeOf<ExternalServiceErrorException>());

        }
        [Test]
        public void CalculateTicketPrice_DalBacaTuPoruku()
        {
            User user = new User { Id = Guid.NewGuid(), NumberOfTicketsPurchasedInTheLastMonth = 5 };
            Fus fus = new Fus(user, true);
            string expMes = "External user service error.";
            Fls fls = new Fls();
            ReservationService servis = new ReservationService(fus, fls);
            Assert.That((Action)(() => servis.CalculateTicketPriceForUser(1000, TicketType.FirstClass, user.Id)),
                Throws.TypeOf<ExternalServiceErrorException>());
            Assert.That(expMes, Is.EqualTo(fls.Message));

        }
        //F3 DISTANCE 
        /*
         *  public double GetDistanceBetweenCities(Guid cityFromId, Guid cityToId)
        {
            double distanceInMiles =_distanceCalculationService.CalculateDistance(cityFromId, cityToId);
            return distanceInMiles * 1.060;
        }
         */
        //km->milje
        //mala(10)+velika udaljenost(2000) + srednja(800) + 0
        [TestCase(10, 10 * 1.060)]
        [TestCase(2000, 2000 * 1.060)]
        [TestCase(800, 800 * 1.060)]
        [TestCase(0, 0)]
        public void GetDistanceBetweenCities_TacnaDistanca(double km, double exp)
        {
            Fdc fdc = new Fdc(km);
            ReservationService servis = new ReservationService(null, null, fdc);
            double rezultat = servis.GetDistanceBetweenCities(Guid.NewGuid(), Guid.NewGuid());
            Assert.That(rezultat, Is.EqualTo(exp).Within(0.0001));
        }
        //F2 PICT 
        //(tipSedista, tezinaPrtljaga, satiPutovanja, pice, karta)
        [TestCaseSource(typeof(MojPARSER), "Parsiraj", new object[] {"MojREZULTAT.txt"})]
        public void RecommendTicketType_PICT(SeatType tipSedista, double tezinaPrtljaga, int satiPutovanja, bool pice, TicketType? exp)
        {

            ReservationService servis = new ReservationService(null,null,null);
            TicketType? rezultat = servis.RecommendTicketType(tipSedista, tezinaPrtljaga, pice, satiPutovanja);
            // stoji ? jer moze vratiti i null 
            Assert.That(rezultat, Is.EqualTo(exp));

        }


    }
}