using System.Data;

namespace Coffee.DTO
{
    public class BillInforDTO
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        public int FoodId { get; set; }
        public int Count { get; set; }
        public BillInforDTO(int id, int billid, int foodid, int count) 
        { 
            Id = id;
            BillId = billid;
            FoodId = foodid;
            Count = count;
        }
        public BillInforDTO(DataRow row)
        {
            Id = (int)row["id"];
            BillId = (int)row["billid"];
            FoodId = (int)row["foodid"];
            Count = (int)row["count"];
        }
    }
}
