namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDirtyStringToCachedAddresses : DbMigration
    {
        public override void Up()
        {
            AddColumn("Addresses", "DirtyString", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Addresses", "DirtyString");
        }
    }
}
