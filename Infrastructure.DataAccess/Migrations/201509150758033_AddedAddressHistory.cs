namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAddressHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "AddressHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmploymentId = c.Int(nullable: false),
                        WorkAddressId = c.Int(nullable: false),
                        HomeAddressId = c.Int(nullable: false),
                        StartTimestamp = c.Long(nullable: false),
                        EndTimestamp = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("Employments", t => t.EmploymentId, cascadeDelete: true)
                .ForeignKey("Addresses", t => t.HomeAddressId, cascadeDelete: true)
                .ForeignKey("Addresses", t => t.WorkAddressId, cascadeDelete: true)
                .Index(t => t.EmploymentId)
                .Index(t => t.HomeAddressId)
                .Index(t => t.WorkAddressId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("AddressHistories", "WorkAddressId", "Addresses");
            DropForeignKey("AddressHistories", "HomeAddressId", "Addresses");
            DropForeignKey("AddressHistories", "EmploymentId", "Employments");
            DropIndex("AddressHistories", new[] { "WorkAddressId" });
            DropIndex("AddressHistories", new[] { "HomeAddressId" });
            DropIndex("AddressHistories", new[] { "EmploymentId" });
            DropTable("AddressHistories");
        }
    }
}
