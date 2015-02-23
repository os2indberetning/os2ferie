using System;
using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.BankAccounts
{
    [TestFixture]
    public class BankAccountTest : BaseControllerTest<BankAccount>
    {

        protected override BankAccount GetPatchReferenceEntity()
        {
            return new BankAccount
            {
                Id = 3,
                Number = "000666",
                Description = "Patched description"
            };
        }

        protected override void AsssertEqualEntities(BankAccount u1, BankAccount u2)
        {
            Assert.AreEqual(u1.Id, u2.Id, "ID of two bank accounts does not match");
            Assert.AreEqual(u1.Number, u2.Number, "Number of two bank accounts does not match");
            Assert.AreEqual(u1.Description, u2.Description, "Description of two bank accounts does not match");
        }


        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<BankAccount>),
                    typeof (BankAccountRepositoryMock))
            };
        }

        protected override BankAccount GetReferenceEntity1()
        {
            return new BankAccount
            {
                Id = 1,
                Number = "123456",
                Description = "Desc 1"
            };
        }

        protected override BankAccount GetReferenceEntity2()
        {
            return new BankAccount
            {
                Id = 2,
                Number = "654321",
                Description = "Desc 1"
             };
        }

        protected override BankAccount GetReferenceEntity3()
        {
            return new BankAccount
            {
                Id = 3,
                Number = "789123",
                Description = "Desc 3"
            };
        }

        protected override BankAccount GetPostReferenceEntity()
        {
            return new BankAccount
                    {
                        Id = 4,
                        Number = "666000",
                        Description = "Posted Description"
                    };
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'Id' : 4,
                        'Number' : '666000',
                        'Description' : 'Posted Description'
                    }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'Number' : '000666',
                        'Description' : 'Patched description'
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/BankAccounts";
        }

        protected override void ReSeed()
        {
            new BankAccountRepositoryMock().ReSeed();
        }
    }
}
