using Coffee.DAL;

namespace Coffee.BUS
{
    public class BillInforBUS
    {
        private static BillInforBUS _instances;

        public static BillInforBUS Instances { get => _instances ?? new BillInforBUS(); private set => _instances = value; }
        private BillInforBUS() { }
        public int InsertBillInfor(int billid, int foodid, int count)
        {
            return ProviderDAL.Instance.ExecuteNonQuery($"sp_insertBillInfors {billid}, {foodid}, {count}");
        }
        public int DelBillInfor(int foodid) => ProviderDAL.Instance.ExecuteNonQuery("delete billinfors where foodid = " + foodid);
    }
}
