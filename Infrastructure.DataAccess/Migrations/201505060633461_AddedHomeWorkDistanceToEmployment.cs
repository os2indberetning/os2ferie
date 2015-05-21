namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHomeWorkDistanceToEmployment : DbMigration
    {
        public override void Up()
        {
            AddColumn("Employments", "HomeWorkDistance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Employments", "HomeWorkDistance");
        }
    }
}
