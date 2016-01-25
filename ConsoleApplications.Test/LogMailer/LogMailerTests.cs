using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices.MailerService.Interface;
using Mail.LogMailer;
using NSubstitute;
using NUnit.Framework;
using Core.ApplicationServices;
using Ninject;

namespace ConsoleApplications.Test.LogMailer
{
    [TestFixture]
    public class LogMailerTests
    {


        [Test]
        public void Send_Everything()
        {

            var mailSub = NSubstitute.Substitute.For<IMailSender>();
            var parserSub = NSubstitute.Substitute.For<ILogParser>();

            parserSub
                .Messages(Arg.Any<List<string>>(), Arg.Any<DateTime>())
                .Returns(new List<string>
                {
                    "Exception doing post of type Core.DomainModel.Substitute",
                    "Exception doing post of type Core.DomainModel.Substitute",
                    "Exception doing post of type Core.DomainModel.Substitute",
                });

            var readerSub = NSubstitute.Substitute.For<ILogReader>();
                
            readerSub.Read(Arg.Any<string>())
                .Returns(new List<string>
                {
                    "28/12/2015 15:21:14 : Exception doing post of type Core.DomainModel.Substitute"
                });


            var logMailer = new Mail.LogMailer.LogMailer(parserSub, readerSub, mailSub);

            logMailer.Send();

            mailSub.Received().SendMail(Arg.Any<string>(), Arg.Any<string>(), @"Web:

Exception doing post of type Core.DomainModel.Substitute
Exception doing post of type Core.DomainModel.Substitute
Exception doing post of type Core.DomainModel.Substitute

DMZ: 

Exception doing post of type Core.DomainModel.Substitute
Exception doing post of type Core.DomainModel.Substitute
Exception doing post of type Core.DomainModel.Substitute

Mail: 

Exception doing post of type Core.DomainModel.Substitute
Exception doing post of type Core.DomainModel.Substitute
Exception doing post of type Core.DomainModel.Substitute");

        }

    }
}
