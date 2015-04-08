namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBoolIsPrimaryToLicensePlate : DbMigration
    {
        public override void Up()
        {
            AddColumn("LicensePlates", "IsPrimary", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("LicensePlates", "IsPrimary");
        }
    }
}
