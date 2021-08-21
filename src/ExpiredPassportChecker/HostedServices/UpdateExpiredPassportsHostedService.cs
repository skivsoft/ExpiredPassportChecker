using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CronExpressionDescriptor;
using ExpiredPassportChecker.Application.Commands.UpdateExpiredPassports;
using ExpiredPassportChecker.Helpers;
using ExpiredPassportChecker.Settings;
using FileFormat.PassportData;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Options = Microsoft.Extensions.Options.Options;

namespace ExpiredPassportChecker.HostedServices
{
    public class UpdateExpiredPassportsHostedService : IHostedService
    {
        private readonly ILogger<UpdateExpiredPassportsHostedService> _logger;
        private readonly IMediator _mediator;
        private readonly InMemoryContainer<PassportDataStorage> _container;
        private readonly MainSettings _settings;

        public UpdateExpiredPassportsHostedService(
            ILogger<UpdateExpiredPassportsHostedService> logger,
            IMediator mediator,
            InMemoryContainer<PassportDataStorage> container,
            IOptions<MainSettings> settings)
        {
            _logger = logger;
            _mediator = mediator;
            _container = container;
            _settings = settings.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var passportDataFileName = Path.ChangeExtension(_settings.SourceFileName, ".pdata");
            if (passportDataFileName != null && File.Exists(passportDataFileName))
            {
                _logger.LogInformation($"Loading passport data from {passportDataFileName}");
                using (var stream = new FileStream(passportDataFileName, FileMode.Open))
                {
                    var reader = new PassportDataReader(stream);
                    _container.Value = reader.ReadFrom();
                    stream.Close();
                }
            }
            else
            {
                await _mediator.Send(new UpdateExpiredPassportsCommand(), cancellationToken);
            }

            var cronExpression = ExpressionDescriptor.GetDescription(_settings.CronSchedule, new CronExpressionDescriptor.Options()
            {
                Use24HourTimeFormat = true,
            });
            Log.Information($"Cron schedule: {cronExpression}");

            RecurringJob.AddOrUpdate(
                () => _mediator.Send(new UpdateExpiredPassportsCommand(), default),
                _settings.CronSchedule);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}