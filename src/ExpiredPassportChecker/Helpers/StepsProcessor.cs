using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExpiredPassportChecker.Helpers
{
    public class StepsProcessor<TContext> : IRequestHandler<TContext>
        where TContext : IRequest
    {
        private readonly ILogger _logger;
        private readonly ICollection<IRequestHandler<TContext>> _processors;
        
        public StepsProcessor(
            ILogger logger,
            IEnumerable<IRequestHandler<TContext>> processors)
        {
            _logger = logger;
            _processors = processors.ToArray();
        }


        public async Task<Unit> Handle(TContext request, CancellationToken cancellationToken)
        {
            var count = _processors.Count;
            var step = 1;
            var totalElapsed = TimeSpan.Zero;
            foreach (var processor in _processors)
            {
                _logger.LogInformation($"Step {step} of {count}: {processor.GetType().Name}");

                var stopwatch = Stopwatch.StartNew();
                await processor.Handle(request, cancellationToken);
                stopwatch.Stop();

                _logger.LogInformation($"Elapsed time: {stopwatch.Elapsed}");
                _logger.LogInformation(new string('-', 80));
                step++;
                totalElapsed += stopwatch.Elapsed;
            }

            _logger.LogInformation($"Total elapsed time: {totalElapsed}");
            _logger.LogInformation(new string('-', 80));
            return Unit.Value;
        }
    }
}