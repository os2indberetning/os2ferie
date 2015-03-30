namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedResponsibleLeaderAndApprovedByToReport : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Reports", "PersonId", "People");
            AddColumn("Reports", "ApprovedById", c => c.Int(nullable: false));
            AddColumn("Reports", "Person_Id", c => c.Int());
            CreateIndex("Reports", "ApprovedById");
            CreateIndex("Reports", "Person_Id");
            AddForeignKey("Reports", "ApprovedById", "People", "Id", cascadeDelete: true);
            AddForeignKey("Reports", "Person_Id", "People", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Reports", "Person_Id", "People");
            DropForeignKey("Reports", "ApprovedById", "People");
            DropIndex("Reports", new[] { "Person_Id" });
            DropIndex("Reports", new[] { "ApprovedById" });
            DropColumn("Reports", "Person_Id");
            DropColumn("Reports", "ApprovedById");
            AddForeignKey("Reports", "PersonId", "People", "Id", cascadeDelete: true);
        }
    }
}
