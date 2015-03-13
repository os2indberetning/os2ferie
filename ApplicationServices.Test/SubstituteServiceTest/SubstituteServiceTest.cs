using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.ApplicationServices;
using Core.DomainModel;
using NUnit.Framework;

namespace ApplicationServices.Test.SubstituteServiceTest
{
    [TestFixture]
    public class SubstituteServiceTest
    {

        private SubstituteService _uut;
        private List<Substitute> repo;

        [SetUp]
        public void SetUp()
        {
            _uut = new SubstituteService();

            repo = new List<Substitute>
            {
                new Substitute()
                {
                    Sub = new Person()
                    {
                        CprNumber = "123123",
                        FirstName = "Jacob",
                        MiddleName = "Overgaard",
                        LastName = "Jensen"
                    },
                    Leader = new Person()
                    {
                       CprNumber = "123123",
                       FirstName = "Morten",
                       LastName = "Rasmussen"
                    },
                    Person =
                        new Person()
                        {
                            CprNumber = "123123",
                            FirstName = "Morten",
                            LastName = "Rasmussen"
                        },
                },
                new Substitute()
                {
                    Sub = new Person()
                    {
                        CprNumber = "123123",
                        FirstName = "Jacob",
                        MiddleName = "Overgaard",
                        LastName = "Jensen"
                    },
                    Leader = new Person()
                    {
                       CprNumber = "123123",
                       FirstName = "Morten",
                       LastName = "Rasmussen"
                    },
                    Person = new Person()
                        {
                            CprNumber = "123123",
                            FirstName = "Jacob",
                            MiddleName = "Overgaard",
                            LastName = "Jensen"
                        },
                }
            };

        }

        [Test]
        public void AddFullName_ShouldAddFullName_ToLeaderSubAndPersons()
        {
            // Precondition
            Assert.AreEqual(null, repo[0].Leader.FullName);
            Assert.AreEqual(null, repo[0].Sub.FullName);

            Assert.AreEqual(null, repo[0].Person.FullName);


            Assert.AreEqual(null, repo[1].Leader.FullName);
            Assert.AreEqual(null, repo[1].Sub.FullName);
            Assert.AreEqual(null, repo[1].Person.FullName);


            // Act
            _uut.AddFullName(repo.AsQueryable());


            // Postcondition
            Assert.AreEqual("Morten Rasmussen", repo[0].Leader.FullName);
            Assert.AreEqual("Jacob Overgaard Jensen", repo[0].Sub.FullName);
            Assert.AreEqual("Morten Rasmussen", repo[0].Person.FullName);

            Assert.AreEqual("Morten Rasmussen", repo[1].Leader.FullName);
            Assert.AreEqual("Jacob Overgaard Jensen", repo[1].Sub.FullName);
            Assert.AreEqual("Jacob Overgaard Jensen", repo[1].Person.FullName);
        }

        [Test]
        public void ScrubCPR_ShouldRemoveCPR_FromLeaderAndSubAndPersons()
        {

            // Precondition

            Assert.AreEqual("123123", repo[0].Leader.CprNumber);
            Assert.AreEqual("123123", repo[0].Sub.CprNumber);
            Assert.AreEqual("123123", repo[0].Person.CprNumber);


            Assert.AreEqual("123123", repo[1].Leader.CprNumber);
            Assert.AreEqual("123123", repo[1].Sub.CprNumber);
            Assert.AreEqual("123123", repo[1].Person.CprNumber);

            // Act
            _uut.ScrubCprFromPersons(repo.AsQueryable());

            // Postcondition
            Assert.AreEqual("", repo[0].Leader.CprNumber);
            Assert.AreEqual("", repo[0].Sub.CprNumber);

            Assert.AreEqual("", repo[0].Person.CprNumber);


            Assert.AreEqual("", repo[1].Leader.CprNumber);
            Assert.AreEqual("", repo[1].Sub.CprNumber);

            Assert.AreEqual("", repo[1].Person.CprNumber);
        }
    }
}
