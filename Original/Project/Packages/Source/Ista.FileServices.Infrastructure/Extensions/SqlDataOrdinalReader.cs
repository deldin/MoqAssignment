using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Ista.FileServices.Infrastructure.Extensions
{
    public class SqlDataOrdinalReader : IDisposable
    {
        private readonly SqlDataReader reader;
        private readonly Dictionary<string, int> dictionary;

        public SqlDataOrdinalReader(SqlDataReader reader)
        {
            this.reader = reader;
            dictionary = new Dictionary<string, int>();
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        public bool Read()
        {
            return reader.Read();
        }

        public bool NextResult()
        {
            dictionary.Clear();
            return reader.NextResult();
        }

        public bool GetBoolean(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetBoolean method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetBoolean(index);
        }

        public DateTime GetDateTime(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetDateTime method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetDateTime(index);
        }

        public decimal GetDecimal(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetDecimal method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetDecimal(index);
        }

        public double GetDouble(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetDouble method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetDouble(index);
        }

        public float GetFloat(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetFloat method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetFloat(index);
        }

        public Guid GetGuid(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetGuid method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetGuid(index);
        }

        public short GetInt16(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetInt16 method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetInt16(index);
        }

        public int GetInt32(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetInt32 method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetInt32(index);
        }

        public long GetInt64(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetInt64 method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetInt64(index);
        }

        public string GetString(string name)
        {
            var index = GetOrdinalOrThrow(name);
            if (reader.IsDBNull(index))
            {
                var message = string.Format("\"{0}\" is Null. This method cannot be called on Null values. Code should be modified to call the TryGetString method.", name);
                throw new SqlNullValueException(message);
            }

            return reader.GetString(index);
        }

        public void TryGetBoolean(string name, Action<bool> action)
        {
            TryGetBoolean(name, action, true);
        }

        public void TryGetBoolean(string name, Action<bool> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
                return;

            reader.TryGetBoolean(index, action);
        }

        public void TryGetDateTime(string name, Action<DateTime> action)
        {
            TryGetDateTime(name, action, true);
        }

        public void TryGetDateTime(string name, Action<DateTime> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
                return;

            reader.TryGetDateTime(index, action);
        }

        public void TryGetDecimal(string name, Action<decimal> action)
        {
            TryGetDecimal(name, action, true);
        }

        public void TryGetDecimal(string name, Action<decimal> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
                return;

            reader.TryGetDecimal(index, action);
        }

        public void TryGetDouble(string name, Action<double> action)
        {
            TryGetDouble(name, action, true);
        }

        public void TryGetDouble(string name, Action<double> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
                return;

            reader.TryGetDouble(index, action);
        }

        public void TryGetFloat(string name, Action<float> action)
        {
            TryGetFloat(name, action, true);
        }

        public void TryGetFloat(string name, Action<float> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
                return;

            reader.TryGetFloat(index, action);
        }

        public void TryGetGuid(string name, Action<Guid> action)
        {
            TryGetGuid(name, action, true);
        }

        public void TryGetGuid(string name, Action<Guid> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
                return;

            reader.TryGetGuid(index, action);
        }

        public void TryGetInt16(string name, Action<short> action)
        {
            TryGetInt16(name, action, true);
        }

        public void TryGetInt16(string name, Action<short> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
                return;

            reader.TryGetInt16(index, action);
        }

        public void TryGetInt32(string name, Action<int> action)
        {
            TryGetInt32(name, action, true);
        }

        public void TryGetInt32(string name, Action<int> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
                return;

            reader.TryGetInt32(index, action);
        }

        public void TryGetInt64(string name, Action<long> action)
        {
            TryGetInt64(name, action, true);
        }

        public void TryGetInt64(string name, Action<long> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
                return;

            reader.TryGetInt64(index, action);
        }

        public void TryGetString(string name, Action<string> action)
        {
            TryGetString(name, action, true);
        }

        public void TryGetString(string name, Action<string> action, bool mustExist)
        {
            int index;
            if (!TryGetOrdinal(name, mustExist, out index))
            {
                action(string.Empty);
                return;
            }

            reader.TryGetString(index, action);
        }

        private int GetOrdinalOrThrow(string name)
        {
            if (dictionary.Count == 0)
                InitializeMapping();

            int index;
            if (!dictionary.TryGetValue(name, out index))
                throw new IndexOutOfRangeException(name);
            
            return index;
        }

        private bool TryGetOrdinal(string name, bool mustExist, out int index)
        {
            if (dictionary.Count == 0)
                InitializeMapping();

            if (!dictionary.TryGetValue(name, out index))
            {
                if (mustExist)
                    throw new IndexOutOfRangeException(name);

                index = -1;
                return false;
            }

            return true;
        }

        private void InitializeMapping()
        {
            for (int fieldIndex = 0, fieldCount = reader.FieldCount; fieldIndex < fieldCount; fieldIndex++)
            {
                var name = reader.GetName(fieldIndex);
                dictionary[name] = fieldIndex;
            }
        }

    }
}
