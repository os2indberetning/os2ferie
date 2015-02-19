namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedDataTypeInDriveReportFromFloatToDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Reports", "Distance", c => c.Double());
            AlterColumn("Reports", "AmountToReimburse", c => c.Double());
            AlterColumn("Reports", "KmRate", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("Reports", "KmRate", c => c.Single());
            AlterColumn("Reports", "AmountToReimburse", c => c.Single());
            AlterColumn("Reports", "Distance", c => c.Single());
        }
    }
}
