namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDefaultKilometerAllowanceToOrg : DbMigration
    {
        public override void Up()
        {
            AddColumn("OrgUnits", "DefaultKilometerAllowance", c => c.Int(nullable: false, defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("OrgUnits", "DefaultKilometerAllowance");
        }
    }
}
