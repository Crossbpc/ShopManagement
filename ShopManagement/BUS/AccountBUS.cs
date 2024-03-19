using Coffee.DTO;
using Coffee.DAL;
using System.Data;

namespace Coffee.BUS
{
    public class AccountBUS
    {
        private static AccountBUS _instances;

        public static AccountBUS Instances { get => _instances ?? new AccountBUS(); private set => _instances = value; }
        private AccountBUS() { }
        public object GetAccount(string username, string password)
        {
            object obj = null;
            var passcode = PasswordCodeDTO.GetCode(password);
            var dt = ProviderDAL.Instance.ExecuteQuery($"select * from accounts where name = '{username}' and password = '{passcode}'");
            if (dt.Rows.Count == 1)
            {
                foreach (DataRow item in dt.Rows)
                {
                    obj = new AccountDTO(item);
                    return obj;
                }
            }
            dt = ProviderDAL.Instance.ExecuteQuery($"select * from accounts where name = '{username}' and password != '{passcode}'");
            if (dt.Rows.Count == 1)
                return obj = AccountError.WrongPassword;
            dt = ProviderDAL.Instance.ExecuteQuery($"select * from accounts where name = '{username}'");
            if (dt.Rows.Count == 0)
                return obj = AccountError.AccountNotFount;

            return obj;
        }
        public int RegisterAccount(string username, string password, string nickname)
        {
            var passcode = PasswordCodeDTO.GetCode(password);
            string admin = $"insert accounts (name, nickname, password, status) values ('{username}', N'{nickname}', '{passcode}', 1)";
            string user = $"insert accounts (name, nickname, password) values ('{username}', N'{nickname}', '{passcode}')";
            var result = ProviderDAL.Instance.ExecuteScalar("select * from accounts");
            if (result == null)
                return ProviderDAL.Instance.ExecuteNonQuery(admin);
            else
                return ProviderDAL.Instance.ExecuteNonQuery(user);

        }
        public int EditAccountProFile(string username, string password, string newpassword, string alias)
        {
            var passcode = PasswordCodeDTO.GetCode(password);
            var newpasscode = PasswordCodeDTO.GetCode(newpassword);
            return ProviderDAL.Instance.ExecuteNonQuery($"sp_EditAccountProFile '{username}', '{passcode}', '{newpasscode}', N'{alias}'");
        }
        public DataTable GetAccountList() => ProviderDAL.Instance.ExecuteQuery("select name [Tên], nickname [Biệt danh], status [Loại tài khoản] from accounts");
        public int AddAccount(string username, string nickname, byte type, string password = "1982408718410113214846658453255177250147212")
        {
            var passcode = PasswordCodeDTO.GetCode(password);
            return ProviderDAL.Instance.ExecuteNonQuery($"insert accounts(name, nickname, status, password) values ('{username}', N'{nickname}', {type}, '{passcode}')");

        }
        public int EditAccount(string username, string nickname, byte type) => ProviderDAL.Instance.ExecuteNonQuery($"update accounts set nickname = N'{nickname}', status = {type} where name = '{username}'");
        public int DelAccount(string username) => ProviderDAL.Instance.ExecuteNonQuery($"delete accounts where name = '{username}'");
        public int ResetPassword(string username) => ProviderDAL.Instance.ExecuteNonQuery($"update accounts set password = '1982408718410113214846658453255177250147212' where name = '{username}'");

    }
}
