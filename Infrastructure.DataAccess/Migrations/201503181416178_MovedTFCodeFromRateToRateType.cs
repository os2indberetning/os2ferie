namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedTFCodeFromRateToRateType : DbMigration
    {
        public override void Up()
        {
            AddColumn("RateTypes", "TFCode", c => c.String(nullable: false, unicode: false));
            DropColumn("Rates", "TFCode");
        }
        
        public override void Down()
        {
            AddColumn("Rates", "TFCode", c => c.String(nullable: false, unicode: false));
            DropColumn("RateTypes", "TFCode");
        }
    }
}
