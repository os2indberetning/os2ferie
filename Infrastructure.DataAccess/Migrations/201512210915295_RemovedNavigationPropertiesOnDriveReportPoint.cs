namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedNavigationPropertiesOnDriveReportPoint : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Addresses", "NextPoint_Id1", "Addresses");
            DropIndex("Addresses", new[] { "NextPoint_Id1" });
            DropColumn("Addresses", "NextPoint_Id1");
        }
        
        public override void Down()
        {
            AddColumn("Addresses", "NextPoint_Id1", c => c.Int());
            CreateIndex("Addresses", "NextPoint_Id1");
            AddForeignKey("Addresses", "NextPoint_Id1", "Addresses", "Id");
        }
    }
}
