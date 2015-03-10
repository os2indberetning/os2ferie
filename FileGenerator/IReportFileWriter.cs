using System.Collections.Generic;

namespace FileGenerator
{
    public interface IReportFileWriter
    {
        void WriteRecordsToFile(ICollection<FileRecord> recordList);
    }
}
