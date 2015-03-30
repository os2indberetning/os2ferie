namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeFullNameOnDriveReportNotIgnored : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "FullName", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Reports", "FullName"); 
        }
    }
}
