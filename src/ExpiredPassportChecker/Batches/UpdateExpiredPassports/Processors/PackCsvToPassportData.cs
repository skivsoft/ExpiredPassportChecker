using System;
using System.IO;
using ExpiredPassportChecker.Batches.UpdateExpiredPassports.Context;
using ExpiredPassportChecker.Settings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExpiredPassportChecker.Batches.UpdateExpiredPassports.Processors
{
    public class PackCsvToPassportData : RequestHandler<ExpiredPassportsContext>
    {
        private readonly MainSettings _settings;

        public PackCsvToPassportData(IOptions<MainSettings> settings)
        {
            _settings = settings.Value;
        }

        protected override void Handle(ExpiredPassportsContext request)
        {
            if (request.CsvFileName == null || !File.Exists(request.CsvFileName))
            {
                return;
            }

            request.Logger.LogInformation("Reading and compressing " + request.CsvFileName);

            var lines = 0;
            using (var reader = new StreamReader(request.CsvFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (lines == 0)
                    {
                        request.Storage.Header = line;
                    }
                    else
                    {
                        request.Storage.Add(line);
                    }

                    lines++;
                }
            }

            if (_settings.EnabledSteps.DeleteCsvAfterProcessing)
            {
                File.Delete(request.CsvFileName);
            }

            request.Logger.LogInformation($"Number of processed records: {lines - 1}");
            request.Logger.LogInformation($"Number of lines with letters: {request.Storage.Strings.Count}");
        }
    }
}