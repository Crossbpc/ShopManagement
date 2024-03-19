using System.Data;

namespace Coffee.DTO
{
    public class TableDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public TableDTO (int id, string name, string status)
        {
            Id = id;
            Name = name;
            Status = status;
        }
        public TableDTO(DataRow row)
        {
            Id = (int)row["id"];
            Name = row["name"].ToString();
            Status = row["status"].ToString();
        }
    }
}
