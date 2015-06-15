namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedPayRoleTimestampToMailNotificationSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("MailNotificationSchedules", "PayRoleTimestamp", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("MailNotificationSchedules", "PayRoleTimestamp");
        }
    }
}
