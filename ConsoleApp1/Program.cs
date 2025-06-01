using System.Text.Json;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

public class SimilarityResult
{
    public int Index { get; set; }
    public int Score { get; set; }
    public string Reason { get; set; }
}

public class ContentBlockDto
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
}
public class Program
{
    static readonly HttpClient client = new HttpClient();
    static string apiKey = "AIzaSyCPOkRXPnfzrFFdysp68mjvLr120v2-2mM"; // Thay bằng API key Gemini của bạn
    static string model = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
    static async Task<string> GetScoreAndReasonFromGemini(string prompt)
    {
        var requestBody = new
        {
            contents = new[]
            {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        }
        };

        var requestJson = JsonSerializer.Serialize(requestBody);
        var request = new HttpRequestMessage(HttpMethod.Post, model);
        request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        // Trích xuất phần text chứa JSON từ phản hồi của Gemini
        var contentText = JObject.Parse(responseString)?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "";
        Console.WriteLine(contentText);
        // Tách JSON array từ contentText
        //var jsonMatch = Regex.Match(contentText, @"\[\s*{.*?}\s*\]", RegexOptions.Singleline);
        //if (!jsonMatch.Success)
        //{
        //    throw new Exception("Không tìm thấy JSON hợp lệ trong phản hồi từ Gemini.");
        //}

        //return jsonMatch.Value;
        return contentText;
    }

    private static string EscapeJson(string input)
    {
        return input.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }

    // ham nhap du lieu
    public static async Task<string> SimilarCheckBatchAsync(
    List<ContentBlockDto> oldList,
    List<ContentBlockDto> newList)
    {
        var allPairs = new List<(ContentBlockDto, ContentBlockDto)>();

        // So sánh new với old
        foreach (var q1 in newList)
            foreach (var q2 in oldList)
                allPairs.Add((q1, q2));

        // So sánh trong newList
        for (int i = 0; i < newList.Count; i++)
            for (int j = i + 1; j < newList.Count; j++)
                allPairs.Add((newList[i], newList[j]));

        // Tạo prompt dạng JSON
        var promptBuilder = new StringBuilder();
        promptBuilder.AppendLine("So sánh mức độ giống nhau giữa các cặp câu hỏi tiếng Hàn sau. Trả về kết quả theo định dạng JSON:");
        promptBuilder.AppendLine("[");
        for (int i = 0; i < allPairs.Count; i++)
        {
            var (q1, q2) = allPairs[i];
            promptBuilder.AppendLine("  {");
            promptBuilder.AppendLine($"    \"index\": {i},");
            promptBuilder.AppendLine($"    \"question1\": \"{EscapeJson(q1.Content)}\",");
            promptBuilder.AppendLine($"    \"question2\": \"{EscapeJson(q2.Content)}\"");
            promptBuilder.AppendLine(i < allPairs.Count - 1 ? "  }," : "  }");
        }
        promptBuilder.AppendLine("]");
        promptBuilder.AppendLine("Mỗi phần tử kết quả JSON nên có: Index, Score (0–10), Reason (bằng tiếng việt).");

        var prompt = promptBuilder.ToString();

        // Gọi Gemini và lấy response JSON
        var text = await GetScoreAndReasonFromGemini(prompt);

        // Parse JSON
        // var parsedResults = JsonSerializer.Deserialize<List<SimilarityResult>>(jsonResponse);

        // var results = new List<SimilarQuestion>();

        //foreach (var result in parsedResults)
        //{
        //    if (result.Score >= 7 && result.Index >= 0 && result.Index < allPairs.Count)
        //    {
        //        var (q1, q2) = allPairs[result.Index];
        //        var id1 = q1.Id < q2.Id ? q1.Id : q2.Id;
        //        var id2 = q1.Id >= q2.Id ? q1.Id : q2.Id;

        //        results.Add(new SimilarQuestion
        //        {
        //            ContentBlockId1 = id1,
        //            ContentBlockId2 = id2,
        //            Score = result.Score,
        //            Reason = result.Reason
        //        });
        //    }
        //}

        return text;
    }
    public static List<SimilarityResult> TryParseSimilarityResults(string contentText)
    {
        var match = Regex.Match(contentText, "```json\\s*(.*?)\\s*```", RegexOptions.Singleline);
        string jsonContent = match.Groups[1].Value;
        Console.WriteLine("JSON content:");
        Console.WriteLine(jsonContent);
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            // Nếu contentText chính là JSON array hợp lệ, thử parse trực tiếp
            var direct = JsonSerializer.Deserialize<List<SimilarityResult>>(jsonContent, options);
            if (direct != null && direct.Count > 0)
                return direct;
        }
        catch
        {
            // Không parse được trực tiếp, thử bằng Regex
            var jsonMatch = Regex.Match(contentText, @"\[\s*{.*?}\s*\]", RegexOptions.Singleline);
            if (jsonMatch.Success)
            {
                try
                {
                    var extracted = JsonSerializer.Deserialize<List<SimilarityResult>>(jsonMatch.Value);
                    if (extracted != null)
                        return extracted;
                }
                catch { }
            }
        }

