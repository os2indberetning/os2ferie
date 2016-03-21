namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedErrorInVacationReports : DbMigration
    {
        public override void Up()
        {
            CreateIndex("VacationReports", "Status");
        }

        public override void Down()
        {
            DropIndex("VacationReports", new[] { "Status" });
        }
    }
}
