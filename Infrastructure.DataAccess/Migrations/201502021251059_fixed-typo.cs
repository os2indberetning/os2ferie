namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedtypo : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "Purpose", c => c.String(unicode: false));
            DropColumn("Reports", "Porpuse");
        }
        
        public override void Down()
        {
            AddColumn("Reports", "Porpuse", c => c.String(unicode: false));
            DropColumn("Reports", "Purpose");
        }
    }
}
