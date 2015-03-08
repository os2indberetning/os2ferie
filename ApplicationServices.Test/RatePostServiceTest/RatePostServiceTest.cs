using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.ApplicationServices;
using Core.DomainModel;
using NUnit.Framework;

namespace ApplicationServices.Test.DriveReportServiceTest
{
    [TestFixture]
    public class RatePostServiceTest
    {

        private RatePostService _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new RatePostService();
        }

        [Test]
        public void DeactivateExistingRate_ShouldNotChangeRepo_WhenNoRatesAreEqual()
        {
            var repo = new List<Rate>
            {
                new Rate()
                {
                    Active = true,
                    Id = 1,
                    Year = 2015,
                    KmRate = 12,
                    TFCode = "Code",
                    TypeId = 1
                },
                new Rate()
                {
                    Active = true,
                    Id = 2,
                    Year = 2016,
                    KmRate = 12,
                    TFCode = "Code",
                    TypeId = 1
                },
                new Rate()
                {
                    Active = true,
                    Id = 3,
                    Year = 2017,
                    KmRate = 12,
                    TFCode = "Code",
                    TypeId = 1
                }
            };

            var newRate = new Rate()
            {
                Active = true,
                Year = 2018,
                KmRate = 12,
                Id = 4,
                TypeId = 1
            };

            Assert.AreEqual(false, _uut.DeactivateExistingRate(repo.AsQueryable(), newRate));
        }

        [Test]
        public void DeactivateExistingRate_ShouldChangeRepo_WhenRatesAreEqual()
        {
            var repo = new List<Rate>
            {
                new Rate()
                {
                    Active = true,
                    Id = 1,
                    Year = 2015,
                    KmRate = 12,
                    TFCode = "Code",
                    TypeId = 1
                },
                new Rate()
                {
                    Active = true,
                    Id = 2,
                    Year = 2016,
                    KmRate = 12,
                    TFCode = "Code",
                    TypeId = 1
                },
                new Rate()
                {
                    Active = true,
                    Id = 3,
                    Year = 2018,
                    KmRate = 12,
                    TFCode = "Code",
                    TypeId = 1
                }
            };

            var newRate = new Rate()
            {
                Active = true,
                Year = 2018,
                KmRate = 12,
                Id = 4,
                TypeId = 1
            };

            Assert.AreEqual(true, _uut.DeactivateExistingRate(repo.AsQueryable(), newRate));
        }


        [Test]
        public void DeactivateExistingRate_ShouldChangeRateActive_ToFalse_WhenRatesAreEqual()
        {
            var repo = new List<Rate>
            {
                new Rate()
                {
                    Active = true,
                    Id = 1,
                    Year = 2015,
                    KmRate = 12,
                    TFCode = "Code",
                    TypeId = 1
                },
                new Rate()
                {
                    Active = true,
                    Id = 2,
                    Year = 2016,
                    KmRate = 12,
                    TFCode = "Code",
                    TypeId = 1
                },
                new Rate()
                {
                    Active = true,
                    Id = 3,
                    Year = 2018,
                    KmRate = 12,
                    TFCode = "Code",
                    TypeId = 1
                }
            };

            var newRate = new Rate()
            {
                Active = true,
                Year = 2018,
                KmRate = 12,
                Id = 4,
                TypeId = 1
            };

            Assert.AreEqual(2018, repo.ElementAt(2).Year);
            Assert.AreEqual(true, repo.ElementAt(2).Active);
            _uut.DeactivateExistingRate(repo.AsQueryable(), newRate);
            Assert.AreEqual(false, repo.ElementAt(2).Active);

        }

      
     
    }
}
