using System.IO;
using ExpiredPassportChecker.Settings;
using FileFormat.PassportData;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpiredPassportChecker.Batches.UpdateExpiredPassports.Context
{
    public class ExpiredPassportsContext : IRequest
    {
        public ILogger Logger { get; set; }
        
        public MainSettings Settings { get; set; }

        public string Bzip2FileName => Settings.SourceFileName;

        public string CsvFileName => Path.GetFileNameWithoutExtension(Settings.SourceFileName);
        
        public PassportDataStorage Storage { get; set; } = new PassportDataStorage();
    }
}