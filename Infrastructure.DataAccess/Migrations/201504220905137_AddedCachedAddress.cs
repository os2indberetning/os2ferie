namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCachedAddress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "CachedAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DirtyAddress = c.String(nullable: false, unicode: false),
                        StreetName = c.String(nullable: false, unicode: false),
                        StreetNumber = c.String(nullable: false, unicode: false),
                        ZipCode = c.Int(nullable: false),
                        Town = c.String(nullable: false, unicode: false),
                        Longitude = c.String(nullable: false, unicode: false),
                        Latitude = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id)                ;
            
        }
        
        public override void Down()
        {
            DropTable("CachedAddresses");
        }
    }
}
