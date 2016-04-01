namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIdentityToReportBase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Addresses", "DriveReportId", "DriveReports");
            DropPrimaryKey("DriveReports");
            DropPrimaryKey("VacationReports");
            AlterColumn("DriveReports", "Id", c => c.Int(nullable: false, identity: true));
            Sql("ALTER TABLE drivereports MODIFY Id INT(10) NOT NULL AUTO_INCREMENT");
            AlterColumn("VacationReports", "Id", c => c.Int(nullable: false, identity: true));
            Sql("ALTER TABLE vacationreports MODIFY Id INT(10) NOT NULL AUTO_INCREMENT");
            AddPrimaryKey("DriveReports", "Id");
            AddPrimaryKey("VacationReports", "Id");
            AddForeignKey("Addresses", "DriveReportId", "DriveReports", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Addresses", "DriveReportId", "DriveReports");
            DropPrimaryKey("VacationReports");
            DropPrimaryKey("DriveReports");
            AlterColumn("VacationReports", "Id", c => c.Int(nullable: false));
            AlterColumn("DriveReports", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("VacationReports", "Id");
            AddPrimaryKey("DriveReports", "Id");
            AddForeignKey("Addresses", "DriveReportId", "DriveReports", "Id");
        }
    }
}
