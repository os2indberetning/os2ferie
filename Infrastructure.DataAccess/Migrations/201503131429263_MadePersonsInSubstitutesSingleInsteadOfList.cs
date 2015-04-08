namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadePersonsInSubstitutesSingleInsteadOfList : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("SubstitutePersons", "Substitute_Id", "Substitutes");
            DropForeignKey("SubstitutePersons", "Person_Id", "People");
            DropIndex("SubstitutePersons", new[] { "Substitute_Id" });
            DropIndex("SubstitutePersons", new[] { "Person_Id" });
            AddColumn("Substitutes", "PersonId", c => c.Int(nullable: false));
            CreateIndex("Substitutes", "PersonId");
            AddForeignKey("Substitutes", "PersonId", "People", "Id", cascadeDelete: true);
            DropTable("SubstitutePersons");
        }
        
        public override void Down()
        {
            CreateTable(
                "SubstitutePersons",
                c => new
                    {
                        Substitute_Id = c.Int(nullable: false),
                        Person_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Substitute_Id, t.Person_Id })                ;
            
            DropForeignKey("Substitutes", "PersonId", "People");
            DropIndex("Substitutes", new[] { "PersonId" });
            DropColumn("Substitutes", "PersonId");
            CreateIndex("SubstitutePersons", "Person_Id");
            CreateIndex("SubstitutePersons", "Substitute_Id");
            AddForeignKey("SubstitutePersons", "Person_Id", "People", "Id", cascadeDelete: true);
            AddForeignKey("SubstitutePersons", "Substitute_Id", "Substitutes", "Id", cascadeDelete: true);
        }
    }
}
