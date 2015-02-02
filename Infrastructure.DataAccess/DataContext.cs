using System.Data.Entity;
using System.Dynamic;
using Core.DomainModel;


namespace Infrastructure.DataAccess
{

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
            DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
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

        /**
         * Sets up 
         */
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
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
        }


        private void ConfigurePropertiesForPerson(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.LastName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.CprNumber).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.PersonId).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.Mail).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.WorkDistanceOverride).IsRequired();

            modelBuilder.Entity<Person>().Property(t => t.CprNumber).IsFixedLength().HasMaxLength(10);
        }

        private void ConfigurePropertiesForAddress(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().Property(p => p.StreetName).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.StreetNumber).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.ZipCode).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.Town).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.Longitude).IsRequired();
            modelBuilder.Entity<Address>().Property(p => p.Lattitude).IsRequired();      
        }

        private void ConfigurePropertiesForPersonalAddress(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonalAddress>().Property(p => p.Type).IsRequired();
            modelBuilder.Entity<PersonalAddress>().HasRequired(p => p.Person);
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
        }

        private void ConfigurePropertiesForRate(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rate>().Property(p => p.Year).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.TFCode).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.KmRate).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.Type).IsRequired();
            modelBuilder.Entity<Rate>().Property(p => p.Active).IsRequired();
        }

        private void ConfigurePropertiesForMailNoficationSchedule(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailNotificationSchedule>().Property(p => p.Date).IsRequired();
            modelBuilder.Entity<MailNotificationSchedule>().Property(p => p.Notified).IsRequired();
            modelBuilder.Entity<MailNotificationSchedule>().Property(p => p.NextGenerationDate).IsRequired();
        }

        private void ConfigurePropertiesForFileGenerationSchedule(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileGenerationSchedule>().Property(p => p.Date).IsRequired();
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
            modelBuilder.Entity<DriveReport>().Property(p => p.Porpuse).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.KmRate).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.DriveDate).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.FourKmRule).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.StartsAtHome).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.EndsAtHome).IsRequired();
            modelBuilder.Entity<DriveReport>().Property(p => p.Licenseplate).IsRequired();
        }

        private void ConfigurePropertiesForReport(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>().Property(p => p.status).IsRequired();
            modelBuilder.Entity<Report>().Property(p => p.CreatedDate).IsRequired();
            modelBuilder.Entity<Report>().Property(p => p.Comment).IsRequired();

            modelBuilder.Entity<Report>().HasRequired(p => p.Person);
            modelBuilder.Entity<Report>().HasRequired(p => p.Employment);
        }

        private void ConfigurePropertiesForEmployment(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employment>().Property(p => p.EmploymentId).IsRequired();
            modelBuilder.Entity<Employment>().Property(p => p.Position).IsRequired();
            modelBuilder.Entity<Employment>().Property(p => p.IsLeader).IsRequired();
            modelBuilder.Entity<Employment>().Property(p => p.StartDate).IsRequired();

            modelBuilder.Entity<Employment>().HasRequired(p => p.Person);
            modelBuilder.Entity<Employment>().HasRequired(p => p.OrgUnit);
        }

        private void ConfigurePropertiesForOrgUnit(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrgUnit>().Property(p => p.OrgId).IsRequired();
            modelBuilder.Entity<OrgUnit>().Property(p => p.ShortDescription).IsRequired();
            modelBuilder.Entity<OrgUnit>().Property(p => p.Level).IsRequired();
        }

        private void ConfigurePropertiesForSubstitute(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Substitute>().Property(p => p.StartDate).IsRequired();

            modelBuilder.Entity<Substitute>().HasRequired(p => p.OrgUnit);
        }
    }
}
