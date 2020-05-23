using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace cw7.Services
{
    public class PasswordHasherService
    {//statyczny handler hashy

        private static byte[] GenerateHashValue(string pswd, byte[] salt)
        {
            var hash = new Rfc2898DeriveBytes(pswd, salt, 20000);
            return hash.GetBytes(32);
        }
        public static string GenerateSaltedHash(string pswd, byte[] salt)
        {

            var saltedhash = GenerateHashValue(pswd, salt);
            return Convert.ToBase64String(saltedhash);
        }
        public static bool Verify(string passrec, string pass, byte[] salt)
        {
            var recievedhash = GenerateSaltedHash(passrec, salt);
            return pass.Equals(recievedhash);
        }
    }
}
