using Newtonsoft.Json;

internal class SeekerMatchingFields
{
    [property: JsonProperty("referenceId")]
    public string ReferenceId { get; set;}
    [property: JsonProperty("dateOfBirth")]
    public DateTime DateOfBirth {get; set;}
    [property: JsonProperty("emailAddress")]
    public string EmailAddress {get; set;}
}