namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("Employments", "EndDate", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("Employments", "EndDateTime");
        }
        
        public override void Down()
        {
            AddColumn("Employments", "EndDateTime", c => c.DateTime(nullable: false, precision: 0));
            DropColumn("Employments", "EndDate");
        }
    }
}
