namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedOrgUnitAddressType : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("OrgUnits", "Address_Id", "Addresses");
            //DropIndex("OrgUnits", new[] { "Address_Id" });
            AlterColumn("Addresses", "Id", c => c.Int(nullable: false));
            AlterColumn("Addresses", "StreetName", c => c.String(unicode: false));
            AlterColumn("Addresses", "StreetNumber", c => c.String(unicode: false));
            AlterColumn("Addresses", "Town", c => c.String(unicode: false));
            AlterColumn("Addresses", "Longitude", c => c.String(unicode: false));
            AlterColumn("Addresses", "Latitude", c => c.String(unicode: false));
            CreateIndex("Addresses", "Id");
            AddForeignKey("Addresses", "Id", "OrgUnits", "Id");
            DropColumn("OrgUnits", "Address_Id");
        }
        
        public override void Down()
        {
            AddColumn("OrgUnits", "Address_Id", c => c.Int());
            DropForeignKey("Addresses", "Id", "OrgUnits");
            DropIndex("Addresses", new[] { "Id" });
            AlterColumn("Addresses", "Latitude", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "Longitude", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "Town", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "StreetNumber", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "StreetName", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "Id", c => c.Int(nullable: false, identity: true));
            CreateIndex("OrgUnits", "Address_Id");
            AddForeignKey("OrgUnits", "Address_Id", "Addresses", "Id");
        }
    }
}
