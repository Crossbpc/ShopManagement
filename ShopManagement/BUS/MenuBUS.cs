using Coffee.DAL;
using Coffee.DTO;
using System.Collections.Generic;
using System.Data;

namespace Coffee.BUS
{
    public class MenuBUS
    {
        private static MenuBUS _instance;

        public static MenuBUS Instance { get => _instance ?? new MenuBUS(); private set => _instance = value; }
        private MenuBUS() { }
        public List<MenuDTO> GetMenuList(int tableid)
        {
            var list = new List<MenuDTO>();
            var data = ProviderDAL.Instance.ExecuteQuery($"sp_Getmenu {tableid}");
            foreach (DataRow item in data.Rows)
            {
                MenuDTO menu = new MenuDTO(item);
                list.Add(menu);
            }
            return list;
        }
       
    }
}
