namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedSubstituteTypeName : DbMigration
    {
        public override void Up()
        {
            AddColumn("Substitutes", "Type", c => c.Int(nullable: false));
            DropColumn("Substitutes", "Application");
            // TODO Test
            Sql("UPDATE Substitutes SET Type = 0");
        }
        
        public override void Down()
        {
            AddColumn("Substitutes", "Application", c => c.Int(nullable: false));
            DropColumn("Substitutes", "Type");
        }
    }
}
