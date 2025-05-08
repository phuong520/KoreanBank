using FuzzySharp;
using KEB.Application.DTOs.AnswerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.Utils
{
    public static  class CompareUtils
    {
        public static bool IsCompletelySimilarToString(this string string1, string string2)
        {
            string[] s1 = string1.Trim().Split(' ');
            string[] s2 = string2.Trim().Split(' ');
            return CollectionRandSimilarityCoefficient(s1, s2) == 1;
        }
        public static bool HashIsSimilarTo(this string string1, string string2, double similarityCoefficient)
        {
            return Levenshtein.GetRatio(string1, string2) > similarityCoefficient;
        }
        public static double CollectionRandSimilarityCoefficient<T>(IEnumerable<T> source, IEnumerable<T> target)
        {
            if (!source.Any() && !target.Any()) return 1;

            var intersection = source.Intersect(target).ToArray();
            int count = 0;

            foreach (var item in source)
            {
                if (intersection.Contains(item)) count++;
            }
            foreach (var item in target)
            {
                if (intersection.Contains(item)) count++;
            }

            return (double)count / (source.Count() + target.Count());
        }
        public static bool Are2ListAnswersSimilar(IEnumerable<AddAnswerDTO> answerList1, IEnumerable<AddAnswerDTO> answerList2)
        {
            int count1 = answerList1.Count();
            int count2 = answerList2.Count();

            if (count1 == 0 && count2 == 0) return true;

            var contents1 = answerList1.Select(x => x.Content).ToHashSet();
            var contents2 = answerList2.Select(x => x.Content);

            return contents2.Any(contents1.Contains);
        }
    }
}
