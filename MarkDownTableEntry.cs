using Newtonsoft.Json.Linq;

namespace ReadMeGenerator
{
    public class MarkDownTableEntry
    {
        public MarkDownTableEntry(dynamic snippet)
        {
            Prefix = ((JValue)((JProperty)((JContainer)((JProperty)snippet).Value).Last)?.Value)
                ?.Value?.ToString();

            Description = ((JContainer)((JContainer)((JContainer)snippet).First)?.First?.Next)
                ?.First?.ToString();
        }

        public string Prefix { get; }
        public string Description { get; }
    }
}