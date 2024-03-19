using Coffee.DAL;
using Coffee.DTO;
using System.Collections.Generic;
using System.Data;

namespace Coffee.BUS
{
    public class TableBUS
    {
        private static TableBUS _instance;

        public static TableBUS Instance { get => _instance ?? new TableBUS(); private set => _instance = value; }
        private TableBUS() { }
        public List<TableDTO> GetTableList()
        {
            var list = new List<TableDTO>();
            var data = ProviderDAL.Instance.ExecuteQuery("select * from Tables");
            foreach (DataRow row in data.Rows)
            {
                TableDTO table = new TableDTO(row);
                list.Add(table);
            }
            return list;
        }
        public void ChangeTable(int tablefirtid, int tablesecondid)
        {
            ProviderDAL.Instance.ExecuteQuery($"sp_ChangeTable {tablefirtid}, {tablesecondid}");
        }
        public int AddTable(string tableName) => ProviderDAL.Instance.ExecuteNonQuery($"insert tables(name, status) values (N'{tableName}', N'Trống')");
        public int DelTable(int tableId) => ProviderDAL.Instance.ExecuteNonQuery($"delete tables where id = " + tableId);
        public int EditTable(TableDTO table) => ProviderDAL.Instance.ExecuteNonQuery($"update tables set name = N'{table.Name}', status = N'{table.Status}' where id = {table.Id}");

    }
}
