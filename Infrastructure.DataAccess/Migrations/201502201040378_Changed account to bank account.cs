namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changedaccounttobankaccount : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "Accounts", newName: "BankAccounts");
            AlterColumn("BankAccounts", "Number", c => c.String(nullable: false, unicode: false));
            AlterColumn("BankAccounts", "Description", c => c.String(nullable: false, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("BankAccounts", "Description", c => c.String(unicode: false));
            AlterColumn("BankAccounts", "Number", c => c.String(unicode: false));
            RenameTable(name: "BankAccounts", newName: "Accounts");
        }
    }
}
