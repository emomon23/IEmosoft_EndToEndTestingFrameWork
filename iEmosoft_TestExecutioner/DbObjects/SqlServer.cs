using aUI.Automation.HelperObjects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace aUI.Automation.DbObjects
{
    public class SqlServer : IDisposable
    {
        private string Connection;
        private SqlConnection Conn;
        public List<List<object>> Results;
        public List<string> Headers;
        public int RowsAffected = -1;

        public SqlServer(string connection)
        {
            Connection = connection;
        }

        public SqlServer(string ip, string catalog, string id, string pw)
        {
            Connection = $"Data Source={ip};Initial Catalog={catalog};Persist Security Info=True;User ID={id};Password={pw}";
            OpenConnection();
        }

        public SqlServer()
        {
            var ip = Config.GetConfigSetting("");
            var id = Config.GetConfigSetting("");
            var pw = Config.GetConfigSetting("");
            var catalog = Config.GetConfigSetting("");

            Connection = $"Data Source={ip};Initial Catalog={catalog};Persist Security Info=True;User ID={id};Password={pw}";
            OpenConnection();
        }

        private void OpenConnection()
        {
            Conn = new SqlConnection(Connection);
            Conn.Open();
        }

        public void ExecuteQuery(QueryBuilder query)
        {
            query.BuildQuery();
            ExecuteQuery(query.Query);
        }

        public void ExecuteQuery(string query)
        {
            Results = new List<List<object>>();
            Headers = new List<string>();

            var cmd = new SqlCommand(query, Conn);
            var result = cmd.ExecuteReader();

            while (result.Read())
            {
                var row = new object[result.FieldCount];
                result.GetValues(row);
                Results.Add(new List<object>(row));
            }
            RowsAffected = Results.Count;

            for (int i = 0; i < result.FieldCount; i++)
            {
                Headers.Add(result.GetName(i));
            }
            result.Close();
        }

        public void ExecuteTransaction(string query)
        {
            var cmd = new SqlCommand(query, Conn);
            RowsAffected = cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            if (Conn != null)
            {
                Conn.Close();
            }
        }
    }
}
