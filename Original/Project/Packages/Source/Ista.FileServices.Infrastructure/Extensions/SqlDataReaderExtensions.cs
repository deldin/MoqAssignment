using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Ista.FileServices.Infrastructure.Extensions
{
    public static class SqlDataReaderExtensions
    {
        public static bool GetBoolean(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetBoolean method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetBoolean(index);
        }

        public static DateTime GetDateTime(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetDateTime method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetDateTime(index);
        }

        public static decimal GetDecimal(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetDecimal method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetDecimal(index);
        }

        public static double GetDouble(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetDouble method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetDouble(index);
        }

        public static float GetFloat(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetFloat method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetFloat(index);
        }

        public static Guid GetGuid(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetGuid method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetGuid(index);
        }

        public static short GetInt16(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetInt16 method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetInt16(index);
        }

        public static int GetInt32(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetInt32 method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetInt32(index);
        }

        public static long GetInt64(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetInt64 method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetInt64(index);
        }

        public static string GetString(this SqlDataReader reader, string name)
        {
            var index = GetOrdinalOrThrow(reader, name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetString method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetString(index);
        }

        public static void TryGetBoolean(this SqlDataReader reader, string name, Action<bool> action)
        {
            TryGetBoolean(reader, name, action, true);
        }

        public static void TryGetBoolean(this SqlDataReader reader, string name, Action<bool> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
                return;

            TryGetBoolean(reader, index, action);
        }

        public static void TryGetBoolean(this SqlDataReader reader, int index, Action<bool> action)
        {
            if (reader.IsDBNull(index))
                return;

            action(reader.GetBoolean(index));
        }

        public static void TryGetDateTime(this SqlDataReader reader, string name, Action<DateTime> action)
        {
            TryGetDateTime(reader, name, action, true);
        }

        public static void TryGetDateTime(this SqlDataReader reader, string name, Action<DateTime> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
                return;

            TryGetDateTime(reader, index, action);
        }

        public static void TryGetDateTime(this SqlDataReader reader, int index, Action<DateTime> action)
        {
            if (reader.IsDBNull(index))
                return;

            action(reader.GetDateTime(index));
        }

        public static void TryGetDecimal(this SqlDataReader reader, string name, Action<decimal> action)
        {
            TryGetDecimal(reader, name, action, true);
        }

        public static void TryGetDecimal(this SqlDataReader reader, string name, Action<decimal> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
                return;

            TryGetDecimal(reader, index, action);
        }

        public static void TryGetDecimal(this SqlDataReader reader, int index, Action<decimal> action)
        {
            if (reader.IsDBNull(index))
                return;

            action(reader.GetDecimal(index));
        }

        public static void TryGetDouble(this SqlDataReader reader, string name, Action<double> action)
        {
            TryGetDouble(reader, name, action, true);
        }

        public static void TryGetDouble(this SqlDataReader reader, string name, Action<double> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
                return;

            TryGetDouble(reader, index, action);
        }

        public static void TryGetDouble(this SqlDataReader reader, int index, Action<double> action)
        {
            if (reader.IsDBNull(index))
                return;

            action(reader.GetDouble(index));
        }

        public static void TryGetFloat(this SqlDataReader reader, string name, Action<float> action)
        {
            TryGetFloat(reader, name, action, true);
        }

        public static void TryGetFloat(this SqlDataReader reader, string name, Action<float> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
                return;

            TryGetFloat(reader, index, action);
        }

        public static void TryGetFloat(this SqlDataReader reader, int index, Action<float> action)
        {
            if (reader.IsDBNull(index))
                return;

            action(reader.GetFloat(index));
        }

        public static void TryGetGuid(this SqlDataReader reader, string name, Action<Guid> action)
        {
            TryGetGuid(reader, name, action, true);
        }

        public static void TryGetGuid(this SqlDataReader reader, string name, Action<Guid> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
                return;

            TryGetGuid(reader, index, action);
        }

        public static void TryGetGuid(this SqlDataReader reader, int index, Action<Guid> action)
        {
            if (reader.IsDBNull(index))
                return;

            action(reader.GetGuid(index));
        }

        public static void TryGetInt16(this SqlDataReader reader, string name, Action<short> action)
        {
            TryGetInt16(reader, name, action, true);
        }

        public static void TryGetInt16(this SqlDataReader reader, string name, Action<short> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
                return;

            TryGetInt16(reader, index, action);
        }

        public static void TryGetInt16(this SqlDataReader reader, int index, Action<short> action)
        {
            if (reader.IsDBNull(index))
                return;

            action(reader.GetInt16(index));
        }

        public static void TryGetInt32(this SqlDataReader reader, string name, Action<int> action)
        {
            TryGetInt32(reader, name, action, true);
        }

        public static void TryGetInt32(this SqlDataReader reader, string name, Action<int> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
                return;

            TryGetInt32(reader, index, action);
        }

        public static void TryGetInt32(this SqlDataReader reader, int index, Action<int> action)
        {
            if (reader.IsDBNull(index))
                return;

            action(reader.GetInt32(index));
        }

        public static void TryGetInt64(this SqlDataReader reader, string name, Action<long> action)
        {
            TryGetInt64(reader, name, action, true);
        }

        public static void TryGetInt64(this SqlDataReader reader, string name, Action<long> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
                return;

            TryGetInt64(reader, index, action);
        }

        public static void TryGetInt64(this SqlDataReader reader, int index, Action<long> action)
        {
            if (reader.IsDBNull(index))
                return;

            action(reader.GetInt64(index));
        }

        public static void TryGetString(this SqlDataReader reader, string name, Action<string> action)
        {
            TryGetString(reader, name, action, true);
        }

        public static void TryGetString(this SqlDataReader reader, string name, Action<string> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(reader, name, mustExist, out index))
            {
                action(string.Empty);
                return;
            }

            TryGetString(reader, index, action);
        }

        public static void TryGetString(this SqlDataReader reader, int index, Action<string> action)
        {
            if (reader.IsDBNull(index))
            {
                action(string.Empty);
                return;
            }

            action(reader.GetString(index));
        }

        private static int GetOrdinalOrThrow(this SqlDataReader reader, string name)
        {
            var index = reader.GetOrdinal(name);
            if (index == -1)
            {
                var message = string.Format("Column \"{0}\" does not exist in the result set.", name);
                throw new ArgumentOutOfRangeException("name", message);
            }

            return index;
        }

        private static bool TryGetOrdinal(this SqlDataReader reader, string name, bool mustExist, out int index)
        {
            if (mustExist)
            {
                index = GetOrdinalOrThrow(reader, name);
                return true;
            }

            index = -1;
            for (int fieldIndex = 0, fieldCount = reader.FieldCount; fieldIndex < fieldCount && index == -1; fieldIndex++)
            {
                if (name.Equals(reader.GetName(fieldIndex), StringComparison.Ordinal))
                    index = fieldIndex;
            }

            return (index != -1);
        }
    }
}
