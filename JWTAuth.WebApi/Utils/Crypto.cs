using System.Security.Cryptography;

namespace JWTAuth.WebApi.Utils
{
    public static class Crypto
    {
        public static string encryptPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool validatePassword(string encryptedPass, string password)
        {
            byte[] hashBytes = Convert.FromBase64String(encryptedPass);

            byte[] salt = new byte[16];
            Array.Copy((byte[])hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            //compare results! byte by byte!
            //starting from 16 in the stored array cause 0-15 are the salt there.
            int ok = 1;
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    ok = 0;
            //if there are no differences between the strings, grant access
            return ok == 1;
        }
    }
}
