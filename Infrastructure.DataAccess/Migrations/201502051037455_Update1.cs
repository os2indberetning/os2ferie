namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Employments", "OrgUnit_Id", "OrgUnits");
            DropForeignKey("Employments", "Person_Id", "People");
            DropIndex("Employments", new[] { "OrgUnit_Id" });
            DropIndex("Employments", new[] { "Person_Id" });
            AddColumn("Reports", "CreatedDateTimestamp", c => c.Long(nullable: false));
            AddColumn("Reports", "EditedDateTimestamp", c => c.Long(nullable: false));
            AddColumn("Reports", "ClosedDateTimestamp", c => c.Long(nullable: false));
            AddColumn("Reports", "ProcessedDateTimestamp", c => c.Long(nullable: false));
            AddColumn("Reports", "DriveDateTimestamp", c => c.Long());
            AddColumn("Employments", "StartDateTimestamp", c => c.Long(nullable: false));
            AddColumn("Employments", "EndDateTimestamp", c => c.Long(nullable: false));
            AddColumn("Substitutes", "StartDateTimestamp", c => c.Long(nullable: false));
            AddColumn("Substitutes", "EndDateTimestamp", c => c.Long(nullable: false));
            AddColumn("FileGenerationSchedules", "DateTimestamp", c => c.Long(nullable: false));
            AddColumn("MailNotificationSchedules", "DateTimestamp", c => c.Long(nullable: false));
            AddColumn("MailNotificationSchedules", "NextGenerationDateTimestamp", c => c.Long(nullable: false));
            AlterColumn("Employments", "OrgUnit_Id", c => c.Int());
            AlterColumn("Employments", "Person_Id", c => c.Int());
            CreateIndex("Employments", "OrgUnit_Id");
            CreateIndex("Employments", "Person_Id");
            AddForeignKey("Employments", "OrgUnit_Id", "OrgUnits", "Id");
            AddForeignKey("Employments", "Person_Id", "People", "Id");
            DropColumn("Reports", "CreatedDate");
            DropColumn("Reports", "EditedDate");
            DropColumn("Reports", "ClosedDate");
            DropColumn("Reports", "ProcessedDate");
            DropColumn("Reports", "DriveDate");
            DropColumn("Employments", "StartDate");
            DropColumn("Employments", "EndDate");
            DropColumn("Substitutes", "StartDate");
            DropColumn("Substitutes", "EndDate");
            DropColumn("FileGenerationSchedules", "Date");
            DropColumn("MailNotificationSchedules", "Date");
            DropColumn("MailNotificationSchedules", "NextGenerationDate");
        }
        
        public override void Down()
        {
            AddColumn("MailNotificationSchedules", "NextGenerationDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("MailNotificationSchedules", "Date", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("FileGenerationSchedules", "Date", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("Substitutes", "EndDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("Substitutes", "StartDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("Employments", "EndDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("Employments", "StartDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("Reports", "DriveDate", c => c.DateTime(precision: 0));
            AddColumn("Reports", "ProcessedDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("Reports", "ClosedDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("Reports", "EditedDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("Reports", "CreatedDate", c => c.DateTime(nullable: false, precision: 0));
            DropForeignKey("Employments", "Person_Id", "People");
            DropForeignKey("Employments", "OrgUnit_Id", "OrgUnits");
            DropIndex("Employments", new[] { "Person_Id" });
            DropIndex("Employments", new[] { "OrgUnit_Id" });
            AlterColumn("Employments", "Person_Id", c => c.Int(nullable: false));
            AlterColumn("Employments", "OrgUnit_Id", c => c.Int(nullable: false));
            DropColumn("MailNotificationSchedules", "NextGenerationDateTimestamp");
            DropColumn("MailNotificationSchedules", "DateTimestamp");
            DropColumn("FileGenerationSchedules", "DateTimestamp");
            DropColumn("Substitutes", "EndDateTimestamp");
            DropColumn("Substitutes", "StartDateTimestamp");
            DropColumn("Employments", "EndDateTimestamp");
            DropColumn("Employments", "StartDateTimestamp");
            DropColumn("Reports", "DriveDateTimestamp");
            DropColumn("Reports", "ProcessedDateTimestamp");
            DropColumn("Reports", "ClosedDateTimestamp");
            DropColumn("Reports", "EditedDateTimestamp");
            DropColumn("Reports", "CreatedDateTimestamp");
            CreateIndex("Employments", "Person_Id");
            CreateIndex("Employments", "OrgUnit_Id");
            AddForeignKey("Employments", "Person_Id", "People", "Id", cascadeDelete: true);
            AddForeignKey("Employments", "OrgUnit_Id", "OrgUnits", "Id", cascadeDelete: true);
        }
    }
}
