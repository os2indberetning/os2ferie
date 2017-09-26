namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sqlsupport : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Addresses", "StreetName", c => c.String(nullable: false));
            AlterColumn("Addresses", "StreetNumber", c => c.String(nullable: false));
            AlterColumn("Addresses", "Town", c => c.String(nullable: false));
            AlterColumn("Addresses", "Longitude", c => c.String(nullable: false));
            AlterColumn("Addresses", "Latitude", c => c.String(nullable: false));
            AlterColumn("Addresses", "Description", c => c.String());
            AlterColumn("Addresses", "DirtyString", c => c.String());
            AlterColumn("DriveReports", "Comment", c => c.String(nullable: false));
            AlterColumn("DriveReports", "Purpose", c => c.String(nullable: false));
            AlterColumn("DriveReports", "LicensePlate", c => c.String());
            AlterColumn("DriveReports", "FullName", c => c.String());
            AlterColumn("DriveReports", "AccountNumber", c => c.String());
            AlterColumn("DriveReports", "TFCode", c => c.String(nullable: false));
            AlterColumn("DriveReports", "UserComment", c => c.String());
            AlterColumn("DriveReports", "RouteGeometry", c => c.String());
            AlterColumn("VacationReports", "Comment", c => c.String(nullable: false));
            AlterColumn("VacationReports", "StartTime", c => c.Time(precision: 7));
            AlterColumn("VacationReports", "EndTime", c => c.Time(precision: 7));
            AlterColumn("VacationReports", "Purpose", c => c.String());
            AlterColumn("People", "FirstName", c => c.String(nullable: false));
            AlterColumn("People", "LastName", c => c.String(nullable: false));
            AlterColumn("People", "Mail", c => c.String(nullable: false));
            AlterColumn("Employments", "Position", c => c.String(nullable: false));
            AlterColumn("OrgUnits", "ShortDescription", c => c.String(nullable: false));
            AlterColumn("OrgUnits", "LongDescription", c => c.String());
            AlterColumn("LicensePlates", "Plate", c => c.String(nullable: false));
            AlterColumn("LicensePlates", "Description", c => c.String(nullable: false));
            AlterColumn("MobileTokens", "Description", c => c.String());
            AlterColumn("MobileTokens", "Token", c => c.String(nullable: false));
            AlterColumn("PersonalRoutes", "Description", c => c.String(nullable: false));
            AlterColumn("AppLogins", "UserName", c => c.String(nullable: false));
            AlterColumn("AppLogins", "Password", c => c.String(nullable: false));
            AlterColumn("AppLogins", "Salt", c => c.String(nullable: false));
            AlterColumn("AppLogins", "GuId", c => c.String());
            AlterColumn("BankAccounts", "Number", c => c.String(nullable: false));
            AlterColumn("BankAccounts", "Description", c => c.String(nullable: false));
            AlterColumn("RateTypes", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("RateTypes", "TFCode", c => c.String(nullable: false, unicode: false));
            AlterColumn("RateTypes", "Description", c => c.String(nullable: false, unicode: false));
            AlterColumn("BankAccounts", "Description", c => c.String(nullable: false, unicode: false));
            AlterColumn("BankAccounts", "Number", c => c.String(nullable: false, unicode: false));
            AlterColumn("AppLogins", "GuId", c => c.String(unicode: false));
            AlterColumn("AppLogins", "Salt", c => c.String(nullable: false, unicode: false));
            AlterColumn("AppLogins", "Password", c => c.String(nullable: false, unicode: false));
            AlterColumn("AppLogins", "UserName", c => c.String(nullable: false, unicode: false));
            AlterColumn("PersonalRoutes", "Description", c => c.String(nullable: false, unicode: false));
            AlterColumn("MobileTokens", "Token", c => c.String(nullable: false, unicode: false));
            AlterColumn("MobileTokens", "Description", c => c.String(unicode: false));
            AlterColumn("LicensePlates", "Description", c => c.String(nullable: false, unicode: false));
            AlterColumn("LicensePlates", "Plate", c => c.String(nullable: false, unicode: false));
            AlterColumn("OrgUnits", "LongDescription", c => c.String(unicode: false));
            AlterColumn("OrgUnits", "ShortDescription", c => c.String(nullable: false, unicode: false));
            AlterColumn("Employments", "Position", c => c.String(nullable: false, unicode: false));
            AlterColumn("People", "FullName", c => c.String(nullable: false, unicode: false));
            AlterColumn("People", "Initials", c => c.String(nullable: false, unicode: false));
            AlterColumn("People", "Mail", c => c.String(nullable: false, unicode: false));
            AlterColumn("People", "LastName", c => c.String(nullable: false, unicode: false));
            AlterColumn("People", "FirstName", c => c.String(nullable: false, unicode: false));
            AlterColumn("VacationReports", "Purpose", c => c.String(unicode: false));
            AlterColumn("VacationReports", "EndTime", c => c.Time(precision: 0));
            AlterColumn("VacationReports", "StartTime", c => c.Time(precision: 0));
            AlterColumn("VacationReports", "Comment", c => c.String(nullable: false, unicode: false));
            AlterColumn("DriveReports", "RouteGeometry", c => c.String(unicode: false));
            AlterColumn("DriveReports", "UserComment", c => c.String(unicode: false));
            AlterColumn("DriveReports", "TFCode", c => c.String(nullable: false, unicode: false));
            AlterColumn("DriveReports", "AccountNumber", c => c.String(unicode: false));
            AlterColumn("DriveReports", "FullName", c => c.String(unicode: false));
            AlterColumn("DriveReports", "LicensePlate", c => c.String(unicode: false));
            AlterColumn("DriveReports", "Purpose", c => c.String(nullable: false, unicode: false));
            AlterColumn("DriveReports", "Comment", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "DirtyString", c => c.String(unicode: false));
            AlterColumn("Addresses", "Description", c => c.String(unicode: false));
            AlterColumn("Addresses", "Latitude", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "Longitude", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "Town", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "StreetNumber", c => c.String(nullable: false, unicode: false));
            AlterColumn("Addresses", "StreetName", c => c.String(nullable: false, unicode: false));
        }
    }
}
