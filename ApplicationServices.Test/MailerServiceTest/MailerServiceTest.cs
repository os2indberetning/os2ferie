using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Core.ApplicationServices;
using Core.ApplicationServices.MailerService;
using Core.ApplicationServices.MailerService.Impl;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Infrastructure.DataAccess;
using Ninject;
using Ninject.MockingKernel;
using Ninject.MockingKernel.NSubstitute;
using Ninject.Modules;
using NSubstitute;
using NUnit.Framework;
using Substitute = NSubstitute.Substitute;


namespace ApplicationServices.Test.MailerServiceTest
{

    [TestFixture]
    public class MailerServiceTest
    {
        [Test]
        public void Run()
        {

            var repMock = Substitute.For<IGenericRepository<DriveReport>>();
            var subMock = Substitute.For<IGenericRepository<Core.DomainModel.Substitute>>();

           

            var mailService = new MailService(repMock, subMock);
            var res = mailService.GetLeadersWithPendingReportsMails();


        }


    }

}