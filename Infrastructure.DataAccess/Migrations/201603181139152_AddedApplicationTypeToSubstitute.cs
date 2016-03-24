 namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedApplicationTypeToSubstitute : DbMigration
    {
        public override void Up()
        {
            AddColumn("Substitutes", "Application", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Substitutes", "Application");
        }
    }
}
