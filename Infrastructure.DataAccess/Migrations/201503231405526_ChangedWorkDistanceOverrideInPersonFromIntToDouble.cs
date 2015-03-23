namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedWorkDistanceOverrideInPersonFromIntToDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("People", "WorkDistanceOverride", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("People", "WorkDistanceOverride", c => c.Int(nullable: false));
        }
    }
}
