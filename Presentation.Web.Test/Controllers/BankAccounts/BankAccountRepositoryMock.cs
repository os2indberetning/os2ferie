using System.Collections.Generic;

namespace Presentation.Web.Test.Controllers.BankAccounts
{
    class BankAccountRepositoryMock : GenericRepositoryMock<Core.DomainModel.BankAccount>
    {
        protected override List<Core.DomainModel.BankAccount> Seed()
        {
            return new List<Core.DomainModel.BankAccount>
            {
                new Core.DomainModel.BankAccount
                {
                    Id = 1,
                    Number = "123456",
                    Description = "Desc 1"
                },
                new Core.DomainModel.BankAccount
                {
                    Id = 2,
                    Number = "654321",
                    Description = "Desc 1"
                },
                new Core.DomainModel.BankAccount
                {
                    Id = 3,
                    Number = "789123",
                    Description = "Desc 3"
                }
            };
        }
    }
}
