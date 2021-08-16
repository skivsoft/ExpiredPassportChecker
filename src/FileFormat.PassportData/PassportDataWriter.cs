using System.IO;
using System.Linq;
using Google.Protobuf;

namespace FileFormat.PassportData
{
    public class PassportDataWriter
    {
        private readonly Stream _stream;
        
        public PassportDataWriter(Stream stream)
        {
            _stream = stream;
        }

        public void WriteTo(PassportDataStorage storage)
        {
            var message = new PassportDataMessage
            {
                CsvHeader = storage.Header,
            };

            var numbers = storage.Numbers.Dictionary
                .Select(x => new NumbersMap
                {
                    SevenDigitsKey = x.Key,
                    ThreeDigitsBitsValue = ByteString.CopyFrom(x.Value),
                })
                .ToList();
            message.NumbersOnlyMap.AddRange(numbers);
            message.OtherLines.AddRange(storage.Strings);
                
            var content = message.ToByteArray();
            _stream.Write(content, 0, content.Length);
        }
    }
}