namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedIsReturnTripOnDriveReportToIsRoundTrip : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "IsRoundTrip", c => c.Boolean());
            DropColumn("Reports", "IsReturnTrip");
        }
        
        public override void Down()
        {
            AddColumn("Reports", "IsReturnTrip", c => c.Boolean());
            DropColumn("Reports", "IsRoundTrip");
        }
    }
}
