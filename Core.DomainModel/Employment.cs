using System;
using System.Collections.Generic;

namespace Core.DomainModel
{
    public class Employment
    {
        //Administrative employees can be identified by their
        //cost centers have a unique prefix
        private const string AdministrativeCostCenterPrefix = "1012";

        public int Id { get; set; }
        public int EmploymentId { get; set; }
        public string Position { get; set; }
        public bool IsLeader { get; set; }
        public long StartDateTimestamp { get; set; }
        public long EndDateTimestamp { get; set; }
        public int EmploymentType { get; set; }
        public int ExtraNumber { get; set; }
        public double WorkDistanceOverride { get; set; }
        public double HomeWorkDistance { get; set; }
        public virtual PersonalAddress AlternativeWorkAddress { get; set; }
        public int? AlternativeWorkAddressId { get; set; }
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public int OrgUnitId { get; set; }
        public virtual OrgUnit OrgUnit { get; set; }
        public long? CostCenter { get; set; }

        /// <summary>
        /// Determines if an employment is administrative based on the
        /// prefix of the cost center (omkostningssted) 
        /// </summary>
        /// <returns></returns>
        public bool IsAdministrativeEmployment()
        {
            var stringRepresentation = Convert.ToString(CostCenter);
            return stringRepresentation.StartsWith(AdministrativeCostCenterPrefix);
        }
    }

}
