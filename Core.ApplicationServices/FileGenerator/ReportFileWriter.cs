using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Configuration;

namespace Core.ApplicationServices.FileGenerator
{
    class ReportFileWriter
    {
        private readonly string _filePathName = GetSetting("KMDFilePath") + @"\" + GetSetting("KMDFileName");

        public void WriteRecordsToFile(ICollection<FileRecord> recordList)
        {
            var existingLineCounter = 0;
            if (File.Exists(_filePathName))
            {
                existingLineCounter += File.ReadLines(_filePathName).Count();
            }
            else
            {
                WriteHeader();
                existingLineCounter++;
            }

            WriteRecords(recordList);

            var totalLineCount = File.ReadLines(_filePathName).Count();

            if ((totalLineCount - existingLineCounter) < recordList.Count)
            {
                var oldLines = ReadOldLines(existingLineCounter);

                using (var newWriter = new StreamWriter(_filePathName))
                {
                    foreach (var entry in oldLines)
                    {
                        newWriter.WriteLine(entry);
                    }
                    newWriter.Close();
                }
                Console.WriteLine("Error not all records were written to file, changes were rolled back");
            }
        }

        private void WriteHeader()
        {
            using (var writer = new StreamWriter(_filePathName))
            {
                writer.WriteLine(GetSetting("KMDHeader"));
                writer.Close();
            }
        }

        private void WriteRecords(ICollection<FileRecord> recordList)
        {
            using (var writer = new StreamWriter(_filePathName, true))
            {
                foreach (var record in recordList)
                {
                    writer.WriteLine(record.ToString());
                }
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }

        private IEnumerable<string> ReadOldLines(int existingLineCounter)
        {
            var tempList = new List<string>();
                using (var newReader = new StreamReader(_filePathName))
                {
                    string line;
                    while ((line = newReader.ReadLine()) != null)
                    {
                        if (tempList.Count == existingLineCounter)
                        {
                            break;
                        }
                        tempList.Add(line);
                    }
                    newReader.Close();
                }
            return tempList;
        }
            
        private static string GetSetting(string key)
        {
            return WebConfigurationManager.AppSettings[key];
        }
    }
}
