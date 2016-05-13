using System;

namespace Core.DomainServices.KMDAbsenceModels
{
    public enum Operation
    {
        Create,
        Edit,
        Delete
    }

    public static class OperationExtension
    {
        public static string AsString(this Operation operation)
        {
            switch (operation)
            {
                case Operation.Create:
                    return "INS";
                case Operation.Edit:
                    return "MOD";
                case Operation.Delete:
                    return "DEL";
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }
    }
}
