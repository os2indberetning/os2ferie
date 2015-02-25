namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "KilometerAllowance", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("Reports", "KilometerAllowance");
        }
    }
}
