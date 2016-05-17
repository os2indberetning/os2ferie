using System;

namespace Core.DomainModel
{
    public enum KMDAbsenceOperation
    {
        Create,
        Edit,
        Delete
    }

    public static class OperationExtension
    {
        public static string AsString(this KMDAbsenceOperation kmdAbsenceOperation)
        {
            switch (kmdAbsenceOperation)
            {
                case KMDAbsenceOperation.Create:
                    return "INS";
                case KMDAbsenceOperation.Edit:
                    return "MOD";
                case KMDAbsenceOperation.Delete:
                    return "DEL";
                default:
                    throw new ArgumentOutOfRangeException(nameof(kmdAbsenceOperation), kmdAbsenceOperation, null);
            }
        }
    }
}
