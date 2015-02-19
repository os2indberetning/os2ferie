namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropertiesToEmployment : DbMigration
    {
        public override void Up()
        {
            AddColumn("Employments", "EmploymentType", c => c.Int(nullable: false));
            AddColumn("Employments", "ExtraNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Employments", "ExtraNumber");
            DropColumn("Employments", "EmploymentType");
        }
    }
}
