namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAddressIdToOrgUnut : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("OrgUnits", "Address_Id", "Addresses");
            DropIndex("OrgUnits", new[] { "Address_Id" });
            RenameColumn(table: "OrgUnits", name: "Address_Id", newName: "AddressId");
            AlterColumn("OrgUnits", "AddressId", c => c.Int(nullable: false));
            CreateIndex("OrgUnits", "AddressId");
            AddForeignKey("OrgUnits", "AddressId", "Addresses", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("OrgUnits", "AddressId", "Addresses");
            DropIndex("OrgUnits", new[] { "AddressId" });
            AlterColumn("OrgUnits", "AddressId", c => c.Int());
            RenameColumn(table: "OrgUnits", name: "AddressId", newName: "Address_Id");
            CreateIndex("OrgUnits", "Address_Id");
            AddForeignKey("OrgUnits", "Address_Id", "Addresses", "Id");
        }
    }
}
