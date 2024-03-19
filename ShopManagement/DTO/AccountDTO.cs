using System;
using System.Data;

namespace Coffee.DTO
{
    public class AccountDTO
    {
        public string Name { get; set; }
        public string NickName { get; set; }
        public byte Status { get; set; }
        public AccountDTO(string name, string nickname, byte status) 
        { 

            Name = name;
            NickName = nickname;
            Status = status;
        }
        public AccountDTO(DataRow row)
        {
            Name = row["name"].ToString();
            NickName = row["nickname"].ToString();
            Status = Convert.ToByte(row["status"]);
        }
    }
    public enum AccountError { WrongPassword, AccountNotFount, UnknowError}
}
