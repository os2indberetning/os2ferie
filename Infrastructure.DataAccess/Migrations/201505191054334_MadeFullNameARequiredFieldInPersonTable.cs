namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeFullNameARequiredFieldInPersonTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("People", "FullName", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("People", "FullName");
        }
    }
}
