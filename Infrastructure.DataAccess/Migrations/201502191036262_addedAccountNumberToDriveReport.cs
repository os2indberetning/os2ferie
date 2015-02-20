namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedAccountNumberToDriveReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "AccountNumber", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Reports", "AccountNumber");
        }
    }
}
