using System;
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
                        LastName = "Jensen",
                        Initials = "JOJ"
                    },
                    Leader = new Person()
                    {
                       CprNumber = "123123",
                       FirstName = "Morten",
                       LastName = "Rasmussen",
                       Initials = "MR"
                    },
                    Person =
                        new Person()
                        {
                            CprNumber = "123123",
                            FirstName = "Morten",
                            LastName = "Rasmussen",
                            Initials = "MR"
                            
                        },
                },
                new Substitute()
                {
                    Sub = new Person()
                    {
                        CprNumber = "123123",
                        FirstName = "Jacob",
                        MiddleName = "Overgaard",
                        LastName = "Jensen",
                        Initials = "JOJ"
                    },
                    Leader = new Person()
                    {
                       CprNumber = "123123",
                       FirstName = "Morten",
                       LastName = "Rasmussen",
                       Initials = "MR"
                    },
                    Person = new Person()
                        {
                            CprNumber = "123123",
                            FirstName = "Jacob",
                            MiddleName = "Overgaard",
                            LastName = "Jensen",
                            Initials = "JOJ"
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
            Assert.AreEqual("Morten Rasmussen [MR]", repo[0].Leader.FullName);
            Assert.AreEqual("Jacob Overgaard Jensen [JOJ]", repo[0].Sub.FullName);
            Assert.AreEqual("Morten Rasmussen [MR]", repo[0].Person.FullName);

            Assert.AreEqual("Morten Rasmussen [MR]", repo[1].Leader.FullName);
            Assert.AreEqual("Jacob Overgaard Jensen [JOJ]", repo[1].Sub.FullName);
            Assert.AreEqual("Jacob Overgaard Jensen [JOJ]", repo[1].Person.FullName);
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

        [Test]
        public void GetStartOfDayTimestamp_shouldreturn_correctvalue()
        {
            var res = _uut.GetStartOfDayTimestamp(1431341025);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(res).ToLocalTime();
            Assert.AreEqual(11, dateTime.Day);
            Assert.AreEqual(0, dateTime.Hour);
            Assert.AreEqual(0, dateTime.Minute);
            Assert.AreEqual(0, dateTime.Second);

        }

        [Test]
        public void GetEndOfDayTimestamp_shouldreturn_correctvalue()
        {
            var res = _uut.GetEndOfDayTimestamp(1431304249);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(res).ToLocalTime();
            Assert.AreEqual(11, dateTime.Day);
            Assert.AreEqual(23, dateTime.Hour);
            Assert.AreEqual(59, dateTime.Minute);
            Assert.AreEqual(59, dateTime.Second);
        }
    }
}
