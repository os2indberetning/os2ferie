namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropertiesToPerson : DbMigration
    {
        public override void Up()
        {
            AddColumn("People", "DistanceFromHomeToBorder", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("People", "DistanceFromHomeToBorder");
        }
    }
}
