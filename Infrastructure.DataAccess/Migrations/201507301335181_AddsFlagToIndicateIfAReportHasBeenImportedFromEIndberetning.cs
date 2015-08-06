namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddsFlagToIndicateIfAReportHasBeenImportedFromEIndberetning : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "IsOldMigratedReport", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("Reports", "IsOldMigratedReport");
        }
    }
}
