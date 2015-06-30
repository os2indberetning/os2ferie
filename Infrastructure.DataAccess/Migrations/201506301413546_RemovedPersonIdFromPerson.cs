namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedPersonIdFromPerson : DbMigration
    {
        public override void Up()
        {
            DropColumn("People", "PersonId");
        }
        
        public override void Down()
        {
            AddColumn("People", "PersonId", c => c.Int(nullable: false));
        }
    }
}
