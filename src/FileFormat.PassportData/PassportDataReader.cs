using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
            var dictionary = new Dictionary<int, byte[]>();
            foreach (var numberMap in message.NumbersOnlyMap)
            {
                if (numberMap.ThreeDigitsBitsValue.Length == PassportDataStorage.PART2_NUM_BYTES)
                {
                    dictionary.Add(numberMap.SevenDigitsKey, numberMap.ThreeDigitsBitsValue.ToByteArray());
                }
                else
                {
                    var bytes = new byte[PassportDataStorage.PART2_NUM_BYTES];
                    numberMap.ThreeDigitsBitsValue.ToByteArray().CopyTo(bytes, 0);
                    dictionary.Add(numberMap.SevenDigitsKey, bytes);
                }
            }

            var numbers = new BitMatrix(PassportDataStorage.PART2_NUM_VALUES, dictionary);
            var result = new PassportDataStorage(numbers, message.OtherLines)
            {
                Header = message.CsvHeader,
            };
            return result;
        }
    }
}