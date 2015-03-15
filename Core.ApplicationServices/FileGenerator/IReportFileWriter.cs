using System.Collections.Generic;

namespace Core.ApplicationServices.FileGenerator
{
    public interface IReportFileWriter
    {
        void WriteRecordsToFile(ICollection<FileRecord> recordList);
    }
}
