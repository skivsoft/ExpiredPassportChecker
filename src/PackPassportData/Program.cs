using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using FileFormat.PassportData;

namespace PackPassportData
{
    static class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: PackPassportData <list_of_expired_passports.csv>");
                return 1;
            }
            
            var fileName = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, args[0]));
            if (File.Exists(fileName))
            {
                var startTime = Stopwatch.StartNew();
                ProcessFile(fileName, fileName + ".pdata");
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

        public static int IntPow(int value, int degree)
        {
            var result = 1;
            for (var i = 0; i < degree; i++)
            {
                result *= value;
            }

            return result;
        }

        public static string MyConvert(long value, int baseNum)
        {
            var result = string.Empty;
            while (value > 0)
            {
                result = (value % baseNum) + result;
                value = value / baseNum;
            }
            return result;
        }
        
        public static void ProcessFile(string inputFileName, string outputFileName)
        {
            var fileSize = new FileInfo(inputFileName).Length;
            Console.WriteLine($"Reading file {inputFileName} {(decimal)fileSize/1000/1000:N} MB ({fileSize} bytes)");

            var hashes = new Dictionary<int, ISet<string>>()
            {
                { 25, new HashSet<string>() },
                { 30, new HashSet<string>() },
            };

            var numChars = 34;
            var numberBase = 2;
            var q = MyConvert(long.Parse("18"), 3);
            var qq = MyConvert(long.Parse("9999999999"), 3);

            var hash = new HashSet<string>();
            
            var storage = new PassportDataStorage();
            var lines = 0;
            var minKey = new int[] { };
            var minCount = int.MaxValue;
            using (var reader = new StreamReader(inputFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (lines == 0)
                    {
                        storage.Header = line;
                    }
                    else
                    {
                        //storage.Add(line);

                        var data = line.Replace(",", string.Empty);
                        if (data.Length == 10 && data.All(char.IsDigit))
                        {
                            var bits = Convert.ToString(long.Parse(data), numberBase).PadLeft(numChars, '0');
                            //var bits = MyConvert(long.Parse(data), numberBase).PadLeft(numChars, '0');
                            foreach (var (key, value) in hashes)
                            {
                                value.Add(bits.Substring(0, key));
                            }
                        }
                    }

                    lines++;
                }
            }

            foreach (var (key, value) in hashes)
            {
                var pow = IntPow(numberBase, (numChars - key));
                var bytes = (pow >> 3) + (pow % 8 == 0 ? 0 : 1);
                Console.WriteLine("{0},{1}: Count = {2} Bytes = {3} Keys = {4} Bits = {5} Size = {6}",
                    key, numChars-key, value.Count, bytes, value.Count * 4, value.Count * bytes, value.Count * 4 + value.Count * bytes);
            }

            Console.WriteLine($"Number of processed records: {lines - 1}");

            using (var stream = new FileStream(outputFileName, FileMode.Create))
            {
                var writer = new PassportDataWriter(stream);
                writer.WriteTo(storage);
                stream.Close();
            }

            fileSize = new FileInfo(outputFileName).Length;
            Console.WriteLine($"Packed into {outputFileName} ({(decimal)fileSize/1000/1000:N} MB)");
        }
    }
}
