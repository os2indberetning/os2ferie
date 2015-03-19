using System;
using System.Collections.Generic;
using System.Linq;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Ninject;
using NUnit.Framework;

namespace ApplicationServices.Test.PersonService
{
    [TestFixture]
    public class PersonServiceTest
    {
        private IQueryable<Person> persons = new List<Person>
        {
            new Person
            {
                Id = 1,
                FirstName = "Morten",
                LastName = "Rasmussen",
                CprNumber = "1234567890",
                Initials = "MR"
            },
            new Person
            {
                Id = 2,
                FirstName = "Morten",
                LastName = "Jørgensen",
                CprNumber = "0987654321",
                Initials = "MJ"
            },
            new Person
            {
                Id = 3,
                FirstName = "Jacob",
                MiddleName = "Overgaard",
                LastName = "Jensen",
                CprNumber = "456456456",
                Initials = "JOJ"
            }
        }.AsQueryable();

        [Test]
        public void ScrubCprsShouldRemoveCprNumbers()
        {
            foreach (var person in persons)
            {
                Assert.AreNotEqual("", person.CprNumber, "Person should have a CPR number before scrubbing");
            }
            persons = NinjectWebKernel.CreateKernel().Get<IPersonService>().ScrubCprFromPersons(persons);
            foreach (var person in persons)
            {
                Assert.AreEqual("", person.CprNumber, "Person should not have a CPR number before scrubbing");
            }
        }

        [Test]
        public void AddFullName_WithMiddleName_ShouldAddCorrectFullName()
        {
            NinjectWebKernel.CreateKernel().Get<IPersonService>().AddFullName(persons);
            Assert.AreEqual("Jacob Overgaard Jensen [JOJ]", persons.Single(p => p.Id == 3).FullName);
        }

        [Test]
        public void AddFullName_WithoutMiddleName_ShouldAddCorrectFullName()
        {
            NinjectWebKernel.CreateKernel().Get<IPersonService>().AddFullName(persons);
            Assert.AreEqual("Morten Rasmussen [MR]", persons.Single(p => p.Id == 1).FullName);
        }

        [Test]
        public void AddFullName_WithPersonsNull_ShouldNotThrowException()
        {
            Assert.DoesNotThrow(() => NinjectWebKernel.CreateKernel().Get<IPersonService>().AddFullName(null));
        }
    }
}
