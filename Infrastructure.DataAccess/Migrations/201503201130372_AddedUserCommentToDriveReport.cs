namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserCommentToDriveReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "UserComment", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Reports", "UserComment");
        }
    }
}
