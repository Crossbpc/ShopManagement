using Coffee.DAL;
using Coffee.DTO;
using System.Collections.Generic;
using System.Data;

namespace Coffee.BUS
{
    public class FoodBUS
    {
        private static FoodBUS _instance;

        public static FoodBUS Instance { get => _instance ?? new FoodBUS(); private set => _instance = value; }
        private FoodBUS() { }
        public List<FoodDTO> GetFoodList(int categoryId)
        {
            var list = new List<FoodDTO>();
            var data = ProviderDAL.Instance.ExecuteQuery("select * from Foods where categoryId = " + categoryId);
            foreach (DataRow item in data.Rows)
            {
                FoodDTO food = new FoodDTO(item);
                list.Add(food);
            }
            return list;
        }
        public DataTable GetFoodList() => ProviderDAL.Instance.ExecuteQuery("select c.name [Danh mục], f.id [Mã món], f.name [Tên món], f.price [Giá] from Foods f join categorys c on f.categoryid = c.id");
        public int AddFood(int categoryid, string name, int price) => ProviderDAL.Instance.ExecuteNonQuery($"insert foods (categoryid, name, price) values ({categoryid}, N'{name}', {price})");
        public int EditFood(int categoryid, string name, int price, int foodid) => ProviderDAL.Instance.ExecuteNonQuery($"update foods set categoryid = {categoryid}, name = N'{name}', price = {price} where id = {foodid}");
        public int DelFood(int foodid) => ProviderDAL.Instance.ExecuteNonQuery("delete foods where id = " + foodid);
        public List<FoodDTO> GetFoodlistByName(string foodname)
        {
            var list = new List<FoodDTO>();
            var data = ProviderDAL.Instance.ExecuteQuery($"select * from Foods where [dbo].[fuConvertToUnsign1](name) like N'%'+[dbo].[fuConvertToUnsign1](N'{foodname}')+N'%'");
            foreach (DataRow item in data.Rows)
            {
                FoodDTO food = new FoodDTO(item);
                list.Add(food);
            }
            return list;
        }
    }
}
