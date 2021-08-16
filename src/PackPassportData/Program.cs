using System;
using System.Diagnostics;
using System.IO;
using FileFormat.PassportData;

namespace PackPassportData
{
    class Program
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

        public static void ProcessFile(string inputFileName, string outputFileName)
        {
            var fileSize = new FileInfo(inputFileName).Length;
            Console.WriteLine($"Reading file {inputFileName} {(decimal)fileSize/1000/1000:N} MB ({fileSize} bytes)");
            
            var graph = new PassportDataStorage();
            var lines = 0;
            using (var reader = new StreamReader(inputFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (lines == 0)
                    {
                        graph.Header = line;
                    }
                    else
                    {
                        graph.Add(line);
                    }

                    lines++;
                }
            }

            Console.WriteLine($"Number of processed records: {lines - 1}");

            using (var stream = new FileStream(outputFileName, FileMode.Create))
            {
                var writer = new PassportDataWriter(stream);
                writer.WriteTo(graph);
                stream.Close();
            }

            fileSize = new FileInfo(outputFileName).Length;
            Console.WriteLine($"Packed into {outputFileName} ({(decimal)fileSize/1000/1000:N} MB)");
        }
    }
}
