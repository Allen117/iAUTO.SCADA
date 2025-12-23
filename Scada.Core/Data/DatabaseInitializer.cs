using System;
using System.IO;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace Scada.Core.Data
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(string connStr) => _connectionString = connStr;

        public void InitializeFromPath(string jsonPath)
        {
            if (!File.Exists(jsonPath)) return;

            string jsonContent = File.ReadAllText(jsonPath);
            using var doc = JsonDocument.Parse(jsonContent);
            var setup = doc.RootElement.GetProperty("DatabaseSetup");

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // 1. 建立所有資料表
            foreach (var table in setup.GetProperty("Tables").EnumerateArray())
            {
                string tableName = table.GetProperty("TableName").GetString();
                string script = table.GetProperty("CreateScript").GetString();

                if (!CheckTableExists(conn, tableName))
                {
                    ExecuteSql(conn, script);
                    Console.WriteLine($"[DB] 資料表 {tableName} 已建立。");
                }
            }

            // 2. 建立所有索引
            foreach (var idx in setup.GetProperty("Indexes").EnumerateArray())
            {
                string idxName = idx.GetProperty("IndexName").GetString();
                string script = idx.GetProperty("CreateScript").GetString();

                // 索引通常與資料表一起建立，這裡可依需求增加判斷
                try { ExecuteSql(conn, script); }
                catch { /* 索引已存在時忽略錯誤 */ }
            }
        }

        private bool CheckTableExists(SqlConnection conn, string tableName)
        {
            string sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @name";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", tableName);
            return (int)cmd.ExecuteScalar() > 0;
        }

        private void ExecuteSql(SqlConnection conn, string sql)
        {
            using var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
    }
}