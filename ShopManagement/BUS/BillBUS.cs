using Coffee.DAL;
using System.Data;

namespace Coffee.BUS
{
    public class BillBUS
    {
        private static BillBUS _instances;

        public static BillBUS Instances { get => _instances ?? new BillBUS(); private set => _instances = value; }
        private BillBUS() { }
        public int GetBillId(int tableid)
        {
            var data = ProviderDAL.Instance.ExecuteScalar($"select id from Bills where tableid = {tableid} and status = 0");
            if (data == null)
                return -1; 
            else
                return (int)data;
        }
        public int InsertBill(int tableid)
        {
            return ProviderDAL.Instance.ExecuteNonQuery("sp_insertBill "+ tableid);
        }
        public int GetMaxBillId() => (int)ProviderDAL.Instance.ExecuteScalar("select max (id) from bills");
        public int CheckOut(int billid, int discount, int finalmoney) => (int)ProviderDAL.Instance.ExecuteNonQuery($"sp_CheckOut {billid}, {discount}, {finalmoney}");
        public DataTable GetstatisticalData(string dateint, string dateout) => ProviderDAL.Instance.ExecuteQuery($"sp_Statistical '{dateint}', '{dateout}'");

    }
}
