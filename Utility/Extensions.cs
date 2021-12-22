using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adventofcode.Utility
{
    static class Extensions
    {
        public static bool IsInRangeOf<T>(this T val, T lower, T upper) where T : IComparable
        {
            return val.CompareTo(lower) >= 0 && val.CompareTo(upper) <= 0;
        }

        public static bool ContentEquals<T>(this T[,] array, T[,] other)
        {
            if (array is null && other is null)
                return true;
            if (array is null || other is null)
                return false;
            if (array.GetLength(0) != other.GetLength(0) || array.GetLength(1) != other.GetLength(1))
                return false;

            for (int y = 0; y < array.GetLength(1); y++)
                for (int x = 0; x < array.GetLength(0); x++)
                    if (!EqualityComparer<T>.Default.Equals(array[x, y], other[x, y]))
                        return false;

            return true;
        }
    }
}
