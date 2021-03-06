namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedAlternativeWorkAddressOnEmploymentToPersonalAddress : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Employments", "AlternativeWorkAddressId", "Addresses");
            DropIndex("Employments", new[] { "AlternativeWorkAddressId" });
            AlterColumn("Employments", "AlternativeWorkAddressId", c => c.Int(nullable: false));
            CreateIndex("Employments", "AlternativeWorkAddressId");
            AddForeignKey("Employments", "AlternativeWorkAddressId", "Addresses", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Employments", "AlternativeWorkAddressId", "Addresses");
            DropIndex("Employments", new[] { "AlternativeWorkAddressId" });
            AlterColumn("Employments", "AlternativeWorkAddressId", c => c.Int());
            CreateIndex("Employments", "AlternativeWorkAddressId");
            AddForeignKey("Employments", "AlternativeWorkAddressId", "Addresses", "Id");
        }
    }
}
