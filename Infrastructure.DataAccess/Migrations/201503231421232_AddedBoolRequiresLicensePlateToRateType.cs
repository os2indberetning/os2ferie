namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBoolRequiresLicensePlateToRateType : DbMigration
    {
        public override void Up()
        {
            AddColumn("RateTypes", "RequiresLicensePlate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("RateTypes", "RequiresLicensePlate");
        }
    }
}
