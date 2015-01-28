using System;

namespace Ista.FileServices.Infrastructure.Extensions
{
    public static class StringArrayExtensions
    {
        /// <summary>
        /// Returns the string value stored in the array at the index supplied.
        /// </summary>
        /// <remarks>
        /// The value returned will be null if the array length is less than
        /// the index supplied or the indexed value is string.Empty.
        /// </remarks>
        /// <param name="collection">string array</param>
        /// <param name="index">index value</param>
        /// <returns>string value in the array at the index supplied or null</returns>
        public static string AtIndex(this string[] collection, int index)
        {
            if (collection.Length <= index)
                return null;

            var value = collection[index];
            if (string.IsNullOrEmpty(value))
                return null;

            return value.Trim();
        }

        /// <summary>
        /// Returns the string value stored in the array at the index supplied.
        /// </summary>
        /// <remarks>
        /// The value returned will be an empty string if the array length is less than
        /// the index supplied or the indexed value is string.Empty.
        /// </remarks>
        /// <param name="collection">string array</param>
        /// <param name="index">index value</param>
        /// <returns>string value in the array at the index supplied or empty string</returns>
        public static string AtIndexOrEmpty(this string[] collection, int index)
        {
            if (collection.Length <= index)
                return string.Empty;

            var value = collection[index];
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Trim();
        }

        /// <summary>
        /// Returns the decimal value stored in the array at the index supplied.
        /// </summary>
        /// <remarks>
        /// The value returned will be 0.0M if the array length is less than
        /// the index supplied or the indexed value cannot be converted to an
        /// decimal.
        /// </remarks>
        /// <param name="collection">string array</param>
        /// <param name="index">index value</param>
        /// <returns>decimal value in the array at the index supplied or 0.0M</returns>
        public static decimal AtIndexDecimal(this string[] collection, int index)
        {
            return AtIndexDecimal(collection, index, 0);
        }

        public static decimal AtIndexDecimal(this string[] collection, int index, int scale)
        {
            if (collection.Length <= index)
                return 0.0M;

            var value = collection[index].Trim();

            decimal parsedValue;
            if (decimal.TryParse(value, out parsedValue))
            {
                var divisor = (decimal)Math.Pow(10, scale);
                return (parsedValue / divisor);
            }

            return 0.0M;
        }

        /// <summary>
        /// Returns the integer value stored in the array at the index supplied.
        /// </summary>
        /// <remarks>
        /// The value returned will be 0 if the array length is less than
        /// the index supplied or the indexed value cannot be converted to an
        /// integer.
        /// </remarks>
        /// <param name="collection">string array</param>
        /// <param name="index">index value</param>
        /// <returns>integer value in the array at the index supplied or 0-zero</returns>
        public static int AtIndexInt(this string[] collection, int index)
        {
            if (collection.Length <= index)
                return 0;

            var value = collection[index].Trim();

            int parsedValue;
            if (int.TryParse(value, out parsedValue))
                return parsedValue;

            return 0;
        }
        
        /// <summary>
        /// Delegates the assignment of the returned string value stored in the array at 
        /// the index supplied.
        /// </summary>
        /// <remarks>
        /// The action will be executed only when the array is greater or equal to the index
        /// supplied or the string value is not string.Empty.
        /// </remarks>
        /// <param name="collection">string array</param>
        /// <param name="index">index value</param>
        /// <param name="command">delegate which has the string value in the array at the index supplied</param>
        public static void TryAtIndex(this string[] collection, int index, Action<string> command)
        {
            if (collection.Length <= index)
                return;

            var value = collection[index];
            if (string.IsNullOrEmpty(value))
                return;

            command(value.Trim());
        }

        public static void TryAtIndexDecimal(this string[] collection, int index, Action<decimal> command)
        {
            TryAtIndexDecimal(collection, index, 0, command);
        }

        public static void TryAtIndexDecimal(this string[] collection, int index, int scale, Action<decimal> command)
        {
            if (collection.Length <= index)
                return;

            var value = collection[index].Trim();

            decimal parsedValue;
            if (decimal.TryParse(value, out parsedValue))
            {
                var divisor = (decimal)Math.Pow(10, scale);
                parsedValue /= divisor;

                command(parsedValue);
            }
        }

        /// <summary>
        /// Delegates the assignment of the returned integer value stored in the array at 
        /// the index supplied.
        /// </summary>
        /// <remarks>
        /// The action will be executed only when the array is greater or equal to the index
        /// supplied or if the indexed value can be converted to an integer.
        /// </remarks>
        /// <param name="collection">string array</param>
        /// <param name="index">index value</param>
        /// <param name="command">delegate which has the integer value in the array at the index supplied</param>
        public static void TryAtIndexInt(this string[] collection, int index, Action<int> command)
        {
            if (collection.Length <= index)
                return;

            var value = collection[index].Trim();

            int parsedValue;
            if (int.TryParse(value, out parsedValue))
                command(parsedValue);
        }
    }
}
