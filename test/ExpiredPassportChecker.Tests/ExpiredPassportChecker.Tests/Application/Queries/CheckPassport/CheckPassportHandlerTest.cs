using System.Threading.Tasks;
using ExpiredPassportChecker.Application.Queries.CheckPassport;
using ExpiredPassportChecker.Helpers;
using FileFormat.PassportData;
using FluentAssertions;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpiredPassportChecker.Tests.Application.Queries.CheckPassport
{
    [TestClass]
    public class CheckPassportHandlerTest
    {
        private readonly IRequestHandler<CheckPassportQuery, CheckPassportResult> _handler;
        private readonly InMemoryContainer<PassportDataStorage> _container;

        public CheckPassportHandlerTest()
        {
            _container = new InMemoryContainer<PassportDataStorage>();
            _container.Value.Add("1234,123456");
            _handler = new CheckPassportHandler(_container);
        }
        
        [TestMethod]
        public async Task Execute_WithPassportNotInList_ReturnsNotExists()
        {
            // arrange
            var request = new CheckPassportQuery
            {
                PassportSeries = "1234",
                PassportNumber = "123457",
            };
            
            // act
            var result = await _handler.Handle(request, default);

            // assert
            result.FoundInExpiredPassports.Should().BeFalse();
        }

        [TestMethod]
        public async Task Execute_WithPassportInList_ReturnsExists()
        {
            // arrange
            var request = new CheckPassportQuery
            {
                PassportSeries = "1234",
                PassportNumber = "123456",
            };
            
            // act
            var result = await _handler.Handle(request, default);

            // assert
            result.FoundInExpiredPassports.Should().BeTrue();
        }

        [TestMethod]
        public async Task Execute_WithExtraSpaces_ReturnsExists()
        {
            // arrange
            var request = new CheckPassportQuery
            {
                PassportSeries = "  1234  ",
                PassportNumber = " 123456 ",
            };
            
            // act
            var result = await _handler.Handle(request, default);

            // assert
            result.FoundInExpiredPassports.Should().BeTrue();
        }
    }
}