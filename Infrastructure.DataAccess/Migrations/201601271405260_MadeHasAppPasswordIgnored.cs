namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeHasAppPasswordIgnored : DbMigration
    {
        public override void Up()
        {
            DropColumn("People", "HasAppPassword");
        }
        
        public override void Down()
        {
            AddColumn("People", "HasAppPassword", c => c.Boolean(nullable: false));
        }
    }
}
