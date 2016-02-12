namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGuIdToAppLogin : DbMigration
    {
        public override void Up()
        {
            AddColumn("AppLogins", "GuId", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("AppLogins", "GuId");
        }
    }
}
