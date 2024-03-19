using System.Data;

namespace Coffee.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public CategoryDTO(DataRow row)
        {
            Id = (int)row["id"];
            Name = row["name"].ToString();
        }
    }
}
