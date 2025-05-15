using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
        public static string ComputePerceptualHash(IFormFile imageFile)
        {
            using var image = Image.Load<Rgba32>(imageFile.OpenReadStream());
            // Resize image to 32x32 and convert to grayscale
            image.Mutate(x => x.Resize(32, 32).Grayscale());

            // Convert image to 8x8 DCT (Discrete Cosine Transform)
            double[,] dct = ComputeDCT(image);

            // Get the top-left 8x8 DCT coefficients (excluding DC coefficient)
            double avg = dct.Cast<double>().Skip(1).Take(64).Average();

            // Generate hash based on average DCT coefficient
            StringBuilder hash = new StringBuilder();
            foreach (var value in dct)
            {
                hash.Append(value > avg ? "1" : "0");
            }

            return hash.ToString();
        }
        // Hàm tính Discrete Cosine Transform (DCT)
        private static double[,] ComputeDCT(Image<Rgba32> image)
        {
            double[,] matrix = new double[32, 32];
            float[] luminance = new float[32 * 32];

            // Chuyển ảnh về mảng đơn chiều
            int index = 0;
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    var pixel = image[x, y];
                    luminance[index] = (pixel.R + pixel.G + pixel.B) / 3f;
                    index++;
                }
            }

            // Tính toán DCT
            for (int u = 0; u < 32; u++)
            {
                for (int v = 0; v < 32; v++)
                {
                    double sum = 0.0;
                    for (int x = 0; x < 32; x++)
                    {
                        for (int y = 0; y < 32; y++)
                        {
                            double cos1 = Math.Cos(((2 * x + 1) * u * Math.PI) / 64);
                            double cos2 = Math.Cos(((2 * y + 1) * v * Math.PI) / 64);
                            sum += luminance[y * 32 + x] * cos1 * cos2;
                        }
                    }

                    sum *= ((u == 0) ? 1.0 / Math.Sqrt(2) : 1.0) * ((v == 0) ? 1.0 / Math.Sqrt(2) : 1.0) / 4.0;
                    matrix[u, v] = sum;
                }
            }

            return matrix;
        }

        public static string ConvertIntegerToRoman(int number)
        {
            if ((number < 1) || (number > 3999)) return "Invalid Number";

            string[] thousands = { "", "M", "MM", "MMM" };
            string[] hundreds = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
            string[] tens = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
            string[] units = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

            return thousands[number / 1000] +
                   hundreds[(number % 1000) / 100] +
                   tens[(number % 100) / 10] +
                   units[number % 10];
        }

        public static string ConvertIntegerToLetter(int number)
        {
            if (number < 1 || number > 26)
            {
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 26.");
            }

            char letter = (char)('A' + number - 1);
            return letter.ToString();
        }

    }
}
