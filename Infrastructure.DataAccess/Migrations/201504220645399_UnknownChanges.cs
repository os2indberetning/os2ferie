namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UnknownChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Addresses", "DriveReportId", c => c.Int());
            AlterColumn("Addresses", "PersonId", c => c.Int());
            AlterColumn("Addresses", "PersonalRouteId", c => c.Int());
            AlterColumn("Addresses", "NextPoint_Id", c => c.Int());
            AlterColumn("Addresses", "NextPoint_Id1", c => c.Int());
            AlterColumn("Reports", "ApprovedById", c => c.Int());
            AlterColumn("Reports", "PersonId", c => c.Int(nullable: false));
            AlterColumn("Reports", "EmploymentId", c => c.Int(nullable: false));
            AlterColumn("Reports", "Person_Id", c => c.Int());
            AlterColumn("Employments", "PersonId", c => c.Int(nullable: false));
            AlterColumn("Employments", "OrgUnitId", c => c.Int(nullable: false));
            AlterColumn("OrgUnits", "ParentId", c => c.Int());
            AlterColumn("Substitutes", "LeaderId", c => c.Int(nullable: false));
            AlterColumn("Substitutes", "SubId", c => c.Int(nullable: false));
            AlterColumn("Substitutes", "PersonId", c => c.Int(nullable: false));
            AlterColumn("Substitutes", "OrgUnitId", c => c.Int(nullable: false));
            AlterColumn("LicensePlates", "PersonId", c => c.Int(nullable: false));
            AlterColumn("MobileTokens", "PersonId", c => c.Int(nullable: false));
            AlterColumn("PersonalRoutes", "PersonId", c => c.Int(nullable: false));
            AlterColumn("Rates", "TypeId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Rates", "TypeId", c => c.Int(nullable: false));
            AlterColumn("PersonalRoutes", "PersonId", c => c.Int(nullable: false));
            AlterColumn("MobileTokens", "PersonId", c => c.Int(nullable: false));
            AlterColumn("LicensePlates", "PersonId", c => c.Int(nullable: false));
            AlterColumn("Substitutes", "OrgUnitId", c => c.Int(nullable: false));
            AlterColumn("Substitutes", "PersonId", c => c.Int(nullable: false));
            AlterColumn("Substitutes", "SubId", c => c.Int(nullable: false));
            AlterColumn("Substitutes", "LeaderId", c => c.Int(nullable: false));
            AlterColumn("OrgUnits", "ParentId", c => c.Int());
            AlterColumn("Employments", "OrgUnitId", c => c.Int(nullable: false));
            AlterColumn("Employments", "PersonId", c => c.Int(nullable: false));
            AlterColumn("Reports", "Person_Id", c => c.Int());
            AlterColumn("Reports", "EmploymentId", c => c.Int(nullable: false));
            AlterColumn("Reports", "PersonId", c => c.Int(nullable: false));
            AlterColumn("Reports", "ApprovedById", c => c.Int());
            AlterColumn("Addresses", "NextPoint_Id1", c => c.Int());
            AlterColumn("Addresses", "NextPoint_Id", c => c.Int());
            AlterColumn("Addresses", "PersonalRouteId", c => c.Int());
            AlterColumn("Addresses", "PersonId", c => c.Int());
            AlterColumn("Addresses", "DriveReportId", c => c.Int());
        }
    }
}
