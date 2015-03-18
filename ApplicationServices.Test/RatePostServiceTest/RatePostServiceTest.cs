using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices;
using Core.DomainModel;
using NUnit.Framework;

namespace ApplicationServices.Test.RatePostServiceTest
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
                    Type = new RateType()
                    {
                        TFCode = "Code"
                    }
                },
                new Rate()
                {
                    Active = true,
                    Id = 2,
                    Year = 2016,
                    KmRate = 12,
                    Type = new RateType()
                    {
                        TFCode = "Code"
                    }
                },
                new Rate()
                {
                    Active = true,
                    Id = 3,
                    Year = 2017,
                    KmRate = 12,
                    Type = new RateType()
                    {
                        TFCode = "Code"
                    }
                }
            };

            var newRate = new Rate()
            {
                Active = true,
                Year = 2018,
                KmRate = 12,
                Id = 4,
                Type = new RateType()
                {
                    TFCode = "Code"
                }
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
                    Type = new RateType()
                    {
                        TFCode = "Code"
                    }
                },
                new Rate()
                {
                    Active = true,
                    Id = 2,
                    Year = 2016,
                    KmRate = 12,
                    Type = new RateType()
                    {
                        TFCode = "Code"
                    }
                },
                new Rate()
                {
                    Active = true,
                    Id = 3,
                    Year = 2018,
                    KmRate = 12,
                    Type = new RateType()
                    {
                        TFCode = "Code"
                    }
                }
            };

            var newRate = new Rate()
            {
                Active = true,
                Year = 2018,
                KmRate = 12,
                Id = 4,
                Type = new RateType()
                {
                    TFCode = "Code"
                }
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
                    Type = new RateType()
                    {
                        TFCode = "Code"
                    }
                },
                new Rate()
                {
                    Active = true,
                    Id = 2,
                    Year = 2016,
                    KmRate = 12,
                    Type = new RateType()
                    {
                        TFCode = "Code"
                    }
                },
                new Rate()
                {
                    Active = true,
                    Id = 3,
                    Year = 2018,
                    KmRate = 12,
                    Type = new RateType()
                    {
                        TFCode = "Code"
                    }
                }
            };

            var newRate = new Rate()
            {
                Active = true,
                Year = 2018,
                KmRate = 12,
                Id = 4,
                Type = new RateType()
                {
                    TFCode = "Code"
                }
            };

            Assert.AreEqual(2018, repo.ElementAt(2).Year);
            Assert.AreEqual(true, repo.ElementAt(2).Active);
            _uut.DeactivateExistingRate(repo.AsQueryable(), newRate);
            Assert.AreEqual(false, repo.ElementAt(2).Active);

        }



    }
}
