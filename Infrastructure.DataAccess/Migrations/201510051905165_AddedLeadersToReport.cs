namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLeadersToReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "ResponsibleLeaderId", c => c.Int());
            AddColumn("Reports", "ActualLeaderId", c => c.Int());
            CreateIndex("Reports", "ActualLeaderId");
            AddForeignKey("Reports", "ActualLeaderId", "People", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Reports", "ActualLeaderId", "People");
            DropIndex("Reports", new[] { "ActualLeaderId" });
            DropColumn("Reports", "ActualLeaderId");
            DropColumn("Reports", "ResponsibleLeaderId");
        }
    }
}
