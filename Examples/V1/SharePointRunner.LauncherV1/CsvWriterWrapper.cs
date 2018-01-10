using CsvHelper;
using CsvHelper.Configuration;
using System.IO;

namespace testContractCallsByLauncher.Launcher
{
    internal class CsvWriterWrapper<T1, T2> where T2 : ClassMap<T1>
    {
        private bool HeaderWritten { get; set; } = false;

        private string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">Path of the file to write</param>
        public CsvWriterWrapper(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Write record to CSV file
        /// </summary>
        /// <param name="record">Record to write</param>
        public void WriteRecord(T1 record)
        {
            using (TextWriter writer = new StreamWriter(FilePath, true))
            using (CsvWriter csv = new CsvWriter(writer))
            {
                csv.Configuration.Delimiter = ";";
                csv.Configuration.QuoteAllFields = true;
                csv.Configuration.RegisterClassMap<T2>();

                if (!HeaderWritten)
                {
                    // Write the header if not already
                    csv.WriteHeader<T1>();
                    csv.NextRecord();

                    HeaderWritten = true;
                }

                csv.WriteRecord(record);
                csv.NextRecord();
            }
        }
    }
}