        return new List<SimilarityResult>();
    }

    public static async Task Main(string[] args)
    {

        Console.OutputEncoding = Encoding.UTF8;
        var oldList = new List<ContentBlockDto>
        {
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "오늘 날씨가 어때요?" }, // Hôm nay thời tiết thế nào?
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "어제 뭐 했어요?" },   // Hôm qua bạn đã làm gì?
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "한국 음식을 좋아해요?" }, // Bạn có thích món ăn Hàn không?
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "주말에 뭐 할 거예요?" }, // Cuối tuần bạn sẽ làm gì?
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "무슨 음악을 자주 들어요?" } // Bạn thường nghe nhạc gì?
        };

        var newList = new List<ContentBlockDto>
        {
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "오늘 기온이 어떤가요?" }, // Hôm nay nhiệt độ thế nào?
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "어제 하루 어떻게 보냈어요?" }, // Hôm qua bạn đã trải qua thế nào?
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "한식 좋아하세요?" }, // Bạn có thích ẩm thực Hàn không?
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "이번 주말 계획 있어요?" }, // Bạn có kế hoạch gì cuối tuần này không?
            new ContentBlockDto { Id = Guid.NewGuid(), Content = "어떤 음악 듣는 걸 좋아해요?" } // Bạn thích nghe loại nhạc nào?
        };
        var text = await SimilarCheckBatchAsync(oldList, newList);
        Console.WriteLine(text);
        var list = TryParseSimilarityResults(text);
        list.ForEach(x => Console.WriteLine(x.Reason));

        //string a = "```json\r\n[\r\n  {\r\n    \"index\": 0,\r\n    \"score\": 9,\r\n    \"reason\": \"C? hai câu h?i d?u h?i v? th?i ti?t hôm nay. '?? ??? ?????' h?i v? nhi?t d?, còn '?? ??? ????' h?i v? th?i ti?t chung.\"\r\n  },\r\n  {\r\n    \"index\": 1,\r\n    \"score\": 1,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, trong khi '?? ? ????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 2,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ??? ?????' h?i v? s? thích d? an Hàn Qu?c. Hoàn toàn không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 3,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '??? ? ? ????' h?i v? k? ho?ch cu?i tu?n. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 4,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ??? ?? ????' h?i v? th? lo?i nh?c yêu thích. Hoàn toàn không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 5,\r\n    \"score\": 1,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ??? ????' h?i v? th?i ti?t ngày hôm nay. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 6,\r\n    \"score\": 8,\r\n    \"reason\": \"C? hai câu h?i d?u h?i v? nh?ng gì da làm hôm qua. '?? ?? ??? ?????' h?i chi ti?t hon so v?i '?? ? ????'\"\r\n  },\r\n  {\r\n    \"index\": 7,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ??? ?????' h?i v? s? thích d? an Hàn Qu?c. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 8,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '??? ? ? ????' h?i v? k? ho?ch cu?i tu?n. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 9,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ??? ?? ????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 10,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ??? ????' h?i v? th?i ti?t hôm nay. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 11,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ? ????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 12,\r\n    \"score\": 10,\r\n    \"reason\": \"Hai câu h?i này hoàn toàn tuong d?ng v? y nghia. '?? ??????' và '?? ??? ?????' d?u h?i v? s? thích d? an Hàn Qu?c.\"\r\n  },\r\n  {\r\n    \"index\": 13,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '??? ? ? ????' h?i v? k? ho?ch cu?i tu?n. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 14,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ??? ?? ????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 15,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ??? ????' h?i v? th?i ti?t hôm nay. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 16,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ? ????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 17,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ??? ?????' h?i v? s? thích d? an Hàn Qu?c. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 18,\r\n    \"score\": 9,\r\n    \"reason\": \"C? hai câu h?i d?u h?i v? k? ho?ch cho cu?i tu?n. '?? ?? ?? ????' và '??? ? ? ????' có y nghia tuong d?ng.\"\r\n  },\r\n  {\r\n    \"index\": 19,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ??? ?? ????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 20,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích, '?? ??? ????' h?i v? th?i ti?t hôm nay. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 21,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích, '?? ? ????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 22,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích, '?? ??? ?????' h?i v? s? thích d? an Hàn Qu?c. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 23,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích, '??? ? ? ????' h?i v? k? ho?ch cu?i tu?n. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 24,\r\n    \"score\": 9,\r\n    \"reason\": \"C? hai câu h?i d?u h?i v? th? lo?i nh?c yêu thích. '?? ?? ?? ? ?????' và '?? ??? ?? ????' có y nghia tuong d?ng.\"\r\n  },\r\n    {\r\n    \"index\": 25,\r\n    \"score\": 1,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, trong khi '?? ?? ??? ?????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 26,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ??????' h?i v? s? thích d? an Hàn Qu?c. Hoàn toàn không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 27,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 28,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích. Hoàn toàn không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 29,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ??????' h?i v? s? thích d? an Hàn Qu?c. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 30,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 31,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 32,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 33,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 34,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  }\r\n]\r\n```\r\n```json\r\n[\r\n  {\r\n    \"index\": 0,\r\n    \"score\": 9,\r\n    \"reason\": \"C? hai câu h?i d?u h?i v? th?i ti?t hôm nay. '?? ??? ?????' h?i v? nhi?t d?, còn '?? ??? ????' h?i v? th?i ti?t chung.\"\r\n  },\r\n  {\r\n    \"index\": 1,\r\n    \"score\": 1,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, trong khi '?? ? ????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 2,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ??? ?????' h?i v? s? thích d? an Hàn Qu?c. Hoàn toàn không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 3,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '??? ? ? ????' h?i v? k? ho?ch cu?i tu?n. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 4,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ??? ?? ????' h?i v? th? lo?i nh?c yêu thích. Hoàn toàn không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 5,\r\n    \"score\": 1,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ??? ????' h?i v? th?i ti?t ngày hôm nay. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 6,\r\n    \"score\": 8,\r\n    \"reason\": \"C? hai câu h?i d?u h?i v? nh?ng gì da làm hôm qua. '?? ?? ??? ?????' h?i chi ti?t hon so v?i '?? ? ????'\"\r\n  },\r\n  {\r\n    \"index\": 7,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ??? ?????' h?i v? s? thích d? an Hàn Qu?c. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 8,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '??? ? ? ????' h?i v? k? ho?ch cu?i tu?n. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 9,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ??? ?? ????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 10,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ??? ????' h?i v? th?i ti?t hôm nay. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 11,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ? ????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 12,\r\n    \"score\": 10,\r\n    \"reason\": \"Hai câu h?i này hoàn toàn tuong d?ng v? y nghia. '?? ??????' và '?? ??? ?????' d?u h?i v? s? thích d? an Hàn Qu?c.\"\r\n  },\r\n  {\r\n    \"index\": 13,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '??? ? ? ????' h?i v? k? ho?ch cu?i tu?n. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 14,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ??? ?? ????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 15,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ??? ????' h?i v? th?i ti?t hôm nay. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 16,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ? ????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 17,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ??? ?????' h?i v? s? thích d? an Hàn Qu?c. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 18,\r\n    \"score\": 9,\r\n    \"reason\": \"C? hai câu h?i d?u h?i v? k? ho?ch cho cu?i tu?n. '?? ?? ?? ????' và '??? ? ? ????' có y nghia tuong d?ng.\"\r\n  },\r\n  {\r\n    \"index\": 19,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ??? ?? ????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 20,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích, '?? ??? ????' h?i v? th?i ti?t hôm nay. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 21,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích, '?? ? ????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 22,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích, '?? ??? ?????' h?i v? s? thích d? an Hàn Qu?c. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 23,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích, '??? ? ? ????' h?i v? k? ho?ch cu?i tu?n. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 24,\r\n    \"score\": 9,\r\n    \"reason\": \"C? hai câu h?i d?u h?i v? th? lo?i nh?c yêu thích. '?? ?? ?? ? ?????' và '?? ??? ?? ????' có y nghia tuong d?ng.\"\r\n  },\r\n    {\r\n    \"index\": 25,\r\n    \"score\": 1,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, trong khi '?? ?? ??? ?????' h?i v? ho?t d?ng hôm qua. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 26,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ??????' h?i v? s? thích d? an Hàn Qu?c. Hoàn toàn không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 27,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 28,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??? ?????' h?i v? nhi?t d? hôm nay, còn '?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích. Hoàn toàn không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 29,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ??????' h?i v? s? thích d? an Hàn Qu?c. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 30,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 31,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ??? ?????' h?i v? m?t ngày hôm qua di?n ra nhu th? nào, '?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 32,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 33,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ??????' h?i v? s? thích d? an Hàn Qu?c, '?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  },\r\n  {\r\n    \"index\": 34,\r\n    \"score\": 0,\r\n    \"reason\": \"'?? ?? ?? ????' h?i v? k? ho?ch cu?i tu?n này, '?? ?? ?? ? ?????' h?i v? th? lo?i nh?c yêu thích. Không liên quan.\"\r\n  }\r\n]\r\n``` viết code để xóa các phần nằm ngoài []"

    }
}
