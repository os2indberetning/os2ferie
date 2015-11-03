using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Dynamic;
using AddressHistoryMigration.Models;
using Core.DomainModel;


namespace Infrastructure.DataAccess
{

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
        }

        public IDbSet<Person> Persons { get; set; }
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<PersonalAddress> PersonalAddresses { get; set; }
        public IDbSet<PersonalRoute> PersonalRoutes { get; set; }
        public IDbSet<Point> Points { get; set; }
        public IDbSet<LicensePlate> License { get; set; }
        public IDbSet<MobileToken> MobileTokens { get; set; }
        public IDbSet<Rate> Rates { get; set; }
        public IDbSet<MailNotificationSchedule> MailNotificationSchedules { get; set; }
        public IDbSet<FileGenerationSchedule> FileGenerationSchedules { get; set; }
        public IDbSet<DriveReportPoint> DriveReportPoints { get; set; }
        public IDbSet<DriveReport> DriveReports { get; set; }
        public IDbSet<Report> Reports { get; set; }
        public IDbSet<Employment> Employments { get; set; }
        public IDbSet<OrgUnit> OrgUnits { get; set; }
        public IDbSet<Substitute> Substitutes { get; set; }
        public IDbSet<BankAccount> BankAccounts { get; set; } 
        public IDbSet<RateType> RateTypes { get; set; }
        public IDbSet<CachedAddress> CachedAddresses { get; set; }
        public IDbSet<AddressHistory> AddressHistory { get; set; }
        public IDbSet<TempAddressHistory> TempAddressHistory { get; set; } 
  

        /**
         * Sets up 
         */
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);           

            modelBuilder.Conventions.Add(new DateTimeOffsetConvention());

            ConfigurePropertiesForPerson(modelBuilder);
            ConfigurePropertiesForAddress(modelBuilder);
            ConfigurePropertiesForPersonalAddress(modelBuilder);
            ConfigurePropertiesForPersonalRoute(modelBuilder);
            ConfigurePropertiesForPoint(modelBuilder);
            ConfigurePropertiesForLicensePlate(modelBuilder);
            ConfigurePropertiesForMobileToken(modelBuilder);
            ConfigurePropertiesForRate(modelBuilder);
            ConfigurePropertiesForMailNoficationSchedule(modelBuilder);
            ConfigurePropertiesForFileGenerationSchedule(modelBuilder);
            ConfigurePropertiesForDriveReportPoint(modelBuilder);
            ConfigurePropertiesForDriveReport(modelBuilder);
            ConfigurePropertiesForReport(modelBuilder);
            ConfigurePropertiesForEmployment(modelBuilder);
            ConfigurePropertiesForOrgUnit(modelBuilder);
            ConfigurePropertiesForSubstitute(modelBuilder);
            ConfigurePropertiesForBankAccount(modelBuilder);
            ConfigurePropertiesForRateType(modelBuilder);
            ConfigurePropertiesForCachedAddress(modelBuilder);
            ConfigurePropertiesForWorkAddress(modelBuilder);
        }

        private void ConfigurePropertiesForPerson(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.LastName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.CprNumber).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.Mail).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.Initials).IsRequired();
            modelBuilder.Entity<Person>().Property(t => t.CprNumber).IsFixedLength().HasMaxLength(10);
            modelBuilder.Entity<Person>().Property(t => t.FullName).IsRequired();
            modelBuilder.Entity<Person>().Ignore(t => t.IsSubstitute);

        }

        private void ConfigurePropertiesForCachedAddress(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CachedAddress>().Property(p => p.IsDirty).IsRequired();
        }

        private void ConfigurePropertiesForAddress(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().Property(p => p.StreetName).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.StreetNumber).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.ZipCode).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.Town).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.Longitude).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.Latitude).IsRequired();      
        }

        private void ConfigurePropertiesForWorkAddress(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkAddress>().Ignore(p => p.OrgUnitId);
        }

        private void ConfigurePropertiesForRateType(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RateType>().Property(p => p.Description).IsRequired();
            modelBuilder.Entity<RateType>().Property(p => p.TFCode).IsRequired();
        }

        private void ConfigurePropertiesForPersonalAddress(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonalAddress>().Property(p => p.Type).IsRequired();
            modelBuilder.Entity<PersonalAddress>().HasRequired(p => p.Person);
        }

        private void ConfigurePropertiesForBankAccount(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>().Property(p => p.Description).IsRequired();
            modelBuilder.Entity<BankAccount>().Property(p => p.Number).IsRequired();
        }

        private void ConfigurePropertiesForPersonalRoute(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonalRoute>().Property(p => p.Description).IsRequired();
            modelBuilder.Entity<PersonalRoute>().HasRequired(p => p.Person);
        }

        private void ConfigurePropertiesForPoint(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Point>().HasRequired(p => p.PersonalRoute);
            modelBuilder.Entity<Point>().HasOptional(p => p.NextPoint).WithOptionalDependent(p => p.PreviousPoint);
        }

        private void ConfigurePropertiesForLicensePlate(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LicensePlate>().Property(p => p.Plate).IsRequired();
            modelBuilder.Entity<LicensePlate>().Property(p => p.Description).IsRequired();
            modelBuilder.Entity<LicensePlate>().HasRequired(p => p.Person);
        }

        private void ConfigurePropertiesForMobileToken(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MobileToken>().Property(p => p.Guid).IsRequired();
            modelBuilder.Entity<MobileToken>().Property(p => p.Status).IsRequired();
            modelBuilder.Entity<MobileToken>().Property(p => p.Token).IsRequired();
            modelBuilder.Entity<MobileToken>().HasRequired(p => p.Person);
            modelBuilder.Entity<MobileToken>().Ignore(p => p.StatusToPresent);
        }

        private void ConfigurePropertiesForRate(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rate>().Property(p => p.Year).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.KmRate).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.TypeId).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.Active).IsRequired();
        }

        private void ConfigurePropertiesForMailNoficationSchedule(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailNotificationSchedule>().Property(p => p.DateTimestamp).IsRequired();
            modelBuilder.Entity<MailNotificationSchedule>().Property(p => p.Notified).IsRequired();
            modelBuilder.Entity<MailNotificationSchedule>().Property(p => p.Repeat).IsRequired();
            
        }

        private void ConfigurePropertiesForFileGenerationSchedule(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileGenerationSchedule>().Property(p => p.DateTimestamp).IsRequired();
            modelBuilder.Entity<FileGenerationSchedule>().Property(p => p.Generated).IsRequired();
        }

        private void ConfigurePropertiesForDriveReportPoint(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriveReportPoint>().HasRequired(p => p.DriveReport);
            modelBuilder.Entity<DriveReportPoint>().HasOptional(p => p.NextPoint).WithOptionalDependent(p => p.PreviousPoint);
        }

        private void ConfigurePropertiesForDriveReport(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriveReport>().Property(p => p.Distance).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.AmountToReimburse).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.Purpose).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.KmRate).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.DriveDateTimestamp).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.FourKmRule).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.StartsAtHome).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.EndsAtHome).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.TFCode).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.IsFromApp).IsRequired();
        }

        private void ConfigurePropertiesForReport(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>().Property(p => p.Status).IsRequired();
            modelBuilder.Entity<Report>().Property(p => p.CreatedDateTimestamp).IsRequired();
            modelBuilder.Entity<Report>().Property(p => p.Comment).IsRequired();
            modelBuilder.Entity<Report>().HasRequired(p => p.Person);
            modelBuilder.Entity<Report>().HasRequired(p => p.Employment);
        }

        private void ConfigurePropertiesForEmployment(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employment>().Property(p => p.EmploymentId).IsRequired();
            modelBuilder.Entity<Employment>().Property(p => p.Position).IsRequired();
            modelBuilder.Entity<Employment>().Property(p => p.IsLeader).IsRequired();
            modelBuilder.Entity<Employment>().Property(p => p.StartDateTimestamp).IsRequired();
        }

        private void ConfigurePropertiesForOrgUnit(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrgUnit>().Property(p => p.OrgId).IsRequired();
            modelBuilder.Entity<OrgUnit>().Property(p => p.ShortDescription).IsRequired();
            modelBuilder.Entity<OrgUnit>().Property(p => p.Level).IsRequired();
        }

        private void ConfigurePropertiesForSubstitute(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Substitute>().Property(p => p.StartDateTimestamp).IsRequired();

            modelBuilder.Entity<Substitute>().HasRequired(p => p.OrgUnit);

            modelBuilder.Entity<Substitute>().HasRequired(p => p.Leader).WithMany(p => p.Substitutes);
            modelBuilder.Entity<Substitute>().HasRequired(p => p.Sub).WithMany(p => p.SubstituteLeaders);
            modelBuilder.Entity<Substitute>().HasRequired(p => p.Person).WithMany(p => p.SubstituteFor);
        }

        public class DateTimeOffsetConvention : Convention
        {
            public DateTimeOffsetConvention()
            {
                this.Properties<DateTimeOffset>().Configure(c => c.HasColumnType("TIMESTAMP"));
            }
        }
    }
}
