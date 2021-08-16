using System;
using System.Collections.Generic;
using System.Linq;

namespace FileFormat.PassportData
{
    public class PassportDataStorage
    {
        private const int PART1_LENGTH = 7;
        private const int PART2_LENGTH = 3;
        public const int PART2_NUM_VALUES = 1000;

        private readonly IBitMatrix _numbers;
        private readonly ISet<string> _strings;

        public PassportDataStorage()
        {
            _numbers = new BitMatrix(PART2_NUM_VALUES);
            _strings = new HashSet<string>();
        }

        public PassportDataStorage(IBitMatrix numbers, IEnumerable<string> strings)
        {
            _numbers = numbers;
            _strings = new HashSet<string>(strings);
        }
        
        public string Header { get; set; }

        public ISet<string> Strings => _strings;

        public IBitMatrix Numbers => _numbers;
        
        public void Add(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (IsOnlyNumbers(value))
            {
                var (row, column) = SplitNumbersValue(value);
                _numbers[row, column] = true;
            }
            else
            {
                _strings.Add(value);
            }
        }

        public bool Contains(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            bool result;
            if (IsOnlyNumbers(value))
            {
                var (row, column) = SplitNumbersValue(value);
                result = _numbers[row, column];
            }
            else
            {
                result = _strings.Contains(value);
            }

            return result;
        }

        private bool IsOnlyNumbers(string value)
        {
            if (value.Length == 11 && value[4] == ',')
            {
                var numbers = value.Substring(0, 4) + value.Substring(5, 6);
                return numbers.All(char.IsDigit);
            }

            return false;
        }

        private (int, int) SplitNumbersValue(string value)
        {
            var onlyNumbers = value.Substring(0, 4) + value.Substring(5, 6);
            var part1 = onlyNumbers.Substring(0, PART1_LENGTH);
            var part2 = onlyNumbers.Substring(PART1_LENGTH, PART2_LENGTH);
            return (int.Parse(part1), int.Parse(part2));
        }
    }
}