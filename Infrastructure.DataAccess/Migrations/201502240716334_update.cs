namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            DropColumn("MobileTokens", "Description");
        }
        
        public override void Down()
        {
            AddColumn("MobileTokens", "Description", c => c.String(unicode: false));
        }
    }
}
