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
                        Lattitude = c.String(nullable: false, unicode: false),
                        Description = c.String(unicode: false),
                        Type = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Person_Id = c.Int(),
                        NextPoint_Id = c.Int(),
                        PersonalRoute_Id = c.Int(),
                        DriveReport_Id = c.Int(),
                        NextPoint_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.Person_Id, cascadeDelete: true)
                .ForeignKey("Addresses", t => t.NextPoint_Id)
                .ForeignKey("PersonalRoutes", t => t.PersonalRoute_Id, cascadeDelete: true)
                .ForeignKey("Reports", t => t.DriveReport_Id, cascadeDelete: true)
                .ForeignKey("Addresses", t => t.NextPoint_Id1)
                .Index(t => t.Person_Id)
                .Index(t => t.NextPoint_Id)
                .Index(t => t.PersonalRoute_Id)
                .Index(t => t.DriveReport_Id)
                .Index(t => t.NextPoint_Id1);
            
            CreateTable(
                "Reports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        status = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false, precision: 0),
                        EditedDate = c.DateTime(nullable: false, precision: 0),
                        Comment = c.String(nullable: false, unicode: false),
                        ClosedDate = c.DateTime(nullable: false, precision: 0),
                        ProcessedDate = c.DateTime(nullable: false, precision: 0),
                        Distance = c.Single(),
                        AmountToReimburse = c.Single(),
                        Porpuse = c.String(unicode: false),
                        KmRate = c.Single(),
                        DriveDate = c.DateTime(precision: 0),
                        FourKmRule = c.Boolean(),
                        StartsAtHome = c.Boolean(),
                        EndsAtHome = c.Boolean(),
                        Licenseplate = c.String(unicode: false),
                        Discriminator = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        Employment_Id = c.Int(nullable: false),
                        Person_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("Employments", t => t.Employment_Id, cascadeDelete: true)
                .ForeignKey("People", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.Employment_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "Employments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmploymentId = c.Int(nullable: false),
                        Position = c.String(nullable: false, unicode: false),
                        IsLeader = c.Boolean(nullable: false),
                        StartDate = c.DateTime(nullable: false, precision: 0),
                        EndDateTime = c.DateTime(nullable: false, precision: 0),
                        OrgUnit_Id = c.Int(nullable: false),
                        Person_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("OrgUnits", t => t.OrgUnit_Id, cascadeDelete: true)
                .ForeignKey("People", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.OrgUnit_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "OrgUnits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrgId = c.Int(nullable: false),
                        ShortDescription = c.String(nullable: false, unicode: false),
                        LongDescription = c.String(unicode: false),
                        Level = c.Int(nullable: false),
                        Parent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("OrgUnits", t => t.Parent_Id)
                .Index(t => t.Parent_Id);
            
            CreateTable(
                "Substitutes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false, precision: 0),
                        EndDate = c.DateTime(nullable: false, precision: 0),
                        OrgUnit_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("OrgUnits", t => t.OrgUnit_Id, cascadeDelete: true)
                .Index(t => t.OrgUnit_Id);
            
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
                        WorkDistanceOverride = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                ;
            
            CreateTable(
                "LicensePlates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Plate = c.String(nullable: false, unicode: false),
                        Description = c.String(nullable: false, unicode: false),
                        Person_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "MobileTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Guid = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Token = c.String(nullable: false, unicode: false),
                        Person_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "PersonalRoutes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, unicode: false),
                        Person_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "FileGenerationSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 0),
                        Generated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                ;
            
            CreateTable(
                "MailNotificationSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 0),
                        Notified = c.Boolean(nullable: false),
                        NextGenerationDate = c.DateTime(nullable: false, precision: 0),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("Addresses", "NextPoint_Id1", "Addresses");
            DropForeignKey("Addresses", "DriveReport_Id", "Reports");
            DropForeignKey("Employments", "Person_Id", "People");
            DropForeignKey("Reports", "Person_Id", "People");
            DropForeignKey("Reports", "Employment_Id", "Employments");
            DropForeignKey("Addresses", "PersonalRoute_Id", "PersonalRoutes");
            DropForeignKey("Addresses", "NextPoint_Id", "Addresses");
            DropForeignKey("PersonalRoutes", "Person_Id", "People");
            DropForeignKey("Addresses", "Person_Id", "People");
            DropForeignKey("MobileTokens", "Person_Id", "People");
            DropForeignKey("LicensePlates", "Person_Id", "People");
            DropForeignKey("Employments", "OrgUnit_Id", "OrgUnits");
            DropForeignKey("Substitutes", "OrgUnit_Id", "OrgUnits");
            DropForeignKey("OrgUnits", "Parent_Id", "OrgUnits");
            DropIndex("PersonalRoutes", new[] { "Person_Id" });
            DropIndex("MobileTokens", new[] { "Person_Id" });
            DropIndex("LicensePlates", new[] { "Person_Id" });
            DropIndex("Substitutes", new[] { "OrgUnit_Id" });
            DropIndex("OrgUnits", new[] { "Parent_Id" });
            DropIndex("Employments", new[] { "Person_Id" });
            DropIndex("Employments", new[] { "OrgUnit_Id" });
            DropIndex("Reports", new[] { "Person_Id" });
            DropIndex("Reports", new[] { "Employment_Id" });
            DropIndex("Addresses", new[] { "NextPoint_Id1" });
            DropIndex("Addresses", new[] { "DriveReport_Id" });
            DropIndex("Addresses", new[] { "PersonalRoute_Id" });
            DropIndex("Addresses", new[] { "NextPoint_Id" });
            DropIndex("Addresses", new[] { "Person_Id" });
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
