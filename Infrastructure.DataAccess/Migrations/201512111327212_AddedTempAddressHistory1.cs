namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTempAddressHistory1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TempAddressHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AktivFra = c.Long(nullable: false),
                        AktivTil = c.Long(nullable: false),
                        MaNr = c.Int(nullable: false),
                        Navn = c.String(unicode: false),
                        HjemmeAdresse = c.String(unicode: false),
                        HjemmePostNr = c.Int(nullable: false),
                        HjemmeBy = c.String(unicode: false),
                        HjemmeLand = c.String(unicode: false),
                        ArbejdsAdresse = c.String(unicode: false),
                        ArbejdsPostNr = c.Int(nullable: false),
                        ArbejdsBy = c.String(unicode: false),
                        HomeIsDirty = c.Boolean(nullable: false),
                        WorkIsDirty = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                ;
            
            AddColumn("AddressHistories", "IsMigrated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("AddressHistories", "IsMigrated");
            DropTable("TempAddressHistories");
        }
    }
}
