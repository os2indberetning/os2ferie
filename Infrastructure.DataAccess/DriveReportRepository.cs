using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Core.DomainModel;
using Core.DomainServices;

namespace Infrastructure.DataAccess
{
    public class DriveReportRepository : IGenericRepository<DriveReport>
    {
        private readonly DataContext _context;

        public DriveReportRepository(DataContext context)
        {
            _context = context;
        }

        public DriveReport Insert(DriveReport entity)
        {
            throw new System.NotImplementedException();
        }

        public DriveReport InsertAndSave(DriveReport entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<DriveReport> AsQueryable()
        {
            IQueryable<DriveReport> result;

            using (_context)
            {
                result = _context.DriveReports;
            }

            if (result.Any())
            {
                return result;
            }

            throw new Exception("No DriveReports found.");
            
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }

        public void Update(DriveReport entity)
        {
            throw new NotImplementedException();
        }

        public void Patch(DriveReport entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(DriveReport entity)
        {
            throw new NotImplementedException();
        }
    }
}