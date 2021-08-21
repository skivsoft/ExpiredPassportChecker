using System;
using System.IO;
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
                CsvHeader = storage.Header ?? string.Empty,
            };

            foreach (var (key, value) in storage.Numbers.Dictionary)
            {
                // Max length optimization
                var length = value.Length;
                while (length > 0 && value[length - 1] == 0)
                {
                    length--;
                }

                message.NumbersOnlyMap.Add(new NumbersMap
                {
                    SevenDigitsKey = key,
                    ThreeDigitsBitsValue = ByteString.CopyFrom(value.AsSpan()[..length]),
                });
            }

            message.OtherLines.AddRange(storage.Strings);

            var content = message.ToByteArray();
            _stream.Write(content, 0, content.Length);
        }
    }
}