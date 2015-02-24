namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDescriptionToMobileToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("MobileTokens", "Description", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("MobileTokens", "Description");
        }
    }
}
