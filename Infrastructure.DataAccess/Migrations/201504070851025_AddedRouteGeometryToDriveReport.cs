namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRouteGeometryToDriveReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("Reports", "RouteGeometry", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Reports", "RouteGeometry");
        }
    }
}
