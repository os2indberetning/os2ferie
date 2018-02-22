using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.KMDVacationService
{
    public class KMDSetAbsenceFailedException : Exception
    {
        public SetAbsenceAttendance.SetAbsenceAttendanceRequest Request { get; set; }

        public KMDSetAbsenceFailedException()
        {
        }

        public KMDSetAbsenceFailedException(string message)
        : base(message)
        {
        }

        public KMDSetAbsenceFailedException(string message, SetAbsenceAttendance.SetAbsenceAttendanceRequest request)
        : base(message)
        {
            Request = request;
        }

        public KMDSetAbsenceFailedException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
