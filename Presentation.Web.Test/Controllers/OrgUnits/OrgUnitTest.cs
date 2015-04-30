using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DomainModel;
using Core.DomainServices;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Presentation.Web.Test.Controllers.OrgUnits
{
    [TestFixture]
    public class OrgUnitTest : BaseControllerTest<OrgUnit>
    {

        protected override OrgUnit GetPatchReferenceEntity()
        {
            return new OrgUnit
            {
                Id = 3,
                Level = 0,
                LongDescription = "Altered by patch",
                ShortDescription = "ID 3 (patched)"
            };
        }

        protected override void AsssertEqualEntities(OrgUnit u1, OrgUnit u2)
        {
            Assert.AreEqual(u1.Id, u2.Id, "ID of two org units does not match");
            Assert.AreEqual(u1.Level, u2.Level, "Level of two org units does not match");
            Assert.AreEqual(u1.LongDescription, u2.LongDescription, "LongDescription of two org units does not match");
            Assert.AreEqual(u1.ShortDescription, u2.ShortDescription, "ShortDescription of two org units does not match");
        }

        protected override async Task DeleteShouldRemoveAnEntity()
        {
            //
            return;
        }

        protected override async Task PatchShouldAlterAnEntity()
        {
            //
            return;
        }

        protected override async Task PostShouldInsertAnEntity()
        {
            //
            return;
        }

        protected override List<KeyValuePair<Type, Type>> GetInjections()
        {
            return new List<KeyValuePair<Type, Type>>
            {
                new KeyValuePair<Type, Type>(typeof (IGenericRepository<OrgUnit>),
                    typeof (OrgUnitRepositoryMock))
            };
        }

        protected override OrgUnit GetReferenceEntity1()
        {
            return new OrgUnit
        {
            Id = 1,
            Level = 0,
            LongDescription = "Long dsc",
            ShortDescription = "ID 1"
        };
        }

        protected override OrgUnit GetReferenceEntity2()
        {
            return new OrgUnit
                    {
                        Id = 2,
                        Level = 0,
                        LongDescription = "Long dsc",
                        ShortDescription = "ID 2"
                    };
        }

        protected override OrgUnit GetReferenceEntity3()
        {
            return new OrgUnit
                    {
                        Id = 3,
                        Level = 0,
                        LongDescription = "Long dsc",
                        ShortDescription = "ID 3"
                    };
        }

        protected override OrgUnit GetPostReferenceEntity()
        {
            return new OrgUnit
                    {
                        Id = 4,
                        Level = 0,
                        LongDescription = "Added by post",
                        ShortDescription = "ID 4"
                    };
        }

        protected override string GetPostBodyContent()
        {
            return @"{
                        'Id' : 4,
                        'OrgId' : 2,
                        'ShortDescription' : 'ID 4',
                        'LongDescription' : 'Added by post',
                        'Level' : 0,
                        'ParentId' : 0
                    }";
        }

        protected override string GetPatchBodyContent()
        {
            return @"{
                        'ShortDescription' : 'ID 3 (patched)',
                        'LongDescription' : 'Altered by patch'
                    }";
        }

        protected override string GetUriPath()
        {
            return "/odata/OrgUnits";
        }

        protected override void ReSeed()
        {
            new OrgUnitRepositoryMock().ReSeed();
        }
    }
}
