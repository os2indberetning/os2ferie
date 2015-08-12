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
            DropIndex("Reports",new[]{"DriveDateTimestamp"});
            DropIndex("Reports",new[]{"status"});
            DropIndex("Employments",new[]{"StartDateTimestamp"});
            DropIndex("Employments",new[]{"EndDateTimestamp"});
            DropIndex("Employments",new[]{"IsLeader"});
            DropIndex("People",new[]{"CprNumber"});
            DropIndex("People",new[]{"IsActive"});
            DropIndex("Addresses",new[]{"Discriminator"});
            DropIndex("MobileTokens",new[]{"Guid"});
        }
    }
}
