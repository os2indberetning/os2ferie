namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSeniorCareAndOptionalVacationTypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("VacationReports", "CareCpr", c => c.String(unicode: false));
            AddColumn("VacationReports", "OptionalText", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("VacationReports", "OptionalText");
            DropColumn("VacationReports", "CareCpr");
        }
    }
}
