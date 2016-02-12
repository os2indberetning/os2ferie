namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAppLogin : DbMigration
    {
        public override void Up()
        {
            AlterColumn("AppLogins", "UserName", c => c.String(nullable: false, unicode: false));
            AlterColumn("AppLogins", "Password", c => c.String(nullable: false, unicode: false));
            AlterColumn("AppLogins", "Salt", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("AppLogins", "Salt", c => c.String(unicode: false));
            AlterColumn("AppLogins", "Password", c => c.String(unicode: false));
            AlterColumn("AppLogins", "UserName", c => c.String(unicode: false));
        }
    }
}
