namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedAlternativeWorkAddressToEmployment : DbMigration
    {
        public override void Up()
        {
            AddColumn("Employments", "AlternativeWorkAddress_Id", c => c.Int());
            CreateIndex("Employments", "AlternativeWorkAddress_Id");
            AddForeignKey("Employments", "AlternativeWorkAddress_Id", "Addresses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Employments", "AlternativeWorkAddress_Id", "Addresses");
            DropIndex("Employments", new[] { "AlternativeWorkAddress_Id" });
            DropColumn("Employments", "AlternativeWorkAddress_Id");
        }
    }
}
