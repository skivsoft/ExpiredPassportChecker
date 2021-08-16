using System.IO;
using ExpiredPassportChecker.Batches.UpdateExpiredPassports.Context;
using FileFormat.PassportData;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpiredPassportChecker.Batches.UpdateExpiredPassports.Processors
{
    public class SavePassportData : RequestHandler<ExpiredPassportsContext>
    {
        protected override void Handle(ExpiredPassportsContext request)
        {
            var outputFileName = request.CsvFileName + ".pdata";
            using (Stream outputFile = new FileStream(outputFileName, FileMode.Create))
            {
                var writer = new PassportDataWriter(outputFile);
                writer.WriteTo(request.Storage);
                outputFile.Close();
            }

            var fileSize = new FileInfo(outputFileName).Length;
            request.Logger.LogInformation($"Packed passport data saved into {outputFileName} ({(decimal)fileSize/1000/1000:N2} MB)");
        }
    }
}