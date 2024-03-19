using System.Data;
using System.Data.SqlClient;

namespace Coffee.DAL
{
    /// <summary>
    /// Đây là kỹ thuật SQL Command, nên dùng công nghệ khác: Entity freamwork và Dapper
    /// </summary>
    public class ProviderDAL
    {
        private static ProviderDAL _instance;

        public static ProviderDAL Instance { get => _instance ?? new ProviderDAL(); private set => _instance = value; }
        private ProviderDAL() { }

        private readonly string connString = "Data Source=.\\sqlexpress;Initial Catalog=StoreManage;Integrated Security=True";
        public DataTable ExecuteQuery(string query)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connString))
            {
                var da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }
            return dt;
        }
        public int ExecuteNonQuery(string query)
        {
            int number;
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var cm = new SqlCommand(query, conn);
                number = cm.ExecuteNonQuery();
                conn.Close();
            }
            return number;
        }
        public object ExecuteScalar(string query)
        {
            object obj;
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var cm = new SqlCommand(query, conn);
                obj = cm.ExecuteScalar();
                conn.Close();
            }
            return obj;
        }
    }
}
