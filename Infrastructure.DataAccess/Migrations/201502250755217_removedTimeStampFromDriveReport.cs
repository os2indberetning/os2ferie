namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedTimeStampFromDriveReport : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.Reports", "Timestamp");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Reports", "Timestamp", c => c.String(unicode: false));
        }
    }
}
