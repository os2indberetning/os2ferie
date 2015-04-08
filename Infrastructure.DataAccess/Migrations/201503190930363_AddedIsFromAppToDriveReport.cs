namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsFromAppToDriveReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "IsFromApp", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("Reports", "IsFromApp");
        }
    }
}
