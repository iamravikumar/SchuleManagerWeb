using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Web;

namespace SchuleManager.Helpers
{
    public static class ConvertDataSet
    {
        private static readonly CreateErrorLogs Clf = new CreateErrorLogs();
        private static readonly string ErrorLogPath = Path.Combine(HttpContext.Current.Server.MapPath("~/ErrorLogs/"));

        public static DataTable ListToDataTable<T>(IList<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));

            var dataTable = new DataTable();

            try
            {
                foreach (PropertyDescriptor prop in properties)
                    dataTable.Columns.Add(prop.Name,
                        Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

                foreach (var item in data)
                {
                    var row = dataTable.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    dataTable.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                Clf.CreateErrorLog(ErrorLogPath, "Error at ListToDataTable Utility method: " + ex);
            }
            return dataTable;
        }
    }
}