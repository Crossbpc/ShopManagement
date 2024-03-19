using Coffee.DAL;
using Coffee.DTO;
using System.Collections.Generic;
using System.Data;

namespace Coffee.BUS
{
    public class CatergoryBUS
    {
        private static CatergoryBUS _instance;

        public static CatergoryBUS Instance { get => _instance ?? new CatergoryBUS(); private set => _instance = value; }
        private CatergoryBUS() { }
        public List<CategoryDTO> GetcategoryList()
        {
            var categorylist = new List<CategoryDTO>();
            var data = ProviderDAL.Instance.ExecuteQuery("select * from categorys");
            foreach (DataRow item in data.Rows)
            {
                CategoryDTO category = new CategoryDTO(item);
                categorylist.Add(category);
            }
            return categorylist;
        }
        public int AddCategory(string categoryName) => ProviderDAL.Instance.ExecuteNonQuery($"insert categorys(name) values (N'{categoryName}')");
        public int DelCategory(int categoryId) => ProviderDAL.Instance.ExecuteNonQuery($"delete categorys where id = " + categoryId);
        public int EditCategory(CategoryDTO category) => ProviderDAL.Instance.ExecuteNonQuery($"update categorys set name = N'{category.Name}' where id = {category.Id}");

    }
}
