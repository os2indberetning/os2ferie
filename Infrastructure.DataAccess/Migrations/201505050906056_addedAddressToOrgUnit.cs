namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedAddressToOrgUnit : DbMigration
    {
        public override void Up()
        {
            AddColumn("OrgUnits", "Address_Id", c => c.Int());
            CreateIndex("OrgUnits", "Address_Id");
            AddForeignKey("OrgUnits", "Address_Id", "Addresses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("OrgUnits", "Address_Id", "Addresses");
            DropIndex("OrgUnits", new[] { "Address_Id" });
            DropColumn("OrgUnits", "Address_Id");
        }
    }
}
