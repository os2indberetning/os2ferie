namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("Reports","DriveDateTimestamp");
            CreateIndex("Reports", "status");
            CreateIndex("Employments","StartDateTimestamp");
            CreateIndex("Employments","EndDateTimestamp");
            CreateIndex("Employments","IsLeader");
            CreateIndex("People","CprNumber");
            CreateIndex("People","IsActive");
            CreateIndex("Addresses","Discriminator");
            CreateIndex("MobileTokens","Guid");
        }
        
        public override void Down()
        {
            DropIndex("Reports","DriveDateTimestamp");
            DropIndex("Reports","status");
            DropIndex("Employments","StartDateTimestamp");
            DropIndex("Employments","EndDateTimestamp");
            DropIndex("Employments","IsLeader");
            DropIndex("People","CprNumber");
            DropIndex("People","IsActive");
            DropIndex("Addresses","Discriminator");
            DropIndex("MobileTokens","Guid");
        }
    }
}
