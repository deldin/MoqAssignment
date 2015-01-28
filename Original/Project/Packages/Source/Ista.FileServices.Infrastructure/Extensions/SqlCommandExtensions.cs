using System;
using System.Data;
using System.Data.SqlClient;

namespace Ista.FileServices.Infrastructure.Extensions
{
    public static class SqlCommandExtensions
    {
        public static SqlCommand AddIfNotEmptyOrDbNull(this SqlCommand command, string parameterName, string value)
        {
            var parameter = new SqlParameter { ParameterName = parameterName };
            if (string.IsNullOrEmpty(value))
                parameter.Value = DBNull.Value;
            else
                parameter.Value = value;

            command.Parameters.Add(parameter);
            return command;
        }

        public static SqlCommand AddWithValue(this SqlCommand command, string parameterName, object value)
        {
            var parameter = new SqlParameter
            {
                ParameterName = parameterName,
                Value = value
            };

            command.Parameters.Add(parameter);
            return command;
        }

        public static SqlCommand AddWithValueOrDbNull(this SqlCommand command, string parameterName, object value)
        {
            var parameter = new SqlParameter
            {
                ParameterName = parameterName,
                Value = value ?? DBNull.Value
            };

            command.Parameters.Add(parameter);
            return command;
        }

        public static SqlCommand AddWithValueOrDbNull<TValue>(this SqlCommand command, string parameterName, TValue value, Func<TValue, bool> predicate)
        {
            var parameter = new SqlParameter { ParameterName = parameterName };
            if (predicate(value))
                parameter.Value = value;
            else
                parameter.Value = DBNull.Value;

            command.Parameters.Add(parameter);
            return command;
        }

        public static SqlCommand AddWithValueOrDbNull<TValue>(this SqlCommand command, string parameterName, TValue? value)
            where TValue : struct
        {
            var parameter = new SqlParameter { ParameterName = parameterName };
            if (value.HasValue)
                parameter.Value = value.Value;
            else
                parameter.Value = DBNull.Value;

            command.Parameters.Add(parameter);
            return command;
        }

        public static SqlCommand AddOutParameter(this SqlCommand command, string parameterName, SqlDbType type)
        {
            var parameter = new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = type,
                Direction = ParameterDirection.Output,
            };

            command.Parameters.Add(parameter);
            return command;
        }

        public static SqlCommand AddOutParameter(this SqlCommand command, string parameterName, SqlDbType type, out SqlParameter parameter)
        {
            parameter = new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = type,
                Direction = ParameterDirection.Output,
            };

            command.Parameters.Add(parameter);
            return command;
        }

        public static SqlDataOrdinalReader ExecuteReaderOrdinal(this SqlCommand command)
        {
            var reader = command.ExecuteReader();
            return new SqlDataOrdinalReader(reader);
        }
    }
}
