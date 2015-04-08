namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAccountTypeToBankAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("BankAccounts", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("BankAccounts", "Type");
        }
    }
}
