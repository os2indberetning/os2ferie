namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCostCenterToEmployment : DbMigration
    {
        public override void Up()
        {
            AddColumn("Employments", "CostCenter", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("Employments", "CostCenter");
        }
    }
}
