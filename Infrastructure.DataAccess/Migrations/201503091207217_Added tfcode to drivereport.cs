namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedtfcodetodrivereport : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "TFCode", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Reports", "TFCode");
        }
    }
}
