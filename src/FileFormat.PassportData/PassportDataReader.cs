using System.IO;
using System.Linq;
using Google.Protobuf;

namespace FileFormat.PassportData
{
    public class PassportDataReader
    {
        private readonly Stream _stream;

        public PassportDataReader(Stream stream)
        {
            _stream = stream;
        }

        public PassportDataStorage ReadFrom()
        {
            var message = PassportDataMessage.Parser.ParseFrom(_stream);
            var dictionary = message.NumbersOnlyMap
                .ToDictionary(key => key.SevenDigitsKey, element => element.ThreeDigitsBitsValue.ToByteArray());
            var numbers = new BitMatrix(PassportDataStorage.PART2_NUM_VALUES, dictionary);
            var result = new PassportDataStorage(numbers, message.OtherLines)
            {
                Header = message.CsvHeader,
            };
            return result;
        }
    }
}