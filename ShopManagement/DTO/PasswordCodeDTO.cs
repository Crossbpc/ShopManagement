using System.Text;
using System.Security.Cryptography;

namespace Coffee.DTO
{
    public class PasswordCodeDTO
    {
        public static string GetCode(string code)
        {
            byte[] bytecode = ASCIIEncoding.ASCII.GetBytes(code);
            byte[] hasdata = new MD5CryptoServiceProvider().ComputeHash(bytecode);
            string result = "";
            foreach (byte item in hasdata) 
            {
                result += item;
            }
            return result;
        }
    }
}
