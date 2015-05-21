namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedMiddleNameFromPerson : DbMigration
    {
        public override void Up()
        {
            DropColumn("People", "MiddleName");
        }
        
        public override void Down()
        {
            AddColumn("People", "MiddleName", c => c.String(unicode: false));
        }
    }
}
