namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIsExtraDistanceToDriveReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "IsExtraDistance", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("Reports", "IsExtraDistance");
        }
    }
}
