using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class Salt
    {
        public static string Generate(string id)
        {
            //byte[] randomBytes = new byte[128 / 8];
            //using (var generator = RandomNumberGenerator.Create())
            //{
            //    generator.GetBytes(randomBytes);
            //    return Convert.ToBase64String(randomBytes);
            //}

            SHA256 sha256Hash = SHA256.Create();
            byte[] hashedIdBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(id));

            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < hashedIdBytes.Length; i++)
            {
                sBuilder.Append(hashedIdBytes[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
