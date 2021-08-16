using System;
using System.Diagnostics;
using System.IO;
using FileFormat.PassportData;

namespace UnpackPassportData
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: UnpackPassportData <list_of_expired_passports.csv.pdata>");
                return 1;
            }
            
            var fileName = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, args[0]));
            if (File.Exists(fileName))
            {
                var startTime = Stopwatch.StartNew();
                ProcessFile(fileName, fileName.Replace(".pdata", string.Empty));
                startTime.Stop();
                Console.WriteLine($"Elapsed time: {startTime.Elapsed}");
            }
            else
            {
                Console.WriteLine($"The file {fileName} does not found.");
                return 1;
            }

            return 0;
            
        }

        public static void ProcessFile(string inputFileName, string outputFileName)
        {
            var fileSize = new FileInfo(inputFileName).Length;
            Console.WriteLine($"Reading file {inputFileName} {(decimal)fileSize/1000/1000:N} MB ({fileSize} bytes)");

            PassportDataStorage storage;
            using (var stream = new FileStream(inputFileName, FileMode.Open))
            {
                var reader = new PassportDataReader(stream);
                storage = reader.ReadFrom();
                stream.Close();
            }

            using (var stream = new FileStream(outputFileName, FileMode.Create))
            using (var writer = new StreamWriter(stream))
            {
                writer.NewLine = "\n";
                writer.WriteLine(storage.Header);
                WriteNumbers(writer, storage);
                WriteStrings(writer, storage);
               
                writer.Flush();
                writer.Close();
                stream.Close();
            }

            fileSize = new FileInfo(outputFileName).Length;
            Console.WriteLine($"Unpacked into {outputFileName} {(decimal)fileSize/1000/1000:N} MB");
        }

        private static void WriteNumbers(StreamWriter writer, PassportDataStorage storage)
        {
            foreach (var (row, bytes) in storage.Numbers.Dictionary)
            {
                var part1 = row.ToString("D7");
                for (var column = 0; column < PassportDataStorage.PART2_NUM_VALUES; column++)
                {
                    if (storage.Numbers[row, column])
                    {
                        var part2 = column.ToString("D3");
                        var line = part1.Substring(0, 4) + "," + part1.Substring(4, 3) + part2;
                        writer.WriteLine(line);
                    }
                }
            }
        }

        private static void WriteStrings(StreamWriter writer, PassportDataStorage storage)
        {
            foreach (var line in storage.Strings)
            {
                writer.WriteLine(line);
            }
        }
    }
}
