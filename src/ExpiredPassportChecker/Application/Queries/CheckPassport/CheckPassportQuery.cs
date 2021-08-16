using MediatR;

namespace ExpiredPassportChecker.Application.Queries.CheckPassport
{
    public class CheckPassportQuery : IRequest<CheckPassportResult>
    {
        /// <summary>
        /// Серия паспорта РФ.
        /// </summary>
        public string PassportSeries { get; set; }
        
        /// <summary>
        /// Номер паспорта РФ.
        /// </summary>
        public string PassportNumber { get; set; }
    }
}