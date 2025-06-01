using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEB.Application.DTOs.GeminiDto
{
    internal class GeminiDto
    {
    }
    public class GeminiRequestBody
    {
        [JsonProperty("contents")]
        public List<Content> Contents { get; set; }
        [JsonProperty("generationConfig")]
        public GenerationConfig GenerationConfig { get; set; }
    }

    public class Content
    {
        [JsonProperty("parts")]
        public List<Part> Parts { get; set; }
    }

    public class Part
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class GenerationConfig
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
       
        [JsonProperty("topK")]
        public int TopK { get; set; }
        [JsonProperty("topP")]
        public double TopP { get; set; }
        [JsonProperty("maxOutputTokens")]
        public int MaxOutputTokens { get; set; }
        //[JsonIgnore]
        //public List<string> StopSequences { get; set; } // Có thể để trống nếu không dùng
    }

    public class GeminiResponse
    {
        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        public Content Content { get; set; }
        // public List<SafetyRating> SafetyRatings { get; set; } // Thêm nếu bạn muốn kiểm tra các rating an toàn
    }
}
