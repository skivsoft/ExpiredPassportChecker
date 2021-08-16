using System.IO;
using ExpiredPassportChecker.Batches.UpdateExpiredPassports.Context;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpiredPassportChecker.Batches.UpdateExpiredPassports.Processors
{
    public class UnpackFromBzip2 : RequestHandler<ExpiredPassportsContext>
    {
        protected override void Handle(ExpiredPassportsContext request)
        {
            if (request.Bzip2FileName == null || !File.Exists(request.Bzip2FileName))
            {
                return;
            }
            
            request.Logger.LogInformation($"Unpacking {request.Bzip2FileName} into {request.CsvFileName}");
            
            byte[ ] dataBuffer = new byte[request.Settings.UnpackBufferSize];
            using (Stream inputFile = new FileStream(request.Bzip2FileName, FileMode.Open, FileAccess.Read))
            {
                using (Stream bzipInStream = new BZip2InputStream(inputFile))
                {
                    using (Stream outputFile = File.Create(request.CsvFileName))
                    {
                        StreamUtils.Copy(bzipInStream, outputFile, dataBuffer);
                    }                
                }
            }

            var fileSize = new FileInfo(request.CsvFileName).Length;
            request.Logger.LogInformation($"Unpacked {(decimal)fileSize / 1000 / 1000:N2} MB ({fileSize} bytes)");
        }
    }
}