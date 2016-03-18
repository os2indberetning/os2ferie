namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVacationAccessSettingToOrgUnit : DbMigration
    {
        public override void Up()
        {
            AddColumn("OrgUnits", "HasAccessToVacation", c => c.Boolean(nullable: false));
            // TODO Test this
            Sql("UPDATE Orgunits SET HasAccessToVacation = 0");
        }
        
        public override void Down()
        {
            DropColumn("OrgUnits", "HasAccessToVacation");
        }
    }
}
