namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVacationBalanceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "VacationBalances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year = c.Int(nullable: false),
                        TotalVacationHours = c.Double(nullable: false),
                        VacationHours = c.Double(nullable: false),
                        TransferredHours = c.Double(nullable: false),
                        FreeVacationHours = c.Double(nullable: false),
                        UpdatedAt = c.Long(nullable: false),
                        EmploymentId = c.Int(nullable: false),
                        PersonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("Employments", t => t.EmploymentId, cascadeDelete: true)
                .ForeignKey("People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.EmploymentId)
                .Index(t => t.PersonId);
            AddColumn("VacationReports", "VacationYear", c => c.Int(nullable: false));
            AddColumn("VacationReports", "VacationHours", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("VacationBalances", "PersonId", "People");
            DropForeignKey("VacationBalances", "EmploymentId", "Employments");
            DropIndex("VacationBalances", new[] { "PersonId" });
            DropIndex("VacationBalances", new[] { "EmploymentId" });
            DropColumn("VacationReports", "VacationHours");
            DropColumn("VacationReports", "VacationYear");
            DropTable("VacationBalances");
        }
    }
}
