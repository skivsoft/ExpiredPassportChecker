using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExpiredPassportChecker.Batches.UpdateExpiredPassports.Context;
using ExpiredPassportChecker.Helpers;
using ExpiredPassportChecker.Settings;
using FileFormat.PassportData;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExpiredPassportChecker.HostedServices
{
    public class UpdateExpiredPassportsHostedService : IHostedService
    {
        private readonly ILogger<UpdateExpiredPassportsHostedService> _logger;
        private readonly IRequestHandler<ExpiredPassportsContext> _steps;
        private readonly MainSettings _settings;
        private readonly InMemoryContainer<PassportDataStorage> _container;

        public UpdateExpiredPassportsHostedService(
            ILogger<UpdateExpiredPassportsHostedService> logger,
            IOptions<MainSettings> options,
            InMemoryContainer<PassportDataStorage> container,
            IEnumerable<IRequestHandler<ExpiredPassportsContext>> processors)
        {
            _logger = logger;
            _settings = options.Value;
            _container = container;
            _steps = new StepsProcessor<ExpiredPassportsContext>(logger, processors);
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var context = new ExpiredPassportsContext()
            {
                Logger = _logger,
                Settings = _settings,
            };
            await _steps.Handle(context, cancellationToken);
            _container.Value = context.Storage;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}