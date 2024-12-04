using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleChat
{
    public class CohereChatResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("message")]
        public CohereMessage Message { get; set; }

        [JsonPropertyName("usage")]
        public CohereUsage Usage { get; set; }
    }

    public class CohereMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public List<CohereContent> Content { get; set; }
    }

    public class CohereContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class CohereUsage
    {
        [JsonPropertyName("billed_units")]
        public CohereTokens BilledUnits { get; set; }

        [JsonPropertyName("tokens")]
        public CohereTokens Tokens { get; set; }
    }

    public class CohereTokens
    {
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; set; }

        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; set; }
    }

}
