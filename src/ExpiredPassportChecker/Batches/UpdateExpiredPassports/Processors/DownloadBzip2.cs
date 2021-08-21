using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ExpiredPassportChecker.Batches.UpdateExpiredPassports.Context;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpiredPassportChecker.Batches.UpdateExpiredPassports.Processors
{
    public class DownloadBzip2 : IRequestHandler<ExpiredPassportsContext>
    {
        public async Task<Unit> Handle(ExpiredPassportsContext request, CancellationToken cancellationToken)
        {
            if (request.Settings?.SourceBaseUrl == null || request.Settings.SourceFileName == null)
            {
                return Unit.Value;
            }

            var sourceUrl = new Uri(new Uri(request.Settings.SourceBaseUrl, UriKind.Absolute), request.Settings.SourceFileName);
            request.Logger.LogInformation("Downloading file " + sourceUrl);

            var timer = new Stopwatch();
            var savePosition = Console.GetCursorPosition();
            using (var client = new WebClient())
            {
                var updateFlag = false;
                var chars = 0;
                client.DownloadProgressChanged += (sender, args) =>
                {
                    if (updateFlag || chars == args.ProgressPercentage / 2)
                    {
                        return;
                    }

                    updateFlag = true;
                    Console.SetCursorPosition(0, savePosition.Top);

                    var left = args.ProgressPercentage / 2;
                    var right = 50 - left;
                    var forecastMilliseconds = (timer.ElapsedMilliseconds * args.TotalBytesToReceive) / args.BytesReceived;
                    var forecastTime = TimeSpan.FromMilliseconds(forecastMilliseconds);
                    var leftTime = forecastTime - timer.Elapsed;
                    Console.Write(
                        "[{0}{1}] {2}",
                        new string('█', left),
                        new string('░', right),
                        leftTime.TotalSeconds == 0 ? string.Empty : leftTime.ToString("hh\\:mm\\:ss"));

                    updateFlag = false;
                    chars = args.ProgressPercentage / 2;
                };
                timer.Start();
                await client.DownloadFileTaskAsync(sourceUrl, request.Bzip2FileName);
                timer.Stop();
                Console.WriteLine();
            }

            var fileSize = new FileInfo(request.Bzip2FileName).Length;
            decimal speed = (decimal)fileSize / (decimal)timer.ElapsedMilliseconds / 1000;
            request.Logger.LogInformation($"Downloaded {(decimal)fileSize / 1000 / 1000:N2} MB ({fileSize} bytes) {speed:N2} MB/s");

            return Unit.Value;
        }
    }
}