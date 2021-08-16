using System.IO;
using ExpiredPassportChecker.Batches.UpdateExpiredPassports.Context;
using ICSharpCode.SharpZipLib.BZip2;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpiredPassportChecker.Batches.UpdateExpiredPassports.Processors
{
    public class RepackBzip2ToPassportData : RequestHandler<ExpiredPassportsContext>
    {
        protected override void Handle(ExpiredPassportsContext request)
        {
            if (request.Bzip2FileName == null || !File.Exists(request.Bzip2FileName))
            {
                return;
            }

            request.Logger.LogInformation($"Repacking {request.Bzip2FileName} to PassportData format");
            
            var lines = 0;
            byte[ ] dataBuffer = new byte[request.Settings.UnpackBufferSize];
            using (Stream inputFile = new FileStream(request.Bzip2FileName, FileMode.Open, FileAccess.Read))
            using (Stream bzipInStream = new BZip2InputStream(inputFile))
            {
                using (var reader = new StreamReader(bzipInStream))
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
            }

            request.Logger.LogInformation($"Number of processed records: {lines - 1}");
        }
    }
}