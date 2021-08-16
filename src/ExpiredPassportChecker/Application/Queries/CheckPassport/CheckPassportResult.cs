namespace ExpiredPassportChecker.Application.Queries.CheckPassport
{
    public class CheckPassportResult
    {
        /// <summary>
        /// Флаг, означающий что запрашиваемая комбинация Серия + Номер паспорта найдена в списки недействительных
        /// паспортов РФ.
        /// </summary>
        public bool FoundInExpiredPassports { get; set; }
    }
}