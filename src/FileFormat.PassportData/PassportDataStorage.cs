using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FileFormat.PassportData
{
    public class PassportDataStorage
    {
        public const int PART2_NUM_VALUES = 1000;
        public const int PART2_NUM_BYTES = (PART2_NUM_VALUES >> 3) + (PART2_NUM_VALUES % 8 == 0 ? 0 : 1);
        private const int PART1_LENGTH = 7;
        private const int PART2_LENGTH = 3;
        private const int COMMA_INDEX = 4;

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
            if (value.Length == PART1_LENGTH + PART2_LENGTH + 1 && value[COMMA_INDEX] == ',')
            {
                for (int index = 0; index < value.Length; index++)
                {
                    if (index != COMMA_INDEX && !char.IsDigit(value[index]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private (int, int) SplitNumbersValue(string value)
        {
            var part1 = 0;
            var part2 = 0;
            var noCommaIndex = 0;
            for (var index = 0; index < value.Length; index++)
            {
                if (index == COMMA_INDEX)
                {
                    continue;
                }

                var digit = value[index] - '0';
                if (noCommaIndex < PART1_LENGTH)
                {
                    part1 = (part1 * 10) + digit;
                }
                else
                {
                    part2 = (part2 * 10) + digit;
                }

                noCommaIndex++;
            }

            return (part1, part2);
        }
    }
}