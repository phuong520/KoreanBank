using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Utils
{
    public static class CommonUntils
    {
        public static string NormalizeName(string name)
        {
            string res = "";
            string[] strings = name.Trim().Split("\\s+");
            string[] nameArr = strings;
            for (int i = 0; i < nameArr.Length; i++)
            {
                nameArr[i] = nameArr[i].ToLower();
                res += nameArr[i][..1] + nameArr[i][1..];
                if (i < nameArr.Length) res += " ";
            }
            return res;
        }
        public static string RandomGenerateString(string charSample, int length)
        {
            string res = "";
            Random random = new();
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(charSample.Length);
                res += charSample[index];
            }
            return res;
        }

        public static string PassParamsToFormat(string emailFormat, params string[] paramsToBePassed)
        {
            return string.Format(emailFormat, paramsToBePassed);
        }
        public static bool IsOlderThan18(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age >= 18;
        }
        public static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x3"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

    }
}
