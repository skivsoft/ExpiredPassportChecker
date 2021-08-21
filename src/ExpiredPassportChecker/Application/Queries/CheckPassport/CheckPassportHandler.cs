using System;
using System.Collections.Immutable;
using ExpiredPassportChecker.Helpers;
using FileFormat.PassportData;
using MediatR;

namespace ExpiredPassportChecker.Application.Queries.CheckPassport
{
    public class CheckPassportHandler : RequestHandler<CheckPassportQuery, CheckPassportResult>
    {
        private readonly InMemoryContainer<PassportDataStorage> _container;

        public CheckPassportHandler(InMemoryContainer<PassportDataStorage> container)
        {
            _container = container;
        }

        protected override CheckPassportResult Handle(CheckPassportQuery query)
        {
            var combined = $"{query.PassportSeries.Trim()},{query.PassportNumber.Trim()}";
            var result = new CheckPassportResult
            {
                FoundInExpiredPassports = _container.Value.Contains(combined),
            };
            return result;
        }
    }
}