namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Person_RecieveMail : DbMigration
    {
        public override void Up()
        {
            AddColumn("People", "RecieveMail", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("People", "RecieveMail");
        }
    }
}
