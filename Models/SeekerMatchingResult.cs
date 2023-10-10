using Newtonsoft.Json;

internal class SeekerMatchingResult
{
    public SeekerMatchingResult()
    {
        UserIds = new List<int>();
    }
    [property: JsonProperty("userIds")]
    public IList<int> UserIds {get; set;}
}