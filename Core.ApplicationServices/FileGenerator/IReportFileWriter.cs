using System.Collections.Generic;

namespace Core.ApplicationServices.FileGenerator
{
    public interface IReportFileWriter
    {
        bool WriteRecordsToFile(ICollection<FileRecord> recordList);
    }
}
