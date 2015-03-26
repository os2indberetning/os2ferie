namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dunno2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Reports", "ApprovedById", "People");
            DropIndex("Reports", new[] { "ApprovedById" });
            AlterColumn("Reports", "ApprovedById", c => c.Int());
            CreateIndex("Reports", "ApprovedById");
            AddForeignKey("Reports", "ApprovedById", "People", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Reports", "ApprovedById", "People");
            DropIndex("Reports", new[] { "ApprovedById" });
            AlterColumn("Reports", "ApprovedById", c => c.Int(nullable: false));
            CreateIndex("Reports", "ApprovedById");
            AddForeignKey("Reports", "ApprovedById", "People", "Id", cascadeDelete: true);
        }
    }
}
