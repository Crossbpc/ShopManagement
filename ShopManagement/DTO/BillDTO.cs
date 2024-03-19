using System;
using System.Data;

namespace Coffee.DTO
{
    public class BillDTO
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int TableId { get; set; }
        public int Discount { get; set; }
        public int TotalMoney { get; set; }
        public DateTime Datein { get; set; }
        public DateTime? Dateout { get; set; }
        public BillDTO(int id, int tableid, string status, int discount, int totalMoney, DateTime datein, DateTime? dateout)
        {
            Id = id;
            Status = status;
            TableId = tableid;
            Discount = discount;
            TotalMoney = totalMoney;
            Datein = datein;
            Dateout = dateout;
        }
        public BillDTO(DataRow row)
        {
            Id = (int)row["id"];
            Status = row["status"].ToString();
            TableId = (int)row["tableid"];
            Discount = (int)row["discount"];
            TotalMoney = (int)row["totalMoney"];
            Datein = (DateTime)row["datein"];
            Dateout = (DateTime?)row["dateout"];
        }
    }  
}
