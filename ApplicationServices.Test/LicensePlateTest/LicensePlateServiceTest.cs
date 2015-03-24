using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Core.DomainServices;
using NSubstitute;
using NUnit.Framework;

namespace ApplicationServices.Test.LicensePlateTest
{
    [TestFixture]
    public class LicensePlateServiceTest
    {
        private ILicensePlateService uut;

        [Test]
        public void MakeLicensePlatePrimary_ShouldSetPrimaryTrue_RestFalse()
        {
            var repoMock = NSubstitute.Substitute.For<IGenericRepository<LicensePlate>>();
            repoMock.AsQueryable().ReturnsForAnyArgs(new List<LicensePlate>()
            {
                new LicensePlate()
                {
                    Description = "Beskrivelse 1",
                    IsPrimary = false,
                    Id = 1,
                    PersonId = 1,
                },
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = false,
                    Id = 2,
                    PersonId = 1,
                },
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = false,
                    Id = 3,
                    PersonId = 1,
                },

            }.AsQueryable());

            uut = new LicensePlateService(repoMock);
            uut.MakeLicensePlatePrimary(2);
            Assert.IsFalse(repoMock.AsQueryable().ElementAt(0).IsPrimary);
            Assert.IsFalse(repoMock.AsQueryable().ElementAt(2).IsPrimary);
            Assert.IsTrue(repoMock.AsQueryable().ElementAt(1).IsPrimary);
        }

        [Test]
        public void MakeLicensePlatePrimaryWithAllTrue_ShouldSetPrimaryTrue_RestFalse()
        {
            var repoMock = NSubstitute.Substitute.For<IGenericRepository<LicensePlate>>();
            repoMock.AsQueryable().ReturnsForAnyArgs(new List<LicensePlate>()
            {
                new LicensePlate()
                {
                    Description = "Beskrivelse 1",
                    IsPrimary = true,
                    Id = 1,
                    PersonId = 1,
                },
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = true,
                    Id = 2,
                    PersonId = 1,
                },
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = true,
                    Id = 3,
                    PersonId = 1,
                },

            }.AsQueryable());

            uut = new LicensePlateService(repoMock);
            uut.MakeLicensePlatePrimary(2);
            Assert.IsFalse(repoMock.AsQueryable().ElementAt(0).IsPrimary);
            Assert.IsFalse(repoMock.AsQueryable().ElementAt(2).IsPrimary);
            Assert.IsTrue(repoMock.AsQueryable().ElementAt(1).IsPrimary);
        }

        [Test]
        public void MakeLicensePlatePrimary_ShouldNotEdit_PlatesBelongingToDifferentUser()
        {
            var repoMock = NSubstitute.Substitute.For<IGenericRepository<LicensePlate>>();
            repoMock.AsQueryable().ReturnsForAnyArgs(new List<LicensePlate>()
            {
                new LicensePlate()
                {
                    Description = "Beskrivelse 1",
                    IsPrimary = false,
                    Id = 1,
                    PersonId = 1,
                },
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = false,
                    Id = 2,
                    PersonId = 1,
                },
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = false,
                    Id = 3,
                    PersonId = 1,
                },
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = true,
                    Id = 4,
                    PersonId = 2,
                },
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = true,
                    Id = 5,
                    PersonId = 2,
                },

            }.AsQueryable());

            uut = new LicensePlateService(repoMock);
            uut.MakeLicensePlatePrimary(2);
            Assert.IsTrue(repoMock.AsQueryable().ElementAt(1).IsPrimary);
            Assert.True(repoMock.AsQueryable().ElementAt(3).IsPrimary);
            Assert.IsTrue(repoMock.AsQueryable().ElementAt(4).IsPrimary);
        }

        [Test]
        public void MakeLicensePlatePrimary_ShouldReturnTrue_WhenPlateExists()
        {
            var repoMock = NSubstitute.Substitute.For<IGenericRepository<LicensePlate>>();
            repoMock.AsQueryable().ReturnsForAnyArgs(new List<LicensePlate>()
            {
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = true,
                    Id = 5,
                    PersonId = 2,
                },

            }.AsQueryable());

            uut = new LicensePlateService(repoMock);
            Assert.IsTrue(uut.MakeLicensePlatePrimary(5));
        }

        [Test]
        public void MakeLicensePlatePrimary_ShouldReturnFalse_WhenPlateDoesntExists()
        {
            var repoMock = NSubstitute.Substitute.For<IGenericRepository<LicensePlate>>();
            repoMock.AsQueryable().ReturnsForAnyArgs(new List<LicensePlate>()
            {
                new LicensePlate()
                {
                    Description = "Beskrivelse 2",
                    IsPrimary = true,
                    Id = 5,
                    PersonId = 2,
                },

            }.AsQueryable());

            uut = new LicensePlateService(repoMock);
            Assert.IsFalse(uut.MakeLicensePlatePrimary(2));
        }
    }
}
