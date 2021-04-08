using aUI.Automation.HelperObjects;
using Npgsql;
using System;
using System.Collections.Generic;

namespace aUI.Automation.DbObjects
{
    public class PostgreSql : IDisposable
    {
        private string Connection;
        private NpgsqlConnection Conn;
        public List<List<string>> Results;
        public List<string> Headers;

        public PostgreSql(string connection)
        {
            Connection = connection;
            OpenConnection();
        }

        public PostgreSql(string server, string id, string pw, string db)
        {
            Connection = $"Server={server};User Id={id};Password={pw};Database={db};Pooling=false;Timeout=300;CommandTimeout=300;";
            OpenConnection();
        }

        public PostgreSql()
        {
            var server = Config.GetConfigSetting("");
            var id = Config.GetConfigSetting("");
            var pw = Config.GetConfigSetting("");
            var db = Config.GetConfigSetting("");

            Connection = $"Server={server};User Id={id};Password={pw};Database={db};Pooling=false;Timeout=300;CommandTimeout=300;";
            OpenConnection();
        }

        private void OpenConnection()
        {
            Conn = new NpgsqlConnection(Connection);
        }

        public void ExecuteQuery(QueryBuilder query)
        {
            query.BuildQuery();
            ExecuteQuery(query.Query);
        }

        public void ExecuteQuery(string query)
        {
            Results = new List<List<string>>();
            Headers = new List<string>();

            var cmd = new NpgsqlCommand(query, Conn);
            var result = cmd.ExecuteReader();

            while (result.Read())
            {
                var row = new string[result.FieldCount];
                result.GetValues(row);
                Results.Add(new List<string>(row));
            }

            for (int i = 0; i < result.FieldCount; i++)
            {
                Headers.Add(result.GetName(i));
            }
        }

        public void Dispose()
        {
            if (Conn != null)
            {
                Conn.Close();
            }
            GC.SuppressFinalize(this);
        }
    }
}
