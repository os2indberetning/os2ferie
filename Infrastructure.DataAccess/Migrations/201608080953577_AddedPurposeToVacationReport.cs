namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPurposeToVacationReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("VacationReports", "Purpose", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("VacationReports", "Purpose");
        }
    }
}
