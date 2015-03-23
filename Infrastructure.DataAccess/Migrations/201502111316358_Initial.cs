namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StreetName = c.String(nullable: false, unicode: false),
                        StreetNumber = c.String(nullable: false, unicode: false),
                        ZipCode = c.Int(nullable: false),
                        Town = c.String(nullable: false, unicode: false),
                        Longitude = c.String(nullable: false, unicode: false),
                        Latitude = c.String(nullable: false, unicode: false),
                        Description = c.String(unicode: false),
                        NextPointId = c.Int(),
                        PreviousPointId = c.Int(),
                        DriveReportId = c.Int(),
                        Type = c.Int(),
                        PersonId = c.Int(),
                        NextPointId1 = c.Int(),
                        PreviousPointId1 = c.Int(),
                        PersonalRouteId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        NextPoint_Id = c.Int(),
                        NextPoint_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.PersonId, cascadeDelete: true)
                .ForeignKey("Addresses", t => t.NextPoint_Id)
                .ForeignKey("PersonalRoutes", t => t.PersonalRouteId, cascadeDelete: true)
                .ForeignKey("Reports", t => t.DriveReportId, cascadeDelete: true)
                .ForeignKey("Addresses", t => t.NextPoint_Id1)
                .Index(t => t.DriveReportId)
                .Index(t => t.PersonId)
                .Index(t => t.PersonalRouteId)
                .Index(t => t.NextPoint_Id)
                .Index(t => t.NextPoint_Id1);
            
            CreateTable(
                "Reports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        status = c.Int(nullable: false),
                        CreatedDateTimestamp = c.Long(nullable: false),
                        EditedDateTimestamp = c.Long(nullable: false),
                        Comment = c.String(nullable: false, unicode: false),
                        ClosedDateTimestamp = c.Long(nullable: false),
                        ProcessedDateTimestamp = c.Long(nullable: false),
                        PersonId = c.Int(nullable: false),
                        EmploymentId = c.Int(nullable: false),
                        Distance = c.Single(),
                        AmountToReimburse = c.Single(),
                        Purpose = c.String(unicode: false),
                        KmRate = c.Single(),
                        DriveDateTimestamp = c.Long(),
                        FourKmRule = c.Boolean(),
                        StartsAtHome = c.Boolean(),
                        EndsAtHome = c.Boolean(),
                        LicensePlate = c.String(unicode: false),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("Employments", t => t.EmploymentId, cascadeDelete: true)
                .ForeignKey("People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId)
                .Index(t => t.EmploymentId);
            
            CreateTable(
                "Employments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmploymentId = c.Int(nullable: false),
                        Position = c.String(nullable: false, unicode: false),
                        IsLeader = c.Boolean(nullable: false),
                        StartDateTimestamp = c.Long(nullable: false),
                        EndDateTimestamp = c.Long(nullable: false),
                        PersonId = c.Int(nullable: false),
                        OrgUnitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("OrgUnits", t => t.OrgUnitId, cascadeDelete: true)
                .ForeignKey("People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId)
                .Index(t => t.OrgUnitId);
            
            CreateTable(
                "OrgUnits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrgId = c.Int(nullable: false),
                        ShortDescription = c.String(nullable: false, unicode: false),
                        LongDescription = c.String(unicode: false),
                        Level = c.Int(nullable: false),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("OrgUnits", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "Substitutes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDateTimestamp = c.Long(nullable: false),
                        EndDateTimestamp = c.Long(nullable: false),
                        LeaderId = c.Int(nullable: false),
                        SubId = c.Int(nullable: false),
                        OrgUnitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.LeaderId, cascadeDelete: true)
                .ForeignKey("OrgUnits", t => t.OrgUnitId, cascadeDelete: true)
                .ForeignKey("People", t => t.SubId, cascadeDelete: true)
                .Index(t => t.LeaderId)
                .Index(t => t.SubId)
                .Index(t => t.OrgUnitId);
            
            CreateTable(
                "People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CprNumber = c.String(nullable: false, maxLength: 10, fixedLength: true, storeType: "nchar"),
                        PersonId = c.Int(nullable: false),
                        FirstName = c.String(nullable: false, unicode: false),
                        MiddleName = c.String(unicode: false),
                        LastName = c.String(nullable: false, unicode: false),
                        Mail = c.String(nullable: false, unicode: false),
                        WorkDistanceOverride = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                ;
            
            CreateTable(
                "LicensePlates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Plate = c.String(nullable: false, unicode: false),
                        Description = c.String(nullable: false, unicode: false),
                        PersonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "MobileTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Guid = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Token = c.String(nullable: false, unicode: false),
                        PersonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "PersonalRoutes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, unicode: false),
                        PersonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "FileGenerationSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTimestamp = c.Long(nullable: false),
                        Generated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                ;
            
            CreateTable(
                "MailNotificationSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTimestamp = c.Long(nullable: false),
                        Notified = c.Boolean(nullable: false),
                        NextGenerationDateTimestamp = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                ;
            
            CreateTable(
                "Rates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year = c.Int(nullable: false),
                        TFCode = c.String(nullable: false, unicode: false),
                        KmRate = c.Single(nullable: false),
                        Type = c.String(nullable: false, unicode: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                ;
            
            CreateTable(
                "SubstitutePersons",
                c => new
                    {
                        Substitute_Id = c.Int(nullable: false),
                        Person_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Substitute_Id, t.Person_Id })                
                .ForeignKey("Substitutes", t => t.Substitute_Id, cascadeDelete: true)
                .ForeignKey("People", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.Substitute_Id)
                .Index(t => t.Person_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Addresses", "NextPoint_Id1", "Addresses");
            DropForeignKey("Addresses", "DriveReportId", "Reports");
            DropForeignKey("Substitutes", "SubId", "People");
            DropForeignKey("SubstitutePersons", "Person_Id", "People");
            DropForeignKey("SubstitutePersons", "Substitute_Id", "Substitutes");
            DropForeignKey("Substitutes", "OrgUnitId", "OrgUnits");
            DropForeignKey("Substitutes", "LeaderId", "People");
            DropForeignKey("Reports", "PersonId", "People");
            DropForeignKey("Reports", "EmploymentId", "Employments");
            DropForeignKey("Addresses", "PersonalRouteId", "PersonalRoutes");
            DropForeignKey("Addresses", "NextPoint_Id", "Addresses");
            DropForeignKey("PersonalRoutes", "PersonId", "People");
            DropForeignKey("Addresses", "PersonId", "People");
            DropForeignKey("MobileTokens", "PersonId", "People");
            DropForeignKey("LicensePlates", "PersonId", "People");
            DropForeignKey("Employments", "PersonId", "People");
            DropForeignKey("Employments", "OrgUnitId", "OrgUnits");
            DropForeignKey("OrgUnits", "ParentId", "OrgUnits");
            DropIndex("SubstitutePersons", new[] { "Person_Id" });
            DropIndex("SubstitutePersons", new[] { "Substitute_Id" });
            DropIndex("PersonalRoutes", new[] { "PersonId" });
            DropIndex("MobileTokens", new[] { "PersonId" });
            DropIndex("LicensePlates", new[] { "PersonId" });
            DropIndex("Substitutes", new[] { "OrgUnitId" });
            DropIndex("Substitutes", new[] { "SubId" });
            DropIndex("Substitutes", new[] { "LeaderId" });
            DropIndex("OrgUnits", new[] { "ParentId" });
            DropIndex("Employments", new[] { "OrgUnitId" });
            DropIndex("Employments", new[] { "PersonId" });
            DropIndex("Reports", new[] { "EmploymentId" });
            DropIndex("Reports", new[] { "PersonId" });
            DropIndex("Addresses", new[] { "NextPoint_Id1" });
            DropIndex("Addresses", new[] { "NextPoint_Id" });
            DropIndex("Addresses", new[] { "PersonalRouteId" });
            DropIndex("Addresses", new[] { "PersonId" });
            DropIndex("Addresses", new[] { "DriveReportId" });
            DropTable("SubstitutePersons");
            DropTable("Rates");
            DropTable("MailNotificationSchedules");
            DropTable("FileGenerationSchedules");
            DropTable("PersonalRoutes");
            DropTable("MobileTokens");
            DropTable("LicensePlates");
            DropTable("People");
            DropTable("Substitutes");
            DropTable("OrgUnits");
            DropTable("Employments");
            DropTable("Reports");
            DropTable("Addresses");
        }
    }
}
