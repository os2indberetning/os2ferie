namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCreatedByIdToSubstitutes : DbMigration
    {
        public override void Up()
        {
            AddColumn("Substitutes", "CreatedById", c => c.Int());
            CreateIndex("Substitutes", "CreatedById");
            AddForeignKey("Substitutes", "CreatedById", "People", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Substitutes", "CreatedById", "People");
            DropIndex("Substitutes", new[] { "CreatedById" });
            DropColumn("Substitutes", "CreatedById");
        }
    }
}
