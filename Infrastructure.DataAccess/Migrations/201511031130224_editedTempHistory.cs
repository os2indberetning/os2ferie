namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editedTempHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("TempAddressHistories", "HomeIsDirty", c => c.Boolean(nullable: false));
            AddColumn("TempAddressHistories", "WorkIsDirty", c => c.Boolean(nullable: false));
            DropColumn("TempAddressHistories", "IsDirty");
        }
        
        public override void Down()
        {
            AddColumn("TempAddressHistories", "IsDirty", c => c.Boolean(nullable: false));
            DropColumn("TempAddressHistories", "WorkIsDirty");
            DropColumn("TempAddressHistories", "HomeIsDirty");
        }
    }
}
