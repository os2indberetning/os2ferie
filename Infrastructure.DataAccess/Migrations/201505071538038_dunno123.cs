namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dunno123 : DbMigration
    {
        public override void Up()
        {
           // RenameColumn(table: "Employments", name: "AlternativeWorkAddress_Id", newName: "AlternativeWorkAddressId");
        }
        
        public override void Down()
        {
          //  RenameColumn(table: "Employments", name: "AlternativeWorkAddressId", newName: "AlternativeWorkAddress_Id");
        }
    }
}
