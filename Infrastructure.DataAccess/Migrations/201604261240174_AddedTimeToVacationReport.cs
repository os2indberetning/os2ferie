namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddedTimeToVacationReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("VacationReports", "StartTime", c => c.Time(precision: 0));
            AddColumn("VacationReports", "EndTime", c => c.Time(precision: 0));
        }

        public override void Down()
        {
            DropColumn("VacationReports", "EndTime");
            DropColumn("VacationReports", "StartTime");
        }
    }
}
