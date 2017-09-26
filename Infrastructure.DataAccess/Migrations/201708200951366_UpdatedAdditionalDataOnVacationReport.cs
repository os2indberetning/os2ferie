namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedAdditionalDataOnVacationReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("VacationReports", "AdditionalData", c => c.String(unicode: false));
            DropColumn("VacationReports", "CareCpr");
        }
        
        public override void Down()
        {
            AddColumn("VacationReports", "CareCpr", c => c.String(unicode: false));
            DropColumn("VacationReports", "AdditionalData");
        }
    }
}
