namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d : DbMigration
    {
        public override void Up()
        {
            AddColumn("Employments", "AlternativeWorkAddressId", c => c.Int(nullable: false));
            CreateIndex("Employments", "AlternativeWorkAddressId");
            AddForeignKey("Employments", "AlternativeWorkAddressId", "Addresses", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Employments", "AlternativeWorkAddressId", "Addresses");
            DropIndex("Employments", new[] { "AlternativeWorkAddressId" });
            DropColumn("Employments", "AlternativeWorkAddressId");
        }
    }
}
