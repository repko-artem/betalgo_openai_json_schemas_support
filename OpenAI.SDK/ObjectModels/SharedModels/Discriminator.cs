namespace Betalgo.Ranul.OpenAI.ObjectModels.SharedModels;

using System.Text.Json.Serialization;

/// <summary>
///     When request bodies may be one of a number of different schemas, a discriminator object can be used to aid in
///     serialization, deserialization, and validation. The discriminator is a specific object in a schema which is used to
///     inform the consumer of the document of an alternative schema based on the value associated with it.
///     The discriminator is propertyName and its value is used to determine the schema to use.
///     https://swagger.io/docs/specification/v3_0/data-models/inheritance-and-polymorphism/#discriminator
/// </summary>
public class Discriminator
{

    /// <summary>
    ///     Name of the property in the JSON object that will be used to determine the schema to use.
    ///     Used when oneOf and anyOf schema compositions are used.
    /// </summary>
    [JsonPropertyName("propertyName")]
    public string? PropertyName { get; set; }

    /// <summary>
    ///     A mapping between the values of the propertyName and the schema names or references.
    ///     This is used to determine which schema to use based on the value of the propertyName
    /// </summary>
    [JsonPropertyName("mapping")]
    public IDictionary<string, string>? Mapping { get; set; }
}
