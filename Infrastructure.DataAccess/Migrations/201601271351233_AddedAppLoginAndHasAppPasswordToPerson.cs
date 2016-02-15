namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAppLoginAndHasAppPasswordToPerson : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "AppLogins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(unicode: false),
                        PersonId = c.Int(nullable: false),
                        Password = c.String(unicode: false),
                        Salt = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            AddColumn("People", "HasAppPassword", c => c.Boolean(nullable: false));
            DropTable("TempAddressHistories");
        }
        
        public override void Down()
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
            
            DropForeignKey("AppLogins", "PersonId", "People");
            DropIndex("AppLogins", new[] { "PersonId" });
            DropColumn("People", "HasAppPassword");
            DropTable("AppLogins");
        }
    }
}
