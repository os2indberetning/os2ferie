namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Person : DbMigration
    {
        public override void Up()
        {
            AlterColumn("People", "WorkDistanceOverride", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("People", "WorkDistanceOverride", c => c.Single(nullable: false));
        }
    }
}
