using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileFormat.PassportData
{
    public class BitMatrix : IBitMatrix
    {
        private readonly IDictionary<int, byte[]> _dictionary;
        private readonly int _width;
        private readonly int _sizeInBytes;

        public BitMatrix(int width)
        {
            _width = width;
            _sizeInBytes = (_width >> 3) + (_width % 8 == 0 ? 0 : 1);
            _dictionary = new Dictionary<int, byte[]>();
        }

        public BitMatrix(int width, IDictionary<int, byte[]> dictionary)
        {
            _width = width;
            _sizeInBytes = (_width >> 3) + (_width % 8 == 0 ? 0 : 1);
            _dictionary = dictionary;
        }

        public IReadOnlyDictionary<int, byte[]> Dictionary => new ReadOnlyDictionary<int, byte[]>(_dictionary);

        public bool this[int row, int column]
        {
            get => GetBit(row, column);
            set => SetBit(row, column, value);
        }

        private void SetBit(int row, int column, bool value)
        {
            if (column < 0 || column > _width - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }

            if (!_dictionary.TryGetValue(row, out var data))
            {
                data = new byte[_sizeInBytes];
                _dictionary.Add(row, data);
            }

            var index = column >> 3;
            var bit = column % 8;
            var mask = 1 << bit;
            if (value)
            {
                data[index] = (byte)(data[index] | mask);
            }
            else
            {
                data[index] = (byte)(data[index] & ~mask);
            }
        }

        private bool GetBit(int row, int column)
        {
            if (column < 0 || column > _width - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }

            if (_dictionary.TryGetValue(row, out var data))
            {
                var index = column >> 3;
                var bit = column % 8;
                var mask = 1 << bit;
                var result = (data[index] & mask) == mask;
                return result;
            }

            return false;
        }
    }
}
