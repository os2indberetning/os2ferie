using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Core.DmzModel;

namespace Infrastructure.DmzDataAccess
{

    public partial class DmzContext : DbContext
    {
        public DmzContext()
            : base("DMZConnection")
        {
        }

        public virtual DbSet<DriveReport> DriveReports { get; set; }
        public virtual DbSet<Employment> Employments { get; set; }
        public virtual DbSet<GPSCoordinate> GPSCoordinates { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Rate> Rates { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriveReport>()
                .HasOptional(e => e.Route)
                .WithRequired(e => e.DriveReport);

            modelBuilder.Entity<DriveReport>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("Id");

            modelBuilder.Entity<Rate>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("Id");

            modelBuilder.Entity<Token>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("Id");

            modelBuilder.Entity<Profile>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("Id");

            modelBuilder.Entity<Employment>()
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasColumnName("Id");

        }
    }
}
