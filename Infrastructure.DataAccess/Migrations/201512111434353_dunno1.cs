namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dunno1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("AddressHistories", "HomeAddressId", "Addresses");
            DropForeignKey("AddressHistories", "WorkAddressId", "Addresses");
            DropIndex("AddressHistories", new[] { "HomeAddressId" });
            DropIndex("AddressHistories", new[] { "WorkAddressId" });
            AlterColumn("AddressHistories", "WorkAddressId", c => c.Int());
            AlterColumn("AddressHistories", "HomeAddressId", c => c.Int());
            CreateIndex("AddressHistories", "HomeAddressId");
            CreateIndex("AddressHistories", "WorkAddressId");
            AddForeignKey("AddressHistories", "HomeAddressId", "Addresses", "Id");
            AddForeignKey("AddressHistories", "WorkAddressId", "Addresses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("AddressHistories", "WorkAddressId", "Addresses");
            DropForeignKey("AddressHistories", "HomeAddressId", "Addresses");
            DropIndex("AddressHistories", new[] { "WorkAddressId" });
            DropIndex("AddressHistories", new[] { "HomeAddressId" });
            AlterColumn("AddressHistories", "HomeAddressId", c => c.Int(nullable: false));
            AlterColumn("AddressHistories", "WorkAddressId", c => c.Int(nullable: false));
            CreateIndex("AddressHistories", "WorkAddressId");
            CreateIndex("AddressHistories", "HomeAddressId");
            AddForeignKey("AddressHistories", "WorkAddressId", "Addresses", "Id", cascadeDelete: true);
            AddForeignKey("AddressHistories", "HomeAddressId", "Addresses", "Id", cascadeDelete: true);
        }
    }
}
