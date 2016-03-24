namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedDriveReportsTableFromReports : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Reports", "EmploymentId", "Employments");
            DropForeignKey("Reports", "ActualLeaderId", "People");
            DropForeignKey("Reports", "ApprovedById", "People");
            DropForeignKey("Reports", "Person_Id", "People");
            DropForeignKey("Reports", "ResponsibleLeaderId", "People");
            DropForeignKey("Addresses", "DriveReportId", "Reports");
            RenameTable(name: "Reports", newName: "DriveReports");
            AddForeignKey("DriveReports", "EmploymentId", "Employments", "Id");
            AddForeignKey("DriveReports", "ActualLeaderId", "People", "Id");
            AddForeignKey("DriveReports", "ApprovedById", "People", "Id");
            AddForeignKey("DriveReports", "Person_Id", "People", "Id");
            AddForeignKey("DriveReports", "ResponsibleLeaderId", "People", "Id");
            AddForeignKey("Addresses", "DriveReportId", "DriveReports", "Id");
            DropColumn("DriveReports", "Discriminator");
        }
        
        public override void Down()
        {
            DropForeignKey("DriveReports", "EmploymentId", "Employments");
            DropForeignKey("DriveReports", "ActualLeaderId", "People");
            DropForeignKey("DriveReports", "ApprovedById", "People");
            DropForeignKey("DriveReports", "Person_Id", "People");
            DropForeignKey("DriveReports", "ResponsibleLeaderId", "People");
            DropForeignKey("Addresses", "DriveReportId", "DriveReports");
            RenameTable(name: "DriveReports", newName: "Reports");
            AddForeignKey("Reports", "EmploymentId", "Employments", "Id");
            AddForeignKey("Reports", "ActualLeaderId", "People", "Id");
            AddForeignKey("Reports", "ApprovedById", "People", "Id");
            AddForeignKey("Reports", "Person_Id", "People", "Id");
            AddForeignKey("Reports", "ResponsibleLeaderId", "People", "Id");
            AddColumn("Reports", "Discriminator", x => x.String());
            AddForeignKey("Addresses", "DriveReportId", "Reports", "Id");
        }
    }
}
