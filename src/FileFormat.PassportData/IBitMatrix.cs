using System.Collections.Generic;

namespace FileFormat.PassportData
{
    public interface IBitMatrix
    {
        IReadOnlyDictionary<int, byte[]> Dictionary { get; }

        bool this[int row, int column] { get; set; }
    }
}
