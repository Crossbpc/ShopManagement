using System.Data;

namespace Coffee.DTO
{
    public class MenuDTO
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public int Money { get; set; }
        public MenuDTO(string name, int count, int price, int money)
        { 
            Name = name;
            Count = count;
            Price = price;
            Money = money;
        }
        public MenuDTO(DataRow row)
        {
            Name = row["name"].ToString();
            Count = (int)row["count"];
            Price = (int)row["price"];
            Money = (int)row["money"];
        }
    }
}
