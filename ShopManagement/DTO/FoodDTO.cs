using System.Data;

namespace Coffee.DTO
{
    public class FoodDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CatergoryId { get; set; }
        public int Price { get; set;}
        public FoodDTO(int id, string name, int catergoryId, int price)
        {
            Id = id;
            Name = name;
            CatergoryId = catergoryId;
            Price = price;
        }
        public FoodDTO(DataRow row)
        {
            Id = (int)row["id"];
            Name = row["name"].ToString();
            CatergoryId = (int)row["categoryId"];
            Price = (int)row["price"];
        }
    }
}
