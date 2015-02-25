namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update3 : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Reports", "KilometerAllowance", c => c.Int());
        }

        public override void Down()
        {
            //DropColumn("dbo.Reports", "KilometerAllowance");
        }
    }
}
