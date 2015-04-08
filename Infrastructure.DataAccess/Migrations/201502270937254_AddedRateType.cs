namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRateType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "RateTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id)                ;
            
            AddColumn("MailNotificationSchedules", "Repeat", c => c.Boolean(nullable: false));
            AddColumn("Rates", "TypeId", c => c.Int(nullable: false));
            CreateIndex("Rates", "TypeId");
            AddForeignKey("Rates", "TypeId", "RateTypes", "Id", cascadeDelete: true);
            DropColumn("MailNotificationSchedules", "NextGenerationDateTimestamp");
            DropColumn("Rates", "Type");
        }
        
        public override void Down()
        {
            AddColumn("Rates", "Type", c => c.String(nullable: false, unicode: false));
            AddColumn("MailNotificationSchedules", "NextGenerationDateTimestamp", c => c.Long(nullable: false));
            DropForeignKey("Rates", "TypeId", "RateTypes");
            DropIndex("Rates", new[] { "TypeId" });
            DropColumn("Rates", "TypeId");
            DropColumn("MailNotificationSchedules", "Repeat");
            DropTable("RateTypes");
        }
    }
}
