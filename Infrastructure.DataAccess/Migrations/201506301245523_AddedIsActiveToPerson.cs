namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsActiveToPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("People", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("People", "IsActive");
        }
    }
}
