namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeResponsibleLeaderNotIgnored : DbMigration
    {
        public override void Up()
        {
            CreateIndex("Reports", "ResponsibleLeaderId");
            AddForeignKey("Reports", "ResponsibleLeaderId", "People", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Reports", "ResponsibleLeaderId", "People");
            DropIndex("Reports", new[] { "ResponsibleLeaderId" });
        }
    }
}
