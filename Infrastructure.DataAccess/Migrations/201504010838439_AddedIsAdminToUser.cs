namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsAdminToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("People", "IsAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("People", "IsAdmin");
        }
    }
}
