using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace EventManagement.Application.Helpers
{
    public static class GetDataTable
    {
        private const string _value = "Nullable";
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            var table = new DataTable(typeof(T).Name);

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in props)
            {
                var column = new DataColumn
                {
                    ColumnName = propertyInfo.Name,
                    DataType = propertyInfo.PropertyType.Name.Contains(_value)
                        ? typeof(string)
                        : propertyInfo.PropertyType
                };

                table.Columns.Add(column);
            }

            foreach (var item in list)
            {
                var values = new object[props.Length];

                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                table.Rows.Add(values);
            }


            return table;
        }
    }
}