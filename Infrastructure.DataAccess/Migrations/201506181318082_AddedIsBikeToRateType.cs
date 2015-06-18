namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsBikeToRateType : DbMigration
    {
        public override void Up()
        {
            AddColumn("RateTypes", "IsBike", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("RateTypes", "IsBike");
        }
    }
}
