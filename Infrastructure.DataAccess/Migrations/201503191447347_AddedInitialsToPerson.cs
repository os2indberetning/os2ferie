namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedInitialsToPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("People", "Initials", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("People", "Initials");
        }
    }
}
