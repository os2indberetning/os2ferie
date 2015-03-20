namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHasAccessToFourKmRuleToOrgUnit : DbMigration
    {
        public override void Up()
        {
            AddColumn("OrgUnits", "HasAccessToFourKmRule", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("OrgUnits", "HasAccessToFourKmRule");
        }
    }
}
