namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedWorkDistanceOverrideToEmployment : DbMigration
    {
        public override void Up()
        {
            AddColumn("Employments", "WorkDistanceOverride", c => c.Double(nullable: false));
            DropColumn("People", "WorkDistanceOverride");
        }
        
        public override void Down()
        {
            AddColumn("People", "WorkDistanceOverride", c => c.Double(nullable: false));
            DropColumn("Employments", "WorkDistanceOverride");
        }
    }
}
