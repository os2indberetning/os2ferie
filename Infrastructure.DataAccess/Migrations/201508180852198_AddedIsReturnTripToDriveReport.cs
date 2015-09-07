namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsReturnTripToDriveReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "IsReturnTrip", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("Reports", "IsReturnTrip");
        }
    }
}
