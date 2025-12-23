using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Scada.Core.Domain;

namespace Scada.Core.Data.Repository
{
    public class HistoryRepository
    {
        private readonly string _connStr;

        public HistoryRepository(string connStr) => _connStr = connStr;

        public void BatchSaveHistory(List<SensorPoint> points)
        {
            using var conn = new SqlConnection(_connStr);
            conn.Open();
            using var trans = conn.BeginTransaction();
            try
            {
                // 使用快閃寫入或簡單迴圈，重點在於 Quality 也要存
                string sql = "INSERT INTO History (SID, Value, Quality, LastUpdate) VALUES (@sid, @val, @q, @time)";

                foreach (var sp in points)
                {
                    if (sp.Value == null) continue; // 沒讀到值就不存

                    using var cmd = new SqlCommand(sql, conn, trans);
                    cmd.Parameters.AddWithValue("@sid", sp.SID);
                    cmd.Parameters.AddWithValue("@val", sp.Value);
                    cmd.Parameters.AddWithValue("@q", (int)sp.Quality); // 存入 1 (Good) 或 0 (Bad)
                    cmd.Parameters.AddWithValue("@time", sp.LastUpdate);
                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch { trans.Rollback(); throw; }
        }
    }
}