namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedVacationReportsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "VacationReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        CreatedDateTimestamp = c.Long(nullable: false),
                        EditedDateTimestamp = c.Long(nullable: false),
                        Comment = c.String(nullable: false, unicode: false),
                        ClosedDateTimestamp = c.Long(nullable: false),
                        ProcessedDateTimestamp = c.Long(nullable: false),
                        ApprovedById = c.Int(),
                        PersonId = c.Int(nullable: false),
                        EmploymentId = c.Int(nullable: false),
                        ResponsibleLeaderId = c.Int(),
                        ActualLeaderId = c.Int(),
                        StartTimestamp = c.Long(nullable: false),
                        EndTimestamp = c.Long(nullable: false),
                        VacationType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)           
                .ForeignKey("People", t => t.ApprovedById)
                .ForeignKey("People", t => t.PersonId, cascadeDelete: true)
                .ForeignKey("Employments", t => t.EmploymentId, cascadeDelete: true)
                .ForeignKey("People", t => t.ResponsibleLeaderId)
                .ForeignKey("People", t => t.ActualLeaderId)
                .Index(t => t.ApprovedById)
                .Index(t => t.PersonId)
                .Index(t => t.EmploymentId)
                .Index(t => t.ResponsibleLeaderId)
                .Index(t => t.ActualLeaderId);
        }
        
        public override void Down()
        {
            DropForeignKey("VacationReports", "ActualLeaderId", "People");
            DropForeignKey("VacationReports", "ResponsibleLeaderId", "People");
            DropForeignKey("VacationReports", "EmploymentId", "Employments");
            DropForeignKey("VacationReports", "PersonId", "People");
            DropForeignKey("VacationReports", "ApprovedById", "People");
            DropIndex("VacationReports", new[] { "ActualLeaderId" });
            DropIndex("VacationReports", new[] { "ResponsibleLeaderId" });
            DropIndex("VacationReports", new[] { "EmploymentId" });
            DropIndex("VacationReports", new[] { "PersonId" });
            DropIndex("VacationReports", new[] { "ApprovedById" });
            DropTable("VacationReports");
        }
    }
}
