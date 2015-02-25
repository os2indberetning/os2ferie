namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fixmigration : DbMigration
    {
        public override void Up()
        {
        //    DropColumn("Reports", "Timestamp");
        }
        
        public override void Down()
        {
          //  AddColumn("Reports", "Timestamp", c => c.String(unicode: false));
        }
    }
}
